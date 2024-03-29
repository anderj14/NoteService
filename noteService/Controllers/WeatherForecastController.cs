using Microsoft.AspNetCore.Mvc;
using NoteService.Models;

namespace NoteService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly NoteDatabaseContext _context;

        public WeatherForecastController(NoteDatabaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("Users")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                List<User> listUsers = _context.Users.ToList();

                if (listUsers != null)
                {
                    return Ok(listUsers);
                }
                return Ok("They are no users in the database");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
