namespace Taller_Industrial_Brenes_Web.Dependencia
{
    public interface IUtilitarios
    {
        Task<HttpResponseMessage> ConsultarClientesAdmin(long UsuarioID);
    }
}