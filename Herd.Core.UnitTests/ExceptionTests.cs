using Herd.Core.Errors;
using Herd.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Herd.Core.UnitTests
{
    public class ExceptionTests
    {
        [Fact]
        public void UserExceptionTest()
        {
            TestExcetionErrorConversion(new UserErrorException("Hello world"), UserError.USER_ERR_TYPE);
            TestExcetionErrorConversion(new UserErrorException("Hello world", new Exception("Hello user")), UserError.USER_ERR_TYPE);
        }

        [Fact]
        public void SystemExceptionTest()
        {
            TestExcetionErrorConversion(new SystemErrorException("Hello world"), SystemError.SYSTEM_ERR_TYPE);
            TestExcetionErrorConversion(new SystemErrorException("Hello world", new Exception("Hello user")), SystemError.SYSTEM_ERR_TYPE);
        }

        private void TestExcetionErrorConversion<T>(ErrorException<T> exception, string expectedErrorType) where T : Error
        {
            var error = exception.Error;
            Assert.NotNull(error);
            Assert.True(error is T);
            Assert.Equal(expectedErrorType, error.Type);
            Assert.Equal(exception.Message, error.Message);
            Assert.True(error.Id > 0);
            var oldID = error.Id;
            Assert.Equal(oldID, exception.Error.Id);
        }
    }
}
