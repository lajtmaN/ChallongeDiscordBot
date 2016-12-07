using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightweightHTTPAPICaller {
    public class CouldNotReceiveResponse : Exception {
        public CouldNotReceiveResponse(string message)
            : base(message) {

        }
    }
}
