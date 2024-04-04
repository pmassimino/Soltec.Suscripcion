using System.IdentityModel.Tokens.Jwt;

namespace Soltec.Suscripcion.Code
{
    public interface ISessionService
    {       
        int IdUsuario { get; set; }
    }
    public class SessionService : ISessionService
    {
        public SessionService(IHttpContextAccessor httpContentAccessor)
        {

            if (httpContentAccessor.HttpContext.Request.Headers.TryGetValue("IdEmpresa", out var idEmpresa))
            {
                this.IdEmpresa = Guid.Parse(idEmpresa);
            }
            if (httpContentAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out var Authorization))
            {
                this.Decodejws(Authorization);
            }
        }
        public Guid IdEmpresa { get; set; }
        public int IdUsuario { get; set; }

        private void Decodejws(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            string authHeader = token;
            authHeader = authHeader.Replace("Bearer ", "");
            var jsonToken = handler.ReadToken(authHeader);
            var tokens = handler.ReadToken(authHeader) as JwtSecurityToken;
            var id = tokens.Claims.First(claim => claim.Type == "idUsuario").Value;
            this.IdUsuario = Convert.ToInt32(id);
        }

    }
}
