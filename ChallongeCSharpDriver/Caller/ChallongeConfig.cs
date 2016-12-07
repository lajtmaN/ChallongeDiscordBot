using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChallongeCSharpDriver.Caller {
    using LightweightHTTPAPICaller;
    public class ChallongeConfig {
        private String apiKey { get; set; }
        public HTTPAPIConfig config { get; private set; }
        public ChallongeConfig(String apiKey) {
            this.apiKey = apiKey;

            QueryParameters defaultParameters = new QueryParameters();
            defaultParameters.Add("api_key", apiKey);
            this.config = new HTTPAPIConfig("https://api.challonge.com/v1/") {
                defaultParameters = defaultParameters, 
                responseType = ResponseType.JSON 
            };
        }
    }
}
