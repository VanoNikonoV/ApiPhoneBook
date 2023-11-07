using ApiPhoneBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneBook.Data;
using PhoneBook.Models;
using System.Data;
using System.Diagnostics;
using System.Net.Mime;

namespace ApiPhoneBook.Controllers
{
    [Route("values")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiController]
    [AllowAnonymous]
    public class ValuesController : ControllerBase
    {
        private readonly PhoneBookContext _context;
        private readonly ILogger<ValuesController> _logger;

        public ValuesController(PhoneBookContext context, ILogger<ValuesController> logger)
        {
            _context = context;
            _logger = logger;
           
            //bool d =  _context.Database.EnsureCreated();
            //Debug.WriteLine("d = " + d.ToString());
        }

        /// <summary>
        /// Позволяет получить данные о всех контатах из базы данных
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<IEnumerable<Contact>>> GetAllContactAsync()
        {
            //_context.Contact.AddRange(
            //       new Contact
            //       {
            //           FirstName = "Иван",
            //           MiddleName = "Иваныч",
            //           LastName = "Иванов",
            //           Telefon = "79827135444",
            //           Address = "Москва",
            //           Description = "Description",

            //       },
            //       new Contact
            //       {
            //           FirstName = "Иван",
            //           MiddleName = "Андреевич",
            //           LastName = "Путин",
            //           Telefon = "79827855040",
            //           Address = "Свердловск",
            //           Description = "Description",

            //       },
            //       new Contact
            //       {
            //           FirstName = "Костя",
            //           MiddleName = "Владимирович",
            //           LastName = "Хмель",
            //           Telefon = "79826665040",
            //           Address = "Тюмень",
            //           Description = "есть",
            //       },
            //       new Contact
            //       {
            //           FirstName = "Петр",
            //           MiddleName = "Иваныч",
            //           LastName = "Степанов",
            //           Telefon = "79827135040",
            //           Address = "Пенза",
            //           Description = "нет",
            //       }
            //   );
            //int r = await _context.SaveChangesAsync();

            if (_context.Contact == null)
            {
                return Problem("Проблема с базой данных!");
            }

            try
            {
                if (_context.Contact == null)
                {
                    return BadRequest();
                }
                
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
        public async Task<ActionResult<IContact>> GetContactAsync(int id)
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
        public async Task <ActionResult> CreateContactAsync([FromBody] Contact contact) 
        { 
            _context.Contact.Add(contact);
            try
            {
               int r = await _context.SaveChangesAsync();
                Debug.WriteLine("r = " + r.ToString());
            }
            catch (DbUpdateException ex) { Debug.WriteLine(ex.Message); }

            catch (OperationCanceledException ex) { Debug.WriteLine(ex.Message); }

            catch (Exception ex)  { Debug.WriteLine(ex.Message); }
            
            
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
        public async Task<ActionResult> DeleteContactAsync(int id) 
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
