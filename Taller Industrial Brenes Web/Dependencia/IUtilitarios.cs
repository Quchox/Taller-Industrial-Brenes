namespace Taller_Industrial_Brenes_Web.Dependencia
{
    public interface IUtilitarios
    {
        Task<HttpResponseMessage> ConsultarUsuarioAdmin(long UsuarioID);
    }
}