using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using OverlayNotepad.Helpers;
using OverlayNotepad.Models;

namespace OverlayNotepad.Services
{
    public class SettingsManager
    {
        public static readonly SettingsManager Instance = new SettingsManager();

        public AppSettings Current { get; private set; } = AppSettings.CreateDefault();

        public string SettingsDirectory =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OverlayNotepad");

        public string SettingsFilePath => Path.Combine(SettingsDirectory, "settings.json");
        public string MemoFilePath => Path.Combine(SettingsDirectory, "memo.txt");
        public string BackupFilePath => Path.Combine(SettingsDirectory, "settings.json.bak");

        private SettingsManager() { }

        public void Load()
        {
            EnsureDirectory();

            if (!File.Exists(SettingsFilePath))
            {
                Current = AppSettings.CreateDefault();
                Save();
                return;
            }

            try
            {
                string json = File.ReadAllText(SettingsFilePath, Encoding.UTF8);
                AppSettings loaded = JsonHelper.Deserialize(json);

                if (loaded != null)
                {
                    Current = loaded;
                }
                else
                {
                    RestoreFromCorrupted();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SettingsManager] Load 실패: {ex.Message}");
                RestoreFromCorrupted();
            }
        }

        public void Save()
        {
            try
            {
                EnsureDirectory();
                string json = JsonHelper.Serialize(Current);
                string tempPath = SettingsFilePath + ".tmp";
                File.WriteAllText(tempPath, json, Encoding.UTF8);

                if (File.Exists(SettingsFilePath))
                    File.Delete(SettingsFilePath);

                File.Move(tempPath, SettingsFilePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SettingsManager] Save 실패: {ex.Message}");
            }
        }

        public void SaveMemo(string text)
        {
            try
            {
                EnsureDirectory();
                string tempPath = MemoFilePath + ".tmp";
                File.WriteAllText(tempPath, text, new UTF8Encoding(false));

                if (File.Exists(MemoFilePath))
                    File.Delete(MemoFilePath);

                File.Move(tempPath, MemoFilePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SettingsManager] SaveMemo 실패: {ex.Message}");
            }
        }

        public string LoadMemo()
        {
            try
            {
                if (!File.Exists(MemoFilePath))
                    return string.Empty;

                return File.ReadAllText(MemoFilePath, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SettingsManager] LoadMemo 실패: {ex.Message}");
                return string.Empty;
            }
        }

        private void RestoreFromCorrupted()
        {
            try
            {
                if (File.Exists(SettingsFilePath))
                    File.Copy(SettingsFilePath, BackupFilePath, overwrite: true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SettingsManager] 백업 실패: {ex.Message}");
            }

            Current = AppSettings.CreateDefault();
            Save();
        }

        private void EnsureDirectory()
        {
            if (!Directory.Exists(SettingsDirectory))
                Directory.CreateDirectory(SettingsDirectory);
        }
    }
}
