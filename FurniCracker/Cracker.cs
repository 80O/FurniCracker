using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FurniCracker
{
    static class Cracker
    {
        public static void Initialize()
        {
            Logging.WriteLine("Input folder: ToCrack, Output folder: Cracked");
            Logging.WriteLine("Preparing to crack...\n", ConsoleColor.Yellow);

            Directory.CreateDirectory("ToCrack");
            Directory.CreateDirectory("Cracked");
            Directory.CreateDirectory("temp");

            var files = Directory.GetFiles("ToCrack", "*.swf");
            Logging.WriteLine("Listed " + files.Length + " files to crack.", ConsoleColor.Blue);

            Parallel.ForEach(files, currentFile =>
            {
                var currentShortName = currentFile.Substring(8).Split('.')[0];
                if (Directory.Exists("temp\\" + currentShortName))
                    Directory.Delete("temp\\" + currentShortName, true);
                Directory.CreateDirectory("temp\\" + currentShortName);
                File.Copy(currentFile, "temp\\" + currentShortName + "\\" + currentShortName + ".swf");
                Decompile(currentShortName);

                var binaries = Directory.GetFiles("temp\\" + currentShortName, "*.bin");
                Crack(currentShortName, binaries);
                Directory.Delete("temp\\" + currentShortName, true);
            });
            Logging.WriteLine("");
            Logging.WriteLine("Done :)", ConsoleColor.Yellow);
            Console.ReadLine();
        }

        private static void Crack(string currentShortName, IEnumerable<string> binaries)
        {
            foreach (var currentFile in binaries)
            {
                var reader = new StreamReader(currentFile);
                var content = reader.ReadToEnd();
                reader.Close();

                if (!content.Contains("<visualizationData"))
                    continue;
                if (content.Contains("<graphics>"))
                {
                    Logging.WriteLine(currentShortName + " is already cracked.", ConsoleColor.Yellow);
                    return;
                }

                // Remove old closing tag for bugged furnis.
                content = content.Replace("</graphics>", "");

                var shortName = FindShortName(currentShortName, content);
                var replace1 = "<visualizationData type=\"" + shortName + "\">";
                var replace2 = "<visualizationData type=\"" + shortName + "\"><graphics>";
                content = content.Replace(replace1, replace2);

                replace1 = "</visualizationData>";
                replace2 = "</graphics></visualizationData>";
                content = content.Replace(replace1, replace2);

                var writer = new StreamWriter(currentFile);
                writer.Write(content);
                writer.Close();

                Compile(currentShortName, new Regex("-(.*).bin").Match(currentFile).Groups[1].ToString());
                try
                {
                    File.Move("temp\\" + currentShortName + "\\" + currentShortName + ".swf", "Cracked\\" + currentShortName + ".swf");
                    Logging.WriteLine(currentShortName + " cracked! ", ConsoleColor.Green);
                }
                catch
                {
                    // Ignored
                }
            }
        }

        private static string FindShortName(string currentShortName, string content)
        {
            if (content.Contains("<visualizationData type=\"" + currentShortName + "\">"))
                return currentShortName;

            return content.Split('\n').First(s => s.Contains("<visualizationData ")).Replace("<visualizationData ", "")
                .Replace("type=", "").Replace("\"", "").Replace(">", "");
        }

        private static void Decompile(string item)
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = "edit.bat",
                Arguments = "d " + item,
                WindowStyle = ProcessWindowStyle.Hidden
            }).WaitForExit();
        }

        private static void Compile(string name, string id)
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = "edit.bat",
                Arguments = "c " + name + " " + id,
                WindowStyle = ProcessWindowStyle.Hidden
            }).WaitForExit();
        }
    }
}
