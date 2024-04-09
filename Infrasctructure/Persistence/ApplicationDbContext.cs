using Domain;
using Domain.Common;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Infrasctructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<Usuario>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        //Clase de Auditoria
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var userName = "system";
            foreach (var entry in ChangeTracker.Entries<BaseDomainModel>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = DateTime.Now;
                        entry.Entity.CreatedBy = userName;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedDate = DateTime.Now;
                        entry.Entity.LastModifiedBy = userName;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);


        }



        public DbSet<AlumnoTransporte> AlumnosTransporte { get; set; }
        public DbSet<AplicaDescuento> DescuentosAplicados { get; set; }
        public DbSet<AreaAcademica> AreasAcademicas { get; set; }
        public DbSet<AutorizacionRetiro> AutorizacionesRetiro { get; set; }
        public DbSet<Banco> Bancos { get; set; }
        public DbSet<BotonPago> BotonesPago { get; set; }
        public DbSet<BotonPagoSucursal> BotonesPagoSucursal { get; set; }
        public DbSet<Calificacion> Calificaciones { get; set; }
        public DbSet<CicloEscolar> CiclosEscolares { get; set; }
        public DbSet<CuentaSucursal> CuentaSucursals { get; set; }
        public DbSet<Descuento> Descuentos { get; set; }
        public DbSet<DescuentoMatricula> DescuentosMatricula { get; set; }
        public DbSet<DetalleRuta> DetallesRuta { get; set; }
        public DbSet<Dia> Dias { get; set; }
        public DbSet<Direccion> Direcciones { get; set; }
        public DbSet<EstadoCuota> EstadosCuota { get; set; }
        public DbSet<EstadoMatricula> EstadosMatricula { get; set; }
        public DbSet<FichaMedica> FichasMedicas { get; set; }
        public DbSet<FichaObservacionMedica> FichasObservacionesMedicas { get; set; }
        public DbSet<Habilidad> Habilidades { get; set; }
        public DbSet<HistorialMedico> HistorialesMedicos { get; set; }
        public DbSet<Matricula> Matriculas { get; set; }
        public DbSet<NivelEscolar> NivelesEscolares { get; set; }
        public DbSet<NivelArea> NivelArea { get; set; }
        public DbSet<ObligacionPendiente> ObligacionesPendientes { get; set; }
        public DbSet<ObservacionMedica> ObservacionesMedicas { get; set; }
        public DbSet<Paralelo> Paralelos { get; set; }
        public DbSet<Persona> Personas { get; set; }
        public DbSet<Plan> Planes { get; set; }
        public DbSet<Profesion> Profesiones { get; set; }
        public DbSet<Receta> Recetas { get; set; }
        public DbSet<RegistroPagos> RegistrosPagos { get; set; }
        public DbSet<Relacion> Relaciones { get; set; }
        public DbSet<RolTarea> RolesTarea { get; set; }
        public DbSet<RolUsuario> RolesUsuarios { get; set; }
        public DbSet<Rubro> Rubros { get; set; }
        public DbSet<Ruta> Rutas { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<SubHabilidad> SubHabilidades { get; set; }
        public DbSet<Sucursal> Sucursales { get; set; }
        public DbSet<Tarea> Tareas { get; set; }
        public DbSet<TipoCalificacion> TiposCalificacion { get; set; }
        public DbSet<TipoCuenta> tipoCuentas { get; set; }
        public DbSet<TipoDireccion> TiposDireccion { get; set; }
        public DbSet<TipoIdentificacion> TiposIdentificacion { get; set; }
        public DbSet<TipoPago> TiposPago { get; set; }
        public DbSet<TipoPeriodo> TiposPeriodo { get; set; }
        public DbSet<TipoEstudiante> TiposEstudiante { get; set; }
        public DbSet<TipoRelacion> TiposRelacion { get; set; }
        public DbSet<TipoServicio> TiposServicio { get; set; }
        public DbSet<Transportista> Transportistas { get; set; }
        public DbSet<UsuarioSucursal> UsuariosSucursal { get; set; }
        public DbSet<ValorCalificacion> ValoresCalificacion { get; set; }
        //public DbSet<Rol> aspnetroles { get; set; }
        public DbSet<JustificaNoSeguro> JustificaNoSeguro { get; set; }

        public DbSet<Documento> Documentos { get; set; }
        public DbSet<EstadoPago> EstadosPago { get; set; }  
        public DbSet<TipoRubro> TiposRubro { get; set; }
        public DbSet<EstudianteInscripcion> EstudiantesInscripcion { get; set; }



    }
}
