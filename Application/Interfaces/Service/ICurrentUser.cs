namespace Application.Interfaces.Service
{
    public interface ICurrentUser
    {
        Guid? GetCurrentUserId();
    }
}