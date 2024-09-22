using Microsoft.Win32;
using SteamSpoofer.Windows;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Navigation;

namespace SteamSpoofer.Utility
{
    public class Spoofer
    {
        private static int processesCount = 0;

        public static List<string> Matches = new List<string>();

        private static readonly RegistryKey[] Hives =
        {
            Registry.ClassesRoot,
            Registry.CurrentUser,
            Registry.LocalMachine,
            Registry.Users,
            Registry.CurrentConfig
        };

        public static async Task SpoofData()
        {
            await TerminateSteam();
            await SearchEntireRegistry("valve");
        }
        private static bool IsSteamRunning()
        {
            return Process.GetProcessesByName("steam").Any();
        }

        private static async Task TerminateSteam()
        {
            await Task.Run(() =>
            {
                if (IsSteamRunning())
                {
                    Helper.SetLogText("terminating_steam");
                    var steamProcess = Process.GetProcessesByName("steam").FirstOrDefault();
                    var steamRelatedProcesses = Process.GetProcesses().Where(p => p.ProcessName.Equals("steamservice",
                        StringComparison.OrdinalIgnoreCase) || p.ProcessName.Equals("steamwebhelper")).ToList();
                    processesCount = steamRelatedProcesses.Count;
                    //foreach (var srp in steamRelatedProcesses)
                    //{
                    //    srp.EnableRaisingEvents = true;
                    //    srp.Exited += Process_Exited;
                    //}
                    steamProcess.Kill();
                }
            });
        }
        //private static void Process_Exited(object? sender, EventArgs e)
        //{
        //    processesCount--;
        //    if (processesCount == 0)
        //    {
        //        Application.Current.Dispatcher.Invoke((Action)delegate //no idea why
        //        {
        //            new DialogWindow("title", "Process is killed") { Owner = Application.Current.Windows.OfType<MainWindow>().First() }.ShowDialog();
        //        });
        //    }
        //}

        public static async Task SearchEntireRegistry(string searchValue)
        {
            foreach (var hive in Hives)
            {
                await SearchRegistryKey(hive, searchValue, hive.Name);
            }
        }

        public static async Task SearchRegistryKey(RegistryKey key, string searchValue, string path)
        {
            var regex = new Regex(searchValue, RegexOptions.IgnoreCase);
            var nonregex = new Regex("steamspoofer", RegexOptions.IgnoreCase);

            await Task.Run(() =>
            {
                foreach (var value in key.GetValueNames())
                {
                    Helper.SetLogText("searching", $"{path}\\{value}");
                    if (regex.IsMatch(value) && !nonregex.IsMatch(value))
                    {
                        Matches.Add($"{path}\\{value}");
                    }
                    var valueData = key.GetValue(value)?.ToString();
                    if (valueData != null && regex.IsMatch(valueData) && !nonregex.IsMatch(valueData))
                    {
                        Matches.Add($"{path}\\{value}: {valueData}");
                    }
                }
            });

            foreach (var subKeyName in key.GetSubKeyNames())
            {
                try
                {
                    using var subKey = key.OpenSubKey(subKeyName, true);
                    if (subKey != null)
                    {
                        Helper.SetLogText("searching", $"{path}\\{subKeyName}");
                        if (regex.IsMatch(subKeyName) && !nonregex.IsMatch(subKeyName))
                        {
                            Matches.Add($"{path}\\{subKeyName}");
                        }
                        await SearchRegistryKey(subKey, searchValue, $"{path}\\{subKeyName}");
                    }
                }
                catch (System.Security.SecurityException) 
                {
                    // ну и хуй с ним
                }
            }
        }
        public static void DeleteFoundEntries()
        {
            foreach (var match in Matches)
            {
                try
                {
                    if (IsValuePath(match))
                        DeleteRegistryValue(match);
                    else
                        DeleteRegistryKey(match);
                }
                catch (Exception)
                {

                }
            }
        }

        private static bool IsValuePath(string path) => path.Contains(":");

        private static void DeleteRegistryValue(string valuePath)
        {
            var pathParts = valuePath.Split('\\');
            var hiveName = pathParts[0];
            var subKeyPath = string.Join("\\", pathParts, 1, pathParts.Length - 2);
            var valueName = pathParts[pathParts.Length - 1].Split(':')[0];

            using var hive = GetRegistryHive(hiveName);
            using var key = hive.OpenSubKey(subKeyPath, true);

            if (key != null)
                key.DeleteValue(valueName, false);
        }

        private static void DeleteRegistryKey(string keyPath)
        {
            var pathParts = keyPath.Split('\\');
            var hiveName = pathParts[0];
            var subKeyPath = string.Join("\\", pathParts, 1, pathParts.Length - 1);
            using var hive = GetRegistryHive(hiveName);
            if (hive != null)
                hive.DeleteSubKeyTree(subKeyPath, false);
        }

        private static RegistryKey GetRegistryHive(string hive)
        {
            return hive switch
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
