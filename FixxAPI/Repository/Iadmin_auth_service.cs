using FixxAPI.DTOs;
using FixxAPI.Models;

namespace FixxAPI.Repository
{
    public interface Iadmin_auth_service
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
        public Task<IEnumerable<users>> get_all_users();
        public Task<users> update_forgot_password_token(string email, string token);
        public Task<users> get_user_by_phone(string phone);
        public Task<users> get_user_by_token(string token);
        public Task<users> reset_password(reset_password_dto dto, byte[] hash, byte[] salt);
    }
}
