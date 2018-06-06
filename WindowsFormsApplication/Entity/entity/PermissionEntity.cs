using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;

namespace Entity.entity
{
    public class PermissionEntity
    {
        private List<string> permissions;
        private bool isEmpty;

        private static readonly ILog log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public PermissionEntity(List<string> permissions)
        {
            this.Permissions = permissions;
            this.isEmpty = !(Permissions.Count > 0);
        }

        public List<string> Permissions { get; set; }

        public bool IsEmpty
        {
            get
            {
                return isEmpty;
            }
        }

        public void addPermission(string permission)
        {
            Permissions.Add(permission);
        }

        public string getPermissionInHexValue()
        {
            string finalBinPermission = Сonstants.EmptyBinPermission;
            if (Permissions.Count > 0)
            {
                foreach (string permission in Permissions)
                {
                    string binPermission = Convert.ToString(
                        Convert.ToInt64(((uint)Enum
                            .Parse(typeof(Permission), permission))
                            .ToString("x8"), 16), 2);
                    finalBinPermission = appendPermission(
                        finalBinPermission.ToCharArray(),
                        32 - binPermission.Length - binPermission.IndexOf("1"));
                }
            }
            return finalBinPermission;
        }

        private string appendPermission(char[] permission, int index)
        {
            for (int i = 0; i < permission.Length; i++)
            {
                if (i == index)
                {
                    permission[i] = '1';
                }
            }
            return new string(permission);
        }

        [Flags]
        public enum Permission : uint
        {
            FILE_READ_DATA = 0x00000001,
            FILE_WRITE_DATA = 0x00000002,
            FILE_APPEND_DATA = 0x00000004,
            FILE_READ_EA = 0x00000008,
            FILE_WRITE_EA = 0x00000010,
            FILE_EXECUTE = 0x00000020,
            FILE_DELETE_CHILD = 0x00000040,
            FILE_READ_ATTRIBUTES = 0x00000080,
            FILE_WRITE_ATTRIBUTES = 0x00000100,
            DELETE = 0x00010000,
            READ_CONTROL = 0x00020000,
            WRITE_DAC = 0x00040000,
            WRITE_OWNER = 0x00080000,
            SYNCHRONIZE = 0x00100000,
            ACCESS_SYSTEM_SECURITY = 0x01000000,
            MAXIMUM_ALLOWED = 0x02000000,
            //Custom permission used reserved 27 bit
            ADMINISTRATOR_FLAG = 0x08000000, 
            GENERIC_ALL = 0x10000000,
            GENERIC_EXECUTE = 0x20000000,
            GENERIC_WRITE = 0x40000000,
            GENERIC_READ = 0x80000000
        }

    }
}
