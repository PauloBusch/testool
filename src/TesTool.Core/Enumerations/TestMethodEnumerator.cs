﻿using TesTool.Core.Models.Enumerators;

namespace TesTool.Core.Enumerations
{
    public class TestMethodEnumerator : EnumeratorBase<TestMethodEnumerator, TestMethod>
    {
        public static readonly TestMethod GENERIC = new("Should{ACTION}Async");
        public static readonly TestMethod GET_ONE = new("ShouldReturn{ARTIFACT}Async");
        public static readonly TestMethod GET_ALL = new("ShouldReturnAllAsync");
        public static readonly TestMethod CREATE = new("ShouldCreateAsync");
        public static readonly TestMethod UPDATE = new("ShouldUpdateAsync");
        public static readonly TestMethod DELETE = new("ShouldDeleteAsync");
    }
}