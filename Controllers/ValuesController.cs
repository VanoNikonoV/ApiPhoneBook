using ApiPhoneBook.Interfaces;
using ApiPhoneBook.Models;
using Microsoft.AspNetCore.Diagnostics;
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
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task <ActionResult> CreateContact([Bind("FirstName,MiddleName,LastName,Telefon,Address,Description")] Contact contact) 
        { 
            if (ModelState.IsValid) 
            {
                _context.Contact.Add(contact);
                await _context.SaveChangesAsync();
                return Ok(contact); //Created
            }
            else 
            {
                foreach (var item in ModelState)
                {
                    // пробегаемся по всем ошибкам
                    foreach (var error in item.Value.Errors)
                    {
                        await Console.Out.WriteLineAsync(error.ToString());
                    }
                }  
            }
            
            //Data.Repository.Add(contact);

            return BadRequest();
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
