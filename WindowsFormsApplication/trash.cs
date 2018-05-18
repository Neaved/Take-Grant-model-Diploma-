private void DumpACEs(ManagementBaseObject[] DaclObject, string fileName, int a)
        {
            log.Debug("fileName: " + fileName);

            Dictionary<string, List<string>> userRights = new Dictionary<string, List<string>>();
            List<string> notExistedUserSids = new List<string>();
            List<string> existedUserSids = new List<string>();



            foreach (ManagementBaseObject mbo in DaclObject)
            {
                log.Debug("-------------------DaclObject Properties------------------------------");
                foreach (PropertyData prop in mbo.Properties)
                {
                    log.Debug("propName: '" + prop.Name + "' - propValue: '" + prop.Value + "'");
                }
                //log.Debug(mbo["AccessMask"] + " - " + mbo["AceFlags"] + " - " + mbo["AceType"]);

                // Access allowed/denied ACE
                bool isAccessAllowed;
                if (mbo["AceType"].ToString() == "1")
                {
                    log.Debug("DENIED ACE TYPE");
                    isAccessAllowed = false;
                }
                else
                {
                    log.Debug("ALLOWED ACE TYPE");
                    isAccessAllowed = true;
                }

                // Dump trustees
                ManagementBaseObject Trustee = ((ManagementBaseObject)(mbo["Trustee"]));
                log.Debug("-------------------Trustee Properties------------------------------");
                foreach (PropertyData prop in Trustee.Properties)
                {
                    log.Debug("propName: '" + prop.Name + "' - propValue: '" + prop.Value + "'");
                }
                //log.Debug("Name: " + Trustee.Properties["Name"].Value +
                //    " - Domain: " + Trustee.Properties["Domain"].Value +
                //    " SID " + Trustee.Properties["SIDString"].Value);


                string userSid = (string)Trustee.Properties["SIDString"].Value;

                //bool isUserExistInDacl = false;
                bool isUserHaveNeedRightForGraph = false;

                if (isAccessAllowed && userAccountSidValue.Contains(userSid))
                {
                    existedUserSids.Add(userSid);

                    // Dump ACE mask in readable form
                    UInt32 mask = (UInt32)mbo["AccessMask"];


                    //// using enum formatting (see emumerating the possibilities.doc)
                    string[] fileRights = Enum.Format(typeof(Mask), mask, "g").Replace(" ", string.Empty).Split(',');
                    List<string> fileRightsToDictionary = new List<string>();
                    foreach (string fileRight in fileRights)
                    {
                        log.Debug("fileRight: " + fileRight);
                        //log.Debug("ContainsKey: " + RIGHTS_Dictionary.ContainsKey(fileRight));

                        if (RIGHTS_Dictionary.ContainsKey(fileRight))
                        {
                            string hexRightValue;
                            RIGHTS_Dictionary.TryGetValue(fileRight, out hexRightValue);
                            fileRightsToDictionary.Add(hexRightValue);


                            log.Debug("*** fileRight " + fileRight);
                            log.Debug("*** hexRightValue " + hexRightValue);

                            //byte[] data = FromHex(hexRightValue);
                            //log.Debug("*** HEX value " + Encoding.ASCII.GetString(data));

                        }
                    }


                    if (isNotEmpty(fileRightsToDictionary))
                    {
                        if (isAdministratorSidValue(userSid))
                        {
                            fileRightsToDictionary.Add("100000000");
                            //fileRightsToDictionary.Add("administratorFlag");

                        }
                        userRights.Add(userSid, fileRightsToDictionary);// Enum.Format(typeof(Mask), mask, "g").Trim().Split(','));
                        isUserHaveNeedRightForGraph = true;
                    }

                    if (!isUserHaveNeedRightForGraph)
                    {
                        log.Debug("user with sid '" + userSid + "' don't have need right on file '" + fileName + "' for graph");
                    }
                    //isUserExistInDacl = true;


                }
                //else
                //{
                //    notExistedUserSids.Add(userSid);
                //  //  log.Debug("there is no such sid-user in security property of " + fileName);
                //}
            }

            if (isEmpty(existedUserSids))
            {
                StringBuilder sb = new StringBuilder();
                foreach (string sid in userAccountSidValue)
                {
                    sb.Append(sid).Append(", ");
                }
                string userSidsStr = sb.ToString();
                log.Debug("there is no such user(s) " + userSidsStr.Substring(0, userSidsStr.Length - 2) + " in security property of " + fileName);
            }
            else if (!existedUserSids.Count.Equals(userAccountItems.Count))
            {
                StringBuilder sb = new StringBuilder();
                foreach (string sid in userAccountSidValue)
                {
                    if (!existedUserSids.Contains(sid))
                    {
                        sb.Append(sid).Append(", ");
                    }
                }
                string notExistedUserSidsStr = sb.ToString();
                log.Debug("there is no such user(s) " + notExistedUserSidsStr.Substring(0, notExistedUserSidsStr.Length - 2) + " in security property of " + fileName);
            }

            if (isNotEmpty(userRights))
            {
                dictionaryRights.Add(fileName, userRights);
            }

        }