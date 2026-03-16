using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using IceCreamNamespace.Models;
using IceCreamNamespace.Services;
using IceCreamNamespace.Interfaces;
using Microsoft.AspNetCore.Authorization;


namespace IceCreamNamespace.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{  
   IUserService services;
    IUserRepository userRepository;
    
    public UsersController(IUserService Userservices, IUserRepository repository)
    {
        this.services=Userservices;
        this.userRepository=repository;
    }

    [HttpPost("signup")]
    [AllowAnonymous]
    public ActionResult Signup([FromBody] User newUser)
    {
        if (string.IsNullOrWhiteSpace(newUser?.Username) || string.IsNullOrWhiteSpace(newUser?.Password))
            return BadRequest("Username and Password are required");
        
        // בדוק אם משתמש כבר קיים
        var allUsers = userRepository.GetAll();
        var existing = allUsers?.FirstOrDefault(u => u.Username.ToLower() == newUser.Username.ToLower());
        if (existing != null)
            return BadRequest("Username already exists");
        
        newUser.IsAdmin = false; // תמיד False לחדשים
        userRepository.Add(newUser);
        return CreatedAtAction(nameof(Signup), new { id = newUser.Id }, newUser);
    }

    [HttpGet()]
    [Authorize]
    public ActionResult<IEnumerable<User>> Get()
    {
        return services.Get();
    }

    [HttpGet("{id}")]
    [Authorize]
    public ActionResult<User> Get(int id)
    {
        var m = services.Get(id);
        if(m==null)
            return NotFound();
        return m;

    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public ActionResult Create(User newUser)
    {
        var postedUser = services.Create(newUser);
      
       return CreatedAtAction(nameof(Create), new { id = postedUser.Id });
    }

    [HttpPut("{id}")]
    [Authorize]
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
    [Authorize(Policy = "AdminOnly")]
    public ActionResult Delete(int id)
    {
        var ans= services.Delete(id);
      
        if(ans==false)
            return NotFound();
        return NoContent();

    }
}