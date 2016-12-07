using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightweightHTTPAPICaller {
    public class InvalidRequestException : Exception {
        public InvalidRequestException(String message)
            : base(message) {

        }
    }
}
