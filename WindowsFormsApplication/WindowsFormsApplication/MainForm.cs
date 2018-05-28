using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using ConjTable.Demo;
using Controller.controller;
using Entity.entity;
using log4net;
using log4net.Config;
using static System.Windows.Forms.ListView;
using static Controller.controller.ControllerUtils;

namespace WindowsFormsApplication
{
    public partial class MainForm : Form
    {
        private static readonly ILog log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public MainForm()
        {
            XmlConfigurator.Configure();
            InitializeComponent();
            //Set default value for textBox1
            setDefaultValueFortextBox1();
        }

        private void setDefaultValueFortextBox1()
        {
            //textBox1.Text = "D:\\testFolder";
        }

        private void showUserAccounts(object sender, EventArgs e)
        {
            UserEntityController controller = UserEntityController.Instance;
            string userEntityControllerException =
                controller.UserEntityControllerException;
            if (showException(userEntityControllerException))
            {
                DialogWithOneButtom error =
                    new DialogWithOneButtom(userEntityControllerException);
                error.Show();
            }
            else
            {
                refreshListView(controller.UserEntities);
            }
        }

        private void refreshListView(List<UserEntity> entities)
        {
            if (isNotEmpty(entities))
            {
                listView2.Items.Clear();
                addUserAccountsToListView(entities);
            }
        }

        private void addUserAccountsToListView(List<UserEntity> userAccounts)
        {
            foreach (UserEntity userAccount in userAccounts)
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
                    DialogWithOneButtom error =
                        new DialogWithOneButtom(fileControllerException);
                    error.Show();
                }
                else
                {
                    foreach (FileEntity fileEntity in controller.FileEntities)
                    {
                        listView1.Items.Add(new ListViewItem(fileEntity.FullFileName));
                    }
                }
            }
            else
            {
                DialogWithOneButtom error =
                    new DialogWithOneButtom("fill directory path");
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
                    DialogWithOneButtom error =
                        new DialogWithOneButtom("select file(s) and user(s)");
                    error.ShowDialog();
                }
                else if (userItemsCount == 0)
                {
                    DialogWithOneButtom error =
                        new DialogWithOneButtom("select user(s)");
                    error.ShowDialog();
                }
                else
                {
                    DialogWithOneButtom error =
                        new DialogWithOneButtom("select file(s)");
                    error.ShowDialog();
                }
            }

        }
    }
}
