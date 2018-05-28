﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.entity
{
    public class ManagementEntity : WithSidEntity
    {
        
        private UInt32 accessMask;

        public ManagementEntity(string name, string sid, UInt32 accessMask) : base(name, sid)
        {
            this.accessMask = accessMask;
        }

        public UInt32 AccessMask
        {
            get
            {
                return accessMask;
            }

            set
            {
                accessMask = value;
            }
        }
    }
}
