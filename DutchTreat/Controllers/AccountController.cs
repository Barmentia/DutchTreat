using DutchTreat.Data.Entities;
using DutchTreat.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DutchTreat.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly SignInManager<StoreUser> _signInManager;
        private readonly UserManager<StoreUser> _userManager;
        private readonly IConfiguration _config;

        public AccountController(ILogger<AccountController> logger, SignInManager<StoreUser> signInManager, UserManager<StoreUser> userManager, IConfiguration config)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _config = config;
        }

        public IActionResult Login()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "App");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    if (Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return Redirect(Request.Query["ReturnUrl"].First());
                    }
                    else
                    {
                        return RedirectToAction("Shop", "App");
                    }
                }
            }

            ModelState.AddModelError("", "Failed to login");

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "App");
        }

        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _signInManager.UserManager.FindByNameAsync(model.Username);

                if (user != null)
                {
                    // It checks if the credentials are ok
                    var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

                    if (result.Succeeded)
                    {
                        // Token creation process

                        // First, we need to get the claims. Claims are a set of properties with well-known values in them, 
                        // that could be stored in the token and used by the client or when it's passed back to the server.
                        var claims = new[]
                        {
                            // To create a clame, we need a type and a value.
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email),

                            // The jti is a unique string that's representative of each token 
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

                            // UniqueName is going to be the username of the user, and is actually mapped to the identity inside 
                            // the user object that is available on every controller and view for that matter.
                            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
                        };

                        // The key is the secret to encrypt the token.
                        //  Some parts of the token aren't encrypted, and some parts are. The information about the individuals or 
                        // about individual claims can be read without decrypting the token, but there are other parts about the 
                        // credential information, who it's tied to, and the expiration that may be encrypted, so that only the server 
                        // when it's getting passed back can actually use it                        
                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"])); // It takes an array of bytes

                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        // It creates the token
                        var token = new JwtSecurityToken(
                          _config["Tokens:Issuer"],
                          _config["Tokens:Audience"],
                          claims,
                          expires: DateTime.Now.AddMinutes(30),
                          signingCredentials: creds);

                        var results = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        };

                        return Created("", results);
                    }
                }
            }

            return BadRequest();
        }
    }
}