namespace Manager.Domain.Repositories
{
    public interface ITokenBaseRepository : IRepository
    {
        Task<TokenBase> GetAsync(Guid ClienteId);
        TokenBase Add(TokenBase item);
        TokenBase Update(TokenBase item);
        TokenBase Delete(TokenBase item);
    }
}