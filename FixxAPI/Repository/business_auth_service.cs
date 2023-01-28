using FixxAPI.DTOs;
using FixxAPI.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using FixxAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FixxAPI.Repository
{
    public class business_auth_service : Ibusiness_auth_service
    {
        public readonly Data_Context _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _HttpContextAccessor;
        public business_auth_service(Data_Context context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _configuration = configuration;
            _HttpContextAccessor = httpContextAccessor;
        }
        public void create_password_hash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public bool verify_password_hash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computed_Hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computed_Hash.SequenceEqual(passwordHash);
            }
        }

        public string create_jwt_token(login_dto dto)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, dto.email),
                new Claim(ClaimTypes.Role, "business")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Token").Value!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken
                (
                claims: claims,
                expires: DateTime.Now.AddHours(5),
                signingCredentials: creds
                );
            string jwt_token = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt_token;
        }

        public string forgot_pasword_token()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public void send_registration_email(string email, string fname)
        {
            string email_from = _configuration.GetSection("EmailSettings:Email_From").Value!;
            string port = _configuration.GetSection("EmailSettings:SMTP_Port").Value!;
            string smtp_server = _configuration.GetSection("EmailSettings:SMTP_Host").Value!;
            string smtp_username = _configuration.GetSection("EmailSettings:SMTP_User").Value!;
            string smtp_password = _configuration.GetSection("EmailSettings:SMTP_Password").Value!;


            MailMessage message = new MailMessage();
            SmtpClient smtp = new SmtpClient();
            message.From = new MailAddress(email_from);
            message.To.Add(new MailAddress(email));
            message.Subject = "Business Registration";
            message.IsBodyHtml = true;
            message.Body = $"Dear <b>{fname}</b>, <br>Thank you for registering your business to Fixx.com.";
            smtp.Port = Convert.ToInt32(port);
            smtp.Host = smtp_server;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(smtp_username, smtp_password);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Send(message);
        }

        public void send_forgot_password_email(string email, string token)
        {
            string email_from = _configuration.GetSection("EmailSettings:Email_From").Value!;
            string port = _configuration.GetSection("EmailSettings:SMTP_Port").Value!;
            string smtp_server = _configuration.GetSection("EmailSettings:SMTP_Host").Value!;
            string smtp_username = _configuration.GetSection("EmailSettings:SMTP_User").Value!;
            string smtp_password = _configuration.GetSection("EmailSettings:SMTP_Password").Value!;

            MailMessage message = new MailMessage();
            SmtpClient smtp = new SmtpClient();
            message.From = new MailAddress(email_from);
            message.To.Add(new MailAddress(email));
            message.Subject = "Forgot Password";
            message.IsBodyHtml = true;
            message.Body = $"Dear <b>{email}</b>, <br>a request to reset your password has been made on your account, use this token <b>{token}</b> to reset your password. Contact us if yo did not initiate this request.";
            smtp.Port = Convert.ToInt32(port);
            smtp.Host = smtp_server;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(smtp_username, smtp_password);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Send(message);
        }

        public string send_forgot_password_sms(string to_number, string token)
        {
            string account_sid = _configuration.GetSection("Twilio:AccountSID").Value!;
            string auth_token = _configuration.GetSection("Twilio:AuthToken").Value!;
            string from_number = _configuration.GetSection("Twilio:SMS_Number").Value!;

            TwilioClient.Init(account_sid, auth_token);

            var message = MessageResource.Create(
            body: $"Your password reset code is : {token}",
            from: new Twilio.Types.PhoneNumber(from_number),
            to: new Twilio.Types.PhoneNumber(to_number)
            );

            return message.Sid;
        }

        public async Task<users> save_user_business(user_add_dto dto)
        {
            create_password_hash(dto.password, out byte[] passwordHash, out byte[] passwordSalt);

            var new_user = new users
            {
                first_name = dto.first_name,
                last_name = dto.last_name,
                phone_number = dto.phone_number,
                email = dto.email,
                address = dto.address,
                city = dto.city,
                state = dto.state,
                zip_code = dto.zip_code,
                password_hash = passwordHash,
                password_salt = passwordSalt,
                created_on = DateTime.Now,
                role = "business",
                terms_agreed = dto.terms_agreed,
            };
            var user = _context.users.Add(new_user);
            await _context.SaveChangesAsync();
            return user.Entity;
        }

        public async Task<users> get_user_by_email(string email)
        {
            var result = await _context.users.FirstOrDefaultAsync(x => x.email == email);
            return result!;
        }
    }
}
