using Manager.Domain.Requests.ConfiguracionGlobal;
using Manager.Domain.Responses.ConfiguracionGlobalResponses;

namespace Manager.Domain.Services.Interfaces
{
    public interface IConfiguracionGlobalService
    {
        Task<IEnumerable<ConfiguracionGlobalResponse>> GetConfiguracionGlobalsAsync();
        Task<ConfiguracionGlobalResponse> GetConfiguracionGlobalFirstOrDefaultAsync();
        Task<ConfiguracionGlobalResponse> GetConfiguracionGlobalAsync(GetConfiguracionGlobalRequest request);
        Task<ConfiguracionGlobalResponse> AddConfiguracionGlobalAsync(AddConfiguracionGlobalRequest request);
        Task<ConfiguracionGlobalResponse> EditConfiguracionGlobalAsync(EditConfiguracionGlobalRequest request);
        Task<ConfiguracionGlobalResponse> DeleteConfiguracionGlobalAsync(DeleteConfiguracionGlobalRequest request);
    }
}