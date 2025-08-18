namespace TaskManager.Models.Dto
{
    public class RegisterDto
    {
        public int LeaderId { get; set; }
        public string Username { get; set; }
        public string? Email { get; set; }
        public string Password { get; set; }
        public string PasswordAgain { get; set; }
    }
}
