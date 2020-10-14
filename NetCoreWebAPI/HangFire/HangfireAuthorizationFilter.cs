using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using NetCoreWebAPI.Common;
using NetCoreWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreWebAPI.HangFire
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private string Login = ConfigHelper.GetAppsettings<HangfireConfig>("Hangfire").Login;
        private string Password = ConfigHelper.GetAppsettings<HangfireConfig>("Hangfire").Password;
        //这里需要配置权限规则
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            var header = httpContext.Request.Headers["Authorization"];
            if (string.IsNullOrWhiteSpace(header))
            {
                SetChallengeResponse(httpContext);
                return false;
            }
            var authValues = System.Net.Http.Headers.AuthenticationHeaderValue.Parse(header);
            if (!"Basic".Equals(authValues.Scheme, StringComparison.InvariantCultureIgnoreCase))
            {
                SetChallengeResponse(httpContext);
                return false;
            }
            var parameter = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(authValues.Parameter));
            var parts = parameter.Split(':');
            if (parts.Length < 2)
            {
                SetChallengeResponse(httpContext);
                return false;
            }
            var username = parts[0];
            var password = parts[1];
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                SetChallengeResponse(httpContext);
                return false;
            }
            if (Login.Equals(username) && Password.Equals(password))
            {
                return true;
            }
            SetChallengeResponse(httpContext);
            return false;
        }

        private void SetChallengeResponse(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = 401;
            httpContext.Response.Headers.Append("WWW-Authenticate", "Basic realm=\"Hangfire Dashboard\"");
            httpContext.Response.WriteAsync("Authentication is required.");
        }
    }
}
