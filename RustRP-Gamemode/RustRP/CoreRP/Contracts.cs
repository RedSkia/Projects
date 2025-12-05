using Oxide.Core;
using System;

namespace CoreRP
{
    internal static class Contracts
    {
        internal interface IChatCommand
        {
            string Command { get; }
            void OnCommand(BasePlayer player, string command, string[] args);
        }
        internal abstract class BaseData<T> where T : class
        {
            private string FilePath => $"{Settings.Name}\\{this.FileName}";
            internal T CachedData { get; set; } = Activator.CreateInstance<T>();
            internal abstract string FileName { get; }
            internal virtual T LoadData(string args = "")
            {
                string path = String.IsNullOrEmpty(args) ? FilePath : $"{FilePath}\\{args}";
                return this.CachedData = Interface.Oxide.DataFileSystem.ReadObject<T>(path) ?? Activator.CreateInstance<T>();
            }
            internal virtual void SaveData(string args = "", object data = null)
            {
                string path = String.IsNullOrEmpty(args) ? FilePath : $"{FilePath}\\{args}";
                object obj = data ?? this.CachedData ?? Activator.CreateInstance<T>();
                Interface.Oxide.DataFileSystem.WriteObject(path, obj);
            }
        }
        internal abstract class BaseScript
        {
            internal bool IsEnabled { get; private set; }
            internal virtual void ScriptCheck()
            {
                if (this.IsEnabled) { return; }
                throw new InvalidOperationException("Script is not enabled.");
            }
            internal virtual void Load(string Namespace = "")
            {
                this.IsEnabled = true;
                FileLogger.LogMessage("Loaded", FileLogger.LogType.Important, false, Namespace?.Split('.')?.ElementAt(1));
            }
            internal virtual void Unload(string Namespace = "")
            {
                this.IsEnabled = false;
                FileLogger.LogMessage("Unloaded", FileLogger.LogType.Important, false, Namespace?.Split('.')?.ElementAt(1));
            }
        }
    }
}