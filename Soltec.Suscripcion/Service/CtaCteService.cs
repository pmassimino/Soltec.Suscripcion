using Newtonsoft.Json;
using Soltec.Suscripcion.Model;
using System.Net.Http.Headers;

namespace Soltec.Suscripcion.Service
{
    public interface ICtaCteService : IServiceBase
    {
        decimal Saldo(string idCuenta, string idCuentaMayor, DateTime fecha);
        Int32 DiasDeuda(string idCuenta, string idCuentaMayor);
        IList<MovCtaCte> List(string idCuenta, string idCuentaMayor,DateTime fecha, DateTime fechaHasta);
    }    
    public class CtaCteService :ServiceBase, ICtaCteService
    {   
        public decimal Saldo(string idCuenta,string idCuentaMayor,DateTime fecha) 
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));            
            client.DefaultRequestHeaders.Add("ApiKey", this.ApiKey);
            string methodUrl = this.baseUrl + "/api/contabilidad/ctacte/" + idCuenta +"/saldo?idCuentaMayor=" + idCuentaMayor;
            var response = client.GetAsync(methodUrl).Result;
            decimal saldo = 0;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {               
                var contents = response.Content.ReadAsStringAsync();
                saldo = JsonConvert.DeserializeObject<decimal>(contents.Result);               
            }
            return saldo;              

        }
        public Int32 DiasDeuda(string idCuenta, string idCuentaMayor)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("ApiKey", this.ApiKey);
            string methodUrl = this.baseUrl + "/api/contabilidad/ctacte/" + idCuenta + "/diasdeuda?idCuentaMayor=" + idCuentaMayor;
            var response = client.GetAsync(methodUrl).Result;
            Int32 dias = 0;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var contents = response.Content.ReadAsStringAsync();
                dias = JsonConvert.DeserializeObject<Int32>(contents.Result);
            }
            return dias;

        }
        public IList<MovCtaCte> List(string idCuenta, string idCuentaMayor, DateTime fecha, DateTime fechaHasta)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("ApiKey", this.ApiKey);
            string fechaString = fecha.ToString("MM-dd-yyyy");
            string fechaHastaString = fechaHasta.ToString("MM-dd-yyyy");
            string methodUrl = this.baseUrl + "/api/contabilidad/ctacte/" + idCuenta + "/?IdCuentaMayor=" + 
                               idCuentaMayor + "&fecha=" + fechaString + "&fechaHasta=" + fechaHastaString; 
            var response = client.GetAsync(methodUrl).Result;
            IList<MovCtaCte> result = null;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var contents = response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<IList<MovCtaCte>>(contents.Result);
            }
            return result;
        }

    }
}
