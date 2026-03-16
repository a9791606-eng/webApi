using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using IceCreamNamespace.Models;
using IceCreamNamespace.Services;
using IceCreamNamespace.Interfaces;
using Google.Apis.Auth; // חובה עבור האימות המאובטח

namespace IceCreamNamespace.Controllers
{
    [ApiController]
    [Route("external")]
    public class ExternalAuthController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        // הכניסי כאן את ה-Client ID שקיבלת מגוגל
        private const string GoogleClientId = "YOUR_GOOGLE_CLIENT_ID.apps.googleusercontent.com";

        public ExternalAuthController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            // שימוש ב-GoogleDefaults.AuthenticationScheme אם קיים, או פשוט "Google"
            var props = new AuthenticationProperties { RedirectUri = Url.Action("GoogleCallback") };
            return Challenge(props, "Google");
        }

        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback() // שינוי ל-async Task
        {
            // שימוש ב-await במקום .Result
            var result = await HttpContext.AuthenticateAsync("External");
            
            if (!result.Succeeded || result.Principal == null)
                return Unauthorized("Google authentication failed.");

            // חילוץ ה-id_token שחזר מגוגל
            var idToken = result.Properties.GetTokenValue("id_token");

            // --- תחילת האימות המאובטח (מה שהמרצה ביקשה) ---
            GoogleJsonWebSignature.Payload payload;
            try
            {
                // כאן השרת שלך פונה לגוגל ומאמת שהטוקן אמיתי ותקין
                payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { GoogleClientId }
                });
            }
            catch (Exception)
            {
                return Unauthorized("Invalid Google Token - Validation failed.");
            }
            // --- סוף אימות שרת ---

            var email = payload.Email;
            var name = payload.Name;

            // מציאת משתמש או יצירה
            var existing = _userRepo.GetAll().FirstOrDefault(u => u.Username == email);
            if (existing == null)
            {
                existing = new User { Username = email, Password = "", IsAdmin = false };
                _userRepo.Add(existing);
            }

            var claims = new List<Claim>
            {
                new Claim("id", existing.Id.ToString()),
                new Claim("username", existing.Username),
                new Claim("type", existing.IsAdmin ? "Admin" : "User")
            };

            var token = IceCreamTokenService.GetToken(claims);
            var jwt = IceCreamTokenService.WriteToken(token);

            // דף HTML פשוט להעברת הטוקן ללקוח
            var html = $@"
                <html>
                <body>
                    <script>
                        localStorage.setItem('icecream_token', '{jwt}');
                        window.location.href = '/index.html';
                    </script>
                    <p>Redirecting...</p>
                </body>
                </html>";

            return Content(html, "text/html");
        }
    }
}