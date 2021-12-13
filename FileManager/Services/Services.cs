using FileManager.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static FileManager.DAO.DAO;

namespace FileManager.Services
{
    public static class Services
    {
        public static Dictionary<string, Color> FileColorSettings { get; set; } = GetUserSettings();
        private static Color DEFAULT_COLOR { get; } = Color.Black;

        private static Dictionary<string, Color> GetUserSettings()
        {
            Dictionary<string, Color> userSettings = ReadUserFileColorSettings();
            if (userSettings == null)
            {
                userSettings = GetDefaultSettings();
                SaveUserFileColorSettings(userSettings);
            }
            return userSettings;
        }

        public static void SaveUserFileColorSettings(Dictionary<string, Color> userSettings)
        {
            FileColorSettings = userSettings;
            DAO.DAO.SaveUserFileColorSettings(userSettings);
        }

        public static Dictionary<string, Color> GetDefaultSettings()
        {
            Dictionary<string, Color> defaultSettings = new Dictionary<string, Color>();
            defaultSettings.Add(".txt", Color.Blue);
            defaultSettings.Add(".dat", Color.Gray);
            defaultSettings.Add(".docx", Color.AliceBlue);
            defaultSettings.Add(".exe", Color.DarkRed);
            defaultSettings.Add(".jpg", Color.GreenYellow);
            defaultSettings.Add(".png", Color.Yellow);
            defaultSettings.Add(".cs", Color.Green);

            return defaultSettings;
        }

        public static string[] GetLogicalDrives()
        {
            return GetLogicalDrivesNames();
        }

        public static Folder[] GetFolders(String directoryName)
        {
            return DAO.DAO.GetFolders(directoryName);
        }

        public static void CreateTreeViewRoot(TreeView tree)
        {
            foreach (String driveName in GetLogicalDrivesNames())
            {
                try
                {
                    tree.Nodes.Add(new TreeNode()
                    {
                        Name = driveName,
                        Text = "Логический диск " + driveName,
                        ImageIndex = 2,
                        SelectedImageIndex = 2
                    });
                    foreach (String nodeName in GetFolders(driveName).Select(f => f.Fullpath))
                    {
                        TreeNode newNode = new TreeNode
                        {
                            Name = nodeName,
                            Text = nodeName.Substring(nodeName.LastIndexOf('\\') + 1),
                            ImageIndex = 0
                        };
                        tree.Nodes[driveName].Nodes.Add(newNode);
                    }
                }
                catch (IOException)
                {
                    // Ignore unaccessible directories
                }
            }
        }

        public static void FillListViewDirectory(ListView listView, String directoryName)
        {
            Folder[] folders = GetFolders(directoryName);
            ListViewItem lw;
            foreach (Folder folder in folders)
            {
                lw = new ListViewItem(new string[] { folder.Name, "", "Папка",
                    folder.LastChange.ToString() }, 0);
                lw.Name = folder.Fullpath;
                listView.Items.Add(lw);
            }
            Entities.File[] files = GetFiles(directoryName);
            foreach (Entities.File file in files)
            {
                lw = new ListViewItem(new string[] { file.Name, file.Length.ToString() + " байт", "Файл",
                    file.LastChange.ToString() }, 1);
                lw.Name = file.Fullpath;
                lw.ForeColor = GetFileColor(file.Name);
                listView.Items.Add(lw);
            }
        }

        private static Color GetFileColor(string fileName)
        {
            int formatStartsAt = fileName.LastIndexOf('.');
            if (formatStartsAt >= 0 &&
                FileColorSettings.ContainsKey(fileName.Substring(formatStartsAt)))
            {
                return FileColorSettings[fileName.Substring(formatStartsAt)];
            }
            return DEFAULT_COLOR;
        }

        public static IComparer GetColumnComparer(int columnNum, bool isAscending)
        {
            switch (columnNum)
            {
                case 0:
                    return GetNameComparer(isAscending);
                case 1:
                    return GetSizeComparer(isAscending);
                case 2:
                    return GetTypeComparer(isAscending);
                case 3:
                    return GetTimeComparer(isAscending);
                default:
                    return GetNameComparer(isAscending);
            }
        }

        static IComparer GetNameComparer(bool isAscending)
        {
            if (isAscending)
            {
                return Comparer<ListViewItem>.Create((x, y) => String.Compare(x.Text, y.Text));
            }
            return Comparer<ListViewItem>.Create((x, y) => -String.Compare(x.Text, y.Text));
        }

        static IComparer GetSizeComparer(bool isAscending)
        {
            if (isAscending)
            {
                return Comparer<ListViewItem>.Create((x, y) => Math.Sign(GetSize(x) - GetSize(y)));
            }
            return Comparer<ListViewItem>.Create((x, y) => -Math.Sign(GetSize(x) - GetSize(y)));
        }

        static IComparer GetTypeComparer(bool isAscending)
        {
            if (isAscending)
            {
                return Comparer<ListViewItem>.Create((x, y) => 
                    String.Compare(x.SubItems[2].Text, y.SubItems[2].Text));
            }
            return Comparer<ListViewItem>.Create((x, y) =>
                -String.Compare(x.SubItems[2].Text, y.SubItems[2].Text));
        }

        static IComparer GetTimeComparer(bool isAscending)
        {
            if (isAscending)
            {
                return Comparer<ListViewItem>.Create((x, y) => DateTime.Compare(GetTime(x), GetTime(y)));
            }
            return Comparer<ListViewItem>.Create((x, y) => -DateTime.Compare(GetTime(x), GetTime(y)));
        }

        static long GetSize(ListViewItem item)
        {
            String sizeText = item.SubItems[1].Text;
            if (sizeText.Length == 0)
            {
                return -1;
            }
            sizeText = sizeText.Substring(0, sizeText.Length - 5);
            return long.Parse(sizeText);
        }

        static DateTime GetTime(ListViewItem item)
        {
            return DateTime.Parse(item.SubItems[3].Text);
        }

        public static void Execute(String fileName)
        {
            DAO.DAO.Execute(fileName);
        }

        public static void UpdateNodesContent(TreeNode treeNode)
        {
            String[] currentSubNodes = new String[treeNode.Nodes.Count];
            for (int i = 0; i < treeNode.Nodes.Count; i++)
            {
                currentSubNodes[i] = treeNode.Nodes[i].Name;
            }
            String[] actualSubDirs = Directory.GetDirectories(@treeNode.Name);
            String[] commonElements = currentSubNodes.Intersect(actualSubDirs).ToArray();
            String[] excessSubNodes = currentSubNodes.Except(commonElements).ToArray();
            String[] extraSubNodes = actualSubDirs.Except(commonElements).ToArray();
            foreach (String nodeName in excessSubNodes)
            {
                treeNode.Nodes.RemoveByKey(nodeName);
            }
            foreach (String nodeName in extraSubNodes)
            {
                TreeNode newNode = new TreeNode
                {
                    Name = nodeName,
                    Text = nodeName.Substring(nodeName.LastIndexOf('\\') + 1),
                    ImageIndex = 0
                };
                treeNode.Nodes.Add(newNode);
            }
            foreach (String nodeName in actualSubDirs)
            {
                try
                {
                    if (treeNode.Nodes[nodeName].Nodes.Count == 0 &&
                        Directory.GetDirectories(@nodeName).Count() > 0)
                    {
                        treeNode.Nodes[nodeName].Nodes.Add(@nodeName + @"\\", "");
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    // Skip not available directories
                }
            }
        }

        public static void MoveFolder(String folderName, String destination)
        {
            destination = destination + "\\" + folderName.Substring(folderName.LastIndexOf('\\') + 1);
            DAO.DAO.MoveFolder(folderName, destination);
        }

        public static void CopyFolder(String folderName, String destination)
        {
            destination = destination + "\\" + folderName.Substring(folderName.LastIndexOf('\\') + 1);
            DAO.DAO.CopyFolder(folderName, destination);
        }

        public static void CopyFile(String fileName, String destination)
        {
            destination = destination + "\\" + fileName.Substring(fileName.LastIndexOf('\\') + 1);
            DAO.DAO.CopyFile(fileName, destination);
        }

        public static void MoveFile(String fileName, String destination)
        {
            destination = destination + "\\" + fileName.Substring(fileName.LastIndexOf('\\') + 1);
            DAO.DAO.MoveFile(fileName, destination);
        }

        public static void DeleteFile(String fileName)
        {
            DAO.DAO.DeleteFile(fileName);
        }

        public static void DeleteFolder(String folderName)
        {
            DAO.DAO.DeleteFolder(folderName);
        }

        public static void RenameFile(String oldName, String newName)
        {
            newName = oldName.Substring(0, oldName.LastIndexOf('\\') + 1) + newName;
            DAO.DAO.RenameFile(oldName, newName);
        }

        public static void RenameFolder(String oldName, String newName)
        {
            newName = oldName.Substring(0, oldName.LastIndexOf('\\') + 1) + newName;
            DAO.DAO.RenameFolder(oldName, newName);
        }

        public static void CreateFile(String fileName, String destination)
        {
            DAO.DAO.CreateFile(destination + "\\" + fileName);
        }

        public static void CreateFolder(String folderName, String destination)
        {
            DAO.DAO.CreateFolder(destination + "\\" + folderName);
        }
    }
}
