using Template.Data.Models;
using Template.Transport.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Template.Controllers
{
    public class IdentityController : ApiController
    {
        private readonly UserManager<User> userManager;
        private readonly AppSettings appSettings;

        public IdentityController(UserManager<User> userManager, IOptions<AppSettings> appSettings)
        {
            this.userManager = userManager;
            this.appSettings = appSettings.Value;
        }

        [Route(nameof(Register))]
        [HttpPost]
        public async Task<ActionResult> Register(RegisterRequestModel model)
        {
            var user = new User
            {
                Email = model.Email,
                UserName = model.Username
            };
            var r = await userManager.CreateAsync(user, model.Password);
            if (r.Succeeded)
            {
                return Ok();
            }
            return BadRequest(r.Errors);
        }

        [Route(nameof(Login))]
        public async Task<ActionResult<string>> Login(LoginRequestModel model)
        {
            var user = await userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                return Unauthorized();
            }
            var isPasswordValid = await userManager.CheckPasswordAsync(user, model.Password);
            if (!isPasswordValid)
            {
                return Unauthorized();
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var encryptedToken = tokenHandler.WriteToken(token);
            return Ok(encryptedToken);
        }
    }
}
