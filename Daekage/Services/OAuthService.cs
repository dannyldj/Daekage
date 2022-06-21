using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Daekage.Core.Models;
using Daekage.Helpers;
using Newtonsoft.Json;

namespace Daekage.Services
{
    public class OAuthService
    {
        private const string CLIENT_ID = "245157208637-1pqq50c8tc2r3opji66jtqj1vb3incm2.apps.googleusercontent.com";
        private const string CLIENT_SECRET = "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";
        private const string AUTHORIZATION_ENDPOINT = "https://accounts.google.com/o/oauth2/v2/auth";
        private const string TOKEN_REQUEST_URI = "https://www.googleapis.com/oauth2/v4/token";

        public OAuthService()
        {
        }

        // ref http://stackoverflow.com/a/3978040
        private static int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        public static async Task GoogleAuth()
        {
            // Generates state and PKCE values.
            string state = UriEncoding.RandomDataBase64Url(32);
            string codeVerifier = UriEncoding.RandomDataBase64Url(32);
            string codeChallenge = UriEncoding.Base64UrlencodeNoPadding(UriEncoding.Sha256(codeVerifier));
            const string codeChallengeMethod = "S256";

            // Creates a redirect URI using an available port on the loopback address.
            var redirectUri = $"http://{IPAddress.Loopback}:{GetRandomUnusedPort()}/";

            // Creates an HttpListener to listen for requests on that redirect URI.
            var http = new HttpListener();
            http.Prefixes.Add(redirectUri);
            http.Start();

            // Creates the OAuth 2.0 authorization request.
            string authorizationRequest = string.Format("{0}?response_type=code&scope=openid%20profile%20email&redirect_uri={1}&client_id={2}&state={3}&code_challenge={4}&code_challenge_method={5}",
                AUTHORIZATION_ENDPOINT,
                Uri.EscapeDataString(redirectUri),
                CLIENT_ID,
                state,
                codeChallenge,
                codeChallengeMethod);

            // Opens request in the browser.
            var psi = new ProcessStartInfo()
            {
                FileName = authorizationRequest,
                UseShellExecute = true
            };
            Process.Start(psi);

            // Waits for the OAuth authorization response.
            var context = await http.GetContextAsync();

            // Sends an HTTP response to the browser.
            var response = context.Response;
            const string responseString = "<html><head><meta http-equiv='refresh' content='10;url=https://google.com'></head><body>Please return to the app.</body></html>";
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            var responseOutput = response.OutputStream;
            _ = responseOutput.WriteAsync(buffer, 0, buffer.Length).ContinueWith((task) =>
            {
                responseOutput.Close();
                http.Stop();
            });

            // Checks for errors.
            if (context.Request.QueryString.Get("error") != null)
            {
                throw new Exception($"OAuth authorization error: {context.Request.QueryString.Get("error")}.");
            }
            if (context.Request.QueryString.Get("code") == null
                || context.Request.QueryString.Get("state") == null)
            {
                throw new Exception("Malformed authorization response. " + context.Request.QueryString);
            }

            string code = context.Request.QueryString.Get("code");
            string incomingState = context.Request.QueryString.Get("state");

            // Compares the receieved state to the expected value, to ensure that
            // this app made the request which resulted in authorization.
            if (incomingState != state)
            {
                throw new Exception($"Received request with invalid state ({incomingState})");
            }

            await GetToken(code, codeVerifier, redirectUri);
            await UserinfoCall();
        }

        public static async Task UserinfoCall()
        {
            if(Application.Current.Properties["AccessToken"] is null) return;
            Application.Current.Properties["Userinfo"] = null;

            // builds the  request
            const string userinfoRequestUri = "https://www.googleapis.com/oauth2/v3/userinfo";

            // sends the request
            var userinfoRequest = (HttpWebRequest)WebRequest.Create(userinfoRequestUri);
            userinfoRequest.Method = "GET";
            userinfoRequest.Headers.Add($"Authorization: Bearer {Application.Current.Properties["AccessToken"]}");
            userinfoRequest.ContentType = "application/x-www-form-urlencoded";
            userinfoRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

            try
            {
                // gets the response
                var userinfoResponse = await userinfoRequest.GetResponseAsync();
                using var userinfoResponseReader = new StreamReader(userinfoResponse.GetResponseStream()!);
                // reads response body
                string userinfoResponseText = await userinfoResponseReader.ReadToEndAsync();

                Application.Current.Properties["Userinfo"] = JsonConvert.DeserializeObject<UserinfoModel>(userinfoResponseText);
            }
            catch (WebException)
            {
                await RefreshToken();
            }
        }

        private static async Task GetToken(string code, string codeVerifier, string redirectUri)
        {
            // builds the  request
            string tokenRequestBody = string.Format("code={0}&redirect_uri={1}&client_id={2}&code_verifier={3}&client_secret={4}&scope=&grant_type=authorization_code",
                code,
                Uri.EscapeDataString(redirectUri),
                CLIENT_ID,
                codeVerifier,
                CLIENT_SECRET);

            // sends the request
            var tokenRequest = (HttpWebRequest)WebRequest.Create(TOKEN_REQUEST_URI);
            tokenRequest.Method = "POST";
            tokenRequest.ContentType = "application/x-www-form-urlencoded";
            tokenRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            byte[] byteVersion = Encoding.ASCII.GetBytes(tokenRequestBody);
            tokenRequest.ContentLength = byteVersion.Length;
            var stream = tokenRequest.GetRequestStream();
            await stream.WriteAsync(byteVersion, 0, byteVersion.Length);
            stream.Close();

            try
            {
                // gets the response
                var tokenResponse = await tokenRequest.GetResponseAsync();

                using var reader = new StreamReader(tokenResponse.GetResponseStream()!);
                // reads response body
                string responseText = await reader.ReadToEndAsync();

                // converts to dictionary
                var tokenEndpointDecoded = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseText);

                Application.Current.Properties["AccessToken"] = tokenEndpointDecoded?["access_token"];
                if (!string.IsNullOrEmpty(tokenEndpointDecoded?["refresh_token"]))
                    Application.Current.Properties["RefreshToken"] = tokenEndpointDecoded["refresh_token"];
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    if (ex.Response is HttpWebResponse response)
                    {
                        using var reader = new StreamReader(response.GetResponseStream()!);
                        // reads response body
                        string responseText = await reader.ReadToEndAsync();
                        throw new Exception("HTTP: " + response.StatusCode + "\n" + responseText);
                    }
                }
            }
        }

        private static async Task RefreshToken()
        {
            Application.Current.Properties["AccessToken"] = string.Empty;

            // builds the  request
            string tokenRequestBody = string.Format("client_id={0}&client_secret={1}&refresh_token={2}&scope=&grant_type=refresh_token",
                CLIENT_ID,
                CLIENT_SECRET,
                Application.Current.Properties["RefreshToken"]);

            // sends the request
            var tokenRequest = (HttpWebRequest)WebRequest.Create(TOKEN_REQUEST_URI);
            tokenRequest.Method = "POST";
            tokenRequest.ContentType = "application/x-www-form-urlencoded";
            tokenRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            byte[] byteVersion = Encoding.ASCII.GetBytes(tokenRequestBody);
            tokenRequest.ContentLength = byteVersion.Length;
            var stream = tokenRequest.GetRequestStream();
            await stream.WriteAsync(byteVersion, 0, byteVersion.Length);
            stream.Close();

            try
            {
                // gets the response
                var tokenResponse = await tokenRequest.GetResponseAsync();

                using var reader = new StreamReader(tokenResponse.GetResponseStream()!);
                // reads response body
                string responseText = await reader.ReadToEndAsync();

                // converts to dictionary
                var tokenEndpointDecoded = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseText);
                string accessToken = tokenEndpointDecoded?["access_token"];

                if (!string.IsNullOrEmpty(accessToken))
                {
                    Application.Current.Properties["AccessToken"] = accessToken;
                    await UserinfoCall();
                    return;
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    if (ex.Response is HttpWebResponse response)
                    {
                        using var reader = new StreamReader(response.GetResponseStream()!);
                        // reads response body
                        string responseText = await reader.ReadToEndAsync();
                        throw new Exception("HTTP: " + response.StatusCode + "\n" + responseText);
                    }
                }
            }

            Application.Current.Properties.Remove("Userinfo");
            Application.Current.Properties.Remove("RefreshToken");
        }
    }
}
