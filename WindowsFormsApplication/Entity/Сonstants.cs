using System.Collections.Generic;

namespace Entity
{
    public class Сonstants
    {
        //Symbol constants
        public const string PipeSymbol = "|";
        public const char PipeSplitSymbolChar = '|';
        public const char BackSlashSplitSymbolChar = '\\';
        public const char CommaSplitSymbolChar = ',';
        public const string LeftParenthesisSymbol = "(";
        public const string RightParenthesisSymbol = ")";
        public const string SpaceSymbol = " ";
        public const string CommaSymbol = ",";
        public const string SpaceAndLeftParenthesisSymbols =
            SpaceSymbol + LeftParenthesisSymbol;
        public const string CommaAndSpaceSymbols = CommaSymbol + SpaceSymbol;

        //Permission constants
        public const string ReadPermission = "FILE_READ_DATA";
        public const string WritePermission = "FILE_WRITE_DATA";
        public const string ExecutePermission = "FILE_EXECUTE";
        public const string GenericAllPermission = "GENERIC_ALL";
        public const string WriteOwnerPermission = "WRITE_OWNER";
        public const string AdministratorPermissionFlag = "100000000";
        public const string WriteOwnerPermissionFlag = "00080000";


        public static Dictionary<string, string> ValidPermissions
            = new Dictionary<string, string>()
        {
            {ReadPermission, "00000001"},
            {WritePermission, "00000002"},
            {ExecutePermission, "00000020"},
            {GenericAllPermission, "10000000"},
            {WriteOwnerPermission, "00080000"}
        };

        public const string AdministratorUserLastSidPartValue = "500";
        public const string AdministratorsGroupLastSidPartValue = "544";

        public static HashSet<string> AdministratorSidVales = new HashSet<string>()
        {
            AdministratorUserLastSidPartValue,
            AdministratorsGroupLastSidPartValue
        };

        //
        public const string ReplaceMacros = "replace_macros";
        public const string ReplaceFileNameMacros = "replace_file_name";
        public const string FileSecuritySettingPath =
            "Win32_logicalFileSecuritySetting.Path='" + ReplaceMacros + "'";

        //Message  constants
        public const string AbsentRightError = "User with sid: '" + ReplaceMacros +
                            "' don't have need right on file '" + ReplaceFileNameMacros +
                            "' for graph.";
        public const string NoSuchUserError = "there is no such user(s) " + ReplaceMacros
            + " in security property of " + ReplaceFileNameMacros + ".";
        public const string FillDirectoryPathError = "Fill directory path.";
        public const string SelectUserError = "Select user(s).";
        public const string SelectFileError = "Select file(s).";
        public const string SelectUserAndFileError = "Select file(s) and user(s).";

        //Property constants
        public const string ReturnValueProperty = "ReturnValue";
        public const string GetSecurityDescriptorProperty = "GetSecurityDescriptor";
        public const string DescriptorProperty = "Descriptor";
        public const string DaclProperty = "Dacl";
        public const string TrusteeProperty = "Trustee";
        public const string NameProperty = "Name";
        public const string FullNameProperty = "FullName";
        public const string SIDProperty = "SID";
        public const string SIDStringProperty = "SIDString";
        public const string AccessMaskProperty = "AccessMask";
        public const string AceTypeProperty = "AceType";
        public const string DomainProperty = "Domain";
        public const string DescriptionProperty = "Description";
        public const string GroupComponentProperty = "GroupComponent";

        public const string ReplaceDomain = "replace_domain";
        public const string ReplaceuserName = "replace_user_name";
        public const string Win32GroupWMI = "Win32_Group";
        public const string Win32UserAccountWMI = "Win32_UserAccount";
        public const string ManagementObjectSearcherQuery =
            "select * from Win32_GroupUser where PartComponent=\"Win32_UserAccount.Domain='"
            + ReplaceDomain + "',Name='" + ReplaceuserName + "'\"";
    }
}
