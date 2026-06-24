using Microsoft.AspNetCore.Mvc;
using KVS_API.Models;

namespace KVS_API.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public UserController(ApplicationDbContext context)
        {
            this._context = context;
        }



    }
}
