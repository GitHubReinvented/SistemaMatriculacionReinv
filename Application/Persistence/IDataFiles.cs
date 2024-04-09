using Application.Models.File;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Persistence
{
    public interface IDataFiles
    {
        Task Borrar(string ruta, string contenedor);
        Task<FileResultProfile> SavePhoto(string contenedor, IFormFile perfil);
    }
}
