using ApiPhoneBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneBook.Data;
using PhoneBook.Models;
using System.Diagnostics;
using System.Net.Mime;

namespace ApiPhoneBook.Controllers
{
    [Route("values")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly PhoneBookContext _context;

        private readonly ILogger<ValuesController> _logger;

        public ValuesController(PhoneBookContext context, ILogger<ValuesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Позволяет получить данные о всех контатах из базы данных
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Contact>>> GetAllContactAsync()
        {
            if (_context.Contact == null)
            {
                return Problem("Проблема с базой данных!");
            }

            try
            {
                var list = await _context.Contact.ToListAsync();

                return Ok(list);
            }
            catch (Exception ex) 
            { Debug.WriteLine(ex.Message); }
            return BadRequest();
        } 

        /// <summary>
        /// Позволяет получить данные о контакте по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор контакта</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        [HttpGet("id")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<IContact>> GetContactAsync(int id)
        {
            if (id == null || _context.Contact == null) return NotFound();

            var contact = await _context.Contact.FirstOrDefaultAsync(m => m.Id == id);

            if (id < 0) { throw new ArgumentException("Значение ниже 0", nameof(id)); }

            if (contact == null) return NotFound();

            return Ok(contact); //contact
        }

        /// <summary>
        /// Позволяет создать новый контакт
        /// </summary>
        /// <param name="contact">Новый контакт</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin,User")]
        public async Task <ActionResult> CreateContactAsync([FromBody] Contact contact) 
        { 
            try
            {
                var isContact = await _context.Contact.FindAsync(contact.Id);

                if (isContact != null) return BadRequest("Уже существует");

                else
                {
                    _context.Contact.Add(contact);

                    await _context.SaveChangesAsync();

                    return Created("~/values/" + contact.Id, contact);
                }

            }
            catch (DbUpdateException ex) { return BadRequest("Проблема с базой данных"); }

            catch (OperationCanceledException ex) { return BadRequest("Кто-то еще добавяет"); }
        }
        
        /// <summary>
        /// Редактирование данных контакта
        /// </summary>
        /// <param name="id">Идентификатор контакта</param>
        /// <param name="contact">Контакт с измененными данными</param>
        /// <returns></returns>
        [HttpPut("id")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateContactAsync(int id, [FromBody] Contact contact) 
        {
            if (id != contact.Id) return BadRequest("id не совпадают");

            try
            {
                _context.Update(contact);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactExists(contact.Id))
                {
                    return NotFound();
                }
                else
                {
                    return Problem("Этот контакт сейчас кто-то редактирует, попробуйте позже");
                }
            }
            return Ok(contact);
        }

        /// <summary>
        /// Удалает контакт по переданному id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("id")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteContactAsync(int id) 
        {
            if (id == default) return NoContent();
  
            var contact = await _context.Contact.FindAsync(id);

            if (contact != null) 
            {
                _context.Contact.Remove(contact);
                await _context.SaveChangesAsync();
            }
            return Ok();
        }

        private bool ContactExists(int id)
        {
            return (_context.Contact?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
