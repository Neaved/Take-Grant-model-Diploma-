using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.entity
{
    public class MatrixElement
    {
        private int row;
        private int column;

        public MatrixElement(int row, int column)
        {
            this.row = row;
            this.column = column;
        }

        public bool compareElementPosition(MatrixElement element)
        {
            bool a = this.row.Equals(element.Row);
            bool b = this.column.Equals(element.Column);

            return this.row.Equals(element.Row) && this.column.Equals(element.Column);
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



    }
}
