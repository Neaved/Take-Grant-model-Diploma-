using System.Drawing;

namespace System.Windows.Forms
{
    public class AdjTable : DataGridView
    {
        private string[][] _matrix;

        public string[][] Matrix
        {
            get
            {
                return _matrix;
            }
        }

        public AdjTable()
        {
            VirtualMode = true;
            ColumnHeadersVisible = false;
            RowHeadersVisible = false;
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
            AllowUserToAddRows = false;
        }

        protected override void OnCellValuePushed(DataGridViewCellValueEventArgs e)
        {
            try
            {
                _matrix[e.RowIndex][e.ColumnIndex] =
                    e.Value == null ? "0" : e.Value.ToString();
            }
            catch (Exception)
            {
                throw;
            }
            base.OnCellValuePushed(e);
        }

        protected override void OnCellValueNeeded(DataGridViewCellValueEventArgs e)
        {
            try
            {
                e.Value = _matrix[e.RowIndex][e.ColumnIndex];
            }
            catch (Exception)
            {
                throw;
            }
            base.OnCellValueNeeded(e);
        }

        public void Build(string[][] matrix)
        {
            _matrix = matrix;
            RowCount = ColumnCount = _matrix.Length;
            Invalidate();
        }
    }
}
