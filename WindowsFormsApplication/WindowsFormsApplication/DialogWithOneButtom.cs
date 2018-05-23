using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;
using log4net.Config;

namespace WindowsFormsApplication
{
    public partial class DialogWithOneButtom : Form
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private const int labelTexstLength = 34;
        public DialogWithOneButtom(string message)
        {
            InitializeComponent();
            XmlConfigurator.Configure();
            label1.Text = wordWrap(message);
        }

        private void confirmButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private string wordWrap(string message)
        {
            if (message.Length > labelTexstLength)
            {
                string[] words = message.Split(' ');
                int wordsCount = words.Length;
                StringBuilder newMessage = new StringBuilder();
                for (int i = 0; i < wordsCount; i++)
                {
                    StringBuilder sb = new StringBuilder(words[i]);
                    if (i + 1 < wordsCount)
                    {
                        for (int j = i + 1; j < wordsCount; j++)
                        {
                            if (sb.Length + words[j].Length > labelTexstLength)
                            {
                                sb.Append("\n");
                                i = j - 1;
                                break;
                            }
                            else
                            {
                                sb.Append(" ").Append(words[j]);
                            }
                        }
                    }
                    newMessage.Append(sb);

                }
                return newMessage.ToString();
            }
            return message;
        }
    }
}
