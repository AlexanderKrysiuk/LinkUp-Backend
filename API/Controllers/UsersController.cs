using API.Domain;
using API.DTOs;
using API.Infrastructure;
using API.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserRepository _userRepository;
        public UsersController(IUserRepository repository) { 
            _userRepository = repository;
        }
        // GET: api/users?search=example
        [HttpGet]
        public ActionResult<IEnumerable<UserDTO>> Get([FromQuery] string? search)
        {
            return Ok( _userRepository.GetUsers(search).Select(user => new UserDTO(user)));
        }

        // GET api/users/5
        [HttpGet("{id}")]
        public ActionResult<UserDTO> Get(int id)
        {
            IUser? user = _userRepository.GetUser(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(new UserDTO(user));
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
