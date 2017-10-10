namespace Herd.Business.Models.Commands
{
    public class LoginUserCommand : Command
    {
        public string Email { get; set; }
        public string PasswordPlainText { get; set; }
    }
}