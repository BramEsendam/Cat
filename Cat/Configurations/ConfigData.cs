﻿using System.IO;
using Cat.Models;
using Newtonsoft.Json;

namespace Cat.Configurations
{
    public static class ConfigData
    {
        private const string ConfigFolder = "Configs";
        private const string ConfigFile = "ConfigData.json";

        public static ConfigDataModel Data;

        static ConfigData()
        {
            if (!Directory.Exists(ConfigFolder)) Directory.CreateDirectory(ConfigFolder);

            if (!File.Exists(ConfigFolder + "/" + ConfigFile))
            {
                Data = new ConfigDataModel();
                var json = JsonConvert.SerializeObject(Data, Formatting.Indented);
                File.WriteAllText(ConfigFolder + "/" + ConfigFile, json);
            }
            else
            {
                var json = File.ReadAllText(ConfigFolder + "/" + ConfigFile);
                Data = JsonConvert.DeserializeObject<ConfigDataModel>(json);
            }
        }
    }
}