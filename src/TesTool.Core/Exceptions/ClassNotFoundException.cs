﻿namespace TesTool.Core.Exceptions
{
    public class ClassNotFoundException : TesToolExceptionBase
    {
        public ClassNotFoundException(string className) : base($"Classe {className} não encontrada.") { }
    }
}
