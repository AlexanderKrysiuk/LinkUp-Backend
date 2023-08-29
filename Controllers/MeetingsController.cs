using LinkUpBackend.Infrastructure;
using Microsoft.AspNetCore.Mvc;
namespace Meetings.Controllers;
[ApiController]
[Route("api/[controller]")]
public class MeetingsController : Controller{
    private readonly MeetingsAPIDbContext dbContext;
    public MeetingsController(MeetingsAPIDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    [HttpGet]
    public IActionResult GetMeetings()
    {
        return Ok(dbContext.Meetings.ToList());
    }
} 