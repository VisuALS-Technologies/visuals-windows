using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Security.AccessControl;
using System.Windows;
using SearchOption = System.IO.SearchOption;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class FileBrowserPage : AppletPage
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public FileBrowserPage()
        {
            InitializeComponent();
            ParentApplet = AppletManager.GetApplet<FileBrowser>();
        }

        private void VFileBrowser_ItemSelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private async void Move_Click(object sender, RoutedEventArgs e)
        {
            VFileBrowser fileBrowser = new VFileBrowser();
            fileBrowser.SelectDirectory = true;
            DialogResponse r = await DialogWindow.Show(fileBrowser);
            if (r.ResponseObject != null)
            {
                string newDirectory = r.ResponseString;
                string selectedFile = (string)FileBrowserSelectionControl.DirList.SelectedItem;
                string newFilePath = Path.Combine(newDirectory, Path.GetFileName(selectedFile));
                move(selectedFile, newFilePath);
                FileBrowserSelectionControl.Refresh();
            }
        }

        private async void move(string path, string newPath)
        {
            try
            {
                if (File.Exists(path))
                {
                    if (!File.Exists(newPath))
                    {
                        if (FilePermissions.HasAccess(path, FileSystemRights.Read & FileSystemRights.Delete) && FilePermissions.HasAccess(Path.GetDirectoryName(newPath), FileSystemRights.Write))
                        {
                            File.Move(path, newPath);
                        }
                        else
                        {
                            await DialogWindow.ShowMessage(LanguageManager.Tokens["fb_unauthorized_access"]);
                        }
                    }
                    else
                    {
                        await DialogWindow.ShowMessage(LanguageManager.Tokens["fb_file_exists"]);
                    }
                }
                else if (Directory.Exists(path))
                {
                    if (!Directory.Exists(newPath))
                    {
                        if (FilePermissions.HasAccess(path, FileSystemRights.Read & FileSystemRights.Delete & FileSystemRights.ListDirectory) && FilePermissions.HasAccess(Path.GetDirectoryName(newPath), FileSystemRights.Write))
                        {
                            Directory.Move(path, newPath);
                        }
                        else
                        {
                            await DialogWindow.ShowMessage(LanguageManager.Tokens["fb_unauthorized_access"]);
                        }
                    }
                    else
                    {
                        await DialogWindow.ShowMessage(LanguageManager.Tokens["fb_file_exists"]);
                    }
                }
                else
                {
                    await DialogWindow.ShowMessage(LanguageManager.Tokens["fb_file_does_not_exist"]);
                }
            }
            catch (Exception e)
            {
                await DialogWindow.ShowMessage(LanguageManager.Tokens["fb_file_action_error"] + '\n' + e.Message);
            }
        }

        private async void Copy_Click(object sender, RoutedEventArgs e)
        {
            string selectedFile = (string)FileBrowserSelectionControl.DirList.SelectedItem;
            if (File.Exists(selectedFile) || Directory.Exists(selectedFile))
            {
                StringCollection files = new StringCollection();
                files.Add(selectedFile);
                Clipboard.SetFileDropList(files);
                DialogWindow.ShowBanner("Copied!");
            }
            else
            {
                await DialogWindow.ShowMessage(LanguageManager.Tokens["fb_file_does_not_exist"]);
            }
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            string selectedFile = (string)FileBrowserSelectionControl.DirList.SelectedItem;
            DialogResponse r = await DialogWindow.ShowMessage(LanguageManager.Tokens["fb_delete_confirm"] + "\n " + Path.GetFileName(selectedFile), LanguageManager.Tokens["yes"], LanguageManager.Tokens["no"]);
            if (r.ResponseObject != null)
            {
                if (r.ResponseString == LanguageManager.Tokens["yes"])
                {
                    delete(selectedFile);
                    FileBrowserSelectionControl.Refresh();
                }
            }
        }

        private async void delete(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    if (FilePermissions.HasAccess(path, FileSystemRights.Delete))
                    {
                        FileSystem.DeleteFile(path, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                    }
                    else
                    {
                        await DialogWindow.ShowMessage(LanguageManager.Tokens["fb_unauthorized_access"]);
                    }
                }
                else if (Directory.Exists(path))
                {
                    if (FilePermissions.HasAccess(path, FileSystemRights.Delete))
                    {
                        FileSystem.DeleteDirectory(path, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                    }
                    else
                    {
                        await DialogWindow.ShowMessage(LanguageManager.Tokens["fb_unauthorized_access"]);
                    }
                }
                else
                {
                    await DialogWindow.ShowMessage(LanguageManager.Tokens["fb_file_does_not_exist"]);
                }
            }
            catch (Exception e)
            {
                await DialogWindow.ShowMessage(LanguageManager.Tokens["fb_file_action_error"] + '\n' + e.Message);
            }
        }

        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            StringCollection files = Clipboard.GetFileDropList();
            foreach (string file in files)
            {
                paste(file, FileBrowserSelectionControl.Directory);
                FileBrowserSelectionControl.Refresh();
            }
        }

        private async void paste(string path, string dir)
        {
            if (File.Exists(path))
            {
                try
                {
                    if (FilePermissions.HasAccess(path, FileSystemRights.Read) && FilePermissions.HasAccess(dir, FileSystemRights.Write))
                    {
                        File.Copy(path, Path.Combine(dir, getAvailableFileName(path, dir)));
                    }
                    else
                    {
                        await DialogWindow.ShowMessage(LanguageManager.Tokens["fb_unauthorized_access"]);
                    }
                }
                catch (Exception e)
                {
                    await DialogWindow.ShowMessage(LanguageManager.Tokens["fb_file_action_error"] + '\n' + e.Message);
                }

            }
            else if (Directory.Exists(path))
            {
                copyFilesRecursively(path, Path.Combine(dir, getAvailableDirName(path, dir)));
            }
            else
            {
                await DialogWindow.ShowMessage(LanguageManager.Tokens["fb_file_does_not_exist"]);
            }
        }

        private async static void copyFilesRecursively(string sourceDir, string targetDir)
        {
            try
            {
                if (Directory.Exists(sourceDir))
                {
                    if (FilePermissions.HasAccess(sourceDir, FileSystemRights.Read & FileSystemRights.ListDirectory) && FilePermissions.HasAccess(Path.GetDirectoryName(targetDir), FileSystemRights.Write))
                    {
                        string[] directories = Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories);

                        Directory.CreateDirectory(targetDir);

                        //Now Create all of the directories
                        foreach (string dirPath in directories)
                        {
                            Directory.CreateDirectory(dirPath.Replace(sourceDir, targetDir));
                        }

                        //Copy all the files & Replaces any files with the same name
                        foreach (string newPath in Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories))
                        {
                            File.Copy(newPath, newPath.Replace(sourceDir, targetDir), true);
                        }
                    }
                    else
                    {
                        await DialogWindow.ShowMessage(LanguageManager.Tokens["fb_unauthorized_access"]);
                    }
                }
                else
                {
                    await DialogWindow.ShowMessage(LanguageManager.Tokens["fb_file_does_not_exist"]);
                }
            }
            catch (Exception e)
            {
                await DialogWindow.ShowMessage(LanguageManager.Tokens["fb_file_action_error"] + '\n' + e.Message);
            }
        }

        private string getAvailableFileName(string filename, string dir)
        {
            string ext = Path.GetExtension(filename);
            string resultFilename = Path.GetFileNameWithoutExtension(filename);
            if (File.Exists(Path.Combine(dir, resultFilename + ext)))
            {
                resultFilename = Path.GetFileNameWithoutExtension(filename) + " - Copy";
            }
            int i = 2;
            while (File.Exists(Path.Combine(dir, resultFilename + ext)))
            {
                resultFilename = Path.GetFileNameWithoutExtension(filename) + " - Copy (" + i.ToString() + ")";
                i++;
            }
            return resultFilename + ext;
        }

        private string getAvailableDirName(string dirname, string dir)
        {
            string resultDirname = Path.GetFileName(dirname);
            if (Directory.Exists(Path.Combine(dir, resultDirname)))
            {
                resultDirname = Path.GetFileName(dirname) + " - Copy";
            }
            int i = 2;
            while (Directory.Exists(Path.Combine(dir, resultDirname)))
            {
                resultDirname = Path.GetFileName(dirname) + " - Copy (" + i.ToString() + ")";
                i++;
            }
            return resultDirname;
        }

        private async void View_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (FileBrowserSelectionControl.DirList.SelectedItem != null)
                {
                    if (File.Exists((string)FileBrowserSelectionControl.DirList.SelectedItem))
                    {
                        if (FilePermissions.HasAccess((string)FileBrowserSelectionControl.DirList.SelectedItem, FileSystemRights.Read))
                        {
                            VMediaViewer viewer = new VMediaViewer();
                            viewer.Source = (string)FileBrowserSelectionControl.DirList.SelectedItem;
                            DialogWindow.Show(viewer);
                        }
                        else
                        {
                            await DialogWindow.ShowMessage(LanguageManager.Tokens["fb_unauthorized_access"]);
                        }
                    }
                    else
                    {
                        await DialogWindow.ShowMessage(LanguageManager.Tokens["fb_file_does_not_exist"]);
                    }
                }
            }
            catch (Exception ex)
            {
                await DialogWindow.ShowMessage(LanguageManager.Tokens["fb_file_action_error"] + '\n' + ex.Message);
            }
        }

        private async void Rename_Click(object sender, RoutedEventArgs e)
        {
            string selectedPath = (string)FileBrowserSelectionControl.DirList.SelectedItem;
            DialogResponse r = await DialogWindow.ShowKeyboardInput(Path.GetFileName(selectedPath));
            if (r.ResponseObject != null)
            {
                string invchars = FileUtils.getInvalidPathChars(r.ResponseString);
                if (invchars != "")
                {
                    await DialogWindow.ShowMessage($"{LanguageManager.Tokens["fb_illegal_path_name"]} {invchars}");
                }
                else
                {
                    string newPath = Path.Combine(Path.GetDirectoryName(selectedPath), r.ResponseString);
                    move(selectedPath, newPath);
                    FileBrowserSelectionControl.Refresh();
                }
            }
        }
    }
}
