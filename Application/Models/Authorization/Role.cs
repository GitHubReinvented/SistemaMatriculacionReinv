using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Authorization
{
    public static class Role
    {
        //Almacenar valores constantes de roles
        public const string SUPERADMIN = nameof(SUPERADMIN);
        public const string ADMIN = nameof(ADMIN);
        public const string USER = nameof(USER);
        public const string REPRESENTANTE = nameof(REPRESENTANTE);
        public const string ESTUDIANTE = nameof(ESTUDIANTE);
        public const string PADRE = nameof(PADRE);
        public const string MADRE = nameof(MADRE);
        public const string GUARDIA = nameof(GUARDIA);
    }
}
