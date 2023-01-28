using FixxAPI.DTOs;
using FixxAPI.Models;

namespace FixxAPI.Repository
{
    public interface Ibusiness_auth_service
    {
        public void create_password_hash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        public bool verify_password_hash(string password, byte[] passwordHash, byte[] passwordSalt);
        public string create_jwt_token(login_dto dto);
        public string forgot_pasword_token();
        public void send_registration_email(string email, string fname);
        public void send_forgot_password_email(string email, string token);
        public string send_forgot_password_sms(string to_number, string token);
        public Task<users> save_user_business(user_add_dto dto);
        public Task<users> get_user_by_email(string email);        
    }
}
