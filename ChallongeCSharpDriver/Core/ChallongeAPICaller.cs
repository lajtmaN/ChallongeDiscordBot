using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallongeCSharpDriver.Core {
    using ChallongeCSharpDriver.Caller;
    using System.Net.Http;

    public interface ChallongeAPICaller {
        Task<ReturnType> GET<ReturnType>(string path, ChallongeQueryParameters parameters);
        Task<bool> DELETE(string path, ChallongeQueryParameters parameters);
        Task<bool> POST(string path, ChallongeQueryParameters parameters);
        Task<ReturnType> POST<ReturnType>(string path, ChallongeQueryParameters parameters);
        Task<bool> PUT(string path, ChallongeQueryParameters parameters);
        Task<ReturnType> PUT<ReturnType>(string path, ChallongeQueryParameters parameters);
    }
}
