
namespace FileManager.Views
{
    partial class FileColorSettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.FileColorSettingsListView = new System.Windows.Forms.ListView();
            this.AddButton = new System.Windows.Forms.Button();
            this.DefaultButton = new System.Windows.Forms.Button();
            this.FileColorSettingsContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.EditToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DeleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TipButton = new System.Windows.Forms.Button();
            this.FileColorSettingsContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // FileColorSettingsListView
            // 
            this.FileColorSettingsListView.HideSelection = false;
            this.FileColorSettingsListView.Location = new System.Drawing.Point(23, 60);
            this.FileColorSettingsListView.Name = "FileColorSettingsListView";
            this.FileColorSettingsListView.Size = new System.Drawing.Size(683, 365);
            this.FileColorSettingsListView.TabIndex = 0;
            this.FileColorSettingsListView.UseCompatibleStateImageBehavior = false;
            // 
            // AddButton
            // 
            this.AddButton.Font = new System.Drawing.Font("Times New Roman", 9.857143F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.AddButton.Location = new System.Drawing.Point(23, 6);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(153, 48);
            this.AddButton.TabIndex = 2;
            this.AddButton.Text = "Добавить";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // DefaultButton
            // 
            this.DefaultButton.Font = new System.Drawing.Font("Times New Roman", 9.857143F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DefaultButton.Location = new System.Drawing.Point(150, 431);
            this.DefaultButton.Name = "DefaultButton";
            this.DefaultButton.Size = new System.Drawing.Size(441, 48);
            this.DefaultButton.TabIndex = 3;
            this.DefaultButton.Text = "Восстановить настройки по-умолчанию";
            this.DefaultButton.UseVisualStyleBackColor = true;
            this.DefaultButton.Click += new System.EventHandler(this.DefaultButton_Click);
            // 
            // FileColorSettingsContextMenuStrip
            // 
            this.FileColorSettingsContextMenuStrip.ImageScalingSize = new System.Drawing.Size(28, 28);
            this.FileColorSettingsContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.EditToolStripMenuItem,
            this.DeleteToolStripMenuItem});
            this.FileColorSettingsContextMenuStrip.Name = "FileColorSettingsContextMenuStrip";
            this.FileColorSettingsContextMenuStrip.Size = new System.Drawing.Size(182, 76);
            // 
            // EditToolStripMenuItem
            // 
            this.EditToolStripMenuItem.Name = "EditToolStripMenuItem";
            this.EditToolStripMenuItem.Size = new System.Drawing.Size(181, 36);
            this.EditToolStripMenuItem.Text = "Изменить";
            this.EditToolStripMenuItem.Click += new System.EventHandler(this.EditToolStripMenuItem_Click);
            // 
            // DeleteToolStripMenuItem
            // 
            this.DeleteToolStripMenuItem.Name = "DeleteToolStripMenuItem";
            this.DeleteToolStripMenuItem.Size = new System.Drawing.Size(181, 36);
            this.DeleteToolStripMenuItem.Text = "Удалить";
            this.DeleteToolStripMenuItem.Click += new System.EventHandler(this.DeleteToolStripMenuItem_Click);
            // 
            // TipButton
            // 
            this.TipButton.Font = new System.Drawing.Font("Times New Roman", 9.857143F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TipButton.Location = new System.Drawing.Point(553, 6);
            this.TipButton.Name = "TipButton";
            this.TipButton.Size = new System.Drawing.Size(153, 48);
            this.TipButton.TabIndex = 4;
            this.TipButton.Text = "Подсказка";
            this.TipButton.UseVisualStyleBackColor = true;
            this.TipButton.Click += new System.EventHandler(this.TipButton_Click);
            // 
            // FileColorSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(730, 484);
            this.Controls.Add(this.TipButton);
            this.Controls.Add(this.DefaultButton);
            this.Controls.Add(this.AddButton);
            this.Controls.Add(this.FileColorSettingsListView);
            this.Name = "FileColorSettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Цвета выделения файлов";
            this.FileColorSettingsContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView FileColorSettingsListView;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.Button DefaultButton;
        private System.Windows.Forms.ContextMenuStrip FileColorSettingsContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem EditToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem DeleteToolStripMenuItem;
        private System.Windows.Forms.Button TipButton;
    }
}