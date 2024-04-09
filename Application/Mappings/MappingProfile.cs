using Application.Models;
using Application.Models.User;
using AutoMapper;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {

            CreateMap<Usuario, UsuarioMap>();
            CreateMap<Persona, PersonaMap>();
            CreateMap<PersonaMap, Persona>();

        }
    }
}
