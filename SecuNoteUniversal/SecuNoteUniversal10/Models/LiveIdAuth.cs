﻿//*********************************************************
// Copyright (c) Microsoft Corporation
// All rights reserved. 
//
// Licensed under the Apache License, Version 2.0 (the ""License""); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
// http://www.apache.org/licenses/LICENSE-2.0 
//
// THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS 
// OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED 
// WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR 
// PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 
//
// See the Apache Version 2.0 License for specific language 
// governing permissions and limitations under the License.
//*********************************************************

using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.OnlineId;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SecuNoteUniversal10.Models
{
    // <summary>
    /// This class contains all Authentication related code for interacting with the OneNote APIs
    /// </summary>
    /// <remarks>
    ///     In our previous github WinStore and WinPhone Code samples we demonstrated how to use the
    ///     LiveSDK to do OAuth against the Microsoft Account service.
    ///     For this Windows 8.1 Universal code sample, we'll demonstrate an alternative way to do
    ///     OAuth using the new Windows.Security.Authentication.OnlineId.OnlineIdauthenticator
    ///     class.
    ///     http://msdn.microsoft.com/en-us/library/windows/apps/windows.security.authentication.onlineid.onlineidauthenticator.aspx
    ///     Both the existing Live SDK approach and this alternative will work in Windows 8.1 universal apps
    ///     NOTE: The usage of the OnlineIdAuthenticator class is based on the Windows universal code sample at
    ///     http://code.msdn.microsoft.com/windowsapps/Windows-account-authorizati-7c95e284
    /// </remarks>
    internal class LiveIdAuth
    {
        // TODO: Replace the below ClientId with your app's ClientId.
        // For more info, see: http://msdn.microsoft.com/en-us/library/office/dn575426(v=office.15).aspx
        private const string ClientId = "000000004C15065A";
        // OneNote APIs support multiple OAuth scopes.
        // As a guideline, always choose the least permissible 'office.onenote*' scope that your app needs.
        // Since this code sample demonstrates multiple aspects of the APIs, it uses the most
        // permissible scope: office.onenote_update.
        // TODO: Replace the below scopes with the least permissions your app needs
        private const string Scopes = "wl.signin onedrive.readwrite onedrive.appfolder";
        private const string LiveApiMeUri = "https://apis.live.net/v5.0/me?access_token=";
        private static readonly OnlineIdAuthenticator Authenticator = new OnlineIdAuthenticator();
        private static string _accessToken;

        public static string AccessToken
        {
            get { return _accessToken; }
            set
            {
                if (AccessToken == value) return;
                _accessToken = value;
            }
        }

        internal static bool IsSignedIn
        {
            //TODO Add signout option to charm bar to then renable CanSignOut
            get { return Authenticator != null /*&& Authenticator.CanSignOut*/&& _accessToken != null; }
        }

        /// <summary>
        ///     Gets a valid authentication token. Also refreshes the access token if it has expired.
        /// </summary>
        /// <remarks>
        ///     Used by the API request generators before making calls to the OneNote APIs.
        /// </remarks>
        /// <returns>valid authentication token</returns>
        internal static async Task<string> GetAuthToken()
        {
            if (string.IsNullOrWhiteSpace(_accessToken))
            {
                try
                {
                    var serviceTicketRequest = new OnlineIdServiceTicketRequest(Scopes, "DELEGATION");
                    var result =
                        await
                            Authenticator.AuthenticateUserAsync(new[] {serviceTicketRequest},
                                CredentialPromptType.PromptIfNeeded);
                    if (result.Tickets[0] != null)
                    {
                        _accessToken = result.Tickets[0].Value;
                        AccessTokenExpiration = DateTimeOffset.UtcNow.AddMinutes(AccessTokenApproxExpiresInMinutes);
                    }
                }
                catch (Exception ex)
                {
                    // Authentication failed
                    if (Debugger.IsAttached)
                        Debugger.Break();
                }
            }
            await RefreshAuthTokenIfNeeded();
            return _accessToken;
        }

        internal static async Task SignOut()
        {
            if (IsSignedIn)
            {
                _accessToken = null;
                await Authenticator.SignOutUserAsync();
            }
        }

        internal static async Task<string> GetUserName()
        {
            var uri = new Uri(LiveApiMeUri + await GetAuthToken());
            var client = new HttpClient();
            var result = await client.GetAsync(uri);

            var jsonUserInfo = await result.Content.ReadAsStringAsync();
            if (jsonUserInfo != null)
            {
                var json = JObject.Parse(jsonUserInfo);
                return json["name"].ToString();
            }
            return string.Empty;
        }

        #region RefreshToken related code

        // Collateral used to refresh access token (only applicable when the app uses the wl.offline_access wl.signin scopes)
        public static DateTimeOffset AccessTokenExpiration { get; private set; }

        public static string RefreshToken { get; private set; }

        private const int AccessTokenApproxExpiresInMinutes = 59;

        private const string MsaTokenRefreshUrl = "https://login.live.com/oauth20_token.srf";
        private const string TokenRefreshContentType = "application/x-www-form-urlencoded";
        private const string TokenRefreshRedirectUri = "https://login.live.com/oauth20_desktop.srf";

        private const string TokenRefreshRequestBody =
            "client_id={0}&redirect_uri={1}&grant_type=refresh_token&refresh_token={2}";

        /// <summary>
        ///     Refreshes the live authentication access token if it has expired
        /// </summary>
        public static async Task<bool> RefreshAuthTokenIfNeeded()
        {
            var refreshed = false;
            if (AccessTokenExpiration.CompareTo(DateTimeOffset.UtcNow) <= 0)
            {
                await AttemptAccessTokenRefresh();
                refreshed = true;
            }
            return refreshed;
        }

        /// <summary>
        ///     This method tries to refresh the access token. The token needs to be
        ///     refreshed continuously, so that the user is not prompted to sign in again
        /// </summary>
        /// <returns></returns>
        private static async Task AttemptAccessTokenRefresh()
        {
            var createMessage = new HttpRequestMessage(HttpMethod.Post, MsaTokenRefreshUrl)
            {
                Content = new StringContent(
                    string.Format(CultureInfo.InvariantCulture, TokenRefreshRequestBody,
                        ClientId,
                        TokenRefreshRedirectUri,
                        RefreshToken),
                    Encoding.UTF8,
                    TokenRefreshContentType)
            };

            var httpClient = new HttpClient();
            var response = await httpClient.SendAsync(createMessage);
            await ParseRefreshTokenResponse(response);
        }

        /// <summary>
        ///     Handle the RefreshToken response
        /// </summary>
        /// <param name="response">The HttpResponseMessage from the TokenRefresh request</param>
        private static async Task ParseRefreshTokenResponse(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                dynamic responseObject = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
                _accessToken = responseObject.access_token;
                AccessTokenExpiration = AccessTokenExpiration.AddSeconds((double) responseObject.expires_in);
                RefreshToken = responseObject.refresh_token;
            }
        }

        #endregion
    }
}