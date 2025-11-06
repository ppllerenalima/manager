namespace Manager.Domain.Services.Interfaces
{
    public interface ITicketService
    {
        Task<BaseResponseGeneric<TicketResponse>> GetOrGenerateActiveTicketAsync(string token, GetTicketRequest request, bool error2244 = false);
    }
}