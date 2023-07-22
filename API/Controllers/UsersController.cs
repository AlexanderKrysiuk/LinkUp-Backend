using API.Domain;
using API.DTOs;
using API.Infrastructure;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private DataService _dataService;
        public UsersController(DataService service) { 
            _dataService = service;
        }
        // GET: api/users
        [HttpGet]
        public ActionResult<IEnumerable<UserDTO>> Get()
        {
            return Ok( _dataService.Users.Select(user => new UserDTO() {Id=user.Id, Login=user.Login, UserType=UserDTO.DetermineUserType(user) }));
        }

        // GET api/users/5
        [HttpGet("{id}")]
        public ActionResult<UserDTO> Get(int id)
        {
            IUser user = _dataService.Users.SingleOrDefault(user => user.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(new UserDTO() { Id = user.Id, Login=user.Login, UserType= UserDTO.DetermineUserType(user) });
        }

        // POST api/users
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/users/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/users/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
