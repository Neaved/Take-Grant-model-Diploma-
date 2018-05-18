using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Form.form
{
    class UserAccountFormExecutor
    {
        public static UserAccountFormExecutor instance = new UserAccountFormExecutor();

        private UserAccountFormExecutor() { }

        internal static UserAccountFormExecutor Instance
        {
            get
            {
                return instance;
            }
        }
    }
}
