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

        var meeting = new Meeting{
            Id = Guid.NewGuid(),
            DateTime = request.DateTime,
            MaxParticipants = request.MaxParticipants,
            Duration = request.Duration,
            Description = request.Description
        };
        var meetingOrganizator = new MeetingOrganizator{
            MeetingId = meeting.Id,
            OrganizatorId = user.Id
        };
        await dbContext.Meetings.AddAsync(meeting);
        await dbContext.MeetingsOrganizators.AddAsync(meetingOrganizator);
        await dbContext.SaveChangesAsync();
        
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
            .ToListAsync();
        return Ok(myMeetings);
    }
}