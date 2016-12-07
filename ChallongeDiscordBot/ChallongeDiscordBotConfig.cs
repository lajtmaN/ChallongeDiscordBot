using System.Configuration;

namespace ChallongeDiscordBot
{
    public class ChallongeDiscordBotConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        public ChallongeDiscordBotCollection Instances
        {
            get { return (ChallongeDiscordBotCollection)this[""]; }
            set { this[""] = value; }
        } 
    }

    public class ChallongeDiscordBotCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ChallongeDiscordBotConfig();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            //set to whatever Element Property you want to use for a key
            return ((ChallongeDiscordBotConfig)element).Name;
        }
    }

    public class ChallongeDiscordBotConfig : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("ApiKey", IsRequired = true)]
        public string ApiKey => (string)this["ApiKey"];

        [ConfigurationProperty("subdomain", IsRequired = true)]
        public string Subdomain => (string)this["subdomain"];
    }
}
