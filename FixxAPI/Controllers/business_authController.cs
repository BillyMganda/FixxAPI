using FixxAPI.DTOs;
using FixxAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FixxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class business_authController : ControllerBase
    {
        private readonly Ibusiness_auth_service _repository;
        public business_authController(Ibusiness_auth_service repository)
        {
            _repository = repository;
        }

        [HttpPost("register"), AllowAnonymous]
        public async Task<IActionResult> Register(user_add_dto request)
        {
            try
            {
                var user = await _repository.get_user_by_email(request.email);
                if (user != null)
                {
                    return BadRequest("user registered, please log in to use the service");
                }
                else
                {
                    _repository.send_registration_email(request.email, request.first_name);
                    await _repository.save_user_business(request);
                    return Ok("user registered successfully");
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "internal server error");
            }
        }
    }
}
