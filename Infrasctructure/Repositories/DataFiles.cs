using Application.Models.File;
using Application.Persistence;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrasctructure.Repositories
{
    public class DataFiles : IDataFiles
    {
        public Task Borrar(string ruta, string contenedor)
        {
            throw new NotImplementedException();
        }

        public Task<FileResultProfile> SavePhoto(string contenedor, IFormFile perfil)
        {
            throw new NotImplementedException();
        }
    }
}
