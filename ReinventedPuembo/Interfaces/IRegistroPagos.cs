using ReinventedPuembo.Models;
using System.Drawing;

namespace ReinventedPuembo.Interfaces
{
    public interface IRegistroPagos
    {
         
        int GuardarPagoXTC(PagoXTarjeta model );

        int GuardarPagoTransferencia(TransferenciaDTO model, string guid);
        int GuardarPagoPyXTC(PagoXTarjeta model);

    }
}
