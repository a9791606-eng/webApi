using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using IceCreamNamespace.Models;
using IceCreamNamespace.Services;
using IceCreamNamespace.Interfaces;
// corrected: use IceCreamNamespace services/interfaces

namespace IceCreamNamespace.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{
    private readonly IUserRepository _userRepo;

    public LoginController(IUserRepository userRepo) { _userRepo = userRepo; }

    [HttpPost]
    public ActionResult<String> Login(User User)
    {
        if (string.IsNullOrWhiteSpace(User?.Username) || string.IsNullOrWhiteSpace(User?.Password))
            return Unauthorized();

        // simple credential check against stored users
        var found = _userRepo.GetAll().Find(u => u.Username == User.Username && u.Password == User.Password);
        if (found == null) return Unauthorized();

        var claims = new List<Claim>
        {
            new Claim("id", found.Id.ToString()),
            new Claim("username", found.Username),
            new Claim("type", found.IsAdmin ? "Admin" : "User")
        };

        var token = IceCreamTokenService.GetToken(claims);

        return new OkObjectResult(IceCreamTokenService.WriteToken(token));
    }
}