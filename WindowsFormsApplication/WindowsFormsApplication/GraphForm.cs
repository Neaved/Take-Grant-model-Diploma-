using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using Controller.controller;
using Entity.entity;
using log4net;
using log4net.Config;
using WindowsFormsApplication;
using static System.Windows.Forms.ListView;
using static Controller.controller.ControllerUtils;

namespace ConjTable.Demo
{
    public partial class GraphForm : Form
    {
        private static readonly ILog log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private string[][] accessMatrix;
        private List<GraphVertexEntity> graphVertexs;
        private List<AccessMatrixElement> ownerElements = new List<AccessMatrixElement>();
        private List<AccessMatrixElement> adminElements = new List<AccessMatrixElement>();

        public GraphForm(SelectedListViewItemCollection fileItems,
            SelectedListViewItemCollection entityItems)
        {
            XmlConfigurator.Configure();
            InitializeComponent();
            AccessMatrixController controller =
                new AccessMatrixController(fileItems, entityItems);
            this.accessMatrix = getAccessMatrix(controller.AccessMatrix);
            this.graphVertexs = controller.GraphVertexs;
            addWarningMessages(controller.WarningMessages);
        }

        private string[][] getAccessMatrix(string[][] adjacencyMatrix)
        {
            int numberOfElements = adjacencyMatrix.Length;
            for (int i = 0; i < numberOfElements; i++)
            {
                for (int j = 0; j < numberOfElements; j++)
                {
                    string binaryPermission = adjacencyMatrix[i][j];

                    if (isNotEmpty(binaryPermission))
                    {
                        char[] binaryPermissionChar = binaryPermission.ToCharArray();
                        if (isAdminElements(binaryPermissionChar))
                        {
                            adminElements.Add(new AccessMatrixElement(i, j));
                        }
                        if (isOwnerElements(binaryPermissionChar))
                        {
                            ownerElements.Add(new AccessMatrixElement(i, j));
                        }
                        adjacencyMatrix[i][j] = getInHexFormat(binaryPermission);
                    }
                    else
                    {
                        adjacencyMatrix[i][j] = "0";
                        continue;
                    }
                }
            }
            return adjacencyMatrix;
        }

        private bool isAdminElements(char[] binaryPermission)
        {
            return binaryPermission[32 - 27 - 1] == '1';
        }

        private bool isOwnerElements(char[] binaryPermission)
        {
            return binaryPermission[32 - 19 - 1] == '1';
        }

        private string getInHexFormat(string binaryPermission)
        {
            return Convert.ToInt64(binaryPermission, 2).ToString("X8");
        }

        private void addWarningMessages(List<string> warningMessages)
        {
            listBox1.Items.Clear();
            if (isNotEmpty(warningMessages))
            {
                foreach (string warningMessage in warningMessages)
                {
                    //log.Error("warningMessage: " + warningMessage);
                    listBox1.Items.Add(warningMessage);
                }
            }

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            conjTable1.Build(accessMatrix);
            conjPanel1.Build(
                accessMatrix, 
                graphVertexs, 
                ownerElements, 
                adminElements);
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            conjPanel1.Build(
                conjTable1.Matrix, 
                graphVertexs, 
                ownerElements, 
                adminElements);
        }

        private void conjTable1_CellValueChanged(object sender,
            DataGridViewCellEventArgs e)
        {
            conjPanel1.Build(
                conjTable1.Matrix, 
                graphVertexs, 
                ownerElements, 
                adminElements);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            AdditionalGraphInfoForm additionalGraphInfoForm =
                new AdditionalGraphInfoForm();
            additionalGraphInfoForm.ShowDialog();
        }
    }
}
