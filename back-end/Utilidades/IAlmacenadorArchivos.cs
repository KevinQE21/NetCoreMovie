using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace back_end.Utilidades
{
    public interface IAlmacenadorAzureStorage
    {
        Task BorrarArchivo(string ruta, string contenedor);
        Task<string> EditarArchivo(string contenedor, IFormFile archivo, string ruta);
        Task<string> GuadarArchivo(string contenedor, IFormFile archivo);
    }
}