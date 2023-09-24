using LinkUpBackend.DTOs;
using LinkUpBackend.Models;
using LinkUpBackend.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using LinkUpBackend.Migrations;
using System.Diagnostics.Eventing.Reader;

namespace Meetings.Controllers;
[ApiController]
[Route("api/[controller]")]
public class MeetingsController : Controller{
    private readonly AppDbContext dbContext;
    private readonly UserManager<User> userManager;
    public MeetingsController(AppDbContext dbContext, UserManager<User> userManager){
        this.dbContext = dbContext;
        this.userManager = userManager;
    }
    [HttpGet]
    public async Task<IActionResult> GetMeetings(){
        return Ok(await dbContext.Meetings.ToListAsync());
    }
    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> GetMeeting([FromRoute] Guid id){
        var meeting = await dbContext.Meetings.FindAsync(id);
        if (meeting == null){
            return NotFound();
        }
        return Ok(meeting);
    }
 
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = "Admin,Contractor")]
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> AddMeeting(AddMeetingRequestDTO request){
        // Searching for users
        var userEmail = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await userManager.FindByEmailAsync(userEmail);

        if (DateTime.TryParse(request.DateTime, out DateTime dateTime)) {
            var meeting = new Meeting
            {
                Id = Guid.NewGuid(),
                DateTime = dateTime.ToUniversalTime(),
                MaxParticipants = request.MaxParticipants,
                Duration = request.Duration,
                Description = request.Description
            };
            var meetingOrganizator = new MeetingOrganizator
            {
                MeetingId = meeting.Id,
                OrganizatorId = user.Id
            };
            await dbContext.Meetings.AddAsync(meeting);
            await dbContext.MeetingsOrganizators.AddAsync(meetingOrganizator);
            await dbContext.SaveChangesAsync();

        }

        return Ok();
    }
    [HttpPut]
    [Route("{id:guid}")]
    public async Task<IActionResult> UpdateMeeting ([FromRoute] Guid id, UpdateMeetingRequestDTO request){
        var meeting = await dbContext.Meetings.FindAsync(id);
        if(meeting != null){
            meeting.DateTime = request.DateTime;
            meeting.MaxParticipants = request.MaxParticipants;
            meeting.Duration = request.Duration;
            meeting.Description = request.Description;

            await dbContext.SaveChangesAsync();

            return Ok(meeting);
        }
        return NotFound();
    }
    [HttpDelete]
    [Route("{id:guid}")]
    public async Task<IActionResult> DeleteMeeting([FromRoute] Guid id){
        var meeting =await dbContext.Meetings.FindAsync(id);
        if (meeting != null)
        {
            dbContext.Remove(meeting);
            await dbContext.SaveChangesAsync();
            return Ok(meeting);
        }
        return NotFound();
    }
    [HttpGet]
    [Route("organizator/{email}")]
    public async Task<IActionResult> GetMeetingsFromOrganizator([FromRoute] string email){
        var contractor = await userManager.FindByEmailAsync(email);
        var organizatorMeetingsIds = await dbContext.MeetingsOrganizators
            .Where(mo => mo.OrganizatorId == contractor.Id.ToString())
            .Select(mo => mo.MeetingId)
            .ToListAsync();
        var meetings = await dbContext.Meetings
            .Where(m => organizatorMeetingsIds.Contains(m.Id))
            .ToListAsync();
        return Ok(meetings);
    }
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet]
    [Route("organizator/my-meetings")]
    public async Task<IActionResult> GetMyMeetingsAsOrganizator(){
        var userEmail = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await userManager.FindByEmailAsync(userEmail);
        var myMeetingsIds = await dbContext.MeetingsOrganizators
            .Where(mo => mo.OrganizatorId == user.Id.ToString())
            .Select(mo => mo.MeetingId)
            .ToListAsync();
        var myMeetings = await dbContext.Meetings
            .Where(m => myMeetingsIds.Contains(m.Id))
            .OrderBy(m => m.DateTime)
            .ToListAsync();
        return Ok(myMeetings);
    }

    [Authorize]
    [HttpPost("{id:guid}/join")]
    public async Task<IActionResult> JoinMeeting([FromRoute] Guid id)
    {

        var userEmail = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await userManager.FindByEmailAsync(userEmail!);
        var meeting = await dbContext.Meetings.FindAsync(id);
        if (meeting == null)
        {
            return NotFound("No such meeting");
        }
        var organizators = await dbContext.MeetingsOrganizators.Where(x => x.MeetingId == meeting.Id).Select(pair => pair.OrganizatorId!).ToListAsync();
        if (organizators.Contains(user!.Id))
        {
            return BadRequest("Tried to join owned meeting");
        }
        var currentParticipants = await dbContext.MeetingsParticipants.Where(x => x.MeetingId == meeting.Id).Select(pair => pair.ParticipantId!).ToListAsync();
        if (currentParticipants.Contains(user.Id))
        {
            return BadRequest("User already in the meeting");
        }
        if (currentParticipants.Count == meeting.MaxParticipants)
        {
            return BadRequest("Meeting already full");
        }
        dbContext.MeetingsParticipants.Add(new MeetingParticipant() { MeetingId = meeting.Id, ParticipantId = user.Id, Participant = user, Meeting = meeting });
        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (Exception)
        {
            return Problem("Due to server error couldn't save the participation in meeting, try to contact service administrator");
        }

        return Ok();
    }

    [Authorize]
    [HttpPost("{id:guid}/leave")]
    public async Task<IActionResult> LeaveMeeting([FromRoute] Guid id)
    {
        var userEmail = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await userManager.FindByEmailAsync(userEmail!);
        var meeting = await dbContext.Meetings.FindAsync(id);
        if (meeting == null)
        {
            return NotFound("No such meeting");
        }
        var userRegisteredParticipation = await dbContext.MeetingsParticipants.FirstOrDefaultAsync(x => x.MeetingId == meeting.Id && x.ParticipantId == user!.Id);
        if (userRegisteredParticipation is null)
        {
            return BadRequest("This user is not participating in selected meeting");
        }
        dbContext.MeetingsParticipants.Remove(userRegisteredParticipation);
        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (Exception)
        {
            return Problem("Due to server error couldn't save the participation in meeting, try to contact service administrator");
        }
        return Ok();
    }

    [Authorize]
    [HttpPost("reschedule")]
    public async Task<IActionResult> RescheduleMeeting([FromBody] RescheduleMeetingDTO rescheduleInfo)
    {
        var userEmail = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await userManager.FindByEmailAsync(userEmail!);
        var oldMeeting = await dbContext.Meetings.FindAsync(rescheduleInfo.OldMeetingId);
        var newMeeting = await dbContext.Meetings.FindAsync(rescheduleInfo.NewMeetingId);
        if (oldMeeting is null)
        {
            return NotFound("Tried to reschedule from non existant meeting");
        }
        if (newMeeting is null)
        {
            return NotFound("Tried to reschedule to non existant meeting");
        }
        var userRegisteredOldParticipation = await dbContext.MeetingsParticipants.FirstOrDefaultAsync(x => x.MeetingId == oldMeeting.Id && x.ParticipantId == user!.Id);
        if (userRegisteredOldParticipation is null)
        {
            return BadRequest("Tried to reschedule from a meeting that the user is not participating in");
        }
        var newMeetingOrganizators = await dbContext.MeetingsOrganizators.Where(x => x.MeetingId == newMeeting.Id).Select(pair => pair.OrganizatorId!).ToListAsync();
        if (newMeetingOrganizators.Contains(user!.Id))
        {
            return BadRequest("Tried to reschedule to owned meeting");
        }
        var currentParticipants = await dbContext.MeetingsParticipants.Where(x => x.MeetingId == newMeeting.Id).Select(pair => pair.ParticipantId!).ToListAsync();
        if (currentParticipants.Contains(user.Id))
        {
            return BadRequest("Tried to reschedule to a meeting that user is already participating in");
        }
        if (currentParticipants.Count == newMeeting.MaxParticipants)
        {
            return BadRequest("Tried to reschedule to an already full meeting");
        }
        dbContext.MeetingsParticipants.Remove(userRegisteredOldParticipation);
        dbContext.MeetingsParticipants.Add(new MeetingParticipant() { Meeting = newMeeting, MeetingId = newMeeting.Id, Participant = user, ParticipantId = user.Id });
        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (Exception)
        {
            return Problem("Due to server error couldn't save the participation in meeting, try to contact service administrator");
        }
        return Ok();
    }

    [HttpGet]
    [Route("client/{id:guid}")]
    public async Task<IActionResult> GetMeetingsByParticipant([FromRoute] Guid id)
    {
        var organizatorMeetingsIds = await dbContext.MeetingsParticipants
            .Where(x => x.ParticipantId == id.ToString())
            .Select(x => x.MeetingId)
            .ToListAsync();
        var meetings = await dbContext.Meetings
            .Where(m => organizatorMeetingsIds.Contains(m.Id))
            .ToListAsync();
        return Ok(meetings);
    }

    [Authorize]
    [HttpGet("client/my-meetings")]
    public async Task<IActionResult> GetMeetingsAsClient()
    {
        var userEmail = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await userManager.FindByEmailAsync(userEmail!);
        var userMeetingsIds = await dbContext.MeetingsParticipants
            .Where(x => x.ParticipantId == user!.Id)
            .Select(x => x.MeetingId)
            .ToListAsync();
        var myMeetings = await dbContext.Meetings
            .Where(meeting => userMeetingsIds.Contains(meeting.Id))
            .ToListAsync();
        return Ok(myMeetings);
    }
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet]
    [Route("upcoming")]
    public async Task<IActionResult> GetUpcomingMeetings() //so far only for admin/contractor
    {
        var userEmail = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await userManager.FindByEmailAsync(userEmail);
        var userRole = await userManager.GetRolesAsync(user);

        //if (userRole.Contains("Client")
        //{
        //    var upcomingMeetingsIds = await dbContext.MeetingsParticipants
        //        .Where(m => m.ParticipantId == user.Id.ToString())
        //        .Select(m => m.MeetingId)
        //        .ToListAsync();

        //    var upcomingMeetings = await dbContext.Meetings
        //        .Where(m => upcomingMeetingsIds.Contains(m.Id))
        //        .OrderBy(m => m.DateTime)
        //        .ToListAsync();
        //}
        //else
        //{

        var myMeetingsIds = await dbContext.MeetingsOrganizators
            .Where(mo => mo.OrganizatorId == user.Id.ToString())
            .Select(mo => mo.MeetingId)
            .ToListAsync();
        var myMeetings = await dbContext.Meetings
            .Where(m => myMeetingsIds.Contains(m.Id))
            .OrderBy(m => m.DateTime)
            .ToListAsync();
        //}
        var upcomingMeetings = IsMeetingArchived(myMeetings, false);

        return Ok(upcomingMeetings);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet]
    [Route("archived")]
    public async Task<IActionResult> GetArchivedMeetings() //so far only for admin/contractor
    {
        var userEmail = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await userManager.FindByEmailAsync(userEmail);
        var userRole = await userManager.GetRolesAsync(user);

        //if (userRole.Contains("Client")
        //{
        //    var upcomingMeetingsIds = await dbContext.MeetingsParticipants
        //        .Where(m => m.ParticipantId == user.Id.ToString())
        //        .Select(m => m.MeetingId)
        //        .ToListAsync();

        //    var upcomingMeetings = await dbContext.Meetings
        //        .Where(m => upcomingMeetingsIds.Contains(m.Id))
        //        .OrderBy(m => m.DateTime)
        //        .ToListAsync();
        //}
        //else
        //{

        var myMeetingsIds = await dbContext.MeetingsOrganizators
            .Where(mo => mo.OrganizatorId == user.Id.ToString())
            .Select(mo => mo.MeetingId)
            .ToListAsync();
        var myMeetings = await dbContext.Meetings
            .Where(m => myMeetingsIds.Contains(m.Id))
            .OrderBy(m => m.DateTime)
            .ToListAsync();
        //}
        var archivedMeetings = IsMeetingArchived(myMeetings);

        return Ok(archivedMeetings);
    }


    private List<Meeting> IsMeetingArchived(List<Meeting> meetingList, bool archive = true)
    {
        List<Meeting> filteredMeetings = new List<Meeting>();

        foreach (var meeting in meetingList)
        {
            bool isArchived = archive && meeting.DateTime.AddMinutes(meeting.Duration) < DateTime.Now.ToUniversalTime();
            bool isUpcoming = !archive && meeting.DateTime.AddMinutes(meeting.Duration) >= DateTime.Now.ToUniversalTime();

            if (isArchived || isUpcoming)
            {
                filteredMeetings.Add(meeting);
            }
        }
        return filteredMeetings;
    }
}