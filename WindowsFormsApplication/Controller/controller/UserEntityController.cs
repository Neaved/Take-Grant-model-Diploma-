using System;
using System.Collections.Generic;
using System.Management;
using System.Reflection;
using Entity;
using Entity.entity;
using log4net;
using log4net.Config;
using static Controller.controller.ControllerUtils;

namespace Controller.controller
{
    public class UserEntityController
    {
        private static readonly ILog log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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
                SelectQuery selectQuery = new SelectQuery(Сonstants.Win32UserAccountWMI);
                ManagementObjectSearcher selectedObjects =
                    new ManagementObjectSearcher(selectQuery);
                foreach (ManagementObject obj in selectedObjects.Get())
                {
                    userEntities.Add(
                        new UserEntity(
                            getFullName(obj),
                            obj[Сonstants.SIDProperty].ToString(),
                            getGroupsForUser(Сonstants.ManagementObjectSearcherQuery
                            .Replace(Сonstants.ReplaceDomain,
                                obj[Сonstants.DomainProperty].ToString())
                            .Replace(Сonstants.ReplaceuserName,
                                obj[Сonstants.NameProperty].ToString())),
                            obj[Сonstants.DescriptionProperty].ToString()));
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
            Object fullName = obj[Сonstants.FullNameProperty];
            if (isNotEmpty(fullName))
            {
                return obj[Сonstants.NameProperty].ToString() +
                    Сonstants.PipeSymbol + fullName.ToString();
            }
            else
            {
                return obj[Сonstants.NameProperty].ToString();
            }
        }

        private HashSet<string> getGroupsForUser(string selectQuery)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(selectQuery);
            HashSet<string> groupNames = new HashSet<string>();
            foreach (ManagementObject mObject in searcher.Get())
            {
                ManagementPath path =
                    new ManagementPath(mObject[Сonstants.GroupComponentProperty].ToString());
                if (path.ClassName == Сonstants.Win32GroupWMI)
                {
                    string[] names = path.RelativePath.Split(Сonstants.CommaSplitSymbolChar);
                    foreach (string name in names)
                    {
                        groupNames.Add(names[1]
                            .Substring(names[1]
                            .IndexOf("=") + 1)
                            .Replace('"', ' ')
                            .Trim());
                    }
                }
            }
            return groupNames;
        }
    }
}
