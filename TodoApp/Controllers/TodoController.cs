using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public TodoController(ApiDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetItemsAsync()
        {
            var items = await _context.Items.ToListAsync();

            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemAsync(int id)
        {
            var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == id);

            if (item is null)
                return NotFound();

            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> CreateItemAsync(Item item)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult("Something went wrong!") { StatusCode = 500 };
            }

            await _context.Items.AddAsync(item);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetItem", new { item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItemAsync(int id, Item item)
        {
            if (id != item.Id)
                return BadRequest();

            var existItem = await _context.Items.FirstOrDefaultAsync(i => i.Id == id);

            if (existItem is null)
                return NotFound();

            existItem.Title = item.Title;
            existItem.Description = item.Description;
            existItem.Done = item.Done;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItemAsync(int id)
        {
            var existItem = await _context.Items.FirstOrDefaultAsync(i => i.Id == id);

            if (existItem is null)
                return NotFound();

            _context.Remove(existItem);

            await _context.SaveChangesAsync();

            return Ok(existItem);
        }
    }
}