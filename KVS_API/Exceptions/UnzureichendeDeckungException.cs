namespace KVS_API.Exceptions;
public class UnzureichendeDeckungException : Exception
{
    public UnzureichendeDeckungException(string nachricht) : base(nachricht)
    {
    }
}
