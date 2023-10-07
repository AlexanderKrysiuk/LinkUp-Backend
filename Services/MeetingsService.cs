using ErrorOr;
using LinkUpBackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LinkUpBackend.Infrastructure;
using LinkUpBackend.ServiceErrors;
using System.Data.SqlTypes;
using LinkUpBackend.DTOs;
using System.Security.Claims;

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

        public async Task<ErrorOr<List<Guid>>> GetJoinedMeetingsIdsByUserId(string userId)
        {
            var userMeetingsIds = await _dbContext.MeetingsParticipants
                .Where(x => x.ParticipantId == userId)
                .Select(x => x.MeetingId)
                .ToListAsync();
            return userMeetingsIds;
        }

        public async Task<ErrorOr<List<Meeting>>> GetJoinedMeetingsByUserId(string userId)
        {
            var errorOrMeetingsIds = await GetJoinedMeetingsIdsByUserId(userId);
            var userMeetingsIds = errorOrMeetingsIds.Value;
            var myMeetings = await _dbContext.Meetings
                .Where(meeting => userMeetingsIds.Contains(meeting.Id))
                .ToListAsync();
            return myMeetings;
        }

        public async Task<ErrorOr<List<Meeting>>> GetJoinedMeetingsByEmail(string userEmail)
        {
            var errorOrUser = await _usersService.GetUserByEmail(userEmail);
            if (errorOrUser.IsError)
            {
                return errorOrUser.Errors;
            }
            return await GetJoinedMeetingsByUserId(errorOrUser.Value.Id);
        }

        public async Task<ErrorOr<List<MeetingDTO>>> AddInformationAboutUserParticipation(List<Meeting> meetings, string? userEmail)
        {
            if (userEmail == null)
            {
                return meetings.Select(meeting => new MeetingDTO(meeting, false)).ToList();
            }
            var errorOrUser = await _usersService.GetUserByEmail(userEmail);
            if (errorOrUser.IsError)
            {
                return errorOrUser.Errors;
            }
            var user = errorOrUser.Value;
            var errorOrJoinedMeetingsIds = await GetJoinedMeetingsIdsByUserId(user.Id);
            if (errorOrJoinedMeetingsIds.IsError)
            {
                return errorOrJoinedMeetingsIds.Errors;
            }
            var joinedMeetingsIds = errorOrJoinedMeetingsIds.Value;
            return meetings.Select(meeting => new MeetingDTO(meeting, joinedMeetingsIds.Contains(meeting.Id))).ToList();
        }

        public async Task<ErrorOr<List<MeetingDTO>>> GetAllMeetings(string? userEmail)
        {
            var meetings = await _dbContext.Meetings.ToListAsync();
            return await AddInformationAboutUserParticipation(meetings, userEmail);
        }

        public async Task<ErrorOr<List<MeetingDTO>>> GetMeetingsByOrganizator(string organizatorEmail, string? userEmail)
        {
            var errorOrOrganizator = await _usersService.GetUserByEmail(organizatorEmail);
            if(errorOrOrganizator.IsError)
            {
                return errorOrOrganizator.Errors;
            }
            var organizator = errorOrOrganizator.Value;
            var meetingsIds = await _dbContext.MeetingsOrganizators.Where(pair => pair.OrganizatorId == organizator.Id).Select(pair => pair.MeetingId).ToListAsync();
            var meetings = await _dbContext.Meetings.Where(meeting => meetingsIds.Contains(meeting.Id)).ToListAsync();
            return await AddInformationAboutUserParticipation(meetings, userEmail);
        }
    }
}
