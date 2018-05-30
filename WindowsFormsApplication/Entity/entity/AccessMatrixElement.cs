namespace Entity.entity
{
    public class AccessMatrixElement
    {
        private int row;
        private int column;

        public AccessMatrixElement(int row, int column)
        {
            this.row = row;
            this.column = column;
        }

        public int Row
        {
            get
            {
                return row;
            }

            set
            {
                row = value;
            }
        }

        public int Column
        {
            get
            {
                return column;
            }

            set
            {
                column = value;
            }
        }

        public bool compareElementPosition(AccessMatrixElement element)
        {
            return Row.Equals(element.Row) && Column.Equals(element.Column);
        }

    }
}
