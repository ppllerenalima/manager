namespace Manager.Domain.Services.Interfaces
{
    public interface ICpeService
    {
        //Task<StatusResponse> StatusCdrAsync(ConsultarCpeRequest request);
        Task<DescargarZipResponse> DescargarZipAsync(string token, DescargarZipRequest request);
    }
}
