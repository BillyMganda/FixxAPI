using Amazon.S3.Model;
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

        [Authorize(Roles = "business")]
        [HttpPut]
        public async Task<IActionResult> Put_Property(property_update_dto request)
        {
            try
            {
                var role = _repository.get_logged_in_role();
                if (role != "business")
                    return Unauthorized();
                await _repository.update_properties(request);
                return Ok("property updated successfully");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "internal server error");
            }
        }

        [Authorize(Roles = "business")]
        [HttpDelete]
        public async Task<IActionResult> Put_Property()
        {
            try
            {
                var role = _repository.get_logged_in_role();
                if (role != "business")
                    return Unauthorized();
                await _repository.delete_properties();
                return Ok("property deleted successfully");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "internal server error");
            }
        }

        [HttpPost("images")]
        [Authorize(Roles = "business")]
        public async Task<IActionResult> UploadImages(IFormFile[] files)
        {
            try
            {
                var role = _repository.get_logged_in_role();
                if (role != "business")
                {
                    return Unauthorized();
                }
                // convert IFormFile to list of FileStreams
                List<MemoryStream> memoryStreams = new List<MemoryStream>();
                foreach (var file in files)
                {
                    var stream = new MemoryStream();
                    await file.CopyToAsync(stream);
                    memoryStreams.Add(stream);
                }
                await _repository.UploadImagesToS3Bucket(memoryStreams);
                return Ok("files uploaded");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "internal server error");
            }
        }

        [HttpGet("images")]
        [Authorize(Roles = "business")]
        public async Task<ActionResult<S3Object>> GetS3Images()
        {
            try
            {
                var role = _repository.get_logged_in_role();
                if (role != "business")
                {
                    return Unauthorized();
                }
                else
                {
                    var images = await _repository.GetImagesFromS3Bucket();
                    return Ok(images);
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "internal server error");
            }
        }

        [Authorize(Roles = "business")]
        [HttpGet("amenities")]
        public async Task<IActionResult> Get_Amenities()
        {
            try
            {
                var role = _repository.get_logged_in_role();
                if (role != "business")
                {
                    return Unauthorized();
                }
                var ammenities = await _repository.get_amenities();
                return Ok(ammenities);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "internal server error");
            }
        }

        [Authorize(Roles = "business")]
        [HttpPost("amenities")]
        public async Task<IActionResult> Post_Amenity(amenity_add_dto dto)
        {
            try
            {
                var role = _repository.get_logged_in_role();
                if (role != "business")
                {
                    return Unauthorized();
                }
                var ammenities = await _repository.add_ammenity(dto);
                return Ok("amenities updated succesfully");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "internal server error");
            }
        }
    }
}
