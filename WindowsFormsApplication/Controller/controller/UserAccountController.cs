using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.entity;
using Controller.controller;
using static Controller.controller.ControllerUtils;
using System.Management;
using log4net;
using System.Reflection;
using log4net.Config;

namespace Controller.controller
{
    public class UserAccountController
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static UserAccountController instance = new UserAccountController();
        private List<UserAccount> userAccounts = new List<UserAccount>();
        private string userAccountControllerException;

        private UserAccountController()
        {
            XmlConfigurator.Configure();
            getWin32UserAccount();
        }

        public static UserAccountController Instance
        {
            get
            {
                return instance;
            }
        }

        public string UserAccountControllerException
        {
            get
            {
                if (isEmpty(userAccountControllerException))
                {
                    return userAccountControllerException;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public List<UserAccount> UserAccounts
        {
            get
            {
                return userAccounts;
            }
        }

        private void getWin32UserAccount()
        {
            SelectQuery selectQuery = new SelectQuery("Win32_UserAccount");
            try
            {
                ManagementObjectSearcher selectedObjects = new ManagementObjectSearcher(selectQuery);
                foreach (ManagementObject obj in selectedObjects.Get())
                {
                    userAccounts.Add(
                        new UserAccount(
                            getFullName(obj),
                            obj["SID"].ToString(),
                            getGroupsForUser(obj["Domain"].ToString(), obj["Name"].ToString()),
                            obj["Description"].ToString()));
                }
            }
            catch (Exception ex)
            {
                userAccountControllerException = ex.ToString();
            }
        }

        private string getFullName(ManagementObject obj)
        {
            Object fullName = obj["FullName"];
            if (isNotEmpty(fullName))
            {
                    return obj["Name"].ToString() + "|" + fullName.ToString();
            }
            else
            {
                return obj["Name"].ToString();
            }
        }

        private HashSet<string> getGroupsForUser(string domain, string userName)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_GroupUser where PartComponent=\"Win32_UserAccount.Domain='" + domain +"',Name='" + userName + "'\"");
            HashSet<string> groupNames = new HashSet<string>();
            foreach (ManagementObject mObject in searcher.Get())
            {
                ManagementPath path = new ManagementPath(mObject["GroupComponent"].ToString());
                if (path.ClassName == "Win32_Group")
                {
                    string[] names = path.RelativePath.Split(',');
                    foreach (string name in names)
                    {
                        groupNames.Add(names[1].Substring(names[1].IndexOf("=") + 1).Replace('"', ' ').Trim());
                    }
                }
            }
            return groupNames;
        }
    }
}
