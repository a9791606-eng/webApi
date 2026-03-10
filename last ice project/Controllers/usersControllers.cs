using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using IceCreamNamespace.Models;
using IceCreamNamespace.Services;
using IceCreamService.interfaces;


namespace IceCreamNamespace.Controllers;

[ApiController]
[Route("user")]
public class UsersController : ControllerBase
{
    private readonly IUserService services;
    public UsersController(IUserService Userservices)
    {
        this.services = Userservices;
    }

    [HttpGet()]
    public ActionResult<IEnumerable<User>> Get()
    {
        return services.Get();
    }

    [HttpGet("{id}")]
    public ActionResult<User> Get(int id)
    {
        var user = services.Get(id);
        if (user == null)
            return NotFound();
        return user;
    }

    [HttpPost]
    public IActionResult Create(User newUser)
    {
        var created = services.Create(newUser);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, User newUser)
    {
        if (id != newUser.Id)
            return BadRequest();

        var existing = services.Get(id);
        if (existing is null)
            return NotFound();

        services.Update(id, newUser);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var user = services.Get(id);
        if (user is null)
            return NotFound();

        services.Delete(id);
        var remaining = services.Get().Count;
        return Content(remaining.ToString());
    }
}