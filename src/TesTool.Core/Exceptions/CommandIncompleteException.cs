namespace TesTool.Core.Exceptions
{
    public class CommandIncompleteException : TesToolExceptionBase
    {
        public CommandIncompleteException() : base("Comando incompleto.") { }
        public CommandIncompleteException(string extraMessage) : base($"Comando incompleto.\n{extraMessage}") { }
    }
}
