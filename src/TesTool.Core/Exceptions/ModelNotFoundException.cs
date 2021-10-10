namespace TesTool.Core.Exceptions
{
    public class ModelNotFoundException : TesToolExceptionBase
    {
        public ModelNotFoundException(string name) : base($"Model {name} não encontrado.") { }
    }
}
