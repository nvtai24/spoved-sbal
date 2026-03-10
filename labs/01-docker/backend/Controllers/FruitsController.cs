using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class FruitsController : ControllerBase
{
    private readonly ILogger<FruitsController> _logger;
    private readonly SpovedContext _dbContext;

    public FruitsController(ILogger<FruitsController> logger, SpovedContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }


    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var fruits = await _dbContext.Fruits
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return Ok(fruits);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var fruit = await _dbContext.Fruits.FindAsync(id);
        if (fruit == null)
        {
            return NotFound();
        }
        return Ok(fruit);
    }

    [HttpGet("random")]
    public async Task<IActionResult> GetRandom()
    {
        var fruits = await _dbContext.Fruits.ToListAsync();
        if (fruits.Count == 0)
        {
            return NotFound();
        }
        var random = new Random();
        var randomFruit = fruits[random.Next(fruits.Count)];
        return Ok(randomFruit);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Fruit fruit)
    {
        _dbContext.Fruits.Add(fruit);
        await _dbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = fruit.Id }, fruit);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var fruit = await _dbContext.Fruits.FindAsync(id);
        if (fruit == null)
        {
            return NotFound();
        }
        _dbContext.Fruits.Remove(fruit);
        await _dbContext.SaveChangesAsync();
        return NoContent();
    }

}