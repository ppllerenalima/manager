namespace Manager.Domain.Services
{
    public interface ITicketService
    {
        Task<TicketResponse> GetTicketAsync(GetTicketRequest request);
        Task<TicketResponse> AddTicketAsync(AddTicketRequest request);
        Task<TicketResponse> EditTicketAsync(EditTicketRequest request);
        //Task<TicketResponse> DeleteTicketAsync(DeleteTicketRequest request);
    }
}