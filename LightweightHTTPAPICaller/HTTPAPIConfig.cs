using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightweightHTTPAPICaller {
    public class HTTPAPIConfig {
        public string httpAddress { get; set; }
        public String responseTypeExtension {
            get {
                switch (responseType) {
                    case ResponseType.JSON:
                        return "json";
                    case ResponseType.XML:
                        return "xml";
                    default:
                        return "json";
                }
            }
        }
        public ResponseType responseType {
            get; set;
        }
        public QueryParameters defaultParameters { get; set; }

        public HTTPAPIConfig(String httpAddress) {
            this.httpAddress = httpAddress;
            this.responseType = ResponseType.JSON;
            this.defaultParameters = new QueryParameters();
        }
    }
}
