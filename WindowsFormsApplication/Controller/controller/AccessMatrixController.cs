using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Entity.entity.Permission;
using static Entity.entity.FileEntity;
using static Entity.entity.WithSidEntity;
using static System.Windows.Forms.ListView;
using static Controller.controller.ControllerUtils;
using System.Security.AccessControl;
using System.IO;
using System.Security.Principal;
using Entity.entity;
using log4net;
using System.Reflection;
using log4net.Config;

namespace Controller.controller
{
    public class AccessMatrixController
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private const string REPLACE_MACROS = "replace_macros";
        private const string File_Security_Setting_Path = "Win32_LogicalFileSecuritySetting.Path='" + REPLACE_MACROS + "'";

        private HashSet<string> ADMINISTRATOR_SID_VALUES = new HashSet<string>() { "500", "544" };


        private List<string> filesFullname;
        private List<UserAccount> selectedUsers;
        private Dictionary<string, Dictionary<string, List<string>>> dictionaryRights = new Dictionary<string, Dictionary<string, List<string>>>();
        private int[][] adjacencyMatrix;
        private List<GraphVertexEntity> graphVertexs;
        private List<string> warningMessages = new List<string>();
        private List<ManagementEntity> usersListWithAllowedAceType = new List<ManagementEntity>();
        private List<ManagementEntity> usersListWithDeniedAceType = new List<ManagementEntity>();

        public Dictionary<string, Dictionary<string, List<string>>> DictionaryRights
        {
            get
            {
                return dictionaryRights;
            }
        }

        public int[][] AdjacencyMatrix
        {
            get
            {
                return adjacencyMatrix;
            }
        }

        public List<GraphVertexEntity> GraphVertexs
        {
            get
            {
                return graphVertexs;
            }

            set
            {
                graphVertexs = value;
            }
        }

        public List<string> WarningMessages
        {
            get
            {
                return warningMessages;
            }
        }

        public AccessMatrixController(SelectedListViewItemCollection fileItems, SelectedListViewItemCollection userAccountItems)
        {
            XmlConfigurator.Configure();
            this.filesFullname = getFileNamesFromItemCollection(fileItems);
            this.selectedUsers = getSelectedUsers(userAccountItems);
            //this.userAccountItems = userAccountItems;
            //this.selectedUserSidValues = getNamesFromItemCollection(userAccountItems, false);
            // this.userAccountGroups = getGroupsFromItemCollection(userAccountItems);
            //log.DebugRange(selectedUserSidValues);
            prepareDictionaryRights();
            buildAccessMatrix();
        }

        private List<string> getFileNamesFromItemCollection(SelectedListViewItemCollection items)
        {
            List<string> fileNames = new List<string>(items.Count);
            foreach (ListViewItem item in items)
            {
                fileNames.Add(item.SubItems[0].Text);
            }
            return fileNames;
        }

        private List<UserAccount> getSelectedUsers(SelectedListViewItemCollection items)
        {
            List<UserAccount> selectedUsers = new List<UserAccount>(items.Count);
            foreach (ListViewItem item in items)
            {
                selectedUsers.Add(
                    new UserAccount(
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

        private void prepareDictionaryRights()
        {
            try
            {
                foreach (string file in filesFullname)
                {
                    ManagementObject managementObject = new ManagementObject(File_Security_Setting_Path.Replace(REPLACE_MACROS, file));
                    ManagementBaseObject outP = managementObject.InvokeMethod("GetSecurityDescriptor", null, null);
                    if (((uint)(outP.Properties["ReturnValue"].Value)) == 0)
                    {
                        ManagementBaseObject Descriptor = ((ManagementBaseObject)(outP.Properties["Descriptor"].Value));
                        ManagementBaseObject[] DaclObject = ((ManagementBaseObject[])(Descriptor.Properties["Dacl"].Value));
                        DumpACEs(DaclObject, file);
                    }

                }

            }
            catch (Exception ex)
            {
                log.Debug(ex.Message);
                log.Debug(ex.StackTrace);
            }

        }

        private void DumpACEs(ManagementBaseObject[] daclObject, string fileName)
        {
            fillUsersListAcсordingToAceTypeValue(daclObject);
            List<string> allowedAceUserSidValues = getAllowedAceUserSidValues();
            List<string> allowedAceUserNames = getAllowedAceUserNames();
            Dictionary<string, List<string>> userRights = new Dictionary<string, List<string>>();
            List<string> existedUserSids = new List<string>();

            log.Debug("----------------------------------------------fileName: " + fileName + " ---------------------------------------------------------------------------");

            foreach (UserAccount selectedUser in selectedUsers)
            {
                log.Debug("----------------------------------------------st for selectedUser: " + selectedUser.ToString() + " ---------------------------------------------------------------------------");

                string selectedUserSidValue = selectedUser.Sid;
                UInt32 accessMask = 0;// 1180063;
                bool isUserHaveNeedRightForGraph = false;

                if (allowedAceUserSidValues.Contains(selectedUserSidValue))
                {
                    existedUserSids.Add(selectedUserSidValue);
                    ManagementEntity allowedAceUser = getManagementEntityBySidValue(selectedUserSidValue);
                    accessMask = getFinalAccessMask(allowedAceUser.AccessMask, getDeniedAccessMaskBySidValue(selectedUserSidValue));
                    log.Debug("final AccessMask: " + accessMask);
                }
                else
                {
                    log.Debug("TODO: logic with groups");

                    List<ManagementEntity> groupsInDacl = getGroupsFromDacl(selectedUser.GroupNames);
                    log.Debug("groupsInDacl count: " + groupsInDacl.Count);

                    if (isNotEmpty(groupsInDacl))
                    {
                        existedUserSids.Add(selectedUserSidValue);
                        if (groupsInDacl.Count == 1)
                        {
                            ManagementEntity allowedAceGroup = groupsInDacl[0];
                            accessMask = getFinalAccessMask(allowedAceGroup.AccessMask, getDeniedAccessMaskBySidValue(allowedAceGroup.Sid));
                            log.Debug("final AccessMask: " + accessMask);
                        }
                        else
                        {
                            ManagementEntity allowedAceGroup = getGroupWithMaxAccessMask(groupsInDacl);
                            accessMask = getFinalAccessMask(allowedAceGroup.AccessMask, getDeniedAccessMaskBySidValue(allowedAceGroup.Sid));
                            log.Debug("final AccessMask: " + accessMask);
                        }
                    }
                }

                if (accessMask != 0)
                {
                    string[] permissions = Enum.Format(typeof(Mask), accessMask, "g").Replace(" ", string.Empty).Split(',');
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

                    if (isNotEmpty(permissionsOnFile))
                    {
                        if (isAdministratorSidValue(selectedUserSidValue))
                        {
                            permissionsOnFile.Add("100000000");
                            //permissionsOnFile.Add("administratorFlag");
                        }
                        userRights.Add(selectedUserSidValue, permissionsOnFile);
                        isUserHaveNeedRightForGraph = true;
                    }

                    if (!isUserHaveNeedRightForGraph)
                    {
                        log.Debug("user with sid '" + selectedUserSidValue + "' don't have need right on file '" + fileName + "' for graph");
                        warningMessages.Add("user with sid '" + selectedUserSidValue + "' don't have need right on file '" + fileName + "' for graph");
                    }

                }

                log.Debug("----------------------------------------------end for selectedUser---------------------------------------------------------------------------");

            }

            fillWarningMessages(existedUserSids, fileName);

            if (isNotEmpty(userRights))
            {
                dictionaryRights.Add(fileName, userRights);
            }

        }
        private void fillUsersListAcсordingToAceTypeValue(ManagementBaseObject[] DaclObject)
        {
            log.Debug("-------------------fillUsersListAcсordingToAceTypeValue st------------------------------");

            foreach (ManagementBaseObject mbo in DaclObject)
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
            List<string> allowedAceUserSidValues = new List<string>(usersListWithAllowedAceType.Count);
            foreach (ManagementEntity allowedAceUser in usersListWithAllowedAceType)
            {
                allowedAceUserSidValues.Add(allowedAceUser.Sid);
            }
            return allowedAceUserSidValues;
        }

        private List<string> getAllowedAceUserNames()
        {
            List<string> allowedAceUserNames = new List<string>(usersListWithAllowedAceType.Count);
            foreach (ManagementEntity allowedAceUser in usersListWithAllowedAceType)
            {
                allowedAceUserNames.Add(allowedAceUser.Name);
            }
            return allowedAceUserNames;
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

        private UInt32 getFinalAccessMask(UInt32 allowedAccessMask, UInt32 deniedAccessMask)
        {
            log.Debug("allowedAccessMask: " + allowedAccessMask);
            log.Debug("deniedAccessMask: " + deniedAccessMask);
            if (deniedAccessMask == 0)
            {
                return allowedAccessMask;
            }
            else
            {
                string allowedAccessMaskBin = getFullBinaryAccessMask(Convert.ToString(allowedAccessMask, 2));
                string deniedAccessMaskBin = getFullBinaryAccessMask(Convert.ToString(deniedAccessMask, 2));
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

        private ManagementEntity getGroupWithMaxAccessMask(List<ManagementEntity> groupsInDacl)
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

                    if ((groupName.Equals(allowedAceUser.Name)) && (allowedAceUser.Sid.Split('-').Length < 8))
                    {
                        groupsInDacl.Add(allowedAceUser);
                    }
                }
            }
            return groupsInDacl;
        }

        private bool isAdministratorSidValue(string sidValue)
        {
            string[] sidParts = sidValue.Trim().Split('-');
            return ADMINISTRATOR_SID_VALUES.Contains(sidParts[sidParts.Length - 1]);
            //return "500".Equals(sidParts[sidParts.Length - 1]);
        }

        private void fillWarningMessages(List<string> existedUserSids, string fileName)
        {
            if (isEmpty(existedUserSids))
            {
                StringBuilder sb = new StringBuilder();
                foreach (UserAccount selectedUser in selectedUsers)
                {
                    sb.Append(selectedUser.Sid).Append(", ");
                }
                string userSidsStr = sb.ToString();
                log.Debug("there is no such user(s) " + userSidsStr.Substring(0, userSidsStr.Length - 2) + " in security property of " + fileName);
                warningMessages.Add("there is no such user(s) " + userSidsStr.Substring(0, userSidsStr.Length - 2) + " in security property of " + fileName);
            }
            else if (!existedUserSids.Count.Equals(selectedUsers.Count))
            {
                StringBuilder sb = new StringBuilder();
                foreach (UserAccount selectedUser in selectedUsers)
                {
                    string selectedUserSidValue = selectedUser.Sid;
                    if (!existedUserSids.Contains(selectedUserSidValue))
                    {
                        sb.Append(selectedUserSidValue).Append(", ");
                    }
                }
                string notExistedUserSidsStr = sb.ToString();
                log.Debug("there is no such user(s) " + notExistedUserSidsStr.Substring(0, notExistedUserSidsStr.Length - 2) + " in security property of " + fileName);
                warningMessages.Add("there is no such user(s) " + notExistedUserSidsStr.Substring(0, notExistedUserSidsStr.Length - 2) + " in security property of " + fileName);
            }
        }

        private void buildAccessMatrix()
        {
            int subjectCount = selectedUsers.Count;
            int objectCount = dictionaryRights.Count;

            List<string> lineElements = new List<string>(subjectCount + objectCount);
            foreach (UserAccount selectedUser in selectedUsers)
            {
                lineElements.Add(selectedUser.Sid);
            }

            foreach (KeyValuePair<string, Dictionary<string, List<string>>> item in dictionaryRights)
            {

                //            lineElements.Add(getFileNameFromPath(item.Key));
                lineElements.Add(item.Key);

            }


            int[][] adjacencyMatrixTemplate = new int[subjectCount + objectCount][];
            for (int i = 0; i < adjacencyMatrixTemplate.Length; i++)
            {
                adjacencyMatrixTemplate[i] = new int[subjectCount + objectCount];
            }

            for (int j = subjectCount; j < adjacencyMatrixTemplate.Length; j++)
            {
                Dictionary<string, List<string>> accountWithTreirRights = new Dictionary<string, List<string>>();
                dictionaryRights.TryGetValue(lineElements[j], out accountWithTreirRights);
                for (int i = 0; i < subjectCount; i++)
                {
                    List<string> hexRightValue;
                    accountWithTreirRights.TryGetValue(lineElements[i], out hexRightValue);
                    adjacencyMatrixTemplate[i][j] = getDemicalRighValue(hexRightValue);

                }
            }

            this.adjacencyMatrix = adjacencyMatrixTemplate;
            this.graphVertexs = getGraphVertexs(lineElements, subjectCount - 1);

            WriteToTextFile writeAdjacencyMatrix = new WriteToTextFile();
            writeAdjacencyMatrix.writeAdjacencyMatrix(adjacencyMatrix, lineElements);
        }

        private List<GraphVertexEntity> getGraphVertexs(List<string> lineElements, int subjectCount)
        {
            int lineElementsCount = lineElements.Count;
            List<GraphVertexEntity> graphVertexs = new List<GraphVertexEntity>(lineElementsCount);
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
            foreach (GraphVertexEntity g in graphVertexs)
                log.Debug("graphVertexs: " + g.ToString());
            return graphVertexs;
        }

        private int getDemicalRighValue(List<string> hexRightValues)
        {
            if (isNotEmpty(hexRightValues))
            {
                int demicalRighValue = 0;
                foreach (string hexRightValue in hexRightValues)
                {
                    //    for (int i = 0; i < 5; i++)
                    //    {
                    //        if (binaryRight[i] == 1)
                    //        {   unchecked((int)myLongValue)
                    if ("100000000".Equals(hexRightValue))
                    {
                        log.Debug("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!isAdmin");
                        demicalRighValue += 9000;
                    }
                    else if ("00080000".Equals(hexRightValue))
                    {
                        log.Debug("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!isOwner");
                        demicalRighValue += 8000;
                    }
                    else
                    {
                        demicalRighValue += unchecked((int)Int64.Parse(hexRightValue, System.Globalization.NumberStyles.HexNumber));
                    }
                    //        }
                    //    }
                }

                return demicalRighValue;
            }
            else
            {
                log.Debug("demicalRighValue = 0");
                return 0;
            }
        }

    }
}
