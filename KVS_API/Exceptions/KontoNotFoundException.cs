namespace KVS_API.Exceptions;
public class KontoNotFoundException : Exception
{
    public KontoNotFoundException(string nachricht) : base(nachricht)
    {
    }
}
