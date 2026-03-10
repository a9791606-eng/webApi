using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using IceCreamNamespace.Models;
using IceCreamNamespace.Services;
using IceCreamService.interfaces;

namespace IceCreamNamespace.Controllers;

[ApiController]
[Route("[controller]")]
public class IceCreamController : ControllerBase
{  
   IICService services;
    public IceCreamController(IICService Iceservices)
    {
        this.services=Iceservices;
    }

    [HttpGet()]
    public ActionResult<IEnumerable<IceCream>> Get()
    {
        return services.Get();
    }

    [HttpGet("{id}")]
    public ActionResult<IceCream> Get(int id)
    {
        var m = services.Get(id);
        if(m==null)
            return NotFound();
        return m;

    }

    [HttpPost]
    public ActionResult Create(IceCream newIceCream)
    {
        var postedIceCream = services.Create(newIceCream);
      
       return CreatedAtAction(nameof(Create), new { id = postedIceCream.Id });
    }

    [HttpPut("{id}")]
    public ActionResult Update(int id, IceCream newIceCream)
    {
        var ans= services.Update( id, newIceCream);
      
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
 
//     [HttpDelete("{id}")]
//     public ActionResult Delete(int id)
//     {
//         var Ice = find(id);
//         if (Ice == null)
//             return NotFound();
//         else
//         {
//             services.Remove(Ice);
//         }  
//          return NoContent();  
        
//     }
// }
