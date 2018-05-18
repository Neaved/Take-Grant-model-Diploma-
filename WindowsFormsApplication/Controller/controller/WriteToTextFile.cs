using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller.controller
{
    class WriteToTextFile
    {

        public void writeToTxt(List<List<String>> fileSystemRights)
        {
            using (StreamWriter writer = new StreamWriter("D:\\log(" + getDateTimePostfix() + ").txt"))
            {
                // foreach(List<String> right in fileSystemRights)
                for (int i = 0; i <= fileSystemRights.Count - 1; i++)
                {
                    int size = fileSystemRights[i].Count;
                    if (size > 2)
                    {
                        //for (int j = 0; j <= 3; j++)
                        writer.WriteLine($"File: {fileSystemRights[i][0]}");
                        writer.WriteLine($"File Owner: {fileSystemRights[i][1]}");
                    }
                    for (int j = 2; j < size; j++)
                    {
                        if (j % 2 == 0)
                        {
                            writer.WriteLine($"Account: {fileSystemRights[i][j]}");
                        }
                        else
                        {
                            writer.WriteLine($"Rights: {fileSystemRights[i][j]}");
                        }
                        //  writer.WriteLine($"{fileSystemRights[i][4]}");

                    }
                    writer.WriteLine($"\n--------------------------------------------\n");
                }
            }
        }

        public void writeAdjacencyMatrix(int[][] adjacencyMatrix, List<string> lineElements)
        {
            using (StreamWriter writer = new StreamWriter("D:\\ProgrammLog\\adjacencyMatrix(" + getDateTimePostfix() + ").txt"))
            {
                for (int i = 0; i < adjacencyMatrix.Length; i++)
                    {
                        for (int j = 0; j < adjacencyMatrix.Length; j++)
                        {
                            writer.Write($"| {adjacencyMatrix[i][j]} ");
                        }
                        writer.WriteLine($"| \t{lineElements[i]} ");
                        // writer.WriteLine($"\n--------------------------------------------\n");
                    }
        }
    }

        public void writeLog(List<string> log)
        {
            using (StreamWriter writer = new StreamWriter("D:\\ProgrammLog\\log(" + getDateTimePostfix() + ").txt"))
            {
                foreach(string logLine in log)
                {
                    writer.WriteLine($"{logLine} ");
                   // writer.Write($"\n\n");
                }
            }
        }


    private string getDateTimePostfix()
    {
        DateTime time = DateTime.Now;
        return time.ToString("yyyy, MM, dd, hh, mm, ss");
    }

}
}
