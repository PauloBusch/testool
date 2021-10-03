namespace TesTool.Core.Models.Configuration
{
    public class Setting
    {
        public Setting(string key)
        {
            Key = key;
        }

        public string Key { get; private set; }
    }
}
