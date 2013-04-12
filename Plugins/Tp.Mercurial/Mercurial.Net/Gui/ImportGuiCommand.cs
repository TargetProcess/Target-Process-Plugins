using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mercurial.Attributes;

namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "import" command:
    /// Import patches to repository/patch queue.
    /// </summary>
    public sealed class ImportGuiCommand : GuiCommandBase<ImportGuiCommand>
    {
        /// <summary>
        /// This is the backing field for the <see cref="Sources"/> property.
        /// </summary>
        private readonly List<string> _Sources = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportGuiCommand"/> class.
        /// </summary>
        public ImportGuiCommand()
            : base("import")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets the collection of source patches to import into the repository
        /// or patch queue.
        /// </summary>
        [RepeatableArgument]
        public Collection<string> Sources
        {
            get
            {
                return new Collection<string>(_Sources);
            }
        }

        /// <summary>
        /// Adds the value to the <see cref="Sources"/> property and
        /// returns this <see cref="ImportGuiCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The value to add to the <see cref="Sources"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="ImportGuiCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public ImportGuiCommand WithSource(string value)
        {
            Sources.Add(value);
            return this;
        }
    }
}