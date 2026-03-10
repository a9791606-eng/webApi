using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using IceCreamNamespace.Models;
using IceCreamNamespace.Services;
using IceCreamService.interfaces;
using System.Collections.Generic;

namespace IceCreamNamespace.Controllers;

[ApiController]
[Route("[controller]")]
public class IceCreamController : ControllerBase
{  
   private readonly IICService services;
    public IceCreamController(IICService Iceservices)
    {
        this.services=Iceservices;
    }

    [HttpGet()]
    public ActionResult<IEnumerable<IceCream>> Get()
    {
        return services.GetAll();
    }

    [HttpGet("{id}")]
    public ActionResult<IceCream> Get(int id)
    {
        var IceCream = services.Get(id);
        if(IceCream==null)
            return NotFound();
        return IceCream;

    }

    [HttpPost]
    public IActionResult Create(IceCream newIceCream)
    {
       services.Add(newIceCream);
       return CreatedAtAction(nameof(Get), new { id = newIceCream.Id }, newIceCream);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, IceCream newIceCream)
    {
        if (id != newIceCream.Id)
            return BadRequest();

        var existingIceCream = services.Get(id);
        if (existingIceCream is null)
            return NotFound();

       services.Update(newIceCream);

       return NoContent();

    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var ice = services.Get(id);

        if (ice is null )
            return NotFound();
        services.Delete(id);

        return Content(services.Count.ToString());

    }
}


