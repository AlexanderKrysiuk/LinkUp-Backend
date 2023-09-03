using LinkUpBackend.DTOs;
using LinkUpBackend.Models;
using LinkUpBackend.Infrastructure;
using LinkUpBackend.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;


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
    
    [HttpPost]
    public async Task<IActionResult> AddMeeting(AddMeetingRequestDTO request){
        var meeting = new Meeting{
            Id = Guid.NewGuid(),
            DateTime = request.DateTime,
            MaxParticipant = request.MaxParticipant,
            Duration = request.Duration,
            Description = request.Description
        };
        var meetingOrganizator = new MeetingOrganizator{
            MeetingId = meeting.Id,
            OrganizatorId = request.OrganizatorId.ToString()
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
            meeting.MaxParticipant = request.MaxParticipant;
            meeting.Duration = request.Duration;
            meeting.Description = request.Description;

            await dbContext.SaveChangesAsync();

            return Ok(meeting);
        }
        return NotFound();
    }
} 