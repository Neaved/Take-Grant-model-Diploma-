using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Entity.entity;
using Controller.controller;
using static Controller.controller.ControllerUtils;
using static System.Windows.Forms.ListView;

namespace WindowsFormsApplication
{
    public partial class UserAccounts : Form
    {
        //public UserAccounts()
        //{
            
        //    UserAccountController controller = UserAccountController.Instance;
        //    string userAccountsException = controller.UserAccountsException;
        //    if (showException(userAccountsException))
        //    {
        //        Form error = new Form();
        //        error.Show();
        //    } else {
        //        refreshListView(controller);
        //    }
        //}

        public UserAccounts(SelectedListViewItemCollection fileItems, SelectedListViewItemCollection entityItems)
        {
            AccessMatrixController controller = new AccessMatrixController(fileItems, entityItems);
            listBox1.DataSource = null;
            //listBox1.DataSource = controller.TestLog;
        }

        public UserAccounts()
        {
            InitializeComponent();

        }

        private void refreshListView(UserAccountController controller)
        {
            listView1.Items.Clear();
            foreach (UserAccount userAccount in controller.UserAccounts)
            {
                ListViewItem lvi = new ListViewItem(userAccount.Name);
                lvi.SubItems.Add(userAccount.Sid);
                lvi.SubItems.Add(userAccount.Description);
                listView1.Items.Add(lvi);
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("test");
        }
    }
}
