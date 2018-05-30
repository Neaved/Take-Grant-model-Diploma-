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

        private int[][] accessMatrix;
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

        private int[][] getAccessMatrix(int[][] adjacencyMatrix)
        {
            int numberOfElements = adjacencyMatrix.Length;

            for (int i = 0; i < numberOfElements; i++)
            {
                for (int j = 0; j < numberOfElements; j++)
                {
                    if (adjacencyMatrix[i][j] > 9000)
                    {
                        adjacencyMatrix[i][j] -= 9000;
                        adminElements.Add(new AccessMatrixElement(i, j));
                    }
                    if (adjacencyMatrix[i][j] > 8000)
                    {
                        adjacencyMatrix[i][j] -= 8000;
                        ownerElements.Add(new AccessMatrixElement(i, j));
                    }
                }
            }
            return adjacencyMatrix;
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
