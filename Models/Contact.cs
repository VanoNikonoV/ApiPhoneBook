using ApiPhoneBook.Models;
using System.ComponentModel.DataAnnotations;

namespace PhoneBook.Models
{
    public class Contact: IContact
    {
        /// <summary>
        /// Идентификатор контакта
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Имя
        /// </summary>
    
        public string FirstName { get; set; }

        /// <summary>
        /// Отчество
        /// </summary>
        
        public string MiddleName { get; set; }

        /// <summary>
        /// Фамилия
        /// </summary>
        
        public string LastName { get; set; }

        /// <summary>
        /// Номер телефона
        /// </summary>
        
        public string Telefon { get; set; }

        /// <summary>
        /// Адрес
        /// </summary>
        
        public string Address { get; set; }
        /// <summary>
        /// Описание 
        /// </summary>
        
        public string? Description { get; set; }
  
    }
}
