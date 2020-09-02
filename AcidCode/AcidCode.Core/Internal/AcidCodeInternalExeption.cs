using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcidCode.Core
{
    class AcidCodeInternalExeption : ApplicationException
    {
        private string _codeExeptionMessage;
        private Exception _exception;

        public AcidCodeInternalExeption(Exception exception)
        {
            _exception = exception;

            if (_exception != null)
            {
                if (_exception.InnerException != null)
                {
                    _codeExeptionMessage = _exception.InnerException.Message;
                }
            }

        }

        public string CodeExeptionMessage { get => _codeExeptionMessage; }
    }
}
