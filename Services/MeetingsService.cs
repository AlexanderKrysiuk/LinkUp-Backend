using ErrorOr;
using LinkUpBackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LinkUpBackend.Infrastructure;
using LinkUpBackend.ServiceErrors;
using System.Data.SqlTypes;
using LinkUpBackend.DTOs;

namespace LinkUpBackend.Services
{
    public class MeetingsService
    {
        private AppDbContext _dbContext;
        private UsersService _usersService;
        public MeetingsService(AppDbContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _usersService = new UsersService(userManager);
        }

        public async Task<ErrorOr<bool>> JoinMeeting(Guid id, string userEmail)
        {
            var errorOrUser = await _usersService.GetUserByEmail(userEmail);
            if(errorOrUser.IsError)
            {
                return errorOrUser.Errors;
            }
            var user = errorOrUser.Value;
            var meeting = await _dbContext.Meetings.FindAsync(id);
            if (meeting == null)
            {
                return Errors.Meeting.NotFound;
            }
            var organizators = await _dbContext.MeetingsOrganizators.Where(x => x.MeetingId == meeting.Id).Select(pair => pair.OrganizatorId!).ToListAsync();
            if (organizators.Contains(user!.Id))
            {
                return Errors.Meeting.JoinedOwned;
            }
            var currentParticipants = await _dbContext.MeetingsParticipants.Where(x => x.MeetingId == meeting.Id).Select(pair => pair.ParticipantId!).ToListAsync();
            if (currentParticipants.Contains(user.Id))
            {
                return Errors.Meeting.AlreadyJoined;
            }
            if (currentParticipants.Count == meeting.MaxParticipants)
            {
                return Errors.Meeting.AlreadyFull;
            }
            _dbContext.MeetingsParticipants.Add(new MeetingParticipant() { MeetingId = meeting.Id, ParticipantId = user.Id, Participant = user, Meeting = meeting });
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return Error.Unexpected(description:"Due to server error couldn't save the participation in meeting, try to contact service administrator");
            }
            return true;
        }

        public async Task<ErrorOr<bool>> LeaveMeeting(Guid id, string userEmail)
        {
            var errorOrUser = await _usersService.GetUserByEmail(userEmail);
            if (errorOrUser.IsError)
            {
                return errorOrUser.Errors;
            }
            var user = errorOrUser.Value;
            var meeting = await _dbContext.Meetings.FindAsync(id);
            if (meeting == null)
            {
                return Errors.Meeting.NotFound;
            }
            var userRegisteredParticipation = await _dbContext.MeetingsParticipants.FirstOrDefaultAsync(x => x.MeetingId == meeting.Id && x.ParticipantId == user!.Id);
            if (userRegisteredParticipation is null)
            {
                return Errors.Meeting.NotAParticipant;
            }
            _dbContext.MeetingsParticipants.Remove(userRegisteredParticipation);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return Error.Unexpected(description: "Due to server error couldn't leave the meeting, try to contact service administrator");
            }
            return true;
        }

        public async Task<ErrorOr<bool>> RescheduleMeeting(RescheduleMeetingDTO rescheduleInfo, string userEmail)
        {
            var errorOrUser = await _usersService.GetUserByEmail(userEmail);
            if (errorOrUser.IsError)
            {
                return errorOrUser.Errors;
            }
            var user = errorOrUser.Value;
            var oldMeeting = await _dbContext.Meetings.FindAsync(rescheduleInfo.OldMeetingId);
            if (oldMeeting == null)
            {
                return Errors.Meeting.NotFound;
            }
            var newMeeting = await _dbContext.Meetings.FindAsync(rescheduleInfo.NewMeetingId);
            if (newMeeting == null)
            {
                return Errors.Meeting.NotFound;
            }
            var userRegisteredOldParticipation = await _dbContext.MeetingsParticipants.FirstOrDefaultAsync(x => x.MeetingId == oldMeeting.Id && x.ParticipantId == user!.Id);
            if (userRegisteredOldParticipation is null)
            {
                return Errors.Meeting.NotAParticipant;
            }
            var newMeetingOrganizators = await _dbContext.MeetingsOrganizators.Where(x => x.MeetingId == newMeeting.Id).Select(pair => pair.OrganizatorId!).ToListAsync();
            if (newMeetingOrganizators.Contains(user!.Id))
            {
                return Errors.Meeting.JoinedOwned;
            }
            var currentParticipants = await _dbContext.MeetingsParticipants.Where(x => x.MeetingId == newMeeting.Id).Select(pair => pair.ParticipantId!).ToListAsync();
            if (currentParticipants.Contains(user.Id))
            {
                return Errors.Meeting.AlreadyJoined;
            }
            if (currentParticipants.Count == newMeeting.MaxParticipants)
            {
                return Errors.Meeting.AlreadyFull;
            }
            _dbContext.MeetingsParticipants.Remove(userRegisteredOldParticipation);
            _dbContext.MeetingsParticipants.Add(new MeetingParticipant() { Meeting = newMeeting, MeetingId = newMeeting.Id, Participant = user, ParticipantId = user.Id });
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return Error.Unexpected(description: "Due to server error couldn't reschedule the meeting, try to contact service administrator");
            }
            return true;
        }
    }
}
