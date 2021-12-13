using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using static FileManager.Services.Services;
using FileManager.Views;
using System.Threading;

namespace FileManager
{
    public partial class MainForm : Form
    {
        private List<String> VisitedAdresses { get; set; } = new List<String>();

        private int CurrentAdressIndex { get; set; } = -1;

        private String CurrentListViewAdress { get; set; } = "";

        private bool[] OrderSigns { get; set; } = new bool[4];

        private List<String> FileBuffer { get; set; } = new List<String>();

        private List<String> FolderBuffer { get; set; } = new List<String>();

        private bool IsCutting { get; set; } = false;

        public MainForm()
        {
            InitializeComponent();
            DirectoryContentListView.ContextMenuStrip = optionsContextMenuStrip;
            DirectoryContentListView.ColumnClick += new ColumnClickEventHandler(ClickOnColumn);
            DirectoryContentListView.Columns.Add(new ColumnHeader() { Text = "Имя", Width = 160 });
            DirectoryContentListView.Columns.Add(new ColumnHeader() { Text = "Размер", Width = 100 });
            DirectoryContentListView.Columns.Add(new ColumnHeader() { Text = "Тип", Width = 60 });
            DirectoryContentListView.Columns.Add(new ColumnHeader() { Text = "Дата изменения", Width = 115 });
            CreateTreeViewRoot(FileCatalogTreeView);
            var daemonListener = new Thread(ListenForNewLogicalDrives);
            daemonListener.IsBackground = true;
            daemonListener.Start();
        }

        private void ListenForNewLogicalDrives()
        {
            while (true)
            {
                Thread.Sleep(3000);
                FileCatalogTreeView.Invoke((Action)delegate
                {
                    foreach (string driveName in GetLogicalDrives())
                    {
                        if (!FileCatalogTreeView.Nodes.ContainsKey(driveName))
                        {
                            try
                            {
                                FileCatalogTreeView.Nodes.Add(new TreeNode()
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
                                    FileCatalogTreeView.Nodes[driveName].Nodes.Add(newNode);
                                }
                            }
                            catch (IOException)
                            {
                                // Ignore unaccessible directories
                            }
                        }
                    }
                    foreach (TreeNode drive in FileCatalogTreeView.Nodes)
                    {
                        if (!GetLogicalDrives().Any(dn => dn.Equals(drive.Name)))
                        {
                            FileCatalogTreeView.Nodes.RemoveByKey(drive.Name);
                        }
                    }
                });
            }
        }



        private void FileCatalogTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (VisitedAdresses.Count == 0)
            {
                toolStripBackButton.Enabled = false;
            }
            else
            {
                toolStripBackButton.Enabled = true;
            }
            if (CurrentAdressIndex < VisitedAdresses.Count - 1)
            {
                VisitedAdresses = VisitedAdresses.Take(CurrentAdressIndex + 1).ToList();
            }
            VisitedAdresses.Add(e.Node.Name);
            CurrentAdressIndex++;
            toolStripForwardButton.Enabled = false;
            DirectoryContentListView.Items.Clear();
            CurrentListViewAdress = e.Node.Name;
            toolStripAdressTextBox.Text = CurrentListViewAdress;
            try
            {
                FillListViewDirectory(DirectoryContentListView, CurrentListViewAdress);
            }
            catch (IOException)
            {
                // Ignore unaccessible directories
            }
            catch (UnauthorizedAccessException)
            {
                // Skip not available directories
            }
        }

        private void IconsListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DirectoryContentListView.View = View.SmallIcon;
        }

        private void ImagesListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DirectoryContentListView.View = View.LargeIcon;
        }

        private void TileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DirectoryContentListView.View = View.Tile;
        }

        private void SimpleListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DirectoryContentListView.View = View.List;
        }

        private void DetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DirectoryContentListView.View = View.Details;
        }

        private void DirectoryContentListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (DirectoryContentListView.SelectedItems[0].ImageIndex == 0)
            {
                // Folder opening
                if (CurrentAdressIndex < VisitedAdresses.Count - 1)
                {
                    VisitedAdresses = VisitedAdresses.Take(CurrentAdressIndex + 1).ToList();
                }
                VisitedAdresses.Add(DirectoryContentListView.SelectedItems[0].Name);
                CurrentListViewAdress = VisitedAdresses[++CurrentAdressIndex];
                toolStripForwardButton.Enabled = false;
                toolStripBackButton.Enabled = true;
                toolStripAdressTextBox.Text = CurrentListViewAdress;
                DirectoryContentListView.Items.Clear();
                try
                {
                    FillListViewDirectory(DirectoryContentListView, CurrentListViewAdress);
                }
                catch (IOException)
                {
                    // Ignore unaccessible directories
                }
                catch (UnauthorizedAccessException)
                {
                    // Skip not available directories
                }
            }
            else
            {
                // File execution
                Execute(DirectoryContentListView.SelectedItems[0].Name);
            }
        }

        private void ClickOnColumn(object sender, ColumnClickEventArgs e)
        {
            ListView currentListView = (ListView)sender;
            currentListView.ListViewItemSorter = GetColumnComparer(e.Column, OrderSigns[e.Column]);
            OrderSigns[e.Column] = !OrderSigns[e.Column];
        }

        private void UpdateDirectoryContentListView()
        {
            DirectoryContentListView.Items.Clear();
            try
            {
                FillListViewDirectory(DirectoryContentListView, CurrentListViewAdress);
            }
            catch (IOException)
            {
                // Ignore unaccessible directories
            }
            catch (UnauthorizedAccessException)
            {
                // Skip not available directories
            }
        }

        private void ColorSettingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var settingsForm = new FileColorSettingsForm();
            settingsForm.Show();
        }

        private void FileCatalogTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            UpdateNodesContent(e.Node);
        }

        private void ToolStripBackButton_Click(object sender, EventArgs e)
        {
            CurrentAdressIndex--;
            CurrentListViewAdress = VisitedAdresses[CurrentAdressIndex];
            toolStripForwardButton.Enabled = true;
            if (CurrentAdressIndex == 0)
            {
                toolStripBackButton.Enabled = false;
            }
            toolStripAdressTextBox.Text = CurrentListViewAdress;
            DirectoryContentListView.Items.Clear();
            try
            {
                FillListViewDirectory(DirectoryContentListView, CurrentListViewAdress);
            }
            catch (IOException)
            {
                // Ignore unaccessible directories
            }
            catch (UnauthorizedAccessException)
            {
                // Skip not available directories
            }
        }

        private void ToolStripForwardButton_Click(object sender, EventArgs e)
        {
            CurrentAdressIndex++;
            CurrentListViewAdress = VisitedAdresses[CurrentAdressIndex];
            if (CurrentAdressIndex + 1 == VisitedAdresses.Count)
            {
                toolStripForwardButton.Enabled = false;
            }
            toolStripBackButton.Enabled = true;
            toolStripAdressTextBox.Text = CurrentListViewAdress;
            DirectoryContentListView.Items.Clear();
            try
            {
                FillListViewDirectory(DirectoryContentListView, CurrentListViewAdress);
            }
            catch (IOException)
            {
                // Ignore unaccessible directories
            }
            catch (UnauthorizedAccessException)
            {
                // Skip not available directories
            }
        }

        private void ToolStripAdressTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                try
                {
                    DirectoryContentListView.Items.Clear();
                    FillListViewDirectory(DirectoryContentListView, toolStripAdressTextBox.Text);
                    if (CurrentAdressIndex < VisitedAdresses.Count - 1)
                    {
                        VisitedAdresses = VisitedAdresses.Take(CurrentAdressIndex + 1).ToList();
                    }
                    VisitedAdresses.Add(toolStripAdressTextBox.Text);
                    CurrentListViewAdress = VisitedAdresses[++CurrentAdressIndex];
                    toolStripForwardButton.Enabled = false;
                    toolStripBackButton.Enabled = true;
                }
                catch (IOException)
                {
                    // Ignore unaccessible directories (go back)
                    toolStripAdressTextBox.Text = CurrentListViewAdress;
                    FillListViewDirectory(DirectoryContentListView, CurrentListViewAdress);
                }
                catch (UnauthorizedAccessException)
                {
                    // Skip not available directories (go back)
                    toolStripAdressTextBox.Text = CurrentListViewAdress;
                    FillListViewDirectory(DirectoryContentListView, CurrentListViewAdress);
                }
            }
        }

        private void ToolStripUpButton_Click(object sender, EventArgs e)
        {
            int lastSlashIndex = toolStripAdressTextBox.Text.LastIndexOf('\\');
            if (lastSlashIndex != toolStripAdressTextBox.Text.Length - 1)
            {
                int firstSlashIndex = toolStripAdressTextBox.Text.IndexOf('\\');
                if (lastSlashIndex == firstSlashIndex)
                {
                    toolStripAdressTextBox.Text = toolStripAdressTextBox.Text.Substring(0, lastSlashIndex + 1);
                }
                else
                {
                    toolStripAdressTextBox.Text = toolStripAdressTextBox.Text.Substring(0, lastSlashIndex);
                }
                try
                {
                    DirectoryContentListView.Items.Clear();
                    FillListViewDirectory(DirectoryContentListView, toolStripAdressTextBox.Text);
                    if (CurrentAdressIndex < VisitedAdresses.Count - 1)
                    {
                        VisitedAdresses = VisitedAdresses.Take(CurrentAdressIndex + 1).ToList();
                    }
                    VisitedAdresses.Add(toolStripAdressTextBox.Text);
                    CurrentListViewAdress = VisitedAdresses[++CurrentAdressIndex];
                    toolStripForwardButton.Enabled = false;
                    toolStripBackButton.Enabled = true;
                }
                catch (IOException)
                {
                    // Ignore unaccessible directories (go back)
                    toolStripAdressTextBox.Text = CurrentListViewAdress;
                    FillListViewDirectory(DirectoryContentListView, CurrentListViewAdress);
                }
                catch (UnauthorizedAccessException)
                {
                    // Skip not available directories (go back)
                    toolStripAdressTextBox.Text = CurrentListViewAdress;
                    FillListViewDirectory(DirectoryContentListView, CurrentListViewAdress);
                }
            }
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IsCutting = false;
            FileBuffer.Clear();
            FolderBuffer.Clear();
            foreach (ListViewItem item in DirectoryContentListView.SelectedItems)
            {
                if (item.ImageIndex == 0)
                {
                    FolderBuffer.Add(item.Name);
                }
                else if (item.ImageIndex == 1)
                {
                    FileBuffer.Add(item.Name);
                }
            }
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (String folderName in FolderBuffer)
            {
                if (IsCutting)
                {
                    MoveFolder(folderName, CurrentListViewAdress);
                }
                else
                {
                    CopyFolder(folderName, CurrentListViewAdress);
                }
            }
            foreach (String fileName in FileBuffer)
            {
                if (IsCutting)
                {
                    MoveFile(fileName, CurrentListViewAdress);
                }
                else
                {
                    CopyFile(fileName, CurrentListViewAdress);
                }
            }
            UpdateDirectoryContentListView();
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IsCutting = true;
            FileBuffer.Clear();
            FolderBuffer.Clear();
            foreach (ListViewItem item in DirectoryContentListView.SelectedItems)
            {
                if (item.ImageIndex == 0)
                {
                    FolderBuffer.Add(item.Name);
                }
                else if (item.ImageIndex == 1)
                {
                    FileBuffer.Add(item.Name);
                }
            }
        }

        private void DeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in DirectoryContentListView.SelectedItems)
            {
                if (item.ImageIndex == 0)
                {
                    DeleteFolder(item.Name);
                }
                else if (item.ImageIndex == 1)
                {
                    DeleteFile(item.Name);
                }
            }
            UpdateDirectoryContentListView();
        }

        private void RenameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DirectoryContentListView.SelectedItems.Count == 0)
            {
                return;
            }
            String newName = DirectoryContentListView.SelectedItems[0].Text;
            if (MainForm.InputBox("Переименование", "Новое имя:", ref newName) == DialogResult.OK)
            {
                if (DirectoryContentListView.SelectedItems[0].ImageIndex == 0)
                {
                    RenameFolder(DirectoryContentListView.SelectedItems[0].Name, newName);
                }
                else if (DirectoryContentListView.SelectedItems[0].ImageIndex == 1)
                {
                    RenameFile(DirectoryContentListView.SelectedItems[0].Name, newName);
                }
            }
            UpdateDirectoryContentListView();
        }

        private void CreateFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String newName = "Новый файл.txt";
            if (MainForm.InputBox("Создание файла", "Имя нового файла:", ref newName) == DialogResult.OK)
            {
                CreateFile(newName, CurrentListViewAdress);
            }
            UpdateDirectoryContentListView();
        }

        private void CreateFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String newName = "Новая папка";
            if (MainForm.InputBox("Создание папки", "Имя новой папки:", ref newName) == DialogResult.OK)
            {
                CreateFolder(newName, CurrentListViewAdress);
            }
            UpdateDirectoryContentListView();
        }

        private void RefreshToolStripButton_Click(object sender, EventArgs e)
        {
            UpdateDirectoryContentListView();
        }

        private static DialogResult InputBox(string title, string promptText, ref String value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }
    }
}