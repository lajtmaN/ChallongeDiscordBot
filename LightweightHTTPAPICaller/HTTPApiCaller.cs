
namespace LightweightHTTPAPICaller
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    public class HTTPAPICaller {
        private HTTPAPIConfig config;
        public HTTPAPICaller(HTTPAPIConfig config) {
            this.config = config;
        }

        public async Task<ReturnType> GET<ReturnType>(string path, QueryParameters parameters) {
            using (var client = new HttpClient()) {
                setUpClient(client, parameters);

                try { 
                    HttpResponseMessage response = await client.GetAsync(buildPath(path) + parameters);
                    return await handleResponse<ReturnType>(response, path, parameters);
                } catch (Exception ex) {
                    throw new InvalidRequestException("Invalid request to " + path + " with " + parameters + ":" + ex);
                }
            }
        }

        public async Task<bool> DELETE(string path, QueryParameters parameters) {
            using (var client = new HttpClient()) {
                setUpClient(client, parameters);

                try {
                    HttpResponseMessage response = await client.DeleteAsync(buildPath(path) + parameters);
                    return handleResponseWithoutReturn(response, path, parameters);
                } catch (Exception ex) {
                    throw new InvalidRequestException("Invalid request to " + path + " with " + parameters + ":" + ex);
                }
            }
        }

        public async Task<bool> POST(string path, QueryParameters parameters) {
            using (var client = new HttpClient()) {
                setUpClient(client, parameters);

                try { 
                    FormUrlEncodedContent urlContent = new FormUrlEncodedContent(parameters.parameters);
                    HttpResponseMessage response = await client.PostAsync(buildPath(path), urlContent);
                    return handleResponseWithoutReturn(response, path, parameters);
                } catch (Exception ex) {
                    throw new InvalidRequestException("Invalid request to " + path + " with " + parameters + ":" + ex);
                }
            }
        }

        public async Task<ReturnType> POST<ReturnType>(string path, QueryParameters parameters) {
            using (var client = new HttpClient()) {
                setUpClient(client, parameters);

                try { 
                    FormUrlEncodedContent urlContent = new FormUrlEncodedContent(parameters.parameters);
                    HttpResponseMessage response = await client.PostAsync(buildPath(path), urlContent);
                    return await handleResponse<ReturnType>(response, path, parameters);
                } catch (Exception ex) {
                    throw new InvalidRequestException("Invalid request to " + path + " with " + parameters + ":" + ex);
                }
            }
        }

        public async Task<bool> PUT(string path, QueryParameters parameters) {
            using (var client = new HttpClient()) {
                setUpClient(client, parameters);

                try { 
                    FormUrlEncodedContent urlContent = new FormUrlEncodedContent(parameters.parameters);
                    HttpResponseMessage response = await client.PutAsync(buildPath(path), urlContent);
                    return handleResponseWithoutReturn(response, path, parameters);
                } catch (Exception ex) {
                    throw new InvalidRequestException("Invalid request to " + path + " with " + parameters + ":" + ex);
                }
            }
        }

        public async Task<ReturnType> PUT<ReturnType>(string path, QueryParameters parameters) {
            using (var client = new HttpClient()) {
                setUpClient(client, parameters);

                try { 
                    FormUrlEncodedContent urlContent = new FormUrlEncodedContent(parameters.parameters);
                    HttpResponseMessage response = await client.PutAsync(buildPath(path), urlContent);
                    return await handleResponse<ReturnType>(response, path, parameters);
                } catch (Exception ex) {
                    throw new InvalidRequestException("Invalid request to " + path + " with " + parameters + ":" + ex);
                }
            }
        }

        private string buildPath(string path) {
            return config.httpAddress + path + "." + config.responseTypeExtension;
        }

        private void setUpClient(HttpClient client, QueryParameters parameters) {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            parameters.Add(config.defaultParameters);
        }

        private async Task<ReturnType> handleResponse<ReturnType>(HttpResponseMessage response, string path, QueryParameters parameters) {
            if (response.IsSuccessStatusCode) {
                return await response.Content.ReadAsAsync<ReturnType>();
            } else {
                Console.WriteLine(response);
                Console.WriteLine(parameters);
                throw new CouldNotReceiveResponse("Invalid request to " + path + " with " + parameters);
            }
        }

        private bool handleResponseWithoutReturn(HttpResponseMessage response, string path, QueryParameters parameters) {
            if (response.IsSuccessStatusCode) {
                return true;
            } else {
                Console.WriteLine(response);
                Console.WriteLine(parameters);
                throw new CouldNotReceiveResponse("Invalid request to " + path + " with " + parameters);
            }
        }
    }
}
