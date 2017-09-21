namespace Herd.Business.Models.Errors
{
    public class HerdAppSystemError : HerdAppError
    {
        public const string SYSTEM_ERR_TYPE = "SYSTEM";

        public HerdAppSystemError()
        {
            Type = SYSTEM_ERR_TYPE;
        }
    }
}