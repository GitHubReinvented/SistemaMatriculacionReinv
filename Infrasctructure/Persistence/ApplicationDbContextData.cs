using Application.Models.Authorization;
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Infrasctructure.Persistence
{
    public class ApplicationDbContextData
    {
        public static async Task LoadDataAsync(
            ApplicationDbContext context,
            UserManager<Usuario> userManager,
            RoleManager<RolUsuario> roleManager,
            ILoggerFactory loggerFactory)
        {
            try
            {
                if (!context.TiposEstudiante.Any())
                {
                    var listTipoPersonas = new List<TipoEstudiante>{
                         new TipoEstudiante
                         {
                             Descripcion = "Estudiante Fundador"
                         },
                          new TipoEstudiante
                         {
                             Descripcion = "Estudiante Hijo Staff"
                         },
                         new TipoEstudiante
                         {
                             Descripcion = "Estudiante Standard"
                         },
                    };
                    await context.AddRangeAsync(listTipoPersonas);
                    await context.SaveChangesAsync();
                }

                if (!context.TiposPago.Any())
                {
                    var listTipoPagos = new List<TipoPago>{
                         new TipoPago
                         {
                             Descripcion = "Transferencia"
                         },
                          new TipoPago
                         {
                             Descripcion = "Tarjeta"
                         },

                    };
                    await context.AddRangeAsync(listTipoPagos);
                    await context.SaveChangesAsync();
                }

                if (!context.EstadosPago.Any())
                {
                    var listEstadoPago = new List<EstadoPago>{
                         new EstadoPago
                         {
                             Descripcion = "Reversado"
                         },
                          new EstadoPago
                         {
                             Descripcion = "Por conciliar"
                         },
                           new EstadoPago
                         {
                             Descripcion = "Pagada"//código en mismo orden que en estado cuota
                         },
                             new EstadoPago
                         {
                             Descripcion = "Anulada"//código en mismo orden que en estado cuota
                         },
                    };
                    await context.AddRangeAsync(listEstadoPago);
                    await context.SaveChangesAsync();
                }


                if (!context.Sucursales.Any())
                {
                    var sucursalPuembo = new Sucursal
                    {
                        Id = 1,
                        Nombre = "Puembo",
                        Direccion = "Puembo",
                        Telefono = "Puembo",
                        Email = "secretaria.general@reinventedpuembo.edu.ec.",
                        RazonSocial= "Red Valles EDURED Cia. Ltda.",
                        RUC= "1793074618001",
                        EmailFacturas= "finanzas@reinventedpuembo.edu.ec",
                        color1 = "#40CCA1",
                        color2 = "#FF8F1F",
                        color3 = "#F7D159",
                        color4 = "#6B5EC8"
                    };

                    await context.AddAsync(sucursalPuembo);
                    var sucursalSC = new Sucursal
                    {
                        Id = 2,
                        Nombre = "Santa Clara",
                        Direccion = "Santa Clara",
                        Telefono = "Santa Clara",
                        Email = "secretaria.general@reinventedsantaclara.edu.ec.",
                        RazonSocial = "Educación Santa Clara Edusantaclara S.A.S.",
                        RUC = "1793196314001",
                        EmailFacturas = "facturas@reinventedsantaclara.edu.ec",
                        color1 = "#90CA46",
                        color2 = "#127DFF",
                        color3 = "#EB1CBF",
                        color4 = "#FFD108"
                    };

                    await context.AddAsync(sucursalSC);
                    await context.SaveChangesAsync();
                }

                if (!context.Profesiones.Any())
                {
                    var listTipoPersonas = new List<Profesion>{
                         new Profesion
                         {
                             Descripcion = "Ing. Civil"
                         },
                          new Profesion
                         {
                             Descripcion = "Medico"
                         },
                         new Profesion
                         {
                             Descripcion = "Comerciante"
                         },
                         new Profesion
                         {
                             Descripcion = "Abogado"
                         },
                    };
                    await context.AddRangeAsync(listTipoPersonas);
                    await context.SaveChangesAsync();
                }

                if (!context.TiposRelacion.Any())
                {
                    var listTipoPersonas = new List<TipoRelacion>{
                         new TipoRelacion
                         {
                             Descripcion = "Madre"
                         },
                          new TipoRelacion
                         {
                             Descripcion = "Padre"
                         },
                         new TipoRelacion
                         {
                             Descripcion = "Apoderado"
                         },
                         new TipoRelacion
                         {
                             Descripcion = "Hermano(a)"
                         },
                          new TipoRelacion
                         {
                             Descripcion = "Primo(a)"
                         },
                         new TipoRelacion
                         {
                             Descripcion = "Abuelo(a)"
                         },
                         new TipoRelacion
                         {
                             Descripcion = "Tío(a)"
                         },
                         new TipoRelacion
                         {
                             Descripcion = "Emplada doméstica"
                         }
                         ,
                         new TipoRelacion
                         {
                             Descripcion = "Otro"
                         }
                    };
                    await context.AddRangeAsync(listTipoPersonas);
                    await context.SaveChangesAsync();
                }


                if (!context.TiposIdentificacion.Any())
                {
                    var listTipoPersonas = new List<TipoIdentificacion>{
                         new TipoIdentificacion
                         {
                             Descripcion = "CEDULA"
                         },
                          new TipoIdentificacion
                         {
                             Descripcion = "PASAPORTE"
                         }
                    };
                    await context.AddRangeAsync(listTipoPersonas);
                    await context.SaveChangesAsync();
                }

                if (!context.TiposDireccion.Any())
                {
                    var listTipoDireccion = new List<TipoDireccion>{
                         new TipoDireccion
                         {
                             Descripcion = "Domicilio"
                         },
                          new TipoDireccion
                         {
                             Descripcion = "Trabajo"
                         },
                          new TipoDireccion
                         {
                             Descripcion = "Familiar"
                         },
                          new TipoDireccion
                         {
                             Descripcion = "Guardería"
                         },
                         new TipoDireccion
                         {
                             Descripcion = "Centro Tareas dirigidas"
                         }
                    };
                    await context.AddRangeAsync(listTipoDireccion);
                    await context.SaveChangesAsync();
                }

                if (!context.EstadosMatricula.Any())
                {
                    var listTipoPersonas = new List<EstadoMatricula>{
                         new EstadoMatricula
                         {
                             Descripcion = "Inscripción"
                         },
                          new EstadoMatricula
                         {
                             Descripcion = "Inscripción rechazada"
                         },
                          new EstadoMatricula
                         {
                             Descripcion = "Registrada"
                         },
                          new EstadoMatricula
                         {
                             Descripcion = "Aceptada"
                         },
                         new EstadoMatricula
                         {
                             Descripcion = "Vigente"
                         }
                         ,
                         new EstadoMatricula
                         {
                             Descripcion = "Anulada"
                         }
                    };
                    await context.AddRangeAsync(listTipoPersonas);
                    await context.SaveChangesAsync();
                }

                if (!context.NivelesEscolares.Any())
                {
                    var listTipoPersonas = new List<NivelEscolar>{
                         new NivelEscolar
                         {
                             Descripcion = "Nest (Inicial 1)",
                             Orden = 10
                         },
                          new NivelEscolar
                         {
                             Descripcion = "PreKinder (Inicial 2)",
                             Orden = 20

                         },
                          new NivelEscolar
                         {
                             Descripcion = "Kinder (Primero EGB)",
                             Orden = 30

                         },
                          new NivelEscolar
                         {
                             Descripcion = "1st Grade (Segundo EGB)",
                             Orden = 40

                         },
                         new NivelEscolar
                         {
                             Descripcion = "2nd Grade (Tercero EGB)",
                             Orden = 50

                         },
                         new NivelEscolar
                         {
                             Descripcion = "3rd Grade (Cuarto EGB)",
                             Orden = 60

                         },
                         new NivelEscolar
                         {
                             Descripcion = "4th Grade (Quinto EGB)",
                             Orden = 70

                         },
                         new NivelEscolar
                         {
                             Descripcion = "5th Grade (Sexto EGB)",
                             Orden = 80

                         },
                         new NivelEscolar
                         {
                             Descripcion = "6th Grade (Séptimo EGB)",
                             Orden = 90

                         },
                         new NivelEscolar
                         {
                             Descripcion = "7th Grade (Octavo EGB)",
                             Orden = 100

                         }
                    };

                    await context.AddRangeAsync(listTipoPersonas);
                    await context.SaveChangesAsync();
                }


                if (!context.TiposPeriodo.Any())
                {
                    var listTipoPersonas = new List<TipoPeriodo>{
                         new TipoPeriodo
                         {
                             Descripcion = "MENSUAL"
                         },
                          new TipoPeriodo
                         {
                             Descripcion = "SEMESTRAL"
                         }
                    };
                    await context.AddRangeAsync(listTipoPersonas);
                    await context.SaveChangesAsync();
                }

                if (!context.TiposServicio.Any())
                {
                    var listTipoServicios = new List<TipoServicio>{
                         new TipoServicio
                         {
                             Descripcion = "Alimentacion"
                         },
                          new TipoServicio
                         {
                             Descripcion = "Transporte"
                         },
                         new TipoServicio
                         {
                             Descripcion = "Seguro"
                         },
                         new TipoServicio
                         {
                             Descripcion = "Uniformes"
                         },
                    };
                    await context.AddRangeAsync(listTipoServicios);
                    await context.SaveChangesAsync();
                }

                //Servicios
                if (!context.Servicios.Any())
                {
                    var AlimentoPuembo = new Servicio
                    {
                        Descripcion = "",
                        Periodos = 2023,
                        CostoPeriodo = 30,
                        Estado = true,
                        Nombre = "Alimentos",
                        Link = "",
                        TipoServicioId = 1,
                        SucursalId = 1, //Puembo                      
                    };

                    await context.AddAsync(AlimentoPuembo);

                    var AlimentoSC = new Servicio
                    {
                        Descripcion = "",
                        Periodos = 2023,
                        CostoPeriodo = 30,
                        Estado = true,
                        Nombre = "Alimentos",
                        Link = "",
                        TipoServicioId = 1,
                        SucursalId = 2, //Santa Clara                      
                    };

                    await context.AddAsync(AlimentoSC);
                    var TransporteP = new Servicio
                    {
                        Descripcion = "",
                        Periodos = 2023,
                        CostoPeriodo = 30,
                        Estado = true,
                        Nombre = "Transporte",
                        Link = "",
                        TipoServicioId = 2,
                        SucursalId = 1, //Puembo                      
                    };

                    await context.AddAsync(TransporteP);
                    var TransporteSC = new Servicio
                    {
                        Descripcion = "",
                        Periodos = 2023,
                        CostoPeriodo = 30,
                        Estado = true,
                        Nombre = "Transporte",
                        Link = "",
                        TipoServicioId = 2,
                        SucursalId = 2, //SC                     
                    };

                    await context.AddAsync(TransporteSC);
                    var SeguroP = new Servicio
                    {
                        Descripcion = "",
                        Periodos = 2023,
                        CostoPeriodo = 30,
                        Estado = true,
                        Nombre = "Seguro",
                        Link = "",
                        TipoServicioId = 3,
                        SucursalId = 1, //Puembo                      
                    };

                    await context.AddAsync(SeguroP);
                    var SeguroSC = new Servicio
                    {
                        Descripcion = "",
                        Periodos = 2023,
                        CostoPeriodo = 30,
                        Estado = true,
                        Nombre = "Seguro",
                        Link = "",
                        TipoServicioId = 3,
                        SucursalId = 2, //SC                     
                    };

                    await context.AddAsync(SeguroSC);
                    var UniformeP = new Servicio
                    {
                        Descripcion = "",
                        Periodos = 2023,
                        CostoPeriodo = 30,
                        Estado = true,
                        Nombre = "Uniforme",
                        Link = "",
                        TipoServicioId = 4,
                        SucursalId = 1, //Puembo                      
                    };

                    await context.AddAsync(UniformeP);
                    var UniformeSC = new Servicio
                    {
                        Descripcion = "",
                        Periodos = 2023,
                        CostoPeriodo = 30,
                        Estado = true,
                        Nombre = "Uniforme",
                        Link = "",
                        TipoServicioId = 4,
                        SucursalId = 2, //SC                     
                    };

                    await context.AddAsync(UniformeSC);
                    await context.SaveChangesAsync();

                }


                if (!context.Bancos.Any())
                {
                    var BancoPichincha = new Banco
                    {
                        Id = 1,
                        Nombre = "Banco Pichincha"
                    };

                    await context.AddAsync(BancoPichincha);
                    var BancoGuayaquil = new Banco
                    {
                        Id = 2,
                        Nombre = "Banco Guayaquil"
                    };

                    await context.AddAsync(BancoGuayaquil);
                    var BancoProdubanco = new Banco
                    {
                        Id = 3,
                        Nombre = "Banco Produbanco"
                    };

                    await context.AddAsync(BancoProdubanco);

                    var BancoPacifico = new Banco
                    {
                        Id = 4,
                        Nombre = "Banco Pacifico"
                    };

                    await context.AddAsync(BancoPacifico);

                    var BancoAustro = new Banco
                    {
                        Id = 5,
                        Nombre = "Banco del Austro"
                    };

                    await context.AddAsync(BancoAustro);

                    var BancoLoja = new Banco
                    {
                        Id = 6,
                        Nombre = "Banco de Loja"
                    };

                    await context.AddAsync(BancoLoja);

                    var BancoInternacional = new Banco
                    {
                        Id = 7,
                        Nombre = "Banco Internacional"
                    };

                    await context.AddAsync(BancoInternacional);

                    await context.SaveChangesAsync();

                }

                if (!context.tipoCuentas.Any())
                {
                    var Ahorro = new TipoCuenta
                    {
                        Id = 1,
                        Descripcion = "Ahorro"
                    };

                    await context.AddAsync(Ahorro);
                    var Corriente = new TipoCuenta
                    {
                        Id = 2,
                        Descripcion = "Corriente"
                    };

                    await context.AddAsync(Corriente);
                    await context.SaveChangesAsync();
                }

                if (!context.CuentaSucursals.Any())
                {
                    var registro = new CuentaSucursal
                    {
                        Id = 1,
                        SucursalId = 1,
                        TipoCuentaId = 2,
                        BancoId = 3,
                        NumeroCuenta = "2005271750",
                        Estado = true,

                    };

                    await context.AddAsync(registro);

                    var registro2 = new CuentaSucursal
                    {
                        Id = 2,
                        SucursalId = 2,
                        TipoCuentaId = 2,
                        BancoId = 2,
                        NumeroCuenta = "41282630",
                        Estado = true,

                    };
                    await context.AddAsync(registro2);
                    await context.SaveChangesAsync();
                }


                if (!context.BotonesPago.Any())
                {
                    var registro = new BotonPago
                    {
                        Id = 1,
                        Descripcion="Pago Medios"

                    };

                    await context.AddAsync(registro);

                    var registro2 = new BotonPago
                    {
                        Id = 2,
                        Descripcion = "PayPhone"

                    };
                    await context.AddAsync(registro2);
                    await context.SaveChangesAsync();
                }

                if (!context.BotonesPagoSucursal.Any())
                {
                    var registro = new BotonPagoSucursal
                    {
                        BotonPagoId = 1,//Abitmedia o pago medios
                        SucursalId = 1,
                        TokenPruebas = "3wv1x3b0eyc5zj8vxnqaiqaeiutgi7pphk4p0nbtrekg-gcpdrzsnlxihqhxgb7vszqlo",
                        TokenProduccion = "v8qixjj-e27yhkmx0hnzudbj6-vedutmsjndmpc6m2x-4gpkeqikoc-vve1nwsl1i8zn1"

                    };

                    await context.AddAsync(registro);

                    var registro2 = new BotonPagoSucursal
                    {
                        BotonPagoId = 1,
                        SucursalId = 2,
                        TokenPruebas = "3wv1x3b0eyc5zj8vxnqaiqaeiutgi7pphk4p0nbtrekg-gcpdrzsnlxihqhxgb7vszqlo",
                        TokenProduccion = "5u4ckk12cojyy5jy2qi4dwmmwakyyy-rqhewiipswei6kejxoxnh-tezcqp9weghrbzzr"

                    };
                    await context.AddAsync(registro2);
                    await context.SaveChangesAsync();
                }



                if (!context.Documentos.Any())
                {
                    var registro = new List<Documento>
                    {
                         new Documento
                         {
                             Nombre = "Contrato",
                             Estado = true,
                             SucursalId = 1,
                         }
                    };
                    await context.AddRangeAsync(registro);
                    await context.SaveChangesAsync();
                }

                if (!context.CiclosEscolares.Any())
                {
                    var listTipoPersonas = new List<CicloEscolar>{
                         new CicloEscolar
                         {
                             FechaInicio = new DateTime(2023,09,04),
                             FechaFin = new DateTime(2024,06,28),
                             TipoPeriodoId = 1,
                             NumeroPeriodos= 10,
                             SucursalId = 1,
                             Estado=true,
                         }
                    };
                    await context.AddRangeAsync(listTipoPersonas);
                    await context.SaveChangesAsync();

                    var CicloSC = new List<CicloEscolar>{
                         new CicloEscolar
                         {
                             FechaInicio = new DateTime(2023,09,04),
                             FechaFin = new DateTime(2024,06,28),
                             TipoPeriodoId = 1,
                             NumeroPeriodos= 10,
                             SucursalId = 2,
                             Estado=true,
                         }
                    };
                    await context.AddRangeAsync(CicloSC);
                    await context.SaveChangesAsync();
                }

                if (!context.Paralelos.Any())
                {
                    var listTipoPersonas = new List<Paralelo>{
                         new Paralelo
                         {
                           Nemonico = 'A',
                           NivelEscolarId = 1,
                           CicloEscolarId = 1,
                         },
                         new Paralelo
                         {
                           Nemonico = 'A',
                           NivelEscolarId = 2,
                           CicloEscolarId = 1,
                         },
                         new Paralelo
                         {
                           Nemonico = 'B',
                           NivelEscolarId = 2,
                           CicloEscolarId = 1,
                         },
                         new Paralelo
                         {
                           Nemonico = 'A',
                           NivelEscolarId = 3,
                           CicloEscolarId = 1,
                         },
                         new Paralelo
                         {
                           Nemonico = 'B',
                           NivelEscolarId = 3,
                           CicloEscolarId = 1,
                         },
                         new Paralelo
                         {
                           Nemonico = 'A',
                           NivelEscolarId = 4,
                           CicloEscolarId = 1,
                         },
                         new Paralelo
                         {
                           Nemonico = 'A',
                           NivelEscolarId = 5,
                           CicloEscolarId = 1,
                         }
                    };
                    await context.AddRangeAsync(listTipoPersonas);
                    await context.SaveChangesAsync();
                }

                if (!context.ObservacionesMedicas.Any())
                {
                    var listTipoPersonas = new List<ObservacionMedica>{
                         new ObservacionMedica
                         {
                             Descripcion = "¿El/la estudiante está vacunado/a contra covid 19?",
                             Estado = true
                         },
                          new ObservacionMedica
                         {
                             Descripcion = "¿Tiene el/la estudiante alguna enfermedad crónica?",
                             Estado = true

                         },
                          new ObservacionMedica
                         {
                             Descripcion = "¿Recibe el/la estudiante medicación continua?",
                             Estado = true

                         },
                          new ObservacionMedica
                         {
                             Descripcion = "¿El/la estudiante ha sido sometido/a a alguna cirugía?",
                             Estado = true

                         }
                    };
                    await context.AddRangeAsync(listTipoPersonas);
                    await context.SaveChangesAsync();
                }


                if (!context.Dias.Any())
                {
                    var listTipoPersonas = new List<Dia>{
                         new Dia
                         {
                             Nombre = "Lunes"
                         },
                          new Dia
                         {
                             Nombre = "Martes"
                         },
                          new Dia
                         {
                             Nombre = "Miércoles"
                         },
                          new Dia
                         {
                             Nombre = "Jueves"
                         },
                          new Dia
                         {
                             Nombre = "Viernes"
                         }
                    };
                    await context.AddRangeAsync(listTipoPersonas);
                    await context.SaveChangesAsync();
                }

                if (!context.EstadosCuota.Any())
                {
                    var listTipoPersonas = new List<EstadoCuota>{
                         new EstadoCuota
                         {
                             Descripcion = "Generada"
                         },
                          new EstadoCuota
                         {
                             Descripcion = "Pendiente"
                         },
                          new EstadoCuota
                         {
                             Descripcion = "Pagada"
                         },
                          new EstadoCuota
                         {
                             Descripcion = "Anulada"
                         },
                          new EstadoCuota
                         {
                             Descripcion = "Por conciliar transferencia"
                         },
                          new EstadoCuota
                         {
                             Descripcion = "Rechazo seguro"
                         }
                          ,
                          new EstadoCuota
                         {
                             Descripcion = "Reversado"
                         }

                    };
                    await context.AddRangeAsync(listTipoPersonas);
                    await context.SaveChangesAsync();
                }

                if (!context.Rubros.Any())
                {
                    var listRegsitros = new List<TipoRubro>{
                         new TipoRubro
                         {

                             Descripcion = "Matrícula"

                         },
                          new TipoRubro
                         {

                             Descripcion ="Pensión"

                         }
                    };
                    await context.AddRangeAsync(listRegsitros);
                    await context.SaveChangesAsync();
                }


                if (!context.Rubros.Any())
                {
                    var list = new List<Rubro>{
                         new Rubro
                         {
                             CicloEscolarId = 1,
                             TipoRubroId = 1,//matrícula
                             Costo = 300,
                         },
                          new Rubro
                         {
                             CicloEscolarId = 1,
                             TipoRubroId = 2,//pensión
                             Costo = 600,
                         },
                          new Rubro
                          {
                             CicloEscolarId = 1,
                             TipoRubroId = 3,//seguro estudiantil
                             Costo = 250,

                          },new Rubro
                         {
                             CicloEscolarId = 2,
                             TipoRubroId = 1,//matrícula
                             Costo = 300,
                         },
                          new Rubro
                         {
                             CicloEscolarId = 2,
                             TipoRubroId = 2,//pensión
                             Costo = 600,
                         },
                          new Rubro
                          {
                             CicloEscolarId = 2,
                             TipoRubroId = 3,//seguro estudiantil
                             Costo = 250,

                          }
                    };
                    await context.AddRangeAsync(list);
                    await context.SaveChangesAsync();
                }

                if (!context.Descuentos.Any())
                {
                    var listTipoPersonas = new List<Descuento>{
                         new Descuento
                         {
                             Descripcion = "Descuento por fundador",
                         },
                         new Descuento
                         {
                             Descripcion = "Descuento por hijo staff",
                         },
                         new Descuento
                         {
                             Descripcion = "Descuento por hermano menor",
                         },
                         new Descuento
                         {
                             Descripcion = "Descuento por ayuda financiera 10%",
                         },
                         new Descuento
                         {
                             Descripcion = "Descuento por ayuda financiera 25%",
                         },
                         new Descuento
                         {
                             Descripcion = "Beca total por rendimiento académico",
                         },
                         new Descuento
                         {
                             Descripcion = "Beca media por rendimiento académico",
                         },
                         new Descuento
                         {
                             Descripcion = "Descuento por pago totalidad",
                         }
                         ,
                         new Descuento
                         {
                             Descripcion = "Descuento por grados iniciales",
                         }
                         ,
                         new Descuento
                         {
                             Descripcion = "Descuento por grados iniciales y ayuda financiera",
                         }
                          ,
                         new Descuento
                         {
                             Descripcion = "Descuento por grados iniciales y ayuda financiera",
                         },
                         new Descuento
                         {
                             Descripcion = "Descuento por ayuda financiera 15%",
                         }
                    };
                    await context.AddRangeAsync(listTipoPersonas);
                    await context.SaveChangesAsync();
                }

                if (!context.DescuentosAplicados.Any())
                {
                    var listTipoPersonas = new List<AplicaDescuento>{
                         new AplicaDescuento
                         {
                            DescuentoId = 1,
                            RubroId = 1,
                            Porcentaje = 5,
                            Estado =1
                         },
                         new AplicaDescuento
                         {
                            DescuentoId = 2,
                            RubroId = 1,
                            Porcentaje = 2,
                            Estado =1
                         },
                         new AplicaDescuento
                         {
                            DescuentoId = 3,
                            RubroId = 2,
                            Porcentaje = 10,
                            Estado =1
                         },
                         new AplicaDescuento
                         {
                            DescuentoId = 4,
                            RubroId = 2,
                            Porcentaje = 2,
                            Estado =1
                         },
                         new AplicaDescuento
                         {
                            DescuentoId = 5,
                            RubroId = 2,
                            Porcentaje = 10,
                            Estado =1
                         },
                         new AplicaDescuento
                         {
                            DescuentoId = 6,
                            RubroId = 2,
                            Porcentaje = 100,
                            Estado =1
                         },
                         new AplicaDescuento
                         {
                            DescuentoId = 7,
                            RubroId = 2,
                            Porcentaje = 50,
                            Estado =1
                         },
                         new AplicaDescuento
                         {
                            DescuentoId = 8,
                            RubroId = 2,
                            Porcentaje = 10,
                            Estado =1
                         }

                    };
                    await context.AddRangeAsync(listTipoPersonas);
                    await context.SaveChangesAsync();
                }


                if (!roleManager.Roles.Any())
                {
                    await roleManager.CreateAsync(new RolUsuario() { Name = Role.SUPERADMIN });
                    await roleManager.CreateAsync(new RolUsuario() { Name = Role.ADMIN });
                    await roleManager.CreateAsync(new RolUsuario() { Name = Role.USER });
                    await roleManager.CreateAsync(new RolUsuario() { Name = Role.ESTUDIANTE });
                    await roleManager.CreateAsync(new RolUsuario() { Name = Role.REPRESENTANTE });
                    await roleManager.CreateAsync(new RolUsuario() { Name = Role.MADRE });
                    await roleManager.CreateAsync(new RolUsuario() { Name = Role.PADRE });
                    await roleManager.CreateAsync(new RolUsuario() { Name = Role.GUARDIA });
                }

                if (!userManager.Users.Any())
                {
                    var usuarioSuperAdmin = new Usuario
                    {
                        Nombres = "Andres",
                        Apellidos = "Moya",
                        Email = "andres-moya101@hotmail.com",
                        UserName = "andresmoyajs",
                    };

                    await userManager.CreateAsync(usuarioSuperAdmin, "Anse25CHDs@");
                    await userManager.AddToRoleAsync(usuarioSuperAdmin, Role.SUPERADMIN);


                    await context.UsuariosSucursal.AddAsync(new UsuarioSucursal
                    {
                        UsuarioId = usuarioSuperAdmin.Id,
                        SucursalId = 1
                    });

                    var usuarioAdmin = new Usuario
                    {
                        Nombres = "Andres2",
                        Apellidos = "Moya",
                        Email = "andres2-moya101@hotmail.com",
                        UserName = "andresmoyajs2",
                        Avatar = ""
                    };

                    await userManager.CreateAsync(usuarioAdmin, "Anse25CHDs@");
                    await userManager.AddToRoleAsync(usuarioAdmin, Role.ADMIN);

                    await context.UsuariosSucursal.AddAsync(new UsuarioSucursal
                    {
                        UsuarioId = usuarioAdmin.Id,
                        SucursalId = 1
                    });

                    var usuarioNormal = new Usuario
                    {
                        Nombres = "Diego",
                        Apellidos = "Moya",
                        Email = "diego@hotmail.com",
                        UserName = "diegomoyads",
                        Avatar = ""
                    };

                    await userManager.CreateAsync(usuarioNormal, "Anse25CHDs@");
                    await userManager.AddToRoleAsync(usuarioNormal, Role.REPRESENTANTE);
                    await context.UsuariosSucursal.AddAsync(new UsuarioSucursal
                    {
                        UsuarioId = usuarioNormal.Id,
                        SucursalId = 2
                    });

                    var usuarioNormal2 = new Usuario
                    {
                        Nombres = "Kevin",
                        Apellidos = "Mina",
                        Email = "kevin@hotmail.com",
                        UserName = "KevinMina",
                        Avatar = ""
                    };

                    await userManager.CreateAsync(usuarioNormal2, "Anse25CHDs@");
                    await userManager.AddToRoleAsync(usuarioNormal2, Role.REPRESENTANTE);
                    await context.UsuariosSucursal.AddAsync(new UsuarioSucursal
                    {
                        UsuarioId = usuarioNormal2.Id,
                        SucursalId = 1
                    });

                    var usuarioGuardia = new Usuario
                    {
                        Nombres = "Diego2",
                        Apellidos = "Moya2",
                        Email = "diego2@hotmail.com",
                        UserName = "diegomoyads2",
                        Avatar = ""
                    };

                    await context.UsuariosSucursal.AddAsync(new UsuarioSucursal
                    {
                        UsuarioId = usuarioGuardia.Id,
                        SucursalId = 1
                    });

                    await userManager.CreateAsync(usuarioGuardia, "Anse25CHDs@");
                    await userManager.AddToRoleAsync(usuarioGuardia, Role.GUARDIA);


                    await context.SaveChangesAsync();

                }
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<ApplicationDbContext>();
                logger.LogError(ex.Message);
            }
        }
    }
}