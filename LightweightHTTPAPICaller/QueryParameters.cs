using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace LightweightHTTPAPICaller {
    public class QueryParameters {
        public List<KeyValuePair<string, string>> parameters {
            private set;
            get;
        }
        public bool hasParameters {
            get {
                return parameters.Count > 0;
            }
        }

        public QueryParameters() {
            parameters = new List<KeyValuePair<string, string>>();
        }

        public void Add(string key, string value) {
            parameters.Add(new KeyValuePair<string, string>(key, value));
        }

        public void Add(QueryParameters otherParameters) {
            otherParameters.parameters.ForEach(entry => Add(entry.Key, entry.Value));
        }

        public override string ToString() {
            List<string> list = new List<string>();
            foreach (KeyValuePair<string, string> entry in parameters) {
                list.Add(entry.Key + "=" + entry.Value);
            }
            return "?" + String.Join("&", list);
        }
    }
}