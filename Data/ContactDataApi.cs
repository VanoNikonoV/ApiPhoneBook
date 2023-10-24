using ApiPhoneBook.Interfaces;
using Newtonsoft.Json;
using PhoneBook.Models;
using System.Text;

namespace ApiPhoneBook.Data
{
    public class ContactDataApi : IContactData
    {
        private HttpClient httpClient { get; set; }

        public ContactDataApi()
        {
            httpClient = new HttpClient();
        }

        public void AddContact(Contact contact)
        {
            string url = @"https://localhost:7169/api/values";

            var r = httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonConvert.SerializeObject(contact), Encoding.UTF8,
                mediaType: "application/json")
                ).Result;
        }

        public void DeleteContact(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Contact> GetAllContact()
        {
            string url = @"https://localhost:7169/api/values";

            string json = httpClient.GetStringAsync(url).Result;

            return JsonConvert.DeserializeObject<IEnumerable<Contact>>(json);
        }

        public Contact GetContact(int id) 
        {
            string url = @"https://localhost:7169/api/values/id";

            string json = httpClient.GetStringAsync(url).Result;

            return JsonConvert.DeserializeObject<Contact>(json);
        }
        public void UpdateContact(Contact contact)
        {
            throw new NotImplementedException();
        }

        public Contact CreateContact(Contact newContact)
        {
            return new Contact { };
        }
    }
}
