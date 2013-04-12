using System;

namespace Mercurial.Gui
{
    /// <summary>
    /// This attribute can be applied to classes descending from <see cref="GuiCommandBase{T}"/>
    /// to specify the requirements for the installed client for the command to be available.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class GuiClientRequirementAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the type of client that needs to be installed for the command to be available.
        /// </summary>
        public GuiClientType ClientType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether the requirements specified in this <see cref="GuiClientRequirementAttribute"/> is met by the
        /// installed client.
        /// </summary>
        public bool AreRequirementsMet
        {
            get
            {
                return GuiClient.ClientType == ClientType;
            }
        }
    }
}