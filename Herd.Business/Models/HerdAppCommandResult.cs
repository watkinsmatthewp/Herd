using Herd.Business.Models.CommandResultData;
using Herd.Business.Models.Errors;
using System.Collections.Generic;
using System.Linq;

namespace Herd.Business.Models
{
    public class HerdAppCommandResult
    {
        private List<Error> _errors = new List<Error>();

        public List<Error> Errors
        {
            get => _errors;
            set => _errors = value ?? new List<Error>();
        }

        public IEnumerable<SystemError> SystemErrors => Errors.OfType<SystemError>();
        public IEnumerable<UserError> UserErrors => Errors.OfType<UserError>();
        public bool HasSystemErrors => SystemErrors.Any();
        public bool HasUserErrors => SystemErrors.Any();
        public bool Success => Errors.Count == 0;
    }

    public class HerdAppCommandResult<T> : HerdAppCommandResult where T : CommandResultData.CommandResultData
    {
        public T Data { get; set; }
    }
}