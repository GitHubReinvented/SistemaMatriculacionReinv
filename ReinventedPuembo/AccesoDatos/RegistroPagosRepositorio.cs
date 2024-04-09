using ReinventedPuembo.Interfaces;
using Infrasctructure.Persistence;
using Domain;
using System.Drawing;
using ReinventedPuembo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ReinventedPuembo.AccesoDatos
{
    public class RegistroPagosRepositorio: IRegistroPagos
    {
        private readonly ApplicationDbContext context;
        public RegistroPagosRepositorio(ApplicationDbContext _contex) 
        {
            context= _contex;
        }

        

        public int GuardarPagoTransferencia(TransferenciaDTO model,string guid)
        {
            var transaction = context.Database.BeginTransaction();

            try
            {
                //1ro registro el pago
                int codigoPago = 0;

                var registroPagos = new RegistroPagos();
                registroPagos.NumeroReferencia = model.numeroReferencia;
                registroPagos.FechaTransaccion = model.fechaTransferencia;
                registroPagos.Valor = model.valor / 100;//divido para cien porque en la url llega multiplicado por cien para evitar el punto decimal en url
                
                registroPagos.EstadoPagoId = 2;//por concilar cuando se registra la transferencia
                string numReferencia = model.numeroReferencia;

                if (registroPagos.Valor==0)
                {
                    registroPagos.CuentaSucursalId = -1;// model.cuentaDestino;
                    registroPagos.TipoPagoId = 3;//precancelada
                    registroPagos.Foto = "";
                }
                else
                {
                    
                    registroPagos.CuentaSucursalId = model.cuentaDestino;
                    registroPagos.TipoPagoId = 1;//transferencia
                    string extensionArchivo = Path.GetExtension(model.fotoTransferencia.FileName);
                    string nombreArchivo = $"{guid}{extensionArchivo}";
                    registroPagos.Foto = nombreArchivo;
                }
                    

                context.RegistrosPagos.Add(registroPagos);
                context.SaveChanges();
                codigoPago = registroPagos.Id;

                //2do afecto las obligaciones,  uno pago cubre una o más obligaciones
                for (int i = 0; i < model.auxiliar.Count(); i++)
                {

                    var registroObligacion = context.ObligacionesPendientes.Find(model.auxiliar[i]);

                    if (registroObligacion == null)
                    {
                        return -1;
                    }

                    registroObligacion.RegistroPagosId = codigoPago;//el codigo del pago que cubre esta obligación
                    registroObligacion.EstadoCuotaId = 5;// Por conciliar obigacion

                    context.Entry(registroObligacion).State = EntityState.Modified;
                    context.SaveChanges();
                };



                transaction.Commit();
                return codigoPago;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }

        }
        public int GuardarPagoXTC(PagoXTarjeta model)
        {

            var transaction = context.Database.BeginTransaction();

            try
            {
                //1ro registro el pago
                int codigoPago = 0;
                
                 var registroPagos = new RegistroPagos();
                 registroPagos.NumeroReferencia = model.NumeroReferencia;
                 registroPagos.Valor = model.Valor / 100;//divido para cien porque en la url llega multiplicado por cien para evitar el punto decimal en url
                 registroPagos.FechaTransaccion = model.FechaTransaccion;
                 registroPagos.NombrePropietarioTarjeta = model.NombrePropietarioTarjeta;
                 registroPagos.EstadoTarjeta = model.EstadoTarjeta;
                 registroPagos.ObservacionTarjeta = model.ObservacionTarjeta;
                 registroPagos.DescripcionTarjeta = model.DescripcionTarjeta;
                 registroPagos.TokenTxTarjeta = model.TokenTxTarjeta;
                 registroPagos.idTxTarjeta = model.idTxTarjeta;
                 registroPagos.codTx = model.codTxTarjeta;
                 registroPagos.CreatedBy = model.Operador;
                 registroPagos.TipoPagoId = 2;//pago por tarjeta
                if(model.EstadoCodigo==1)//pagado
                    registroPagos.EstadoPagoId = 3;//pagada
                else
                    registroPagos.EstadoPagoId= 2;//llega 0  PENDIENTE DE PAGO -> Por conciliar

                context.RegistrosPagos.Add(registroPagos);
                 context.SaveChanges();
                 codigoPago = registroPagos.Id;

                //2do afecto las obligaciones,  uno pago cubre una o más obligaciones
                for (int i = 0; i < model.ObligacionPendienteId.Count(); i++)
                {
                    
                    var registroObligacion = context.ObligacionesPendientes.Find(model.ObligacionPendienteId[i]);

                    if (registroObligacion == null)
                    {
                        return -1;
                    }

                    registroObligacion.RegistroPagosId = codigoPago;//el codigo del pago que cubre esta obligación
                    if (model.EstadoCodigo == 1)//pagado
                        registroObligacion.EstadoCuotaId = 3;//pagada
                    else
                        registroObligacion.EstadoCuotaId = 5;//llega 0  PENDIENTE DE PAGO -> Por conciliar obigacion a 5

                    context.Entry(registroObligacion).State = EntityState.Modified;
                    context.SaveChanges();
                };

                

                transaction.Commit();
                return codigoPago;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }


        //payphone
        public int GuardarPagoPyXTC(PagoXTarjeta model)
        {

            var transaction = context.Database.BeginTransaction();

            try
            {
                //1ro registro el pago
                int codigoPago = 0;

                var registroPagos = new RegistroPagos();
                registroPagos.NumeroReferencia = model.NumeroReferencia;
                registroPagos.Valor = model.Valor / 100;//divido para cien porque en la url llega multiplicado por cien para evitar el punto decimal en url
                registroPagos.FechaTransaccion = model.FechaTransaccion;
                registroPagos.NombrePropietarioTarjeta = model.NombrePropietarioTarjeta;
                registroPagos.EstadoTarjeta = model.EstadoTarjeta;
                registroPagos.ObservacionTarjeta = model.ObservacionTarjeta;
                registroPagos.DescripcionTarjeta = model.DescripcionTarjeta;
                registroPagos.TokenTxTarjeta = model.TokenTxTarjeta;
                registroPagos.idTxTarjeta = model.idTxTarjeta;
                registroPagos.codTx = model.codTxTarjeta;
                registroPagos.CreatedBy = model.Operador;
                registroPagos.TipoPagoId = 2;//pago por tarjeta
                if (model.EstadoCodigo == 3)//pagado
                    registroPagos.EstadoPagoId = 3;//pagada
                else
                    registroPagos.EstadoPagoId = 2;//llega  cancelado

                context.RegistrosPagos.Add(registroPagos);
                context.SaveChanges();
                codigoPago = registroPagos.Id;

                //2do afecto las obligaciones,  uno pago cubre una o más obligaciones
                for (int i = 0; i < model.ObligacionPendienteId.Count(); i++)
                {

                    var registroObligacion = context.ObligacionesPendientes.Find(model.ObligacionPendienteId[i]);

                    if (registroObligacion == null)
                    {
                        return -1;
                    }

                    registroObligacion.RegistroPagosId = codigoPago;//el codigo del pago que cubre esta obligación
                    if (model.EstadoCodigo == 3)//pagado
                        registroObligacion.EstadoCuotaId = 3;//pagada
                    else
                        registroObligacion.EstadoCuotaId = 5;//llega 0  PENDIENTE DE PAGO -> Por conciliar obigacion a 5

                    context.Entry(registroObligacion).State = EntityState.Modified;
                    context.SaveChanges();
                };



                transaction.Commit();
                return codigoPago;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

    }
}
