using FileManager.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace FileManager.DAO
{
    public static class DAO
    {
        private const string FILE_COLOR_SETTINGS_FILENAME = "settings.dat";

        public static String[] GetLogicalDrivesNames()
        {
            return Environment.GetLogicalDrives();
        }

        public static Folder[] GetFolders(String directoryName)
        {
            List<Folder> folders = new List<Folder>();
            foreach (String folderName in Directory.GetDirectories(@directoryName))
            {
                FileInfo info = new FileInfo(@folderName);
                folders.Add(new Folder(folderName, info.LastWriteTime));
            }
            return folders.ToArray();
        }

        public static Entities.File[] GetFiles(String directoryName)
        {
            List<Entities.File> files = new List<Entities.File>();
            foreach (String fileName in Directory.GetFiles(@directoryName))
            {
                FileInfo info = new FileInfo(@fileName);
                files.Add(new Entities.File(fileName, info.LastWriteTime, info.Length));
            }
            return files.ToArray();
        }

        public static void Execute(String fileName)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = fileName;
            process.Start();
        }

        public static void CopyFolder(String source, String destination)
        {
            var stack = new Stack<AdressVector>();
            stack.Push(new AdressVector(source, destination));

            while (stack.Count > 0)
            {
                var folders = stack.Pop();
                Directory.CreateDirectory(folders.Destination);
                foreach (var file in Directory.GetFiles(folders.Source, "*.*"))
                {
                    System.IO.File.Copy(file, Path.Combine(folders.Destination, Path.GetFileName(file)), true);
                }

                foreach (var folder in Directory.GetDirectories(folders.Source))
                {
                    stack.Push(new AdressVector(folder, Path.Combine(folders.Destination, Path.GetFileName(folder))));
                }
            }
        }

        public static bool MoveFolder(String source, String destination)
        {
            try
            {
                Directory.Move(source, destination);
                return true;
            }
            catch (IOException)
            {
                return false;
            }
        }

        public static bool CopyFile(String source, String destination)
        {
            try
            {
                System.IO.File.Copy(source, destination, true);
                return true;
            }
            catch (IOException)
            {
                return false;
            }
        }

        public static void MoveFile(String source, String destination)
        {
            System.IO.File.Move(source, destination);
        }

        public static void DeleteFile(String source)
        {
            System.IO.File.Delete(source);
        }

        public static void DeleteFolder(String source)
        {
            Directory.Delete(source, true);
        }

        public static void RenameFile(String oldName, String newName)
        {
            System.IO.File.Move(oldName, newName);
        }

        public static void RenameFolder(String oldName, String newName)
        {
            Directory.Move(oldName, newName);
        }

        public static void CreateFile(String fileName)
        {
            using (System.IO.File.Create(fileName)) { }
        }

        public static void CreateFolder(String folderName)
        {
            Directory.CreateDirectory(folderName);
        }

        public static Dictionary<string, Color> ReadUserFileColorSettings()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Dictionary<string, Color> userSettings;
            try
            {
                using (FileStream fs = new FileStream(FILE_COLOR_SETTINGS_FILENAME, FileMode.Open))
                {
                    userSettings = (Dictionary<string, Color>)formatter.Deserialize(fs);
                }
            }
            catch (FileNotFoundException)
            {
                userSettings = null;
            }
            catch (SerializationException)
            {
                userSettings = null;
            }
            return userSettings;
        }

        public static void SaveUserFileColorSettings(Dictionary<string, Color> userSettings)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(FILE_COLOR_SETTINGS_FILENAME, FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, userSettings);
            }
        }
    }
}
