using System.Collections.Generic;
using Tp.Integration.Common;

namespace Tp.SourceControl
{
    public class TpUserData : DataTransferObject
    {
        public static readonly HashSet<UserField> RequiredFields = new HashSet<UserField>
        {
            UserField.FirstName, UserField.LastName, UserField.Login, UserField.Email, UserField.ActiveDirectoryName
        };

        public TpUserData(UserDTO user)
        {
            ID = user.ID;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Login = user.Login;
            Email = user.Email;
            ActiveDirectoryName = user.ActiveDirectoryName;
        }

        public TpUserData()
        {            
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public string ActiveDirectoryName { get; set; }
    }
}