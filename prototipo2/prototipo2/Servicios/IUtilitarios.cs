using prototipo2.Models;

namespace prototipo2.Servicios
{
    public interface IUtilitarios
    {
    
        string GenerarContrasenna(int longitud);
        string GenerarToken(long IdUsuario);
        string Encrypt(string texto);
        void EnviarCorreo(string destinatario, string asunto, string cuerpo);
    }
}
