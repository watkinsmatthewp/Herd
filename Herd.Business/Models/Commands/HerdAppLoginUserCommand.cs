namespace Herd.Business.Models.Commands
{
    public class HerdAppLoginUserCommand : HerdAppCommand
    {
        public string Email { get; set; }
        public string PasswordPlainText { get; set; }
    }
}