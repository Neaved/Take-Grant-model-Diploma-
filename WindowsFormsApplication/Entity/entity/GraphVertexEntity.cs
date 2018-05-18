using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.entity
{
    public class GraphVertexEntity : BaseEntity
    {
        private bool isObject;

        public GraphVertexEntity(string name, bool isObject) : base(name)
        {
            this.isObject = isObject;
        }

        public bool IsObject
        {
            get
            {
                return isObject;
            }

            set
            {
                isObject = value;
            }
        }
    }
}
