namespace API.Controllers;
using API.Data;
using API.DataEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class UsersController : BaseApiController
{
    private readonly IUserRepository _repository;

    public UsersController(IUserRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppUser>>> GetAllAsync()
    {
        var users = await _repository.GetAllAsync();

        return Ok(users);
    }

    [HttpGet("{id:int}")] // api/v1/users/2
    public async Task<ActionResult<AppUser>> GetUsersByIdAsync(int id)
    {
        var user = await _repository.GetByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return user;
    }

    [HttpGet("{username}")] // api/v1/users/Calamardo
    public async Task<ActionResult<AppUser>> GetByIdAsync(string username)
    {
        var user = await _repository.GetByUsernameAsync(username);

        if (user == null)
        {
            return NotFound();
        }

        return user;
    }
}