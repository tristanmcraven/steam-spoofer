using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamSpoofer.Utility
{
    public class Entry
    {
        public string Path { get; set; }
        public string? ValueName { get; set; }
        public string? ValueData { get; set; }

        public string[] PathParts { get; }
        public string HiveName { get; }
        public string SubkeyPath { get; }
        public RegistryKey Hive { get; }

        public Entry(string path, string? valueName, string? valueData)
        {
            Path = path;
            ValueName = valueName;
            ValueData = valueData;

            PathParts = SetPathParts();
            HiveName = SetHiveName();
            SubkeyPath = SetSubkeyPath();
            Hive = SetHive();
        }

        public override string ToString() => $"{Path}:{ValueName}:{ValueData}";

        private string[] SetPathParts() => Path.Split('\\');

        private string SetHiveName() => PathParts[0];

        private string SetSubkeyPath() => string.Join('\\', PathParts, 1, PathParts.Length - 1);

        private RegistryKey SetHive()
        {
            return HiveName switch
            {
                "HKEY_LOCAL_MACHINE" => Registry.LocalMachine,
                "HKEY_CURRENT_USER" => Registry.CurrentUser,
                "HKEY_CLASSES_ROOT" => Registry.ClassesRoot,
                "HKEY_USERS" => Registry.Users,
                "HKEY_CURRENT_CONFIG" => Registry.CurrentConfig,
                _ => null
            };
        }
    }
}
