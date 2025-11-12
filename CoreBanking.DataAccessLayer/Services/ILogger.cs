
namespace CoreBanking.DataAccessLayer.Services
{
    public interface ILogger<T>
    {
        void LogError(Exception ex, string v, Guid id);
        void LogError(Exception ex, string v);
    }
}