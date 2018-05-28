using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Controller.controller.ControllerUtils;
using Entity.entity;
using Controller.controller;
using System.Management;

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
                SelectQuery selectQuery = new SelectQuery("Win32_Group");
                ManagementObjectSearcher selectedObjects = new ManagementObjectSearcher(selectQuery);
                foreach (ManagementObject obj in selectedObjects.Get())
                {
                    groups.Add(
                        new Group(
                            obj["Name"].ToString(),
                            obj["SID"].ToString()
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
