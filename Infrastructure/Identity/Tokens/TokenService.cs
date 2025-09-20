using ABCShared.Library.Constants;
using ABCShared.Library.Models.Requests.Token;
using ABCShared.Library.Models.Responses.Token;
using Application;
using Application.Exceptions;
using Application.Features.Identity.Tokens;
using Finbuckle.MultiTenant.Abstractions;
using Infrastructure.Constants;
using Infrastructure.Identity.Models;
using Infrastructure.Tenancy;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity.Tokens
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IMultiTenantContextAccessor<ABCSchoolTenantInfo> _tenantContextAccessor;
        private readonly JwtSettings _jwtSettings;
        private readonly IDatabase _redisDb;

        public TokenService(UserManager<ApplicationUser> userManager , RoleManager<ApplicationRole> roleManager
            , IMultiTenantContextAccessor<ABCSchoolTenantInfo> tenantContextAccessor , IOptions<JwtSettings> jwtSettings , IConnectionMultiplexer redis)
        {
            _userManager=userManager;
            _roleManager=roleManager;
            _tenantContextAccessor=tenantContextAccessor;
              _jwtSettings=jwtSettings.Value;
            _redisDb = redis.GetDatabase();
        }
        public async Task<TokenResponse> LoginAsync(TokenRequest request)
        {
           // validation
           if(!_tenantContextAccessor.MultiTenantContext.TenantInfo.IsActive)
            {
                throw new UnauthorizedException(["Tenant subscription is not active . contact Administrator"]);
            }
            var userInDb = await _userManager.FindByNameAsync(request.Username)
                 ?? throw new UnauthorizedException(["Authentication Not successful"]);
            if(!await _userManager.CheckPasswordAsync(userInDb , request.Password))
            {
                throw new UnauthorizedException(["Incorrect userName or Password"]);
            }
            if(!userInDb.IsActive)
            {
                throw new UnauthorizedException(["User Not Active Contact Administrator"]);
            }
            if(_tenantContextAccessor.MultiTenantContext.TenantInfo.Id is not TenancyConstants.Root.Id)
            {
              if(_tenantContextAccessor.MultiTenantContext.TenantInfo.ValidUpTo < DateTime.UtcNow)
                {
                    throw new UnauthorizedException(["Tenant Subscription has expired"]);
                }

            }
            return await GenerateTokenAndUpdateUserAsync(userInDb);
        }

        public async Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var userPrincipal = GetClaimsPrincipalFromExpirngToken(request.CurrentJwt);
            var userEmail = userPrincipal.GetEmail();

            var userInDb = await _userManager.FindByEmailAsync(userEmail) ?? throw new UnauthorizedException(["Authentication failed"]);

            var cacheKey = $"refresh_token:{userInDb.Id}";

            var storedToken = await _redisDb.StringGetAsync(cacheKey);

            if (userInDb.RefreshToken != request.CurrentRefreshToken || userInDb.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                throw new UnauthorizedException(["Invalid token"]);
            }

            return await GenerateTokenAndUpdateUserAsync(userInDb);
        }


        private ClaimsPrincipal GetClaimsPrincipalFromExpirngToken(string expiringToken)
        {
            var tokenValidationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false ,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero,
                RoleClaimType = ClaimTypes.Role,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(expiringToken, tokenValidationParams, out var securityToken);

            if(securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256 , StringComparison.InvariantCultureIgnoreCase))
            {
                throw new UnauthorizedException(["Invalid token provider"]);
            }

            return principal;
        }



        private async Task<TokenResponse> GenerateTokenAndUpdateUserAsync(ApplicationUser user)
        {
            // Generate token
         var newJwt = await GenerateToken(user);
            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryTimeInDays);
            var cacheKey = $"refresh_token:{user.Id}";
            await _redisDb.StringSetAsync(cacheKey, user.RefreshToken, TimeSpan.FromDays(_jwtSettings.RefreshTokenExpiryTimeInDays));

            await _userManager.UpdateAsync(user);
            return new TokenResponse
            {
                Jwt =  newJwt,
                RefreshToken = user.RefreshToken,
                RefreshTokenExpiryDate = DateTime.UtcNow.AddDays(1),
            };

        }


        private async Task <string> GenerateToken(ApplicationUser user)
        {
            // encrypted token
            return GenerateEncryptedToken(GenerateSigningCredentials(), await GetUserClaims(user));
        }


        private string GenerateEncryptedToken(SigningCredentials signingCredentials , IEnumerable<Claim> claims)
        {
            var token = new JwtSecurityToken
                (
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.TokenExpiryTimeInMinutes),
                signingCredentials: signingCredentials
                );
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

        private SigningCredentials GenerateSigningCredentials()
        {
            byte[] secret = Encoding.UTF8.GetBytes(_jwtSettings.Secret);
            return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
        }

        private async Task <IEnumerable<Claim>> GetUserClaims(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            var roleClaims = new List<Claim>();
            var permissionClaims = new List<Claim>();
            foreach (var userRole in userRoles)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, userRole));
                var currentRole = await _roleManager.FindByNameAsync(userRole);

                var allPermissionsForCurrentRole = await _roleManager.GetClaimsAsync(currentRole);
                permissionClaims.AddRange(allPermissionsForCurrentRole);
            }

            var claims = new List<Claim>()
           {

                new (ClaimTypes.NameIdentifier , user.Id),
                new (ClaimTypes.Email , user.Email),
                new (ClaimTypes.Name , user.FirstName),
                new (ClaimTypes.Surname , user.LastName),
                new (ClaimConstants.Tenant , _tenantContextAccessor.MultiTenantContext.TenantInfo.Id),
                new (ClaimTypes.MobilePhone , user.PhoneNumber ?? string.Empty)
        }.Union(roleClaims).Union(userClaims).Union(permissionClaims);
            return claims;
        }

        private string GenerateRefreshToken()
        {
            byte[] randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
