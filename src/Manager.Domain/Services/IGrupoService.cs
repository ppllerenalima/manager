namespace Manager.Domain.Services
{
    public interface IGrupoService
    {
        Task<IEnumerable<GrupoResponse>> GetGruposAsync();
        Task<GrupoResponse> GetGrupoAsync(GetGrupoRequest request);
        Task<GrupoResponse> AddGrupoAsync(AddGrupoRequest request);
        Task<GrupoResponse> EditGrupoAsync(EditGrupoRequest request);
        Task<GrupoResponse> DeleteGrupoAsync(DeleteGrupoRequest request);
    }
}