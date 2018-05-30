using System;

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
