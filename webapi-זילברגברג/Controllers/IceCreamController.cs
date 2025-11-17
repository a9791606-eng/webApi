using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using angular_אנגולר.Services;
using angular_אנגולר.Models;
using angular_אנגולר.Interface;


namespace angular_אנגולר.Controllers;

[ApiController]
[Route("[controller]")]
public class IceCreamController : ControllerBase
{  
   IICService services;
    public IceCreamController(IICService services)
    {
        this.services=services;
    }

    [HttpGet()]
    public ActionResult Get()
    {
        return Ok("Welcome! Use Post to place an order.");
        
    }

    // [HttpGet("{id}")]
    // public ActionResult<IEnumerable<Ice>> Get(int id)
    // {
    //    var Ice= IceCream.FirstOrDefault(p => p.id == id);
    //    if(Ice== null)
    //         return NotFound();
    //    return Ice;
    // }

    [HttpPost]
    public async Task<ActionResult> PostAsync(Order order)
    {
        await services.Transmit(order);

            return Ok();
    }; 
   
//    [HttpPut("{id}")]
//     public ActionResult Update(int id, Ice Ice)
//     {
//         var Ice = find(id);
//         if (Ice == null)
//             return NotFound();
//         if (IceCream.Id != Ice.Id)
//             return BadRequest();

//         var index = IceCream.IndexOf(Ice);
//         IceCream[index] = Ice;

//         return NoContent();
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        var Ice = find(id);
        if (Ice == null)
            return NotFound();
        IceCream.Remove(Ice);
        /*var index = list.IndexOf(pizza);
        list.RemoveAt(index);*/
        return NoContent();
    }
}
