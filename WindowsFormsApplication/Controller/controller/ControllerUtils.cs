using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Entity.entity;
using Microsoft.Office.Interop.Excel;
                                  
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

        public static void writeAccessMatrix(string[][] accessMatrix, List<string> lineElements)
        {
            createDirectoryIfNotExist();
            using (StreamWriter writer = new StreamWriter(path + "\\accessMatrix(" + getDateTimePostfix() + ").txt"))
            {
                for (int i = 0; i < accessMatrix.Length; i++)
                {
                    for (int j = 0; j < accessMatrix.Length; j++)
                    {
                        string element = accessMatrix[i][j];
                        if (isNotEmpty(element))
                        {
                            writer.Write($"| {element} ");
                        }
                        else
                        {
                            writer.Write($"| {Entity.Сonstants.EmptyBinPermission} ");
                        }
                    }
                    writer.WriteLine($"| \t{lineElements[i]} ");
                }
            }
        }

        private static void createDirectoryIfNotExist()
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private static string getDateTimePostfix()
        {
            DateTime time = DateTime.Now;
            return time.ToString("yyyy, MM, dd, hh, mm, ss");
        }

        public static void exportToExel(string[][] accessMatrix, List<string> lineElements)
        {
            createDirectoryIfNotExist();
            int length = accessMatrix.Length + 2;
            Application excelApp = new Application();
            excelApp.Application.Workbooks.Add(Type.Missing);
            excelApp.Columns.ColumnWidth = 50;
            for (int i = 2; i < length; i++)
            {
                excelApp.Cells[1, i] = excelApp.Cells[i, 1] = lineElements[i - 2];
            }

            for (int i = 2; i < length; i++)
            {
                int rowIndex = i - 2;
                for (int j = 2; j < length; j++)
                {
                    int columnIndex = j - 2;
                    string element = accessMatrix[rowIndex][columnIndex];
                    if (isNotEmpty(element))
                    {
                        excelApp.Cells[i, j] = element;
                    }
                }
            }
            excelApp.Visible = true;
        }
    }
}
