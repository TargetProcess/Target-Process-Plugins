using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mercurial.Attributes;

namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "email" command:
    /// Send changesets by email.
    /// </summary>
    /// <remarks>
    /// This command is only available for the <see cref="GuiClientType.PyQT"/> client type.
    /// </remarks>
    [GuiClientRequirement(ClientType = GuiClientType.PyQT)]
    public sealed class EmailGuiCommand : GuiCommandBase<EmailGuiCommand>
    {
        /// <summary>
        /// This is the backing field for the <see cref="Revisions"/> property.
        /// </summary>
        private readonly Collection<RevSpec> _Revisions = new Collection<RevSpec>(new List<RevSpec>());

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailGuiCommand"/> class.
        /// </summary>
        public EmailGuiCommand()
            : base("email")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets the collection of revisions to send by email.
        /// </summary>
        [RepeatableArgument]
        public Collection<RevSpec> Revisions
        {
            get
            {
                return _Revisions;
            }
        }

        /// <summary>
        /// Adds all the values to the <see cref="Revisions"/> collection property and
        /// returns this <see cref="EmailGuiCommand"/> instance.
        /// </summary>
        /// <param name="values">
        /// An array of values to add to the <see cref="Revisions"/> collection property.
        /// </param>
        /// <returns>
        /// This <see cref="EmailGuiCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public EmailGuiCommand WithRevisions(params RevSpec[] values)
        {
            if (values != null)
                foreach (string value in values)
                    Revisions.Add(value);

            return this;
        }
    }
}