namespace Herd.Core.Errors
{
    public class UserError : Error
    {
        public const string USER_ERR_TYPE = "USER";

        public UserError()
        {
            Type = USER_ERR_TYPE;
        }
    }
}