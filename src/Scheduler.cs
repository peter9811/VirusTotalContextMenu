using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace VirusTotalContextMenu
{
    public class Scheduler
    {
        private readonly Timer _timer;
        private readonly List<ScheduledScan> _scheduledScans;
        private readonly string _settingsFilePath;

        public Scheduler(string settingsFilePath)
        {
            _settingsFilePath = settingsFilePath;
            _scheduledScans = LoadScheduledScans();
            _timer = new Timer(60000); // Check every minute
            _timer.Elapsed += TimerElapsed;
            _timer.Start();
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            foreach (var scan in _scheduledScans)
            {
                if (scan.ShouldRun())
                {
                    Task.Run(() => RunScan(scan));
                }
            }
        }

        private async Task RunScan(ScheduledScan scan)
        {
            try
            {
                await Program.VirusScanFile(scan.FilePaths.ToArray());
                scan.LastRun = DateTime.Now;
                SaveScheduledScans();
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
            }
        }

        private List<ScheduledScan> LoadScheduledScans()
        {
            if (!File.Exists(_settingsFilePath))
                return new List<ScheduledScan>();

            var json = File.ReadAllText(_settingsFilePath);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<ScheduledScan>>(json) ?? new List<ScheduledScan>();
        }

        private void SaveScheduledScans()
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(_scheduledScans, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(_settingsFilePath, json);
        }

        private void LogError(string message)
        {
            var logFilePath = Path.Combine(Path.GetDirectoryName(_settingsFilePath)!, "error.log");
            File.AppendAllText(logFilePath, $"{DateTime.Now}: {message}{Environment.NewLine}");
        }

        public void AddScheduledScan(ScheduledScan scan)
        {
            _scheduledScans.Add(scan);
            SaveScheduledScans();
        }

        public void RemoveScheduledScan(ScheduledScan scan)
        {
            _scheduledScans.Remove(scan);
            SaveScheduledScans();
        }

        public List<ScheduledScan> GetScheduledScans()
        {
            return _scheduledScans;
        }
    }

    public class ScheduledScan
    {
        public List<string> FilePaths { get; set; }
        public DateTime LastRun { get; set; }
        public TimeSpan Interval { get; set; }
        public bool RunAtStartup { get; set; }
        public bool RunAtShutdown { get; set; }
        public bool SkipOnBattery { get; set; }

        public bool ShouldRun()
        {
            if (SkipOnBattery && SystemInformation.PowerStatus.PowerLineStatus != PowerLineStatus.Online)
                return false;

            if (RunAtStartup && LastRun == DateTime.MinValue)
                return true;

            if (RunAtShutdown && LastRun == DateTime.MinValue)
                return true;

            return DateTime.Now - LastRun >= Interval;
        }
    }
}
