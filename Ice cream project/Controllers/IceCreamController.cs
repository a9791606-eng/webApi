using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using IceCreamNamespace.Models;
using IceCreamNamespace.Interfaces;

namespace IceCreamNamespace.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class IceCreamController : ControllerBase
    {  
        private readonly IICService _services;

        public IceCreamController(IICService iceCreamService)
        {
            _services = iceCreamService;
        }

        [HttpGet]
        public ActionResult<List<IceCream>> GetAll() => _services.GetAll();

        [HttpGet("{id}")]
        public ActionResult<IceCream> Get(int id)
        {
            var ice = _services.Get(id);
            if(ice == null) return NotFound();
            return ice;
        }

        [HttpPost]
        public IActionResult Create(IceCream newIceCream)
        {
            _services.Create(newIceCream);
            return CreatedAtAction(nameof(Get), new { id = newIceCream.Id }, newIceCream);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, IceCream newIceCream)
        {
            if (id != newIceCream.Id) return BadRequest();

            var existing = _services.Get(id);
            if (existing == null) return NotFound();

            _services.Update(newIceCream);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var ice = _services.Get(id);
            if (ice == null) return NotFound();

            _services.Delete(id);
            return NoContent();
        }
    }
}