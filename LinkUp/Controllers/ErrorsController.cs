using Microsoft.AspNetCore.Mvc;

namespace LinkUp.Controllers;

public class ErrorController : ControllerBase
{
    [Route("/error")]
    //[HttpGet]
    public IActionResult Error()
    {
        return Problem();
    }
}