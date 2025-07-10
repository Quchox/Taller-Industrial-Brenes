using System.Net.Http.Headers;

namespace Taller_Industrial_Brenes_Web.Dependencia
{
    public class Utilitarios : IUtilitarios
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _accessor;
        private readonly string _apiUrl;

        public Utilitarios(IHttpClientFactory httpClient, IConfiguration config, IHttpContextAccessor accessor)
        {
            _httpClient = httpClient;
            _config = config;
            _accessor = accessor;
            _apiUrl = _config["ApiUrl"];

        }

        public async Task<HttpResponseMessage> ConsultarClientesAdmin(long UsuarioID)
        {
            using (var api = _httpClient.CreateClient())
            {
                var url = $"{_apiUrl.TrimEnd('/')}/Clientes/listado?UsuarioID={UsuarioID}";
                var token = _accessor.HttpContext!.Session.GetString("Token");

                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("⚠️ Token no encontrado en la sesión.");
                }

                api.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                return await api.GetAsync(url);
            }
        }
    }
}