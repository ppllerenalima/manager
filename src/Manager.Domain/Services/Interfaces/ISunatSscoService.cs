namespace Manager.Domain.Services.Interfaces
{
    public interface ISunatSscoService
    {
        Task<(bool Actualizado, string Mensaje)> SincronizarPadronAsync(CancellationToken cancellationToken = default);
    }
}
