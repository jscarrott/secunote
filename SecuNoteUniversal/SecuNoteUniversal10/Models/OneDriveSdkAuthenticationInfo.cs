using System;
using System.Threading.Tasks;
using OneDrive;

namespace SecuNoteUniversal10.Models
{
    internal class OneDriveSdkAuthenticationInfo : IAuthenticationInfo
    {
        public OneDriveSdkAuthenticationInfo(string accessToken, DateTimeOffset expirationTime,
            string refreshToken = null)
        {
            AccessToken = accessToken;
            TokenExpiration = expirationTime;
            TokenType = "Bearer";
            RefreshToken = refreshToken;
        }

        public OneDriveSdkAuthenticationInfo(string token)
        {
            AccessToken = token;
            TokenExpiration = LiveIdAuth.AccessTokenExpiration;
            TokenType = "Bearer";
            RefreshToken = LiveIdAuth.RefreshToken;
        }

        //public string AccessToken
        //{
        //    get { return LiveIdAuth.AccessToken; }
        //    set { LiveIdAuth.AccessToken = value; }
        //}
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public DateTimeOffset TokenExpiration { get; set; }
        public string RefreshToken { get; set; }

        public async Task<bool> RefreshAccessTokenAsync()
        {
            return await LiveIdAuth.RefreshAuthTokenIfNeeded();
        }

        public string AuthorizationHeaderValue => string.Concat(TokenType, " ", AccessToken);
    }
}