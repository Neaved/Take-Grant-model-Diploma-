using Controller.controller;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Entity.entity;
using static System.Windows.Forms.ListView;


namespace ConjTable.Demo
{
    public partial class GraphForm : Form
    {

        private int[][] _matrix;
        private List<GraphVertexEntity> graphVertexs;
        private List<MatrixElement> ownerElements = new List<MatrixElement>();
        private List<MatrixElement> adminElements = new List<MatrixElement>();

        public GraphForm()
        {
            InitializeComponent();
        }

        public GraphForm(SelectedListViewItemCollection fileItems, SelectedListViewItemCollection entityItems)
        {
            InitializeComponent();
            AccessMatrixController controller = new AccessMatrixController(fileItems, entityItems);
            this._matrix = getAccessMatrix(controller.AdjacencyMatrix);
            this.graphVertexs = controller.GraphVertexs;
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
                        adminElements.Add(new MatrixElement(i, j));
                    }
                    if (adjacencyMatrix[i][j] > 8000)
                    {
                        adjacencyMatrix[i][j] -= 8000;
                        ownerElements.Add(new MatrixElement(i, j));
                    }
                }
            }
            return adjacencyMatrix;
        }

        //int[,] _matrix = new int[,]
        //                {
        //        {0, 0, 1},
        //        {0, 0, 1},
        //        {0, 0, 0},
        //    };
        //{
        //    {0, 1, 1, 1, 1},
        //    {1, 0, 1, 1, 1},
        //    {1, 1, 0, 1, 1},
        //    {1, 1, 1, 0, 1},
        //    {1, 1, 1, 0, 1},
        //};

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            conjTable1.Build(_matrix);
            conjPanel1.Build(_matrix, graphVertexs, ownerElements, adminElements);
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            conjPanel1.Build(conjTable1.Matrix, graphVertexs, ownerElements, adminElements);
        }

        private void conjTable1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            conjPanel1.Build(conjTable1.Matrix, graphVertexs, ownerElements, adminElements);
        }
    }
}
