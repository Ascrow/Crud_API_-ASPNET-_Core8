using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Context;
using Api.Models;
using Api.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigin")]
    public class UsuarioController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuarioController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Usuario
        [HttpGet]
        [Authorize(Roles = "1")]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

        // GET: api/Usuario/5
        [HttpGet("{id}")]
        [Authorize(Roles = "1")]
        public async Task<ActionResult<Usuario>> GetUsuario(long id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }

        // PUT: api/Usuario/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize (Roles = "1")]
        public async Task<IActionResult> PutUsuario(long id, Usuario usuario)
        {
            // Buscar el usuario existente en la base de datos por su ID
            var existingUser = await _context.Usuarios.FindAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            // Actualizar solo los campos necesarios, excluyendo el ID
            existingUser.Nombre = usuario.Nombre;
            if (!string.IsNullOrEmpty(usuario.Pass))
            {
                //Encripta la contraseña utilizando la funcion de utils.
                existingUser.Pass = PassUtils.HashPassword(usuario.Pass);
            }
            existingUser.Rol = usuario.Rol;

            // Marcar el usuario existente como modificado
            _context.Entry(existingUser).State = EntityState.Modified;

            try
            {
                // Guardar los cambios en la base de datos
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Usuario
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "1")]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            // Verificar si ya existe un usuario con el mismo nombre
            var existingUser = await _context.Usuarios.FirstOrDefaultAsync(u => u.Nombre == usuario.Nombre);
            if (existingUser != null)
            {
                return Conflict(new { message = "Ya existe un usuario con este nombre." });
            }

            var user = new Usuario
            {
                Nombre = usuario.Nombre,
                Pass = PassUtils.HashPassword(usuario.Pass),
                Rol = usuario.Rol
            };

            _context.Usuarios.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsuario", new { id = user.Id }, user);
        }

        // DELETE: api/Usuario/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> DeleteUsuario(long id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsuarioExists(long id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}
