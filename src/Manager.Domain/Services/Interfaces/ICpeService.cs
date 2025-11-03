namespace Manager.Domain.Services.Interfaces
{
    public interface ICpeService
    {
        Task<BaseResponseGeneric<DescargarZipResponse>> DescargarZipAsync(string token, DescargarZipRequest request);
        Task<BaseResponseGeneric<DescargarZipResponse>> DescargarPorConsultaCpeAsync(string token, DescargarZipRequest request);
        Task<BaseResponseGeneric<string>> ActualizarPermisosAsync(string token, ActualizarPermisosRequest request);
    }
}
