using ApiPhoneBook.Models;
using PhoneBook.Models;

namespace ApiPhoneBook.Data
{
    public static class Repository
    {
        static List<IContact> Data { get; }
        static int nextId = 2;
        static Repository()
        {
            Data = new List<IContact>()
            {
                new Contact() 
                {   Id = 0, 
                    FirstName = "Сергей", 
                    MiddleName = "Степанович",
                    LastName = "Жуков", 
                    Address = "г.Екатеринбург", 
                    Telefon = "+79847685049", 
                    Description = "ааааа"
                },
                new Contact()
                {   Id = 1,
                    FirstName = "Валентин",
                    MiddleName = "Иванович",
                    LastName = "Пахлебкин",
                    Address = "г.Санкт-Петербург",
                    Telefon = "+79847775049",
                    Description = "оооооо"
                }
            };
        }

        public static List<IContact> GetAll() => Data;

        public static Contact? Get(int id) => Data.FirstOrDefault(x => x.Id == id) as Contact;

        public static void Add(Contact contact)
        {
            contact.Id = nextId++;
            Data.Add(contact);
        }

        public static void Update(Contact contact) 
        {
            var index = Data.FindIndex(Data => Data.Id == contact.Id);
            if (index == -1) return;
            Data[index] = contact;
        }

        public static void Delete(int id) 
        {
            var contact = Get(id);
            if (contact == null) return;
            Data.Remove(contact);
        }
    }
}
