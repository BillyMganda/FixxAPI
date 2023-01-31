using FixxAPI.DTOs;
using FixxAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace FixxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("fixx_cors")]
    public class propertiesController : ControllerBase
    {
        private readonly Iproperty_service _repository;
        public propertiesController(Iproperty_service service)
        {
            _repository = service;
        }

        [Authorize(Roles = "business")]
        [HttpGet]
        public async Task<IActionResult> Get_Property()
        {
            try
            {
                var role = _repository.get_logged_in_role();
                if(role != "business")
                    return Unauthorized();
                var property = await _repository.get_property_by_user();
                if(property == null)
                    return NotFound();
                return Ok(property);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "internal server error");
            }
        }

        [Authorize(Roles = "business")]
        [HttpPost]
        public async Task<IActionResult> Post_Property(property_create_dto request)
        {
            try
            {
                var prop = await _repository.get_property_by_user();
                if (prop != null)
                    return BadRequest("property available for business");
                var property = await _repository.add_property_initial(request);
                return Ok("property added successfully");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "internal server error");
            }
        }
    }
}
