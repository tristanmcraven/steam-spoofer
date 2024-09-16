using Microsoft.Win32;
using SteamSpoofer.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Printing;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace SteamSpoofer.Utility
{
    public class Spoofer
    {
        private static int processesCount = 0;

        public static List<string> matches = new List<string>();

        private static readonly RegistryKey[] RootKeys =
{
        Registry.ClassesRoot,
        Registry.CurrentUser,
        Registry.LocalMachine,
        Registry.Users,
        Registry.CurrentConfig
    };

        public static void SpoofData()
        {
            TerminateSteam();
            SearchEntireRegistry("steam");

        }
        private static bool IsSteamRunning()
        {
            return Process.GetProcessesByName("steam").Any();
        }

        private static void TerminateSteam()
        {
            if (IsSteamRunning())
            {
                var steamProcess = Process.GetProcessesByName("steam").FirstOrDefault();
                var steamRelatedProcesses = Process.GetProcesses().Where(p => p.ProcessName.Equals("steamservice",
                    StringComparison.OrdinalIgnoreCase) || p.ProcessName.Equals("steamwebhelper")).ToList();
                processesCount = steamRelatedProcesses.Count;
                foreach (var srp in steamRelatedProcesses)
                {
                    srp.EnableRaisingEvents = true;
                    srp.Exited += Process_Exited;
                }
                steamProcess.Kill();

            }
        }
        private static void Process_Exited(object? sender, EventArgs e)
        {
            processesCount--;
            if (processesCount == 0)
            {
                Application.Current.Dispatcher.Invoke((Action)delegate //no idea why
                {
                    new DialogWindow("title", "Process is killed") { Owner = Application.Current.Windows.OfType<MainWindow>().First() }.ShowDialog();
                });
            }
        }

        public static void SearchEntireRegistry(string searchValue)
        {
            var results = new List<string>();
            foreach (var rootKey in RootKeys)
            {
                results.AddRange(SearchRegistry(searchValue, rootKey));
            }
            matches = results;
        }

        public static List<string> SearchRegistry(string searchValue, RegistryKey rootKey)
        {
            var results = new List<string>();
            SearchRegistryKey(rootKey, searchValue, results, rootKey.Name);
            return results;
        }

        private static void SearchRegistryKey(RegistryKey key, string searchValue, List<string> results, string path)
        {
            var regex = new Regex(searchValue, RegexOptions.IgnoreCase);

            if (regex.IsMatch(path))
            {
                results.Add($"Found in Key Name: {path}");
            }

            foreach (var valueName in key.GetValueNames())
            {
                if (regex.IsMatch(valueName))
                {
                    results.Add($"Found in Value Name: {path}\\{valueName}");
                }

                var value = key.GetValue(valueName)?.ToString();
                if (value != null && regex.IsMatch(value))
                {
                    results.Add($"Found in Value Data: {path}\\{valueName}: {value}");
                }
            }

            foreach (var subKeyName in key.GetSubKeyNames())
            {
                try
                {
                    using var subKey = key.OpenSubKey(subKeyName);
                    if (subKey != null)
                    {
                        SearchRegistryKey(subKey, searchValue, results, $"{path}\\{subKeyName}");
                    }
                }
                catch (System.Security.SecurityException)
                {
                    // ну и хyй с ним
                }
            }
        }
    }
}
