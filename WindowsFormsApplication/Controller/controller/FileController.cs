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
using log4net;
using log4net.Config;
using System.Reflection;

namespace Controller.controller
{
    public class FileController
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private List<FileInfo> files = new List<FileInfo>();
        private string fileControllerException;

        public FileController(string directoryName)
        {
            XmlConfigurator.Configure();
            getFileFromDirectory(directoryName);
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
                if (isNotEmpty(fileControllerException))
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
                fileControllerException = ex.Message;
                log.Error("Exception Message: " + ex.Message);
                log.Error("Exception StackTrace: " + ex.StackTrace);
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
