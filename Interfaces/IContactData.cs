using PhoneBook.Models;

namespace ApiPhoneBook.Interfaces
{
    public interface IContactData
    {
        IEnumerable<Contact> GetAll();
        void Add(Contact contact);
        void Update(Contact contact);
        void Delete(int id);
    }
}
