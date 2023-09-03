using Microsoft.AspNetCore.Mvc;

namespace LinkUpBackend.Controllers
{
    public class ErrorController : ApiController
    {
        [HttpGet("/error")]
        public IActionResult Error()
        {
            return Problem();
        }
    }
}
