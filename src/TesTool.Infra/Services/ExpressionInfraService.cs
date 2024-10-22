﻿using Bogus;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Infra.Services
{
    public class ExpressionInfraService : IExpressionInfraService
    {
        public async Task<string> BuildBogusExpressionAsync(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression)) return default;
            if (!expression.Contains("=>")) return expression;

            try
            {
                var expressionRegex = new Regex(@"(.*)=>(.*)").Match(expression);
                if (!expressionRegex.Success || expressionRegex.Groups.Count < 3) return expression;

                var parameterName = expressionRegex.Groups[1].Value?
                    .Replace("(", string.Empty)
                    .Replace(")", string.Empty)
                    .Trim();
                var bodyCode = expressionRegex.Groups[2].Value?
                    .Replace("{", string.Empty)
                    .Replace("}", string.Empty)
                    .Replace(";", string.Empty)
                    .Trim();

                if (string.IsNullOrWhiteSpace(parameterName)) return expression;
                if (string.IsNullOrWhiteSpace(bodyCode)) return expression;

                var codeRun = $@"
                    var {parameterName}=new Faker();
                    return {bodyCode};
                ";
                var imports = ScriptOptions.Default
                    .WithReferences(typeof(Faker).Assembly)
                    .WithImports("Bogus");
                var result = await CSharpScript.EvaluateAsync<object>(codeRun, imports);
                return SerializeValue(result);
            } catch
            {
                return expression;
            }
        }

        private static string SerializeValue(object result)
        {
            var stringValue = JsonSerializer.Serialize(result);
            if (result is decimal) stringValue += "m";
            if (result is Guid) stringValue = $"new Guid({stringValue})";
            if (result is DateTime) stringValue = $"DateTime.Parse({stringValue})";
            if (result is DateTimeOffset) stringValue = $"DateTimeOffset.Parse({stringValue})";
            return stringValue;
        }
    }
}
