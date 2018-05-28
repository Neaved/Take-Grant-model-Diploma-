using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.entity
{
    public class Permission
    {
        [Flags]
        public enum Mask : uint
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
            GENERIC_ALL = 0x10000000,
            GENERIC_EXECUTE = 0x20000000,
            GENERIC_WRITE = 0x40000000,
            GENERIC_READ = 0x80000000
        }

        //double a = 0x80000000;

        public const string READ_PERMISSION = "FILE_READ_DATA";
        public const string WHITE_PERMISSION = "FILE_WRITE_DATA";
        public const string EXECUTE_PERMISSION = "FILE_EXECUTE";
        public const string GENERIC_ALL_PERMISSION = "GENERIC_ALL";


        public static Dictionary<string, string> VALID_PERMISSIONS = new Dictionary<string, string>()
        {
            {READ_PERMISSION, "00000001"},
            {WHITE_PERMISSION, "00000002"},
            {EXECUTE_PERMISSION, "00000020"},
            {GENERIC_ALL_PERMISSION, "10000000"},
            {"WRITE_OWNER", "00080000"}
        };
    }
}
