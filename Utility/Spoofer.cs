using Microsoft.Win32;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace SteamSpoofer.Utility
{
    public class Spoofer
    {
        private static int processesCount = 0;

        private static TaskCompletionSource<bool> steamTerminationTcs;

        public static List<string> Matches = new List<string>();

        public static List<string> throwAways = new List<string>();

        public static List<string[]> zxc1 = new List<string[]>();

        public static string[] searchValues = {"valve", "steam", "dota"};

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
            if (IsSteamRunning())
                await TerminateSteam();
            await Task.Run(() => SearchEntireRegistry(searchValues));
            foreach (var match in Matches)
            {
                if (match.Contains("::"))
                {
                    throwAways.Add(match);
                    zxc1.Add(match.Split(':'));
                }
            }
        }
        private static bool IsSteamRunning() => Process.GetProcessesByName("steam").Any();

        private static async Task TerminateSteam()
        {
            Helper.SetLogText("terminating_steam");
            var steamProcess = Process.GetProcessesByName("steam").FirstOrDefault();
            var steamRelatedProcesses = Process.GetProcesses().Where(p => p.ProcessName.Equals("steamservice",
                StringComparison.OrdinalIgnoreCase) || p.ProcessName.Equals("steamwebhelper")).ToList();
            processesCount = steamRelatedProcesses.Count;
            steamTerminationTcs = new TaskCompletionSource<bool>();
            foreach (var srp in steamRelatedProcesses)
            {
                srp.EnableRaisingEvents = true;
                srp.Exited += Process_Exited;
            }
            steamProcess.Kill();

            await steamTerminationTcs.Task;
        }

        private static async void Process_Exited(object? sender, EventArgs e)
        {
            processesCount--;
            if (processesCount == 0)
            {
                steamTerminationTcs.TrySetResult(true);
            }
        }

        public static void SearchEntireRegistry(string[] searchValue)
        {
            Helper.SetLogText("searching_registry");
            foreach (var hive in Hives)
            {
                SearchRegistryKey(hive, searchValue, hive.Name);
            }
        }

        public static void SearchRegistryKey(RegistryKey key, string[] searchValue, string path)
        {
            var regex = new Regex(String.Join('|', searchValue), RegexOptions.IgnoreCase);
            var nonregex = new Regex("steamspoofer", RegexOptions.IgnoreCase);

            foreach (var value in key.GetValueNames())
            {
                if (regex.IsMatch(value) && !nonregex.IsMatch(value))
                {
                    Matches.Add($"{path}:{value}");
                }
                var valueData = key.GetValue(value)?.ToString();
                if (valueData != null && regex.IsMatch(valueData) && !nonregex.IsMatch(valueData))
                {
                    Matches.Add($"{path}:{value}:{valueData}");
                }
            }

            foreach (var subKeyName in key.GetSubKeyNames())
            {
                try
                {
                    using var subKey = key.OpenSubKey(subKeyName);
                    if (subKey != null)
                    {
                        //Helper.SetLogText("searching", $"{path}\\{subKeyName}"); //хуета полная
                        if (regex.IsMatch(subKeyName) && !nonregex.IsMatch(subKeyName))
                        {
                            Matches.Add($"{path}\\{subKeyName}");
                        }
                        SearchRegistryKey(subKey, searchValue, $"{path}\\{subKeyName}");
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
            {
                if (valueName == "")
                    key.SetValue("", null);
                else
                    key.DeleteValue(valueName, false);
            }

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
