namespace Manager.Domain.Mappers
{
    public class TicketMapper : ITicketMapper
    {
        public Ticket Map(AddTicketRequest request)
        {
            if (request == null) return null;

            var token = new Ticket
            {
                IsInactive = request.IsInactive,

                CodProceso = request.CodProceso,
                CodEstadoProceso = request.CodEstadoProceso,
                DesProceso = request.DesProceso,
                PerTributario = request.PerTributario,

                NumTicket = request.NumTicket,
                FecCargaImportacion = request.FecCargaImportacion,
                HoraCargaImportacion = request.HoraCargaImportacion,
                CodEstadoEnvio = request.CodEstadoEnvio,
                DesEstadoEnvio = request.DesEstadoEnvio,

                CodTipoAchivoReporte = request.CodTipoAchivoReporte,
                NomArchivoReporte = request.NomArchivoReporte,

                ClienteId = request.ClienteId,
            };

            return token;
        }

        public Ticket Map(EditTicketRequest request)
        {
            if (request == null) return null;

            var tokenCliente = new Ticket
            {
                Id = request.Id,
                IsInactive = request.IsInactive,

                CodProceso = request.CodProceso,
                CodEstadoProceso = request.CodEstadoProceso,
                DesProceso = request.DesProceso,
                PerTributario = request.PerTributario,

                NumTicket = request.NumTicket,
                FecCargaImportacion = request.FecCargaImportacion,
                HoraCargaImportacion = request.HoraCargaImportacion,
                CodEstadoEnvio = request.CodEstadoEnvio,
                DesEstadoEnvio = request.DesEstadoEnvio,

                CodTipoAchivoReporte = request.CodTipoAchivoReporte,
                NomArchivoReporte = request.NomArchivoReporte,

                ClienteId = request.ClienteId
            };

            return tokenCliente;
        }

        public TicketResponse Map(Ticket request)
        {
            if (request == null) return null;

            var response = new TicketResponse
            {
                Id = request.Id,

                CodProceso = request.CodProceso,
                CodEstadoProceso = request.CodEstadoProceso,
                DesProceso = request.DesProceso,
                PerTributario = request.PerTributario,

                NumTicket = request.NumTicket,
                FecCargaImportacion = request.FecCargaImportacion,
                HoraCargaImportacion = request.HoraCargaImportacion,
                CodEstadoEnvio = request.CodEstadoEnvio,
                DesEstadoEnvio = request.DesEstadoEnvio,

                CodTipoAchivoReporte = request.CodTipoAchivoReporte,
                NomArchivoReporte = request.NomArchivoReporte,

                ClienteId = request.ClienteId,
            };

            return response;
        }
    }
}