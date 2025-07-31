using Manager.Domain.Entities;
using Manager.Domain.Requests.Ticket;
using Manager.Domain.Responses.TicketResponses;

namespace Manager.Domain.Mappers
{
    public interface ITicketMapper
    {
        Ticket Map(AddTicketRequest request);
        Ticket Map(EditTicketRequest request);
        TicketResponse Map(Ticket request);
    }
}