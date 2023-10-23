using ApiPhoneBook.Interfaces;
using ApiPhoneBook.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhoneBook.Models;

namespace ApiPhoneBook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IContactData contactData;

        public ValuesController(IContactData contactData)
        {
           this.contactData = contactData;
        }

        [HttpGet]
        public ActionResult<List<IContact>> GetAll() => Data.Repository.GetAll();

        [HttpGet("id")]
        public ActionResult<IContact> Get(int id)
        {
            var contact = Data.Repository.Get(id);

            if (contact == null) return NotFound();

            return Ok(contact); //contact
        }

        [HttpPost]
        public IActionResult Create(Contact contact) 
        { 
            Data.Repository.Add(contact);

            return CreatedAtAction(nameof(Create), new Contact { Id = contact.Id});
        }

        [HttpPut("id")]
        public IActionResult Update(int id, Contact contact) 
        { 
            if (id != contact.Id) return BadRequest();

            var existingContact = Data.Repository.Get(id);

            if (existingContact == null) return NotFound();
            
            Data.Repository.Update(contact);

            return NoContent();
        
        }

        [HttpDelete("id")]
        public IActionResult Delete(int id) 
        { 
            var contact = Data.Repository.Get(id);

            if (contact == null) return NotFound();

            Data.Repository.Delete(id);

            return NoContent();
        }
    }
}
