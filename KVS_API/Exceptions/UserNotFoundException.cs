namespace KVS_API.Exceptions;
public class UserNotFoundException : Exception
{
    public UserNotFoundException(string nachricht) : base(nachricht)
    {
    }
}
