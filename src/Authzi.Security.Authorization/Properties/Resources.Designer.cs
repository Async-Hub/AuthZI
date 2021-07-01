// <auto-generated />

using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Authzi.Security.Authorization.Properties
{
    internal static class Resources
    {
        private static readonly ResourceManager _resourceManager
            = new ResourceManager("Authzi.Security.Authorization.Resources", typeof(Resources).GetTypeInfo().Assembly);

        /// <summary>
        /// AuthorizationPolicy must have at least one requirement.
        /// </summary>
        internal static string Exception_AuthorizationPolicyEmpty
        {
            get => GetString("Exception_AuthorizationPolicyEmpty");
        }

        /// <summary>
        /// AuthorizationPolicy must have at least one requirement.
        /// </summary>
        internal static string FormatException_AuthorizationPolicyEmpty()
            => GetString("Exception_AuthorizationPolicyEmpty");

        /// <summary>
        /// The AuthorizationPolicy named: '{0}' was not found.
        /// </summary>
        internal static string Exception_AuthorizationPolicyNotFound
        {
            get => GetString("Exception_AuthorizationPolicyNotFound");
        }

        /// <summary>
        /// The AuthorizationPolicy named: '{0}' was not found.
        /// </summary>
        internal static string FormatException_AuthorizationPolicyNotFound(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("Exception_AuthorizationPolicyNotFound"), p0);

        /// <summary>
        /// At least one role must be specified.
        /// </summary>
        internal static string Exception_RoleRequirementEmpty
        {
            get => GetString("Exception_RoleRequirementEmpty");
        }

        /// <summary>
        /// At least one role must be specified.
        /// </summary>
        internal static string FormatException_RoleRequirementEmpty()
            => GetString("Exception_RoleRequirementEmpty");

        private static string GetString(string name, params string[] formatterNames)
        {
            var value = _resourceManager.GetString(name);

            System.Diagnostics.Debug.Assert(value != null);

            if (formatterNames != null)
            {
                for (var i = 0; i < formatterNames.Length; i++)
                {
                    value = value.Replace("{" + formatterNames[i] + "}", "{" + i + "}");
                }
            }

            return value;
        }
    }
}
