using Microsoft.AspNetCore.Mvc;

namespace LinkUp.Controllers;

public class ErrorController : ControllerBase
{
    [Route("/error")]
    public IActionResult Error()
    {
        return Problem();
    }
}