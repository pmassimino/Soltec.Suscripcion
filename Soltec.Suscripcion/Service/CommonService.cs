using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Soltec.Suscripcion.Service
{
    public interface ICommonService:IServiceBase 
    {
        bool IsRunning();
    }
    public class CommonService:ServiceBase,ICommonService
    {        
        public bool IsRunning() 
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("ApiKey", this.ApiKey);
            string methodUrl = this.baseUrl + "/api/isRuning";
            var response = client.GetAsync(methodUrl).Result;
            Boolean result = false;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = true;
            }
            return result;
        }
        
        
    }
}
