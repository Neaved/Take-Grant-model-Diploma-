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
using System.Timers;
using System.Diagnostics;

namespace Controller.controller
{
    public class FileController
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private List<FileEntity> fileEntities = new List<FileEntity>();
        private string fileControllerException;

        public FileController(string directoryName)
        {
            XmlConfigurator.Configure();
            getFilesFromDirectory(directoryName);
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

        public List<FileEntity> FileEntities
        {
            get
            {
                return fileEntities;
            }

            set
            {
                fileEntities = value;
            }
        }

        private void getFilesFromDirectory(string directoryName)
        {
            DirectoryInfo directory = new DirectoryInfo(directoryName);
            //Stopwatch stopwatch = new Stopwatch();
            //log.Debug("time start ");
            //stopwatch.Start();
            findFileInDirectory(directory);
            //stopwatch.Stop();
            //log.Debug("time stop ");
            //log.Debug("time TotalMilliseconds " + stopwatch.Elapsed.TotalMilliseconds);
        }

        private void findFileInDirectory(DirectoryInfo directory)
        {
            try
            {
                foreach (FileInfo file in directory.GetFiles())
                {
                    fileEntities.Add(
                        new FileEntity(
                            file.Name,
                            file.FullName
                            ));
                }
                foreach (DirectoryInfo subDir in directory.GetDirectories())
                {
                    findFileInDirectory(subDir);
                }
            }
            catch (UnauthorizedAccessException uaEx)
            {
                log.Error("Exception Message: " + uaEx.Message);
            }
            catch (Exception ex)
            {
                fileControllerException = ex.Message;
                log.Error("Exception Message: " + ex.Message);
                log.Error("Exception StackTrace: " + ex.StackTrace);
            }
        }

    }
}
