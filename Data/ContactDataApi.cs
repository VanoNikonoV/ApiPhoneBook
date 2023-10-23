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

        public void Add(Contact contact)
        {
            string url = @"https://localhost:7000/api/values";

            var r = httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonConvert.SerializeObject(contact), Encoding.UTF8,
                mediaType: "application/json")
                ).Result;
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Contact> GetAll()
        {
            string url = @"https://localhost:7000/api/values";

            string json = httpClient.GetStringAsync(url).Result;

            return JsonConvert.DeserializeObject<IEnumerable<Contact>>(json);
        }

        public void Update(Contact contact)
        {
            throw new NotImplementedException();
        }
    }
}
