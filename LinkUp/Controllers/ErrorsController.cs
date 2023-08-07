using Microsoft.AspNetCore.Mvc;

namespace LinkUp.Controllers;

public class ErrorController : ControllerBase
{
    [Route("/error")]
    protected IActionResult Error()
    {
        return Problem();
    }
}