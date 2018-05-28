using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller.controller
{
    public class ControllerUtils
    {
        private const string path = "C:\\Take Grant Programm Adjacency Matrix\\";

        public static bool isEmpty(Object value)
        {
            if (value == null)
            {
                return true;
            }
            else if (value is ICollection)
            {
                return ((ICollection)value).Count < 1;
            }
            else if (value is IDictionary)
            {
                return ((IDictionary)value).Count < 1;
            }
            else if (value is string)
            {
                return string.Empty.Equals((string)value.ToString());
            }
            return false;
        }

        public static bool isNotEmpty(Object value)
        {
            return !isEmpty(value);
        }

        public static bool showException(string exceptionMessage)
        {
            return isNotEmpty(exceptionMessage);
        }

        public static void writeAccessMatrix(int[][] accessMatrix, List<string> lineElements)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            using (StreamWriter writer = new StreamWriter(path + "\\accessMatrix(" + getDateTimePostfix() + ").txt"))
            {
                for (int i = 0; i < accessMatrix.Length; i++)
                {
                    for (int j = 0; j < accessMatrix.Length; j++)
                    {
                        writer.Write($"| {accessMatrix[i][j]} ");
                    }
                    writer.WriteLine($"| \t{lineElements[i]} ");
                }
            }
        }

        private static string getDateTimePostfix()
        {
            DateTime time = DateTime.Now;
            return time.ToString("yyyy, MM, dd, hh, mm, ss");
        }
    }
}
