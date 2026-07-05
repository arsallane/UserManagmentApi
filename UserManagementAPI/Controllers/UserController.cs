using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UserManagementAPI.Repositories;
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _repo;

    public UsersController(IUserRepository repo)
    {
        _repo = repo;
    }

    // GET: api/users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    { 
        var users = await _repo.GetAllAsync();

        return Ok(users);
    }

    // GET: api/users/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    { 
        try
        {
            var user = await _repo.GetByIdAsync(id);
 
            if (user == null)
                return NotFound(new { error = "User not found" });

            return Ok(user);
        }
        catch (Exception ex)
        { 
            return StatusCode(500, new { error = "Database lookup failed", details = ex.Message });
        }
    }

    // POST: api/users
    [HttpPost]
    public async Task<ActionResult<User>> CreateUser(User user)
    {
      
        if (string.IsNullOrWhiteSpace(user.Name))
            return BadRequest(new { error = "Name cannot be empty" });

  
        if (!Regex.IsMatch(user.Email ?? "", @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            return BadRequest(new { error = "Invalid email format" });

        try
        {
            var created = await _repo.AddAsync(user);
            return CreatedAtAction(nameof(GetUser), new { id = created.Id }, created);
        }
        catch (Exception ex)
        { 
            return StatusCode(500, new { error = "Failed to create user", details = ex.Message });
        }
    }

    // PUT: api/users/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, User user)
    {
    
        if (id != user.Id)
            return BadRequest(new { error = "ID mismatch" });
 
        if (string.IsNullOrWhiteSpace(user.Name))
            return BadRequest(new { error = "Name cannot be empty" });

        if (!Regex.IsMatch(user.Email ?? "", @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            return BadRequest(new { error = "Invalid email format" });

        try
        {
            var existing = await _repo.GetByIdAsync(id);
 
            if (existing == null)
                return NotFound(new { error = "User not found" });

            await _repo.UpdateAsync(user);
            return NoContent();
        }
        catch (Exception ex)
        { 
            return StatusCode(500, new { error = "Failed to update user", details = ex.Message });
        }
    }

    // DELETE: api/users/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            var existing = await _repo.GetByIdAsync(id);
 
            if (existing == null)
                return NotFound(new { error = "User not found" });

            await _repo.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
   
            return StatusCode(500, new { error = "Failed to delete user", details = ex.Message });
        }
    }
}
