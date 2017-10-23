namespace Herd.Business.Models.Commands
{
    public class LoginHerdUserCommand : Command
    {
        public string Email { get; set; }
        public string PasswordPlainText { get; set; }
    }
}