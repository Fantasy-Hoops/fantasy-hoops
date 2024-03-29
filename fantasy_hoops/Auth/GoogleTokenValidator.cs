using Google.Apis.Auth;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace fantasy_hoops.Auth
{
    public class GoogleTokenValidator : ISecurityTokenValidator
    {
        private readonly JwtSecurityTokenHandler _tokenHandler;

        public GoogleTokenValidator()
        {
            _tokenHandler = new JwtSecurityTokenHandler();
        }

        public bool CanValidateToken => true;

        public int MaximumTokenSizeInBytes { get; set; } = TokenValidationParameters.DefaultMaximumTokenSizeInBytes;

        public bool CanReadToken(string securityToken)
        {
            return _tokenHandler.CanReadToken(securityToken);
        }

        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            validatedToken = null;
            var payload = GoogleJsonWebSignature.ValidateAsync(securityToken, new GoogleJsonWebSignature.ValidationSettings()).Result; // here is where I delegate to Google to validate

            var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Iss, payload.Issuer),
                    new Claim(JwtRegisteredClaimNames.Azp, payload.Audience.ToString()),
                    new Claim(JwtRegisteredClaimNames.Aud, payload.Audience.ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub, payload.Subject),
                    new Claim(JwtRegisteredClaimNames.Email, payload.Email),
                    new Claim("email_verified", payload.EmailVerified.ToString(), ClaimValueTypes.Boolean),
                    new Claim(ClaimTypes.Name, payload.Name),
                    new Claim("picture", payload.Picture),
                    new Claim(JwtRegisteredClaimNames.GivenName, payload.GivenName),
                    new Claim(JwtRegisteredClaimNames.FamilyName, payload.FamilyName),
                    new Claim("locale", payload.Locale),
                    new Claim(JwtRegisteredClaimNames.Iat, payload.IssuedAtTimeSeconds.ToString(), ClaimValueTypes.Integer32),
                    new Claim(JwtRegisteredClaimNames.Exp, payload.ExpirationTimeSeconds.ToString(), ClaimValueTypes.Integer32),
                    new Claim(JwtRegisteredClaimNames.Jti, payload.JwtId)
                };

            var principle = new ClaimsPrincipal();
            principle.AddIdentity(new ClaimsIdentity(claims, "Password"));
            return principle;
        }
    }
}
