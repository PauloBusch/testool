namespace TesTool.Core.Exceptions
{
    public class UnsupportedCommandException : TesToolExceptionBase
    {
        public UnsupportedCommandException(string details) : base($"Comando não suportado. \n{details}") { }
    }
}
