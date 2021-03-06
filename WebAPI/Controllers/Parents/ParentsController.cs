using System;
using Microsoft.AspNetCore.Mvc;
using Domain.Parents;
using Domain.Users;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace WebAPI.Controllers.Parents
{
    [ApiController]
    [Route("[controller]")]
    public class ParentsController : ControllerBase
    {
        public readonly IParentsService _parentsService;
        public readonly IUsersService _usersService;
        public ParentsController(IUsersService usersService, IParentsService parentsService)
        {
            _parentsService = parentsService;
            _usersService = usersService;
        }
        
        [HttpPost]
        [Authorize(Roles = "School")]
        public IActionResult Create(CreateParentRequest request)
        {
            var response = _parentsService.Create(request.Name, request.CPF, request.PhoneNumber, request.BirthDate, request.Email, request.Registration);

            if (!response.IsValid)
            {
                return BadRequest(response.Errors);
            }

            return Ok(response.Id);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "School")]
        public IActionResult Remove(Guid id)
        {
            var parentRemoved = _parentsService.Remove(id);

            if (!parentRemoved)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetByID(Guid id)
        {
            var parent = _parentsService.Get(id);

            if (parent == null)
            {
                return NotFound();
            }

            return Ok(parent);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll(string name)
        {
            var parents = _parentsService.GetAll();
            
            if (!string.IsNullOrWhiteSpace(name))
            {
                var transformedName = name.ToLower().Trim();
                parents = parents.Where(x => x.Name.ToLower().Contains(transformedName));
            }
            
            return Ok(parents.OrderBy(x => x.Name));
        }
    }
}