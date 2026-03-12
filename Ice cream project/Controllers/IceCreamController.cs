using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography.X509Certificates;
using IceCreamNamespace.Models;
using IceCreamNamespace.Services;
using IceCreamService.interfaces;

namespace IceCreamNamespace.Controllers
{

    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class IceCreamController : ControllerBase
    {  
    IICService services;
        public IceCreamController(IICService IceCreamService)
        {
            this.services=IceCreamService;
        }

        [HttpGet()]
        public ActionResult<List<IceCream>> GetAll()
        {
            return services.GetAll();
        }

        [HttpGet("{id}")]
        public ActionResult<IceCream> Get(int id)
        {
            var Ice = services.Get(id);
            if(Ice==null)
                return NotFound();
            return Ice;

        }

        [HttpPost]
        public IActionResult Create(IceCream newIceCream)
        {
            services.Add(newIceCream);
            return CreatedAtAction(nameof(Create), new { id = newIceCream.Id },newIceCream);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, IceCream newIceCream)
        {
           if (id != newIceCream.Id)
                return BadRequest();

            var existingIceCream = services.Get(id);
            if (existingIceCream is null)
                return  NotFound();

            services.Update(newIceCream);

            return NoContent();

        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var IceCream = services.Get(id);
            if (IceCream is null)
                return  NotFound();

            services.Delete(id);

            return Content(services.Count.ToString());
        }
    }
}
