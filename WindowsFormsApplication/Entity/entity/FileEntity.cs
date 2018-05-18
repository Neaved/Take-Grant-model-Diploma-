using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.entity
{
    public class FileEntity : BaseEntity
    {
        private string pathToFile;
        private bool isDirectory;

        public FileEntity(string name, string pathToFile, bool isDirectory) : base(name)
        {
            this.pathToFile = pathToFile;
            this.isDirectory = isDirectory;
        }

        //public FileEntity(string pathToFile)
        //{
        //    this.pathToFile = pathToFile;
        //    this.fileName = getFileNameFromPath();
        //}

        public static string getFileNameFromPath(string pathToFile)
        {
            string[] pathElements = pathToFile.Split('\\');
            return pathElements[pathElements.Length - 1];
        }

    }
}
