using Newtonsoft.Json;
using System;
using System.IO;

namespace ElectronDashboardMeteringDataAdapter
{
    public class ConfigurationManager
    {
        public string DataHost { get; set; } = "10.10.10.5";
        public string DataDbName { get; set; } = "data_db_name";
        public string DataUserName { get; set; } = "uname";
        public string DataPassword { get; set; } = "pass";
        public string configFilename = "meteringConfig.json";

        public string GetAppDataPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        public void Initialize()
        {
            string path = GetAppDataPath();
            string jsonPath = path + "\\" + configFilename;

            if (File.Exists(jsonPath))
            {
                ConfigurationManager configObj = JsonConvert.DeserializeObject<ConfigurationManager>(File.ReadAllText(jsonPath));
                CloneFromObject(configObj);
            }
            else
            {
                Save();
            }
        }

        public void Save()
        {
            string ConfigJSONStr = JsonConvert.SerializeObject(this, Formatting.Indented);

            string path = GetAppDataPath();
            string jsonPath = path + "\\" + configFilename;

            File.WriteAllText(jsonPath, ConfigJSONStr);
        }

        public void CloneFromObject(ConfigurationManager configuration)
        {
            DataHost = configuration.DataHost;
            DataDbName = configuration.DataDbName;
            DataUserName = configuration.DataUserName;
            DataPassword = configuration.DataPassword;
        }
    }
}