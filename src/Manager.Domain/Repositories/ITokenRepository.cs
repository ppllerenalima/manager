namespace Manager.Domain.Repositories
{
    public interface ITokenRepository : IRepository
    {
        Task<Token> GetAsync(Guid ClienteId);
        Token Add(Token item);
        Token Update(Token item);
        Token Delete(Token item);
    }
}