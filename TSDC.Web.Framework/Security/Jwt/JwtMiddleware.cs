using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using TSDC.Service.Master;

namespace TSDC.Web.Framework
{
    public class JwtMiddleware
    {
        #region Fields
        private readonly RequestDelegate _requestDelegate;
        private readonly AppSettings _appSettings;
        #endregion

        #region Ctor
        public JwtMiddleware(
            RequestDelegate requestDelegate,
            IOptions<AppSettings> appSettings)
        {
            _requestDelegate = requestDelegate;
            _appSettings = appSettings.Value;
        }
        #endregion

        #region Methods
        public async Task Invoke(HttpContext context, IUserService userService)
        {
            var token = context.Request.Headers["Authorization"]
                .FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                await AttachUserToContextAsync(context, userService, token);
            }

            await _requestDelegate(context);
        }
        #endregion

        #region Utilities
        private async Task AttachUserToContextAsync(HttpContext context, IUserService userService, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "Id").Value);

                context.Items["User"] = await userService.GetByIdAsync(userId);
            }
            catch
            {

            }
        }
        #endregion
    }
}
