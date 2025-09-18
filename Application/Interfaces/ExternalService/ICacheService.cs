namespace Application.Interfaces.ExternalService
{
    public interface ICacheService
    {
        Task AddAsync<T>(string key, T Value,TimeSpan? span);
        Task<T?> GetAsync<T>(string key);
        Task DeleteAsync<T>(string key);
    }
}
