namespace Manager.Domain.Services.Interfaces
{
    public interface IGrupoService
    {
        Task<(IEnumerable<GrupoResponse> Items, int Total)> GetGruposAsync(string? search, int pageIndex, int pageSize);
        Task<GrupoResponse> GetGrupoAsync(GetGrupoRequest request);
        Task<GrupoResponse> AddGrupoAsync(AddGrupoRequest request);
        Task<GrupoResponse> EditGrupoAsync(EditGrupoRequest request);
        Task<GrupoResponse> DeleteGrupoAsync(DeleteGrupoRequest request);
    }
}