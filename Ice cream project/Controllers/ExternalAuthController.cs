using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using IceCreamNamespace.Models;
using IceCreamNamespace.Services;
using IceCreamNamespace.Interfaces;

namespace IceCreamNamespace.Controllers
{
    [ApiController]
    [Route("external")]
    public class ExternalAuthController : ControllerBase
    {
        private readonly IUserRepository _userRepo;

        public ExternalAuthController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var props = new AuthenticationProperties { RedirectUri = Url.Action("GoogleCallback") };
            return Challenge(props, "Google");
        }

        [HttpGet("google-callback")]
        public IActionResult GoogleCallback()
        {
            var result = HttpContext.AuthenticateAsync("External").Result;
            if (!result.Succeeded || result.Principal == null)
                return Unauthorized();

            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value ?? result.Principal.FindFirst("email")?.Value;
            var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value ?? result.Principal.FindFirst("name")?.Value;

            // find or create local user
            var existing = _userRepo.GetAll().Find(u => u.Username == email);
            if (existing == null)
            {
                var newUser = new User { Username = email, Password = "", IsAdmin = false };
                _userRepo.Add(newUser);
                existing = newUser;
            }

            var claims = new List<Claim>
            {
                new Claim("id", existing.Id.ToString()),
                new Claim("username", existing.Username),
                new Claim("type", existing.IsAdmin ? "Admin" : "User")
            };

            var token = IceCreamTokenService.GetToken(claims);
            var jwt = IceCreamTokenService.WriteToken(token);

            // return a simple page that sets the token in localStorage and closes the popup
            var html = $@"<html><body><script>
localStorage.setItem('icecream_token', '{jwt}');
window.location = '/index.html';
</script></body></html>";

            return Content(html, "text/html");
        }
    }
}
