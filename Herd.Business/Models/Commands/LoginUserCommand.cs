namespace Herd.Business.Models.Commands
{
    public class LoginUserCommand : HerdAppCommand
    {
        public string Email { get; set; }
        public string PasswordPlainText { get; set; }
    }
}