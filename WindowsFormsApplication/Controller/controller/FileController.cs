using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Controller.controller.ControllerUtils;
using Entity.entity;
using Controller.controller;
using System.Management;

namespace Controller.controller
{
    public class FileController
    {
        private static FileController instance = new FileController();
        private List<FileInfo> files = new List<FileInfo>();
        private string fileControllerException;

        public FileController() { }

        public FileController(string directoryName)
        {
            getFileFromDirectory(directoryName);
        }

        public static FileController Instance
        {
            get
            {
                return instance;
            }
        }

        public List<FileInfo> Files
        {
            get
            {
                return files;
            }
        }

        public string FileControllerException
        {
            get
            {
                if (isEmpty(fileControllerException))
                {
                    return fileControllerException;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public void getFileFromDirectory(string directoryName)
        {
            try
            {
                DirectoryInfo directory = new DirectoryInfo(directoryName);
                findFileInDirectory(directory);
            }
            catch (Exception ex)
            {
                fileControllerException = ex.ToString();
            }
        }

        private void findFileInDirectory(DirectoryInfo directory)
        {
            foreach (FileInfo file in directory.GetFiles())
            {
                files.Add(file);
            }
            foreach (DirectoryInfo subDir in directory.GetDirectories())
            {
                findFileInDirectory(subDir);
            }
        }

    }
}
