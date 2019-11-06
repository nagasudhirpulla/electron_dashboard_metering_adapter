namespace ElectronDashboardMeteringDataAdapter
{
    public class SettingsEditVM
    {
        public ConfigurationManager ConfigManager { get; set; } = new ConfigurationManager();

        public SettingsEditVM()
        {
            ConfigManager.Initialize();
        }

        public void Save()
        {
            ConfigManager.Save();
        }

        public string DataHost { get { return ConfigManager.DataHost; } set { ConfigManager.DataHost = value; } }
        public string DataDbName { get { return ConfigManager.DataDbName; } set { ConfigManager.DataDbName = value; } }
        public string DataUserName { get { return ConfigManager.DataUserName; } set { ConfigManager.DataUserName = value; } }
        public string DataPassword { get { return ConfigManager.DataPassword; } set { ConfigManager.DataPassword = value; } }
    }
}