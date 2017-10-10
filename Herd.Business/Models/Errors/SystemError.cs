namespace Herd.Business.Models.Errors
{
    public class SystemError : Error
    {
        public const string SYSTEM_ERR_TYPE = "SYSTEM";

        public SystemError()
        {
            Type = SYSTEM_ERR_TYPE;
        }
    }
}