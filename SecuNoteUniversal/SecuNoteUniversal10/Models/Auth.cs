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

using System.Threading.Tasks;

namespace SecuNoteUniversal10.Models
{
    public enum AuthProvider
    {
        MicrosoftAccount
    }

    /// <summary>
    ///     This class contains all Authentication related code for interacting with the OneNote APIs
    /// </summary>
    /// <remarks>
    ///     Based on the context, we either will authenticate user for O365 or for Live ID
    /// </remarks>
    internal static class Auth
    {
        /// <summary>
        ///     Gets a valid authentication token for the selected Authentication provider
        /// </summary>
        /// <remarks>
        ///     Used by the API request generators before making calls to the OneNote APIs.
        /// </remarks>
        /// <returns>valid authentication token</returns>
        internal static async Task<string> GetAuthToken(AuthProvider provider)
        {
            return await LiveIdAuth.GetAuthToken();
        }

        internal static async Task SignOut(AuthProvider provider)
        {
            await LiveIdAuth.SignOut();
        }

        internal static bool IsSignedIn(AuthProvider provider)
        {
            return LiveIdAuth.IsSignedIn;
        }

        internal static async Task<string> GetUserName(AuthProvider provider)
        {
            return await LiveIdAuth.GetUserName();
        }
    }
}