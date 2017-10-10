using System;

namespace Herd.Web.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class AuthenticationNotRequiredAttribute : Attribute
    {
    }
}