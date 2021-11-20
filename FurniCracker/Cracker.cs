using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace FurniCracker
{
    static class Cracker
    {
        public static void Initialize()
        {
            Logging.WriteLine("Preparing to crack...\n", ConsoleColor.Yellow);

            string[] Files = Directory.GetFiles("ToCrack", "*.swf");
            Logging.WriteLine("Listed " + Files.Length + " files to crack.", ConsoleColor.Blue);

            foreach (string CurrentFile in Files)
            {
                string CurrentShortName = CurrentFile.Substring(8).Split('.')[0];
                if (Directory.Exists("temp\\" + CurrentShortName))
                    Directory.Delete("temp\\" + CurrentShortName, true);
                Directory.CreateDirectory("temp\\" + CurrentShortName);
                File.Copy(CurrentFile, "temp\\" + CurrentShortName + "\\" + CurrentShortName + ".swf");
                Decompile(CurrentShortName);

                string[] Binaries = Directory.GetFiles("temp\\" + CurrentShortName, "*.bin");
                Crack(CurrentShortName, Binaries);
                Directory.Delete("temp\\" + CurrentShortName, true);
            }
            Logging.WriteLine("");
            Logging.WriteLine("Done :)", ConsoleColor.Yellow);
            Console.ReadLine();
        }

        private static void Crack(string CurrentShortName, string[] Binaries)
        {
            foreach (string CurrentFile in Binaries)
            {
                StreamReader Reader = new StreamReader(CurrentFile);
                string Content = Reader.ReadToEnd();
                Reader.Close();

                if (!Content.Contains("<visualizationData"))
                    continue;
                if (Content.Contains("<graphics>"))
                {
                    Logging.WriteLine(CurrentShortName + " is already cracked.", ConsoleColor.Yellow);
                    return;
                }

                string replace1 = "<visualizationData type=\"" + CurrentShortName + "\">";
                string replace2 = "<visualizationData type=\"" + CurrentShortName + "\"><graphics>";
                Content = Content.Replace(replace1, replace2);

                replace1 = "</visualizationData>";
                replace2 = "</graphics></visualizationData>";
                Content = Content.Replace(replace1, replace2);

                StreamWriter Writer = new StreamWriter(CurrentFile);
                Writer.Write(Content);
                Writer.Close();

                Compile(CurrentShortName, new Regex("-(.*).bin").Match(CurrentFile).Groups[1].ToString());
                File.Move("temp\\" + CurrentShortName + "\\" + CurrentShortName + ".swf", "Cracked\\" + CurrentShortName + ".swf");
                Logging.WriteLine(CurrentShortName + " cracked! ", ConsoleColor.Green);
            }
        }

        private static void Decompile(string Item)
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = "edit.bat",
                Arguments = "d " + Item,
                WindowStyle = ProcessWindowStyle.Hidden
            }).WaitForExit();
        }

        private static void Compile(string Name, string Id)
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = "edit.bat",
                Arguments = "c " + Name + " " + Id,
                WindowStyle = ProcessWindowStyle.Hidden
            }).WaitForExit();
        }
    }
}
