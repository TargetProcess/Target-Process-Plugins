using System.ComponentModel;
using Mercurial.Attributes;

namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "search" command:
    /// Grep/search dialog.
    /// </summary>
    /// <remarks>
    /// This command is only available for the <see cref="GuiClientType.PyQT"/> client type.
    /// </remarks>
    [GuiClientRequirement(ClientType = GuiClientType.PyQT)]
    public sealed class SearchGuiCommand : GuiCommandBase<SearchGuiCommand>
    {
        /// <summary>
        /// This is the backing field for the <see cref="CaseSensitivity"/> property.
        /// </summary>
        private bool _CaseSensitivity = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchGuiCommand"/> class.
        /// </summary>
        public SearchGuiCommand()
            : base("search")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use case sensitive search.
        /// Default value is <c>true</c>.
        /// </summary>
        [BooleanArgument(FalseOption = "--ignorecase")]
        [DefaultValue(true)]
        public bool CaseSensitivity
        {
            get
            {
                return _CaseSensitivity;
            }

            set
            {
                _CaseSensitivity = value;
            }
        }

        /// <summary>
        /// Sets the <see cref="CaseSensitivity"/> property to the specified value and
        /// returns this <see cref="SearchGuiCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="CaseSensitivity"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="SearchGuiCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public SearchGuiCommand WithCaseSensitivity(bool value)
        {
            CaseSensitivity = value;
            return this;
        }
    }
}