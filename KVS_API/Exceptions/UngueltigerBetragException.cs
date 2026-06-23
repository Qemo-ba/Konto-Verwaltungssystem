namespace KVS_API.Exceptions;
public class UngueltigerBetragException : Exception
{
    public UngueltigerBetragException(string nachricht) : base(nachricht)
    {
    }
}
