﻿using System.Threading.Tasks;
using TesTool.Core.Models.Metadata;
using TesTool.Core.Models.Templates.Comparator;

namespace TesTool.Core.Interfaces.Services
{
    public interface ICompareService
    {
        string GetNamespace();
        string GetDirectoryBase();
        bool IsComparableClasses(Class source, Class target);
        string GetComparatorName(string sourceClassName, string targetClassName);
        string GetComparatorNameOrDefault(string sourceClassName, string targetClassName);
        Task<Class> GetComparatorClassAsync(string sourceClassName, string targetClassName);
        Task<CompareDynamic> GetCompareDynamicAsync(Class source, Class target);
        Task<CompareStatic> GetCompareStaticAsync(Class source, Class target);
    }
}
