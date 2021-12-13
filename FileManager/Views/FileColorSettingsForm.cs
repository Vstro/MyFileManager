using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static FileManager.Services.Services;


namespace FileManager.Views
{
    public partial class FileColorSettingsForm : Form
    {
        public Dictionary<string, Color> FileColorSettings { get; set; } = Services.Services.FileColorSettings;

        public FileColorSettingsForm()
        {
            InitializeComponent();
            FileColorSettingsListView.ContextMenuStrip = FileColorSettingsContextMenuStrip;
            FileColorSettingsListView.Columns.Add(new ColumnHeader() { Text = "Расширение файла", Width = 170 });
            FileColorSettingsListView.Columns.Add(new ColumnHeader() { Text = "Цвет представления", Width = 175 });
            FileColorSettingsListView.View = View.Details;
            RefreshCurrentSettingsView();
        }

        private void RefreshCurrentSettingsView()
        {
            FileColorSettingsListView.Items.Clear();
            foreach (var key in FileColorSettings.Keys)
            {
                var lw = new ListViewItem(new string[]
                {
                    key,
                    FileColorSettings[key].ToArgb().ToString()
                });
                lw.ForeColor = FileColorSettings[key];
                FileColorSettingsListView.Items.Add(lw);
            }
        }

        private void EditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string format = FileColorSettingsListView.SelectedItems[0].Text;
            Color color = FileColorSettingsListView.SelectedItems[0].ForeColor;
            if (FileColorInputBox(ref format, ref color) == DialogResult.OK)
            {
                FileColorSettings.Remove(FileColorSettingsListView.SelectedItems[0].Text);
                if (FileColorSettings.Keys.ToArray().Any(key => key.Equals(format)))
                {
                    MessageBox.Show("Некорректное расширение! Расширения в списке настроек должны быть уникальны!", "Ошибка ввода");
                    FileColorSettings.Add(FileColorSettingsListView.SelectedItems[0].Text,
                        FileColorSettingsListView.SelectedItems[0].ForeColor);
                }
                else
                {
                    FileColorSettings.Add(format, color);
                }
                RefreshCurrentSettingsView();
            }
        }

        private void DeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in FileColorSettingsListView.SelectedItems)
            {
                FileColorSettings.Remove(item.Text);
            }
            SaveUserFileColorSettings(FileColorSettings);
            RefreshCurrentSettingsView();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            string format = ".";
            Color color = Color.Empty;
            if (FileColorInputBox(ref format, ref color) == DialogResult.OK)
            {
                if (FileColorSettings.Keys.ToArray().Any(key => key.Equals(format))
                    || format[0] != '.')
                {
                    MessageBox.Show("Некорректное расширение! Расширения в списке настроек должны быть уникальны и начинаться с \".\"!", "Ошибка ввода");
                    return;
                }
                FileColorSettings.Add(format, color);
                RefreshCurrentSettingsView();
            }
        }

        private void DefaultButton_Click(object sender, EventArgs e)
        {
            FileColorSettings = GetDefaultSettings();
            SaveUserFileColorSettings(FileColorSettings);
            RefreshCurrentSettingsView();
        }

        private static DialogResult FileColorInputBox(ref string format, ref Color color)
        {
            Form form = new Form();
            Label formatLabel = new Label();
            Label colorLabel = new Label();
            TextBox formatTextBox = new TextBox();
            TextBox colorTextBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = color == Color.Empty ? "Новая настройка" : "Редактирование настройки";
            formatLabel.Text = "Расширение файла";
            colorLabel.Text = "Цвет представления";
            formatTextBox.Text = format;
            colorTextBox.Text = color == Color.Empty ? "" : color.ToArgb().ToString();

            buttonOk.Text = "ОК";
            buttonCancel.Text = "Отмена";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            formatLabel.SetBounds(10, 20, 100, 20);
            colorLabel.SetBounds(10, 50, 100, 20);
            formatTextBox.SetBounds(110, 20, 100, 20);
            colorTextBox.SetBounds(110, 50, 100, 20);
            buttonOk.SetBounds(110, 90, 75, 23);
            buttonCancel.SetBounds(200, 90, 75, 23);

            formatTextBox.Anchor = formatTextBox.Anchor | AnchorStyles.Right;
            colorTextBox.Anchor = colorTextBox.Anchor | AnchorStyles.Right;

            form.Controls.AddRange(new Control[] 
            { 
                formatLabel, 
                colorLabel,
                formatTextBox,
                colorTextBox,
                buttonOk, 
                buttonCancel 
            });
            form.ClientSize = new Size(400, 125);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            if (dialogResult != DialogResult.OK) return dialogResult;

            format = formatTextBox.Text;
            if (int.TryParse(colorTextBox.Text, out int argbColor))
            {
                color = Color.FromArgb(argbColor);
            }
            else
            {
                dialogResult = DialogResult.Cancel;
                MessageBox.Show("Некорректный цветовой код! Возможно следует заглянуть в подсказку?", "Ошибка ввода");
            }
            return dialogResult;
        }

        private void TipButton_Click(object sender, EventArgs e)
        {
            string tip = $"Чёрный:   {Color.Black.ToArgb()}\n" +
                $"Белый:   {Color.White.ToArgb()}\n" +
                $"Серый:   {Color.Gray.ToArgb()}\n" +
                $"Красный:   {Color.Red.ToArgb()}\n" +
                $"Синий:   {Color.Blue.ToArgb()}\n" +
                $"Зелёный:   {Color.Green.ToArgb()}\n" +
                $"Оранжевый:   {Color.Orange.ToArgb()}\n" +
                $"Фиолетовый:   {Color.Purple.ToArgb()}\n" +
                $"Жёлтый:   {Color.Black.ToArgb()}\n" +
                $"Голубой:   {Color.AliceBlue.ToArgb()}\n" +
                $"Бордовый:   {Color.DarkRed.ToArgb()}\n" +
                $"Травяной:   {Color.GreenYellow.ToArgb()}\n";
            MessageBox.Show(tip, "Коды классических цветов");
        }
    }
}
