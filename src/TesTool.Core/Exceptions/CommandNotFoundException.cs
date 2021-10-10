namespace TesTool.Core.Exceptions
{
    public class CommandNotFoundException : TesToolExceptionBase
    {
        public CommandNotFoundException() : base("Comando não encontrado.") { }
        public CommandNotFoundException(string extraMessage) : base($"Comando não encontrado.\n{extraMessage}") { }
    }
}
