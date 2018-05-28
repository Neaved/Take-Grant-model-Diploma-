using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.entity
{
    public class BaseEntity
    {
        private string name;

        public BaseEntity() { }
        public BaseEntity(string name)
        {
            this.name = name;
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
