﻿using System;
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
        private static GroupController instance = new GroupController();
        private List<Group> groups = new List<Group>();
        private string groupsException;

        public GroupController()
        {
            getWin32Group();
        }
 
        public static GroupController Instance
        {
            get
            {
                return instance;
            }
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
            SelectQuery selectQuery = new SelectQuery("Win32_Group");
            try
            {
                ManagementObjectSearcher selectedObjects = new ManagementObjectSearcher(selectQuery);
                foreach (ManagementObject obj in selectedObjects.Get())
                {
                    groups.Add(
                        new Group(
                            obj["Name"].ToString(),
                            obj["SID"].ToString()
                            //, obj["Description"].ToString()
                            ));
                }
            }
            catch (Exception ex)
            {
                groupsException = ex.ToString();
            }
        }
    }
}