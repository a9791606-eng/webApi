using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using IceCreamNamespace.Models;
using IceCreamNamespace.Services;
using IceCreamNamespace.Interfaces;


namespace IceCreamNamespace.Controllers;

[ApiController]
[Route("user")]
public class UsersController : ControllerBase
{  
   IUserService services;
    public UsersController(IUserService Userservices)
    {
        this.services=Userservices;
    }

    [HttpGet()]
    public ActionResult<IEnumerable<User>> Get()
    {
        return services.Get();
    }

    [HttpGet("{id}")]
    public ActionResult<User> Get(int id)
    {
        var m = services.Get(id);
        if(m==null)
            return NotFound();
        return m;

    }

    [HttpPost]
    public ActionResult Create(User newUser)
    {
        var postedUser = services.Create(newUser);
      
       return CreatedAtAction(nameof(Create), new { id = postedUser.Id });
    }

    [HttpPut("{id}")]
    public ActionResult Update(int id, User newUser)
    {
        var ans= services.Update( id, newUser);
      
        if(ans==1)
          return NotFound();

        if(ans==2)
           return BadRequest();

       
        return NoContent();

    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        var ans= services.Delete(id);
      
        if(ans==false)
            return NotFound();
        return NoContent();

    }
}