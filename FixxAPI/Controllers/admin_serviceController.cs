using AutoMapper;
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
    public class admin_serviceController : ControllerBase
    {
        private readonly Iadmin_service _repository;
        private IMapper _Mapper;
        public admin_serviceController(Iadmin_service repository, IMapper Mapper)
        {
            _repository = repository;
            _Mapper = Mapper;
        }
                
        [HttpPost("property-categories"), Authorize(Roles = "admin")]
        public async Task<IActionResult> PostCategory(string name)
        {
            try
            {
                var role = _repository.get_logged_in_role();
                if (role != "admin")
                    return Unauthorized();
                await _repository.add_property_category(name);
                return Ok("property category added successfully");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "internal server error");
            }
        }

        [HttpGet("property-categories"), AllowAnonymous]
        public async Task<ActionResult> GetCategories()
        {
            try
            {
                var results = await _repository.get_property_categories();
                var mapped_results = _Mapper.Map<List<property_category_return_dto>>(results);
                return Ok(mapped_results);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "internal server error");
            }
        }

        [HttpDelete("property-categories"), Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var role = _repository.get_logged_in_role();
                if (role != "admin")
                    return Unauthorized();
                await _repository.delete_property_category(id);
                return Ok("property category deleted successfully");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "internal server error");
            }
        }

        //property type
        [HttpPost("property-types"), Authorize(Roles = "admin")]
        public async Task<IActionResult> PostType(string name)
        {
            try
            {
                var role = _repository.get_logged_in_role();
                if (role != "admin")
                    return Unauthorized();
                await _repository.add_property_type(name);
                return Ok("property type added successfully");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "internal server error");
            }
        }

        [HttpGet("property-types"), AllowAnonymous]
        public async Task<ActionResult> GetTypes()
        {
            try
            {
                var results = await _repository.get_property_types();
                var mapped_results = _Mapper.Map<List<property_type_return_dto>>(results);
                return Ok(mapped_results);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "internal server error");
            }
        }

        [HttpDelete("property-types"), Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteType(int id)
        {
            try
            {
                var role = _repository.get_logged_in_role();
                if (role != "admin")
                    return Unauthorized();
                await _repository.delete_property_type(id);
                return Ok("property type deleted successfully");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "internal server error");
            }
        }

        //prop_categ_type_amen
        [HttpGet("property-all"), Authorize(Roles = "admin")]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var role = _repository.get_logged_in_role();
                if (role != "admin")
                    return Unauthorized();
                var results = await _repository.get_all_prop_categ_type_amen();                
                return Ok(results);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "internal server error");
            }
        }

        [HttpGet("property-one"), Authorize(Roles = "admin")]
        public async Task<ActionResult> GetOne(Guid id)
        {
            try
            {
                var role = _repository.get_logged_in_role();
                if (role != "admin")
                    return Unauthorized();
                var results = await _repository.get_one_prop_categ_type_amen(id);
                return Ok(results);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "internal server error");
            }
        }
    }
}
