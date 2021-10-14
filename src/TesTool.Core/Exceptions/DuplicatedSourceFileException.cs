namespace TesTool.Core.Exceptions
{
    public class DuplicatedSourceFileException : TesToolExceptionBase
    {
        public DuplicatedSourceFileException(string fileName) : base($"Arquivo {fileName} já existe.") { }
    }
}
