namespace KVS_API.Exceptions;
public class MaxKontenErreichtException : Exception
{
    public MaxKontenErreichtException(string nachricht) : base(nachricht)
    {
    }
}