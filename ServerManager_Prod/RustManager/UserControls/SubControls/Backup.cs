using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RustManager.UserControls.SubControls
{
    public partial class Backup : UserControl
    {
        public Backup()
        {
            InitializeComponent();
        }

        int BackupFolderCounter = 0;
        private void Backup_Load(object sender, EventArgs e)
        {
            DeleteOldBackups();
            LoadTree();
            treeView1.DrawMode = TreeViewDrawMode.OwnerDrawText;

        }

        #region Functions
        void DeleteOldBackups()
        {
            string BackupPath = $"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/BACKUP";
            var directories = Directory.GetDirectories(BackupPath);

            int BackupDaysLimit = -14;
            DateTime now = DateTime.Now;


            foreach (var Folder in directories)
            {
                try
                {
                    var x = Convert.ToDateTime(Path.GetFileName(Folder));



                    if (x < DateTime.Now.AddDays(BackupDaysLimit - 1))
                    {

                        Directory.Delete(Folder, true);
                    }
                } catch { }
  

            }
        }

        void LoadTree()
        {
            BackupFolderCounter = 0;
            treeView1.Nodes.Clear();
            string BackupPath = $"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/BACKUP";


            var directories = Directory.GetDirectories(BackupPath);

            treeView1.BeginUpdate();

            foreach (var Folder in directories)
            {
                try
                {
                    treeView1.Nodes.Add(Path.GetFileName(Folder), Path.GetFileName(Folder), 0, 0);
                    var Subdirectories = Directory.GetDirectories(Folder);
                    foreach (var SubFolder in Subdirectories)
                    {
                        treeView1.Nodes[BackupFolderCounter].Nodes.Add(Path.GetFileName(SubFolder), Path.GetFileName(SubFolder), 1, 1);
                    }
                    BackupFolderCounter++;
                } catch { }

            }



            treeView1.EndUpdate();
        }

        void MakeBackupDir(string sourceDir, string targetDir)
        {
            Directory.CreateDirectory(targetDir);

            foreach (var file in Directory.GetFiles(sourceDir))
                File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)));

            foreach (var directory in Directory.GetDirectories(sourceDir))
                MakeBackupDir(directory, Path.Combine(targetDir, Path.GetFileName(directory)));
        }


        void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {

            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, true);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }
        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {

            foreach (TreeNode node in treeView1.Nodes)
            {
                if (e.Node != treeView1.SelectedNode)
                {
                    node.Collapse();

                }
            }

            e.Node.Toggle();


        }

        #endregion



        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {


            LoadTree();

            TreeNode[] treeNodes = treeView1.Nodes
            .Cast<TreeNode>()
            .Where(r => r.Text != dateTimePicker1.Value.ToShortDateString())
            .ToArray();
            foreach (var i in treeNodes)
            {
                i.Remove();

            }

            if (treeView1.Nodes.Count <= 0)
            {
                treeView1.Nodes.Add("No Result");
            }




        }

        private void customButton1_Click(object sender, EventArgs e)
        {
            LoadTree();
        }

        private void customButton3_Click(object sender, EventArgs e)
        {
            MakeBackupDir($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/server/SERVER", $"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/BACKUP/{DateTime.Now.ToShortDateString()}/{DateTime.Now.ToString("HH;mm")}");
        }



        private void customButton2_Click(object sender, EventArgs e)
        {
            try
            {
                // Get Current Root Node
                TreeNode CurrentRootNode = treeView1.SelectedNode;
                while (CurrentRootNode.Parent != null)
                {
                    CurrentRootNode = CurrentRootNode.Parent;
                }

                if (Directory.Exists($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/BACKUP/{CurrentRootNode.Text}/{treeView1.SelectedNode.Text}"))
                {
                    string title = "WARNING";
                    string message = $"Do you want to override {Data.AppData.Default.CurrentServer} ?";
                    MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                    DialogResult result = MessageBox.Show(message, title, buttons, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                    if (result == DialogResult.Yes)
                    {
                        string sourceDir = $"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/BACKUP/{CurrentRootNode.Text}/{treeView1.SelectedNode.Text}";
                        string targetDir = $"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/server/SERVER";



                        Directory.Delete($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/server/SERVER", true);
                        DirectoryCopy(sourceDir, targetDir, true);


                        MessageBox.Show("Backup was successfully applied", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);


                    }

                }
                else
                {
                    MessageBox.Show("Backup could not be applied", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } catch { }
            
        }



        SolidBrush greenBrush = new SolidBrush(Color.FromArgb(10, 20, 30));
        private void treeView1_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            if (e.Node.IsSelected)
            {
                e.Graphics.FillRectangle(greenBrush, e.Bounds);
            }
            else
                e.Graphics.FillRectangle(Brushes.Transparent, e.Bounds);

            TextRenderer.DrawText(e.Graphics, e.Node.Text, e.Node.TreeView.Font, e.Node.Bounds, e.Node.ForeColor = Color.White);
        }
    }

}
