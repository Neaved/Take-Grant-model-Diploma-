using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Entity.entity;
using log4net;
using log4net.Config;
using static System.Windows.Forms.ListView;
using static Controller.controller.ControllerUtils;
using static Entity.entity.FileEntity;
using static Entity.entity.Permission;
using static Entity.entity.WithSidEntity;

namespace Controller.controller
{
    public class AccessMatrixController
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private const string ADMINISTRATOR_PERMISSION_FLAG = "100000000";
        
        private const string REPLACE_MACROS = "replace_macros";
        private const string File_Security_Setting_Path =
            "Win32_logicalFileSecuritySetting.Path='" + REPLACE_MACROS + "'";

        private HashSet<string> ADMINISTRATOR_SID_VALUES = new HashSet<string>()
        {
            "500",
            "544"
        };
        private List<string> existedUserSids = new List<string>();

        private List<string> selectedFiles;
        private List<UserEntity> selectedUsers;
        private Dictionary<string, Dictionary<string, List<string>>> dictionaryPermissions =
            new Dictionary<string, Dictionary<string, List<string>>>();
        private List<GraphVertexEntity> graphVertexs;
        private int[][] accessMatrix;
        private List<ManagementEntity> usersListWithAllowedAceType =
            new List<ManagementEntity>();
        private List<ManagementEntity> usersListWithDeniedAceType =
            new List<ManagementEntity>();
        private List<string> warningMessages = new List<string>();

        public List<GraphVertexEntity> GraphVertexs
        {
            get
            {
                return graphVertexs;
            }
        }

        public List<string> WarningMessages
        {
            get
            {
                return warningMessages;
            }
        }

        public int[][] AccessMatrix
        {
            get
            {
                return accessMatrix;
            }
        }

        public AccessMatrixController(SelectedListViewItemCollection fileItems,
            SelectedListViewItemCollection userAccountItems)
        {
            XmlConfigurator.Configure();
            try
            {
                this.selectedFiles = getSelectedFiles(fileItems);
                this.selectedUsers = getSelectedUsers(userAccountItems);
                //Stopwatch stopwatch = new Stopwatch();
                //log.Debug("time start ");
                //stopwatch.Start();
                prepareDictionaryPermissions();
                buildAccessMatrix();
                //stopwatch.Stop();
                //log.Debug("time stop ");
                //log.Debug("time TotalMilliseconds " + stopwatch.Elapsed.TotalMilliseconds);
            }
            catch (Exception ex)
            {
                log.Error("Exception Message: " + ex.Message);
                log.Error("Exception StackTrace: " + ex.StackTrace);
            }

        }

        private List<string> getSelectedFiles(
            SelectedListViewItemCollection items)
        {
            List<string> fileNames = new List<string>(items.Count);
            foreach (ListViewItem item in items)
            {
                fileNames.Add(item.SubItems[0].Text);
            }
            return fileNames;
        }

        private List<UserEntity> getSelectedUsers(SelectedListViewItemCollection items)
        {
            List<UserEntity> selectedUsers = new List<UserEntity>(items.Count);
            foreach (ListViewItem item in items)
            {
                selectedUsers.Add(
                    new UserEntity(
                        item.SubItems[0].Text,
                        item.SubItems[1].Text,
                        getGroupNames(item.SubItems[2].Text.Split(','))
                        )
                    );
            }
            return selectedUsers;
        }

        private HashSet<string> getGroupNames(string[] groups)
        {
            HashSet<string> groupNames = new HashSet<string>();
            foreach (string group in groups)
            {
                groupNames.Add(group.Trim());
            }
            return groupNames;
        }

        private void prepareDictionaryPermissions()
        {
            foreach (string fileFullName in selectedFiles)
            {
                ManagementObject managementObject =
                    new ManagementObject(
                        File_Security_Setting_Path.Replace(REPLACE_MACROS, fileFullName));
                ManagementBaseObject managementBaseObject =
                    managementObject.InvokeMethod("GetSecurityDescriptor", null, null);
                if (((uint)(managementBaseObject.Properties["ReturnValue"].Value)) == 0)
                {
                    ManagementBaseObject securityDescriptor =
                        ((ManagementBaseObject)(managementBaseObject.Properties["Descriptor"].Value));
                    ManagementBaseObject[] daclObject =
                        ((ManagementBaseObject[])(securityDescriptor.Properties["Dacl"].Value));
                    Dictionary<string, List<string>> usersPermissions = getUsersPermissions(daclObject, fileFullName);
                    if (isNotEmpty(usersPermissions))
                    {
                        dictionaryPermissions.Add(fileFullName, usersPermissions);
                    }
                }

            }
        }

        private Dictionary<string, List<string>> getUsersPermissions(ManagementBaseObject[] daclObject, string fileName)
        {
            existedUserSids.Clear();
            fillUsersListAcсordingToAceTypeValue(daclObject);
            Dictionary<string, List<string>> usersPermissions =
                new Dictionary<string, List<string>>();

            log.Debug("----------------------------------------------fileName: " + fileName + " ---------------------------------------------------------------------------");

            foreach (UserEntity selectedUser in selectedUsers)
            {
                log.Debug("----------------------------------------------st for selectedUser: " + selectedUser.ToString() + " ---------------------------------------------------------------------------");

                string selectedUserSidValue = selectedUser.Sid;
                UInt32 accessMask = getAccessMask(
                    selectedUserSidValue,
                    selectedUser.GroupNames,
                    getAllowedAceUserSidValues().Contains(selectedUserSidValue));
                log.Debug("final AccessMask: " + accessMask);
                bool isUserHaveNeedRightForGraph = false;
                if (accessMask != 0)
                {
                    List<string> permissionsOnFile = getPermissionsOnFile(
                        Enum
                        .Format(typeof(Mask), accessMask, "g")
                        .Replace(" ", string.Empty)
                        .Split(','));

                    if (isNotEmpty(permissionsOnFile))
                    {
                        if (isAdministratorSidValue(selectedUser.LastSidPart))
                        {
                            permissionsOnFile.Add(ADMINISTRATOR_PERMISSION_FLAG);
                            //permissionsOnFile.Add("administratorFlag");
                        }
                        usersPermissions.Add(selectedUserSidValue, permissionsOnFile);
                        isUserHaveNeedRightForGraph = true;
                    }

                    if (!isUserHaveNeedRightForGraph)
                    {
                        log.Debug("user with sid '" + selectedUser.LastSidPart + "' don't have need right on file '" + fileName + "' for graph");
                        warningMessages.Add(
                            "user with sid '" + selectedUser.LastSidPart +
                            "' don't have need right on file '" + fileName + "' for graph");
                    }

                }

                log.Debug("----------------------------------------------end for selectedUser---------------------------------------------------------------------------");

            }
            fillWarningMessages(fileName);
            return usersPermissions;
        }

        private void fillUsersListAcсordingToAceTypeValue(
            ManagementBaseObject[] daclObject)
        {
            log.Debug("-------------------fillUsersListAcсordingToAceTypeValue st------------------------------");

            usersListWithAllowedAceType.Clear();
            usersListWithDeniedAceType.Clear();
            foreach (ManagementBaseObject mbo in daclObject)
            {
                //log.Debug("-------------------DaclObject Properties------------------------------");
                //foreach (PropertyData prop in mbo.Properties)
                //{
                //    log.Debug("propName: '" + prop.Name + "' - propValue: '" + prop.Value + "'");
                //}
                //log.Debug("---------------------------------------------------------------------");
                //log.Debug("");

                ManagementBaseObject Trustee = ((ManagementBaseObject)(mbo["Trustee"]));
                ManagementEntity managementEntity = new ManagementEntity(
                    (string)Trustee.Properties["Name"].Value,
                    (string)Trustee.Properties["SIDString"].Value,
                    (UInt32)mbo["AccessMask"]);

                //log.Debug("-------------------Trustee Properties------------------------------");
                //foreach (PropertyData prop in Trustee.Properties)
                //{
                //    log.Debug("propName: '" + prop.Name + "' - propValue: '" + prop.Value + "'");
                //}
                //log.Debug("---------------------------------------------------------------------");
                //log.Debug("");

                if (mbo["AceType"].ToString() == "0")
                {
                    log.Debug("ALLOWED ACE TYPE");
                    usersListWithAllowedAceType.Add(managementEntity);
                }
                else
                {
                    log.Debug("DENIED ACE TYPE");
                    usersListWithDeniedAceType.Add(managementEntity);
                }

            }
            log.Debug("-------------------fillUsersListAcсordingToAceTypeValue end------------------------------");
        }

        private List<string> getAllowedAceUserSidValues()
        {
            List<string> allowedAceUserSidValues =
                new List<string>(usersListWithAllowedAceType.Count);
            foreach (ManagementEntity allowedAceUser in usersListWithAllowedAceType)
            {
                allowedAceUserSidValues.Add(allowedAceUser.Sid);
            }
            return allowedAceUserSidValues;
        }

        private UInt32 getAccessMask(string selectedUserSidValue, HashSet<string> selectedUserGroupNames, bool isSelectedUserDefinedInDacl)
        {
            if (isSelectedUserDefinedInDacl)
            {
                existedUserSids.Add(selectedUserSidValue);
                ManagementEntity allowedAceUser =
                    getManagementEntityBySidValue(selectedUserSidValue);
                return getFinalAccessMask(
                    allowedAceUser.AccessMask,
                    getDeniedAccessMaskBySidValue(selectedUserSidValue));
            }
            else
            {
                List<ManagementEntity> groupsInDacl =
                    getGroupsFromDacl(selectedUserGroupNames);
                if (isNotEmpty(groupsInDacl))
                {
                    existedUserSids.Add(selectedUserSidValue);
                    ManagementEntity allowedAceGroup;
                    if (groupsInDacl.Count == 1)
                    {
                        allowedAceGroup = groupsInDacl[0];
                    }
                    else
                    {
                        allowedAceGroup = getGroupWithMaxAccessMask(groupsInDacl);
                    }
                    return getFinalAccessMask(
                        allowedAceGroup.AccessMask,
                        getDeniedAccessMaskBySidValue(allowedAceGroup.Sid));
                }
                log.Debug("groupsInDacl is Empty");
                return 0;
            }
        }

        private ManagementEntity getManagementEntityBySidValue(string sidValue)
        {
            foreach (ManagementEntity allowedAceUser in usersListWithAllowedAceType)
            {
                if (sidValue.Equals(allowedAceUser.Sid))
                {
                    return allowedAceUser;
                }
            }
            return null;
        }

        private UInt32 getDeniedAccessMaskBySidValue(string userSid)
        {
            foreach (ManagementEntity deniedAceUser in usersListWithDeniedAceType)
            {
                if (userSid.Equals(deniedAceUser.Sid))
                {
                    return deniedAceUser.AccessMask;
                }
            }

            return 0;
        }

        private UInt32 getFinalAccessMask(
            UInt32 allowedAccessMask, UInt32 deniedAccessMask)
        {
            log.Debug("allowedAccessMask: " + allowedAccessMask);
            log.Debug("deniedAccessMask: " + deniedAccessMask);
            if (deniedAccessMask == 0)
            {
                return allowedAccessMask;
            }
            else
            {
                string allowedAccessMaskBin =
                    getFullBinaryAccessMask(Convert.ToString(allowedAccessMask, 2));
                string deniedAccessMaskBin =
                    getFullBinaryAccessMask(Convert.ToString(deniedAccessMask, 2));
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < deniedAccessMaskBin.Length; i++)
                {
                    if ("1".Equals(deniedAccessMaskBin[i]))
                    {
                        sb.Append("0");
                    }
                    else
                    {
                        sb.Append(allowedAccessMaskBin[i]);
                    }
                }
                return Convert.ToUInt32(sb.ToString(), 2);
            }
        }

        private string getFullBinaryAccessMask(string binaryAccessMask)
        {
            int maskLength = binaryAccessMask.Length;
            if (maskLength < 32)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < 32 - maskLength; i++)
                {
                    sb.Append("0");
                }
                sb.Append(binaryAccessMask);
                return sb.ToString();
            }
            return binaryAccessMask;
        }

        private ManagementEntity getGroupWithMaxAccessMask(
            List<ManagementEntity> groupsInDacl)
        {
            UInt32 accessMask = 0;
            ManagementEntity groupWithMaxAccessMask = null;
            foreach (ManagementEntity group in groupsInDacl)
            {
                UInt32 groupAccessMask = group.AccessMask;
                if (groupAccessMask > accessMask)
                {
                    accessMask = groupAccessMask;
                    groupWithMaxAccessMask = group;
                }
                log.Debug("getGroupWithMaxAccessMask accessMask: " + accessMask);
            }
            return groupWithMaxAccessMask;
        }

        private List<ManagementEntity> getGroupsFromDacl(HashSet<string> groupNames)
        {
            List<ManagementEntity> groupsInDacl = new List<ManagementEntity>();
            foreach (string groupName in groupNames)
            {
                //log.Debug("groupName: " + groupName);
                //log.Debug("groupName Length: " + groupName.Length);
                foreach (ManagementEntity allowedAceUser in usersListWithAllowedAceType)
                {
                    //log.Debug("allowedAceUser.Name: " + allowedAceUser.Name);
                    //log.Debug("allowedAceUser.Name Length: " + allowedAceUser.Name.Length);
                    //log.Debug("allowedAceUser.Sid: " + allowedAceUser.Sid);
                    //log.Debug("allowedAceUser.Sid.Length: " + allowedAceUser.Sid.Split('-').Length);

                    if ((groupName.Equals(allowedAceUser.Name))
                            && (allowedAceUser.Sid.Split('-').Length < 8))
                    {
                        groupsInDacl.Add(allowedAceUser);
                    }
                }
            }
            return groupsInDacl;
        }

        private List<string> getPermissionsOnFile(string[] permissions)
        {
            List<string> permissionsOnFile = new List<string>();
            foreach (string permission in permissions)
            {
                log.Debug("permission: " + permission);
                if (VALID_PERMISSIONS.ContainsKey(permission))
                {
                    string hexRightValue;
                    VALID_PERMISSIONS.TryGetValue(permission, out hexRightValue);
                    permissionsOnFile.Add(hexRightValue);
                    log.Debug("permissionsOnFile add: " + permission);
                }
            }
            return permissionsOnFile;
        }

        private bool isAdministratorSidValue(string lastSidPart)
        {
            return ADMINISTRATOR_SID_VALUES.Contains(lastSidPart);
            //return "500".Equals(sidParts[sidParts.Length - 1]);
        }

        private void fillWarningMessages(string fileName)
        {
            StringBuilder sb = new StringBuilder();
            if (isEmpty(existedUserSids))
            {
                foreach (UserEntity selectedUser in selectedUsers)
                {
                    sb.Append(selectedUser.LastSidPart).Append(", ");
                }

            }
            else if (!existedUserSids.Count.Equals(selectedUsers.Count))
            {
                foreach (UserEntity selectedUser in selectedUsers)
                {
                    if (!existedUserSids.Contains(selectedUser.Sid))
                    {
                        sb.Append(selectedUser.LastSidPart).Append(", ");
                    }
                }
            }
            string userSidsStr = sb.ToString();
            if (isNotEmpty(userSidsStr))
            {
                log.Debug("there is no such user(s) " + userSidsStr.Substring(0, userSidsStr.Length - 2) + " in security property of " + fileName);
                warningMessages.Add(
                    "there is no such user(s) " + userSidsStr.Substring(
                        0, userSidsStr.Length - 2) +
                    " in security property of " + fileName);
            }
        }

        private void buildAccessMatrix()
        {
            int subjectCount = selectedUsers.Count;
            int objectCount = dictionaryPermissions.Count;

            List<string> lineElements = getSortedlineElements(subjectCount, objectCount);

            int[][] accessMatrixTemplate = new int[subjectCount + objectCount][];
            for (int i = 0; i < accessMatrixTemplate.Length; i++)
            {
                accessMatrixTemplate[i] = new int[subjectCount + objectCount];
            }

            for (int j = subjectCount; j < accessMatrixTemplate.Length; j++)
            {
                Dictionary<string, List<string>> userWithTheirPermissions =
                    new Dictionary<string, List<string>>();
                dictionaryPermissions.TryGetValue(lineElements[j], out userWithTheirPermissions);
                for (int i = 0; i < subjectCount; i++)
                {
                    List<string> hexRightValue;
                    userWithTheirPermissions.TryGetValue(lineElements[i], out hexRightValue);
                    accessMatrixTemplate[i][j] = getDemicalPermissionValue(hexRightValue);

                }
            }
            log.Debug(accessMatrixTemplate.Length);
            this.accessMatrix = accessMatrixTemplate;
            this.graphVertexs = getGraphVertexs(lineElements, subjectCount - 1);

            writeAccessMatrix(accessMatrix, lineElements);
        }

        private List<string> getSortedlineElements(int subjectCount, int objectCount)
        {
            List<string> lineElements = new List<string>(subjectCount + objectCount);

            List<string> sortedSelectedUserSid = new List<string>(subjectCount);
            foreach (UserEntity selectedUser in selectedUsers)
            {
                sortedSelectedUserSid.Add(selectedUser.Sid);
            }
            sortedSelectedUserSid.Sort();

            List<string> sortedSelectedFileName = new List<string>(objectCount);
            foreach (KeyValuePair<string, Dictionary<string, List<string>>> item
                in dictionaryPermissions)
            {

                sortedSelectedFileName.Add(item.Key);
            }
            sortedSelectedFileName.Sort();

            lineElements.AddRange(sortedSelectedUserSid);
            lineElements.AddRange(sortedSelectedFileName);

            return lineElements;
        }

        private List<GraphVertexEntity> getGraphVertexs(
            List<string> lineElements, int subjectCount)
        {
            int lineElementsCount = lineElements.Count;
            List<GraphVertexEntity> graphVertexs =
                new List<GraphVertexEntity>(lineElementsCount);
            for (int i = 0; i < lineElementsCount; i++)
            {
                if (i > subjectCount)
                {
                    graphVertexs.Add(
                        new GraphVertexEntity(
                            getFileNameFromPath(lineElements[i]),
                            true
                            )
                        );
                }
                else
                {
                    graphVertexs.Add(
                        new GraphVertexEntity(
                            getLastValueFromSid(lineElements[i]),
                            false
                            )
                        );
                }
            }
            //foreach (GraphVertexEntity g in graphVertexs)
            //log.Debug("graphVertexs: " + g.ToString());
            return graphVertexs;
        }

        private int getDemicalPermissionValue(List<string> hexPermissionValues)
        {
            if (isNotEmpty(hexPermissionValues))
            {
                int demicalPermissionValue = 0;
                foreach (string hexRightValue in hexPermissionValues)
                {
                    //    for (int i = 0; i < 5; i++)
                    //    {
                    //        if (binaryRight[i] == 1)
                    //        {   unchecked((int)myLongValue)
                    if ("100000000".Equals(hexRightValue))
                    {
                        log.Debug("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!isAdmin");
                        demicalPermissionValue += 9000;
                    }
                    else if ("00080000".Equals(hexRightValue))
                    {
                        log.Debug("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!isOwner");
                        demicalPermissionValue += 8000;
                    }
                    else
                    {
                        demicalPermissionValue += unchecked((int)Int64.Parse(
                            hexRightValue, System.Globalization.NumberStyles.HexNumber));
                    }
                    //        }
                    //    }
                }

                return demicalPermissionValue;
            }
            else
            {
                log.Debug("demicalPermissionValues = 0");
                return 0;
            }
        }

    }
}
