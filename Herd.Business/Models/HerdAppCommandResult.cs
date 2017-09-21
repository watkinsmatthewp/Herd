using Herd.Business.Models.Errors;
using System.Collections.Generic;
using System.Linq;

namespace Herd.Business.Models
{
    public class HerdAppCommandResult
    {
        private List<HerdAppError> _errors = new List<HerdAppError>();

        public List<HerdAppError> Errors
        {
            get => _errors;
            set => _errors = value ?? new List<HerdAppError>();
        }

        public IEnumerable<HerdAppSystemError> SystemErrors => Errors.OfType<HerdAppSystemError>();
        public IEnumerable<HerdAppUserError> UserErrors => Errors.OfType<HerdAppUserError>();
        public bool HasSystemErrors => SystemErrors.Any();
        public bool HasUserErrors => SystemErrors.Any();
        public bool Success => Errors.Count == 0;
    }

    public class HerdAppCommandResult<T> : HerdAppCommandResult where T : HerdAppCommandResultData
    {
        public T Data { get; set; }
    }
}