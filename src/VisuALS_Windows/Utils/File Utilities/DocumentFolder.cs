using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VisuALS_WPF_App
{
    public class DocumentFolderEventArgs : EventArgs
    {
        public string filename;
        public string extension;

        public DocumentFolderEventArgs(string filename_, string extension_ = null)
        {
            filename = filename_;
            extension = extension_;
        }
    }

    public class DocumentFolder
    {
        string FolderPath;
        Func<string, bool> Filter;

        public event EventHandler<DocumentFolderEventArgs> FileWritten;
        public event EventHandler<DocumentFolderEventArgs> FileDeleted;

        public DocumentFolder(string dirpath, Func<string, bool> filter = null)
        {
            FolderPath = dirpath;
            if (filter == null)
            {
                Filter = x => true;
            }
            else
            {
                Filter = filter;
            }
            Directory.CreateDirectory(dirpath);
        }


        public string GetNewFileName()
        {
            string filename = "New File";
            int c = 0;
            while (Directory.GetFiles(FolderPath, filename + ".*").Count() > 0)
            {
                filename = "New File (" + c.ToString() + ")";
                c++;
            }
            return filename;
        }

        public FileStream GetFileStream(string filename)
        {
            return File.Open(GetFilePath(filename), FileMode.Open);
        }

        public string GetFullFilename(string filename)
        {
            return GetFilePath(filename).Split('\\').Last();
        }

        public string GetFileExtension(string filename)
        {
            return GetFilePath(filename).Split('.').Last();
        }

        public string GetFilePath(string filename)
        {
            return Directory.GetFiles(FolderPath, filename + ".*")[0];
        }

        public string GetFileContents(string filename)
        {
            return File.ReadAllText(GetFilePath(filename));
        }

        public List<string> GetFileAsList(string filename)
        {
            return File.ReadAllLines(GetFilePath(filename)).ToList();
        }

        public List<string> GetFileNames()
        {
            var a = Directory.GetFiles(FolderPath);
            var b = a.Where(Filter);
            var c = b.Select(x => Path.GetFileNameWithoutExtension(x)).ToList();
            return c;
        }

        public void WriteToFile(string filename, string contents)
        {
            File.WriteAllText(GetFilePath(filename), contents);
            OnFileWritten(this, new DocumentFolderEventArgs(filename, GetFileExtension(filename)));
        }

        public void WriteLinesToFile(string filename, IEnumerable<string> lines)
        {
            File.WriteAllLines(GetFilePath(filename), lines);
            OnFileWritten(this, new DocumentFolderEventArgs(filename, GetFileExtension(filename)));
        }

        public void AppendLineToFile(string filename, string line)
        {
            File.AppendAllText(GetFilePath(filename), line + "\n");
            OnFileWritten(this, new DocumentFolderEventArgs(filename, GetFileExtension(filename)));
        }

        public void InsertLineIntoFile(string filename, int index, string line)
        {
            List<string> lines = GetFileAsList(filename);
            lines.Insert(index, line);
            WriteLinesToFile(filename, lines);
        }

        public void RemoveLineFromFile(string filename, string line)
        {
            List<string> lines = GetFileAsList(filename);
            lines.Remove(line);
            WriteLinesToFile(filename, lines);
        }

        public void RemoveLineFromFile(string filename, int linenum)
        {
            List<string> lines = GetFileAsList(filename);
            lines.RemoveAt(linenum);
            WriteLinesToFile(filename, lines);
        }

        public void AppendTextToFile(string filename, string text)
        {
            File.AppendAllText(GetFilePath(filename), text);
            OnFileWritten(this, new DocumentFolderEventArgs(filename, GetFileExtension(filename)));
        }

        public bool FileExists(string filename)
        {
            return Directory.GetFiles(FolderPath, filename + ".*").Count() > 0;
        }

        public void NewFile(string filename, string extension)
        {
            if (!FileExists(filename))
            {
                File.Create(Path.Combine(FolderPath, filename + "." + extension)).Close();
                OnFileWritten(this, new DocumentFolderEventArgs(filename, GetFileExtension(filename)));
            }
        }

        public void NewFile(string filename, string extension, string contents)
        {
            if (!FileExists(filename))
            {
                File.Create(Path.Combine(FolderPath, filename + "." + extension)).Close();
                WriteToFile(filename, contents);
            }
        }

        public void DeleteFile(string filename)
        {
            DocumentFolderEventArgs args = new DocumentFolderEventArgs(filename, GetFileExtension(filename));
            File.Delete(GetFilePath(filename));
            OnFileDeleted(this, args);
        }

        public void RenameFile(string filename, string newname)
        {
            string old_path = GetFilePath(filename);
            string extension = GetFileExtension(filename);
            string new_path = Path.Combine(FolderPath, newname + "." + extension);
            DocumentFolderEventArgs deleteArgs = new DocumentFolderEventArgs(filename, extension);
            File.Move(old_path, new_path);
            OnFileDeleted(this, deleteArgs);
            OnFileWritten(this, new DocumentFolderEventArgs(filename, extension));
        }

        public void MoveToNewDirectory(string newdirpath)
        {
            foreach (string file in Directory.GetFiles(FolderPath))
            {
                File.Move(file, Path.Combine(newdirpath, file.Split('\\').Last()));
            }
            Directory.Delete(FolderPath);
            FolderPath = newdirpath;
        }

        public void OnFileWritten(object sender, DocumentFolderEventArgs e)
        {
            if (FileWritten != null)
            {
                FileWritten(sender, e);
            }
        }

        public void OnFileDeleted(object sender, DocumentFolderEventArgs e)
        {
            if (FileDeleted != null)
            {
                FileDeleted(sender, e);
            }
        }
    }
}
