﻿using API.Domain;
using API.utils.validators;
using API.DTOs;
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

        // POST api/users/signup
        [HttpPost("signup")]
        public ActionResult<UserDTO> Post([FromBody] UserRegistrationDTO userData)
        {
            ValidationResult validationResult = UserValidator.ValidateRegistrationData(userData, _userRepository.GetUsers(null).ToList());
            if (validationResult.Success)
            {
                IUser addedUser = _userRepository.CreateUser(userData);
                return CreatedAtAction(nameof(Get), new { id = addedUser.Id },new UserDTO(addedUser));
            }
            switch (validationResult.Code)
            {
                case 400: return BadRequest(validationResult.Message);
                case 401: return Unauthorized(validationResult.Message);
                case 409: return Conflict(validationResult.Message);
                default: throw new Exception(validationResult.Message);
            }
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
