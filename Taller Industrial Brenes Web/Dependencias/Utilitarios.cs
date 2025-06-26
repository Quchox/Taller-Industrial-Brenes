using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;

namespace Taller_Industrial_Brenes_Web.Dependencias
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

        public HttpResponseMessage ConsultarClientesAdmin(long UsuarioID)
        {
            using (var api = _httpClient.CreateClient())
            {
                var url = $"{_apiUrl.TrimEnd('/')}/Admin/ListadoAdmin?Id={UsuarioID}";
                api.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessor.HttpContext!.Session.GetString("Token"));

                var response = api.GetAsync(url).Result;

                return response;
            }
        }


    }
}
