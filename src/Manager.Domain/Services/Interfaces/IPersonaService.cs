namespace Manager.Domain.Services.Interfaces
{
    public interface IPersonaService
    {
        Task<IEnumerable<PersonaResponse>> GetPersonasAsync();
        Task<PersonaResponse> GetPersonaAsync(GetPersonaRequest request);
        Task<PersonaResponse> AddPersonaAsync(AddPersonaRequest request);
        Task<PersonaResponse> EditPersonaAsync(EditPersonaRequest request);
        Task<PersonaResponse> DeletePersonaAsync(DeletePersonaRequest request);
    }
}