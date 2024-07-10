using Microsoft.AspNetCore.Mvc;
using Api.Models;
using Api.Context;
using Microsoft.EntityFrameworkCore;
using Api.Utils;
using Microsoft.AspNetCore.Cors;

namespace Api.Controllers
{
    [Route("api/")]
    [ApiController]
    [EnableCors("AllowSpecificOrigin")]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public LoginController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login login)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Nombre == login.Nombre);
            if (user == null || !PassUtils.VerifyPassword(login.Pass, user.Pass))
            {
                return Unauthorized(new { message = "Usuario o contraseña incorrectos." });
            }
            var token = JwtUtils.GenerateJwtToken(user, _configuration);
            return Ok(token);
        }
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Aquí puedes manejar el logout si necesitas realizar alguna acción específica en el servidor.
            return Ok(new { message = "Logout successful" });
        }
    }
}
