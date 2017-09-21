namespace Herd.Business.Models.Errors
{
    public class HerdAppUserError : HerdAppError
    {
        public const string USER_ERR_TYPE = "USER";

        public HerdAppUserError()
        {
            Type = USER_ERR_TYPE;
        }
    }
}