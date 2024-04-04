using Newtonsoft.Json;
using Soltec.Suscripcion.Model;
using System.Net.Http.Headers;

namespace Soltec.Suscripcion.Service
{
    public interface ISujetoService : IServiceBase
    {
        IList<Sujeto> List();
        Sujeto FindOne(string id);
    }    
    public class SujetoService : ServiceBase, ISujetoService
    {   
        public IList<Sujeto> List() 
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));            
            client.DefaultRequestHeaders.Add("ApiKey", this.ApiKey);
            string methodUrl = this.baseUrl + "/api/contabilidad/sujeto/";
            var response = client.GetAsync(methodUrl).Result;
            IList<Sujeto> result = null;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {               
                var contents = response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<IList<Sujeto>>(contents.Result);               
            }
            return result;              

        }
        public Sujeto FindOne(string id)
        {
            if (string.IsNullOrEmpty(id)) 
            {
                return null;
            }
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("ApiKey", this.ApiKey);
            string methodUrl = this.baseUrl + "/api/contabilidad/sujeto/" + id;
            var response = client.GetAsync(methodUrl).Result;
            Sujeto result = null;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var contents = response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<Sujeto>(contents.Result);
            }
            return result;

        }


    }
}
