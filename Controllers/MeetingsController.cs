using LinkUpBackend.DTOs;
using LinkUpBackend.Models;
using LinkUpBackend.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

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
    [Route("organizator/{id:guid}")]
    public async Task<IActionResult> GetMeetingsFromOrganizator([FromRoute] Guid id){
        var organizatorMeetingsIds = await dbContext.MeetingsOrganizators
            .Where(mo => mo.OrganizatorId == id.ToString())
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
            .ToListAsync();
        return Ok(myMeetings);
    }

    [Authorize]
    [HttpPost("client/join")]
    public async Task<IActionResult> JoinMeeting([FromBody] Guid id)
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
    [HttpPost("client/leave")]
    public async Task<IActionResult> LeaveMeeting([FromBody] Guid id)
    {
        return Problem(statusCode: 405);
    }

    [Authorize]
    [HttpPost("client/reschedule")]
    public async Task<IActionResult> RescheduleMeeting([FromBody] RescheduleMeetingDTO rescheduleInfo)
    {
        return Problem(statusCode: 405);
    }

    [Authorize]
    [HttpGet("client/my-meetings")]
    public async Task<IActionResult> GetMeetingsAsClient()
    {
        return Problem(statusCode: 405);
    }
}