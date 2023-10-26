using ApiPhoneBook.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneBook.Data;
using PhoneBook.Models;
using System.Net.Mime;

namespace ApiPhoneBook.Controllers
{
    [Route("api/values")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiController]
    public class ValuesController : Controller
    {
        private readonly PhoneBookContext _context;

        public ValuesController( PhoneBookContext context)
        {
           this._context = context;
        }

        /// <summary>
        /// Отправляет все контаткы из базы данных
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<IContact>>> GetAllContact()
        {
            if (_context.Contact == null)
            {
                return Problem("Проблема с базой данных!");
            }

            return Ok(await _context.Contact.ToListAsync());
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
        public async Task<ActionResult<IContact>> GetContact(int id)
        {
            if (id == null || _context.Contact == null) return NotFound();

            var contact = await _context.Contact.FirstOrDefaultAsync(m => m.Id == id);

            //var contact = Data.Repository.Get(id);

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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task <ActionResult> CreateContact([FromBody] Contact contact) 
        { 
            _context.Contact.Add(contact);
            await _context.SaveChangesAsync();
            return Ok(contact); //Created
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
        public async Task<ActionResult> UpdateContact(int id, [FromBody] Contact contact) 
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
        public async Task<ActionResult>DeleteContact(int id) 
        {
            if (id == default) return NoContent();
  
            var contact = await _context.Contact.FindAsync(id);

            if (contact != null) 
            {
                _context.Contact.Remove(contact);
                await _context.SaveChangesAsync();
                return Ok();
            }
            return NotFound();
        }

        private bool ContactExists(int id)
        {
            return (_context.Contact?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
