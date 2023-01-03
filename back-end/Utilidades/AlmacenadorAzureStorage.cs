using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace back_end.Utilidades
{
    public class AlmacenadorAzureStorage : IAlmacenadorAzureStorage
    {
        private string connectionString;
        public AlmacenadorAzureStorage(IConfiguration configuration)
        {
            this.connectionString = configuration.GetConnectionString("AzureStorage");
        }

        public async Task<string> GuadarArchivo(string contenedor, IFormFile archivo)
        {
            var client = new BlobContainerClient(this.connectionString, contenedor);
            await client.CreateIfNotExistsAsync();

            client.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

            var extension = Path.GetExtension(archivo.FileName);
            var archivoNombre = $"{Guid.NewGuid()}{extension}";
            var blob = client.GetBlobClient(archivoNombre);

            await blob.UploadAsync(archivo.OpenReadStream());

            return blob.Uri.ToString();
        }

        public async Task BorrarArchivo(string ruta, string contenedor)
        {
            if (string.IsNullOrEmpty(ruta)) return;

            var client = new BlobContainerClient(this.connectionString, contenedor);
            await client.CreateIfNotExistsAsync();

            var archivo = Path.GetFileName(ruta);
            var blob = client.GetBlobClient(archivo);

            await blob.DeleteIfExistsAsync();
        }

        public async Task<string> EditarArchivo(string contenedor, IFormFile archivo, string ruta)
        {
            await this.BorrarArchivo(ruta, contenedor);

            return await this.GuadarArchivo(contenedor, archivo);
        }
    }
}
