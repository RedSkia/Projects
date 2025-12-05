using CoreRP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ScriptBundler
{
    internal sealed class Program
    {
        private static string rootPath => Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "..\\..\\.."));
        private static string codePath => $"{rootPath}\\{Settings.Name}";
        private static string resultPath => $"E:\\Skrivebord\\Dev Server\\LiveServer\\Oxide\\plugins\\{Settings.Name}.cs";

        #region Disable WinControls
        private const int MF_BYCOMMAND = 0x00000000;
        private const int SC_MINIMIZE = 0xF020;
        private const int SC_MAXIMIZE = 0xF030;
        private const int SC_SIZE = 0xF000;

        [DllImport("user32.dll")]
        private static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();
        #endregion Disable WinControls
        private static void Main(string[] args)
        {
            #region Disable WinControls
            IntPtr handle = GetConsoleWindow();
            IntPtr sysMenu = GetSystemMenu(handle, false);
            if (handle != IntPtr.Zero)
            {
                DeleteMenu(sysMenu, SC_MINIMIZE, MF_BYCOMMAND);
                DeleteMenu(sysMenu, SC_MAXIMIZE, MF_BYCOMMAND);
                DeleteMenu(sysMenu, SC_SIZE, MF_BYCOMMAND);
            }
            #endregion Disable WinControls

            Console.SetWindowSize(50, 5);
            Console.SetBufferSize(50, 5);
            Console.Title = Assembly.GetExecutingAssembly().GetName().Name;


            var globalFiles = Directory.GetFiles($"{codePath}", "*.cs", SearchOption.TopDirectoryOnly);
            var coreFiles = Directory.GetFiles($"{codePath}\\CoreRP", "*.cs", SearchOption.AllDirectories);
            var zoneManagerFiles = Directory.GetFiles($"{codePath}\\ZoneManager", "*.cs", SearchOption.AllDirectories);

            var Files = new[] {
                globalFiles,
                coreFiles,
                zoneManagerFiles,
            }.SelectMany(x => x).OrderBy(x => x == Settings.Name);


            SortedSet<string> usingLines = new SortedSet<string>();
            SortedSet<string> definitionsLines = new SortedSet<string>();
            List<string> fileLines = new List<string>();
            foreach (var file in Files)
            {
                string[] lines = File.ReadAllLines(file); /*All lines*/

                definitionsLines.UnionWith(lines.Where(line => line.StartsWith("#define"))); /*Lines with #define*/
                usingLines.UnionWith(lines.Where(line => line.StartsWith("using"))); /*Lines with using*/

                /*Everything else*/
                fileLines.AddRange(lines.Where(line =>
                !line.StartsWith("#define") &&
                !line.StartsWith("using")
                ));
            }
            var ResultFileLines = new[] { definitionsLines.ToArray(), usingLines.ToArray(), fileLines.ToArray() }.SelectMany(line => line);

            File.WriteAllLines(resultPath, ResultFileLines);
            Console.Clear();
            Console.WriteLine($"Sucess! \"{resultPath}\"");
        }
    }
}