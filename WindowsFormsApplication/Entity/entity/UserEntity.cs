using System.Collections.Generic;
using System.Reflection;
using System.Text;
using log4net;
using log4net.Config;


namespace Entity.entity
{
    public class UserEntity : WithSidEntity
    {
        private static readonly ILog log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private string fullName;
        private HashSet<string> groupNames;
        private string description;

        public UserEntity() { }
        public UserEntity(string name, string sid, HashSet<string> groupNames,
            string description) : base(name, sid)
        {
            XmlConfigurator.Configure();
            setFullName(name);
            this.groupNames = groupNames;
            this.description = description;
        }

        public UserEntity(string name, string sid,
            HashSet<string> groupNames) : base(name, sid)
        {
            this.groupNames = groupNames;
        }

        public string FullName
        {
            get
            {
                return fullName;
            }

            set
            {
                fullName = value;
            }
        }
        public HashSet<string> GroupNames
        {
            get
            {
                return groupNames;
            }

            set
            {
                groupNames = value;
            }
        }

        public string Description
        {
            get
            {
                return description;
            }

            set
            {
                description = value;
            }
        }

        public string GroupNamesInString()
        {
            if (GroupNames.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (string groupName in GroupNames)
                {
                    sb.Append(groupName + Сonstants.CommaAndSpaceSymbols);
                }
                string groupNames = sb.ToString();
                return groupNames.Substring(0, groupNames.Length - 2);
            }
            else
            {
                return string.Empty;
            }
        }

        private void setFullName(string name)
        {
            if (name.Contains(Сonstants.PipeSymbol))
            {
                string[] namePart = name.Split(Сonstants.PipeSplitSymbolChar);
                Name = namePart[0].Trim();
                FullName = name
                    .Replace(Сonstants.PipeSymbol, Сonstants.SpaceAndLeftParenthesisSymbols)
                    + Сonstants.RightParenthesisSymbol;
            }
            else
            {
                FullName = Name;
            }
        }
    }
}