namespace Herd.Business.Models.Commands
{
    public class CreateHerdUserCommand : Command
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordPlainText { get; set; }
    }
}