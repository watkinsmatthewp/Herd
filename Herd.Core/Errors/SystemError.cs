namespace Herd.Core.Errors
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