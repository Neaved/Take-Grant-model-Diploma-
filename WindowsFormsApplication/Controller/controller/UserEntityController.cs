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
    public class UserEntityController
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static UserEntityController instance = new UserEntityController();
        private List<UserEntity> userEntities = new List<UserEntity>();
        private string userEntityControllerException;

        private UserEntityController()
        {
            XmlConfigurator.Configure();
            getWin32UserAccount();
        }

        public static UserEntityController Instance
        {
            get
            {
                return instance;
            }
        }

        public List<UserEntity> UserEntities
        {
            get
            {
                return userEntities;
            }

            set
            {
                userEntities = value;
            }
        }

        public string UserEntityControllerException
        {
            get
            {
                if (isEmpty(userEntityControllerException))
                {
                    return userEntityControllerException;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        private void getWin32UserAccount()
        {
            try
            {
                SelectQuery selectQuery = new SelectQuery("Win32_UserAccount");
                ManagementObjectSearcher selectedObjects = new ManagementObjectSearcher(selectQuery);
                foreach (ManagementObject obj in selectedObjects.Get())
                {
                    userEntities.Add(
                        new UserEntity(
                            getFullName(obj),
                            obj["SID"].ToString(),
                            getGroupsForUser(obj["Domain"].ToString(), obj["Name"].ToString()),
                            obj["Description"].ToString()));
                }
            }
            catch (Exception ex)
            {
                userEntityControllerException = ex.Message;
                log.Error("Exception Message: " + ex.Message);
                log.Error("Exception StackTrace: " + ex.StackTrace);
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
