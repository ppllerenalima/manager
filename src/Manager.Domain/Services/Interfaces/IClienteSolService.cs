namespace Manager.Domain.Services.Interfaces
{
    public interface IClienteSolService
    {
        Task<BaseResponseGeneric<SunatAuthResponse>> AccessTokenAsync(SunatAuthRequest request, CancellationToken cancellationToken = default);
    }
}