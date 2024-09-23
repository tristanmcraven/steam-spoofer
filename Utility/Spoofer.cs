using Microsoft.Win32;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace SteamSpoofer.Utility
{
    public class Spoofer
    {
        private static int processesCount = 0;

        private static TaskCompletionSource<bool> steamTerminationTcs;

        public static List<string> Matches = new List<string>();

        public static List<Entry> Entries = new List<Entry>();

        public static List<string> throwAways = new List<string>();

        public static List<string[]> zxc1 = new List<string[]>();

        public static string[] searchValues = { "valve", "steam", "dota" };

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
            await Task.Run(() => DeleteFoundEntries());
        }
        private static bool IsSteamRunning() => Process.GetProcessesByName("steam").Any();

        private static async Task TerminateSteam()
        {
            Helper.SetLogText("terminating_steam");
            var steamProcess = Process.GetProcessesByName("steam").FirstOrDefault();
            var steamRelatedProcesses = Process.GetProcesses().Where(p => p.ProcessName.Equals("steamservice", StringComparison.OrdinalIgnoreCase) || p.ProcessName.Equals("steamwebhelper")).ToList();
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
                    Entries.Add(new Entry(path, value, null));
                }
                var valueData = key.GetValue(value)?.ToString();
                if (valueData != null && regex.IsMatch(valueData) && !nonregex.IsMatch(valueData))
                {
                    Entries.Add(new Entry(path, value, valueData));
                }
            }

            foreach (var subKeyName in key.GetSubKeyNames())
            {
                try
                {
                    using var subKey = key.OpenSubKey(subKeyName);
                    if (subKey != null)
                    {
                        if (regex.IsMatch(subKeyName) && !nonregex.IsMatch(subKeyName))
                        {
                            Entries.Add(new Entry($"{path}\\{subKeyName}", null, null));
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
            Helper.SetLogText("deleting_entries");
            foreach (var entry in Entries)
            {
                try
                {
                    if (IsValuePath(entry))
                        DeleteRegistryValue(entry);
                    else
                        DeleteRegistryKey(entry);
                }
                catch (Exception)
                {
                    Debug.WriteLine($"Fail deleting: {entry.Path}:{entry.ValueName}:{entry.ValueData}");
                }
            }
        }

        private static bool IsValuePath(Entry entry) => entry.ValueName != null;

        private static void DeleteRegistryValue(Entry entry)
        {
            using var hive = entry.Hive;
            using var key = hive.OpenSubKey(entry.SubkeyPath, true);
            if (key != null)
            {
                if (entry.ValueName == "")
                    key.SetValue("", null);
                else
                    key.DeleteValue(entry.ValueName!, false);
            }
        }

        private static void DeleteRegistryKey(Entry entry)
        {
            using (var hive = entry.Hive)
            {
                if (hive != null)
                    hive.DeleteSubKeyTree(entry.SubkeyPath, false);
            }
        }
    }
}
