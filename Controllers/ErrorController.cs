using Microsoft.AspNetCore.Mvc;

namespace LinkUpBackend.Controllers
{
    public class ErrorController : ControllerBase
    {
        [HttpGet("/error")]
        public IActionResult Error()
        {
            return Problem();
        }
    }
}
