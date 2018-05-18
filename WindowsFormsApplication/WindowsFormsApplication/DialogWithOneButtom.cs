using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication
{
    public partial class DialogWithOneButtom : Form
    {

        public DialogWithOneButtom(string message)
        {
            InitializeComponent();
            label1.Text = message;
        }

        private void confirmButton_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}
