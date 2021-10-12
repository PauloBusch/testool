namespace TesTool.Core.Exceptions
{
    public class DuplicatedClassException : TesToolExceptionBase
    {
        public DuplicatedClassException(string className) : base($"Classe {className} já existe.") { }
    }
}
