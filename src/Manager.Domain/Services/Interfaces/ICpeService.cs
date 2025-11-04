namespace Manager.Domain.Services.Interfaces
{
    public interface ICpeService
    {
        Task<BaseResponseGeneric<DescargarZipResponse>> DescargarZipAsync(string token, DescargarZipRequest request, CancellationToken cancellationToken = default);
        Task<BaseResponseGeneric<DescargarZipResponse>> DescargarPorConsultaCpeAsync(string token, DescargarZipRequest request, CancellationToken cancellationToken = default);
        Task<BaseResponseGeneric<string>> ActualizarPermisosAsync(string token, ActualizarPermisosRequest request, CancellationToken cancellationToken = default);
    }
}
