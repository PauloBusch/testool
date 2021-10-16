namespace TesTool.Core.Models.Enumerators
{
    public class HelpClass
    {
        public HelpClass(string name, string regex = default)
        {
            Name = name;
            Regex = regex ?? $"^{name}$";
        }

        public string Name { get; private set; }
        public string Regex { get; private set; }
    }
}
