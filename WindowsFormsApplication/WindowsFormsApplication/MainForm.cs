using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Controller.controller.ControllerUtils;
using Entity.entity;
using Controller.controller;
using System.Management;
using static System.Windows.Forms.ListView;
using ConjTable.Demo;
using log4net;
using log4net.Config;
using System.Reflection;

namespace WindowsFormsApplication
{
    public partial class MainForm : Form
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public MainForm()
        {
            XmlConfigurator.Configure();
            InitializeComponent();
            textBox1.Text = "D:\\testFolder";
        }

        private void showUserAccounts(object sender, EventArgs e)
        {
            UserAccountController controller = UserAccountController.Instance;
            string userAccountControllerException = controller.UserAccountControllerException;
            if (showException(userAccountControllerException))
            {
                DialogWithOneButtom error = new DialogWithOneButtom(userAccountControllerException);
                error.Show();
            }
            else
            {
                refreshListView(controller.UserAccounts);
            }
        }

        private void refreshListView(List<UserAccount> entities)
        {
            if (isNotEmpty(entities))
            {
                listView2.Items.Clear();
                addUserAccountsToListView(entities);
            }
        }

        private void addUserAccountsToListView(List<UserAccount> userAccounts)
        {
            foreach (UserAccount userAccount in userAccounts)
            {
                ListViewItem lvi = new ListViewItem(userAccount.FullName);
                lvi.SubItems.Add(userAccount.Sid);
                lvi.SubItems.Add(userAccount.GroupNamesInString());
                lvi.SubItems.Add(userAccount.Description);
                listView2.Items.Add(lvi);
            }
        }

        private void ShowFilesInDirectory(object sender, EventArgs e)
        {
            updateFileFieldView(textBox1.Text);
        }

        private void updateFileFieldView(string directoryName)
        {
            if (isNotEmpty(directoryName))
            {
                FileController controller = new FileController(directoryName);
                string fileControllerException = controller.FileControllerException;
                listView1.Items.Clear();
                if (showException(fileControllerException))
                {
                    DialogWithOneButtom error = new DialogWithOneButtom(fileControllerException);
                    error.Show();
                }
                else
                {
                    foreach (FileInfo file in controller.Files)
                    {
                        listView1.Items.Add(new ListViewItem(file.FullName));
                    }
                }
            }
            else
            {
                DialogWithOneButtom error = new DialogWithOneButtom("fill directory path");
                error.Show();
            }
        }

        private void clearFileFieldView(object sender, EventArgs e)
        {
            textBox1.Clear();
            listView1.Items.Clear();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SelectedListViewItemCollection fileItems = listView1.SelectedItems;
            SelectedListViewItemCollection userItems = listView2.SelectedItems;

            int fileItemsCount = fileItems.Count;
            int userItemsCount = userItems.Count;

            if (fileItemsCount > 0 && userItemsCount > 0)
            {
                GraphForm graphForm = new GraphForm(fileItems, userItems);
                graphForm.Show();
            }
            else
            {
                if (fileItemsCount == 0 && userItemsCount == 0)
                {
                    DialogWithOneButtom error = new DialogWithOneButtom("select file(s) and user(s)");
                    error.ShowDialog();
                }
                else if (userItemsCount == 0)
                {
                    DialogWithOneButtom error = new DialogWithOneButtom("select user(s)");
                    error.ShowDialog();
                }
                else
                {
                    DialogWithOneButtom error = new DialogWithOneButtom("select file(s)");
                    error.ShowDialog();
                }
            }

        }
    }
}
