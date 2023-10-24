using ApiPhoneBook.Interfaces;
using ApiPhoneBook.Models;
using Microsoft.AspNetCore.Http;
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

        //[HttpGet]
        //public IActionResult GetAllContact() => View(contactData.GetAllContact());

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Contact>>> GetAllContact()
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
        public async Task<ActionResult<Contact>> GetContact(int id)
        {
            if (id == null || _context.Contact == null) return NotFound();

            var contact = await _context.Contact.FirstOrDefaultAsync(m => m.Id == id);

            //var contact = Data.Repository.Get(id);

            if (id < 0) { throw new ArgumentException("Значение ниже 0", nameof(id)); }

            if (contact == null) return NotFound();

            return Ok(contact); //contact
        }

        [HttpPost]
        public IActionResult CreateContact(Contact contact) 
        { 
            Data.Repository.Add(contact);

            return CreatedAtAction(nameof(CreateContact), new Contact { Id = contact.Id});
        }

        [HttpPut("id")]
        public IActionResult UpdateContact(int id, Contact contact) 
        { 
            if (id != contact.Id) return BadRequest();

            var existingContact = Data.Repository.Get(id);

            if (existingContact == null) return NotFound();
            
            Data.Repository.Update(contact);

            return NoContent();
        
        }

        [HttpDelete("id")]
        public IActionResult DeleteContact(int id) 
        { 
            var contact = Data.Repository.Get(id);

            if (contact == null) return NotFound();

            Data.Repository.Delete(id);

            return NoContent();
        }
    }
}
