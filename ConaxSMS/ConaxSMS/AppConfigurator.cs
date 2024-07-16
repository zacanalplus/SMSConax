using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConaxSMS
{
    class AppConfigurator
    {
        private Configuration config;
        private string NoSuchKey = "No such key exists";
        //private List<KeyValuePair<string, string> > cfgList;
        private KeyValueConfigurationCollection cfgList;
        //private List<string> errcdLst = new List<string>();
        //private const string errcdKey = "errCodes";

        public AppConfigurator(string configPath)
        {
            if (configPath == "")
            {
                throw new EmptyPathException();
            }
            if (!File.Exists(configPath))
            {
                throw new InvalidPathException();
            }
            ConfigFilePath = configPath;
            config = ConfigurationManager.OpenExeConfiguration(ConfigFilePath);
            LoadConfig();
        }
        public string ConfigFilePath
        {
            get; set;
        }

        private void LoadConfig()
        {
            if (config.AppSettings.Settings.Count != 0)
            {
                cfgList = new KeyValueConfigurationCollection();
                foreach (KeyValueConfigurationElement i in config.AppSettings.Settings)
                {
                    cfgList.Add(i);
                    //if(i.Key == errcdKey)
                    //{
                    //    string[] errCds = i.Value.Split(';');
                    //    if (errCds.Length > 0)
                    //    {
                    //        foreach(string errCd in errCds)
                    //        {
                    //            errcdLst.Add(errCd);
                    //        }
                    //    }
                    //}
                }
            }
        }

        public string GetConfigValue(string key)
        {
            if (cfgList.AllKeys.Contains<string>(key))
            {
                return cfgList[key].Value;
            }
            return NoSuchKey;
        }

        public void AddConfigValue(string key, string value)
        {
            if (!config.AppSettings.Settings.AllKeys.Contains(key))
            {
                config.AppSettings.Settings.Add(key, value);
                config.Save(ConfigurationSaveMode.Minimal);
            }
        }

        public string GetDirectConfigValue(string key)
        {
            if (config.AppSettings.Settings.AllKeys.Contains(key))
                return config.AppSettings.Settings[key].Value;
            else
                return NoSuchKey;
        }
    }

    class EmptyPathException : Exception
    {
        public EmptyPathException() : base("Empty config file path, please select valid config file") { }
        public EmptyPathException(string msg) : base(msg) { }
        public EmptyPathException(string msg, Exception inner) : base(msg, inner) { }
    }
    class InvalidPathException : Exception
    {
        public InvalidPathException() : base("Invalid config file path, please select valid config file again") { }
        public InvalidPathException(string msg) : base(msg) { }
        public InvalidPathException(string msg, Exception inner) : base(msg, inner) { }
    }
}
