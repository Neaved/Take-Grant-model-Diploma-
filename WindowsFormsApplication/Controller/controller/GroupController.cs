using System;
using System.Collections.Generic;
using System.Management;
using Entity;
using Entity.entity;

namespace Controller.controller
{
    public class GroupController
    {
        private List<Group> groups = new List<Group>();
        private string groupsException;

        public GroupController()
        {
            getWin32Group();
        }

        public List<Group> Groups
        {
            get
            {
                return groups;
            }
        }

        public string GroupsException
        {
            get
            {
                return groupsException;
            }
        }

        private void getWin32Group()
        {
            try
            {
                SelectQuery selectQuery = new SelectQuery(Сonstants.Win32GroupWMI);
                ManagementObjectSearcher selectedObjects =
                    new ManagementObjectSearcher(selectQuery);
                foreach (ManagementObject obj in selectedObjects.Get())
                {
                    groups.Add(
                        new Group(
                            obj[Сonstants.NameProperty].ToString(),
                            obj[Сonstants.SIDProperty].ToString()
                            ));
                }
            }
            catch (Exception ex)
            {
                groupsException = ex.Message;
            }
        }
    }
}
