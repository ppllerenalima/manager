namespace Manager.Domain.Services
{
    public interface ITicketService
    {
        Task<TicketResponse> GetTicketAsync(GetTicketRequest request);
        Task<TicketResponse> AddTicketAsync(AddTicketRequest request);
        Task<TicketResponse> EditTicketAsync(EditTicketRequest request);
        Task<TicketResponse> GetOrGenerateActiveTicketAsync(string token, GetTicketRequest request, bool Error2244 = false);
    }
}