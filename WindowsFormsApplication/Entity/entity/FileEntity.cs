namespace Entity.entity
{
    public class FileEntity : BaseEntity
    {
        private string fullFileName;

        public FileEntity(string name, string fullFileName) : base(name)
        {
            this.fullFileName = fullFileName;
        }

        public string FullFileName
        {
            get
            {
                return fullFileName;
            }

            set
            {
                fullFileName = value;
            }
        }

        public static string getFileNameFromPath(string pathToFile)
        {
            string[] pathElements = pathToFile.Split(Сonstants.BackSlashSplitSymbolChar);
            return pathElements[pathElements.Length - 1];
        }

    }
}
