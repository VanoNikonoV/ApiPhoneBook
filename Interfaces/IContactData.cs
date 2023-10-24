using PhoneBook.Models;

namespace ApiPhoneBook.Interfaces
{
    public interface IContactData
    {
        IEnumerable<Contact> GetAllContact();
        Contact GetContact(int id);
        Contact CreateContact(Contact newContact);
        void AddContact(Contact contact);
        void UpdateContact(Contact contact);
        void DeleteContact(int id);
    }
}
