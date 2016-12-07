
namespace ChallongeCSharpDriver.Caller {
    using LightweightHTTPAPICaller;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using ChallongeCSharpDriver.Core;

    public class ChallongeHTTPClientAPICaller : HTTPAPICaller, ChallongeAPICaller {
        private ChallongeConfig config;

        public ChallongeHTTPClientAPICaller(ChallongeConfig config) : base(config.config) {
            this.config = config;
        }

        public Task<ReturnType> GET<ReturnType>(string path, ChallongeQueryParameters parameters) {
            return base.GET<ReturnType>(path, parameters);
        }

        public Task<bool> DELETE(string path, ChallongeQueryParameters parameters) {
            return base.PUT(path, parameters);
        }

        public Task<bool> PUT(string path, ChallongeQueryParameters parameters) {
            return base.PUT(path, parameters);
        }

        public Task<ReturnType> PUT<ReturnType>(string path, ChallongeQueryParameters parameters) {
            return base.PUT<ReturnType>(path, parameters);
        }

        public Task<bool> POST(string path, ChallongeQueryParameters parameters) {
            return base.POST(path, parameters);
        }

        public Task<ReturnType> POST<ReturnType>(string path, ChallongeQueryParameters parameters) {
            return base.POST<ReturnType>(path, parameters);
        }
    }
}
