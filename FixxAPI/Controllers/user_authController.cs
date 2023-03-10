using FixxAPI.DTOs;
using FixxAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FixxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class user_authController : ControllerBase
    {
        private readonly Iuser_auth_service _repository;
        public user_authController(Iuser_auth_service service)
        {
            _repository = service;
        }

        [HttpPost("register"), AllowAnonymous]
        public async Task<IActionResult> Register(user_add_dto request)
        {
            try
            {
                var user_email = await _repository.get_user_by_email(request.email);
                var user_phone = await _repository.get_user_by_phone(request.phone_number);
                if (user_email != null || user_phone != null)
                {
                    return BadRequest("phone or email registered, please log in to use our service");
                }
                else
                {
                    await _repository.save_user(request);
                    _repository.send_registration_email(request.email, request.first_name);
                    return Ok("user registered successfully");
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "internal server error");
            }
        }

        [HttpPost("login"), AllowAnonymous]
        public async Task<IActionResult> Login(login_dto request)
        {
            try
            {
                var user = await _repository.get_user_by_email(request.email);
                if (user == null)
                {
                    return NotFound("user not found, please register to use our service");
                }
                bool istrue = _repository.verify_password_hash(request.password, user.password_hash, user.password_salt);
                if (!istrue)
                {
                    return BadRequest("incorrect email or password");
                }

                string jwt_token = _repository.create_jwt_token(request);
                return Ok(jwt_token);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "internal server error");
            }
        }

        [HttpPost("forgot-password-email"), AllowAnonymous]
        public async Task<IActionResult> ForgotPasswordEmail(string email)
        {
            try
            {
                var user = await _repository.get_user_by_email(email);
                if (user == null || user.forgot_password_token != "")
                {
                    return NotFound("unknown error, please try again later");
                }
                else
                {
                    string token = _repository.forgot_pasword_token();
                    await _repository.update_forgot_password_token(email, token);
                    _repository.send_forgot_password_email(email, token);
                    return Ok("check your email for password reset code");
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "internal server error");
            }
        }

        [HttpPost("forgot-password-sms"), AllowAnonymous]
        public async Task<IActionResult> ForgotPasswordSMS(string phone)
        {
            try
            {
                var user = await _repository.get_user_by_phone(phone);
                if (user == null || user.forgot_password_token != "")
                {
                    return NotFound("unknown error, please try again later");
                }
                else
                {
                    string token = _repository.forgot_pasword_token();
                    await _repository.update_forgot_password_token(user.email, token);
                    _repository.send_forgot_password_sms(phone, token);
                    return Ok("check your phone for password reset code");
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "internal server error");
            }
        }

        [HttpPost("reset-password"), AllowAnonymous]
        public async Task<IActionResult> ResetPassword(reset_password_dto request)
        {
            try
            {
                var user = await _repository.get_user_by_token(request.token);
                if (user == null)
                {
                    return NotFound("token not found");
                }
                _repository.create_password_hash(request.new_password, out byte[] hash_, out byte[] salt_);
                var res = await _repository.reset_password(request, hash_, salt_);
                if (res == null)
                {
                    return BadRequest("invalid token");
                }
                else
                {
                    return Ok("password reset successful");
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "internal server error");
            }
        }
    }
}
