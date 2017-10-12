using Herd.Core.Errors;
using System.Collections.Generic;
using System.Linq;

namespace Herd.Business.Models
{
    public class CommandResult
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
        public bool HasUserErrors => UserErrors.Any();
        public bool Success => Errors.Count == 0;
    }

    public class CommandResult<T> : CommandResult where T : CommandResultDataObject
    {
        public T Data { get; set; }
    }
}