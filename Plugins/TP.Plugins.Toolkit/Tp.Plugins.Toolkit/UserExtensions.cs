using System.Linq;
using Tp.Core;

// ReSharper disable once CheckNamespace

namespace Tp.Integration.Common
{
    public static class UserExtensions
    {
        public static bool IsNonDeletedUser(this UserDTO user)
        {
            return !user.DeleteDate.HasValue;
        }

        public static string FullName(this IGeneralUserDTO user)
        {
            if (user == null)
                return null;

            var result = new[] { user.FirstName, user.LastName }.ToString(" ").Trim();

            return result.IsNullOrEmpty() ? user.Email : result;
        }
    }

    public static class EntityTypeExtensions
    {
        public static string HumanFriendlyType(this string entityTypeName)
        {
            return entityTypeName.ShortenEntityType().Humanize();
        }

        public static string ShortenEntityType(this string entityTypeName)
        {
            return entityTypeName.Replace("Tp.BusinessObjects.", "");
        }
    }
}
