using ContosoUniversity.Data.Entities;
using ContosoUniversity.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ContosoUniversity.Web.Controllers
{
    // model after http://www.blinkingcaret.com/2017/09/06/secure-web-api-in-asp-net-core/
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class TokenController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;

        public TokenController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _config = config;
        }

        public IActionResult Get()
        {
            return Ok("Congrats, you're authenticated.");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] TokenViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                    if (result.Succeeded)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                        };

                        // support multiple audiences
                        var audiences = _config.GetSection("Authentication:Tokens:Audiences").Get<string[]>();
                        foreach (var item in audiences)
                        {
                            claims.Add(new Claim("aud", item));
                        }

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Authentication:Tokens:Key"]));
                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        var token = new JwtSecurityToken(_config["Authentication:Tokens:Issuer"],
                            null,
                            claims,
                            expires: DateTime.Now.AddMinutes(30),
                            signingCredentials: creds);

                        return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
                    }
                }
            }

            return BadRequest("Could not create token");
        }
    }
}
