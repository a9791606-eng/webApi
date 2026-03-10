using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using IceCreamNamespace.Models;
using IceCreamNamespace.Services;

namespace IceCreamNamespace.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{
    public LoginController() { }

    [HttpPost]
    public ActionResult<string> Login(User user)
    {
        var dt = DateTime.Now;

        if (user == null || user.FirstName != "test" || user.Password != $"t{dt.Year}#{dt.Day}!")
        {
            return Unauthorized();
        }

        var claims = new List<Claim>
        {
            new Claim("id", "11235813"),
            new Claim("username", "test"),
        };

        var token = IceCreamTokenService.GetToken(claims);

        return new OkObjectResult(IceCreamTokenService.WriteToken(token));
    }
}