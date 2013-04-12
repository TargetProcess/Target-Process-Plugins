using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Mercurial.Gui
{
    /// <summary>
    /// This is the base class for <see cref="DragCopyGuiCommand"/> and <see cref="DragMoveGuiCommand"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The actual type descending from <see cref="DragCopyMoveGuiCommandBase{T}"/>, used to generate type-correct
    /// methods in this base class.
    /// </typeparam>
    public abstract class DragCopyMoveGuiCommandBase<T> : GuiCommandBase<T>
        where T : DragCopyMoveGuiCommandBase<T>
    {
        /// <summary>
        /// This is the backing field for the <see cref="SourceFiles"/> property.
        /// </summary>
        private readonly Collection<string> _SourceFiles = new Collection<string>(new List<string>());

        /// <summary>
        /// This is the backing field for the <see cref="Destination"/> property.
        /// </summary>
        private string _Destination = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="DragCopyMoveGuiCommandBase{T}"/> class.
        /// </summary>
        /// <param name="command">
        /// The name of the command that will be passed to the TortoiseHg command line client.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="command"/> is <c>null</c> or empty.</para>
        /// </exception>
        protected DragCopyMoveGuiCommandBase(string command)
            : base(command)
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets the collection of files to copy or move.
        /// </summary>
        public Collection<string> SourceFiles
        {
            get
            {
                return _SourceFiles;
            }
        }

        /// <summary>
        /// Gets or sets the destination directory to copy or move the files to.
        /// Default value is <see cref="string.Empty"/>.
        /// </summary>
        [DefaultValue("")]
        public string Destination
        {
            get
            {
                return _Destination;
            }

            set
            {
                _Destination = value;
            }
        }

        /// <summary>
        /// Adds all the values to the <see cref="SourceFiles"/> collection property and
        /// returns this command instance.
        /// </summary>
        /// <param name="values">
        /// An array of values to add to the <see cref="SourceFiles"/> collection property.
        /// </param>
        /// <returns>
        /// This command instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public T WithSourceFiles(params string[] values)
        {
            if (values != null)
                foreach (string value in values)
                    SourceFiles.Add(value);

            return (T)this;
        }

        /// <summary>
        /// Sets the <see cref="Destination"/> property to the specified value and
        /// returns this command instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Destination"/> property.
        /// </param>
        /// <returns>
        /// This command instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public T WithDestination(string value)
        {
            Destination = value;
            return (T)this;
        }

        /// <summary>
        /// Validates the command configuration. This method should throw the necessary
        /// exceptions to signal missing or incorrect configuration (like attempting to
        /// add files to the repository without specifying which files to add.)
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// <para>At least one file in <see cref="SourceFiles"/> has to be specified.</para>
        /// <para>- or -</para>
        /// <para><see cref="Destination"/> has to be specified.</para>
        /// </exception>
        public override void Validate()
        {
            base.Validate();

            if (SourceFiles.Count == 0)
                throw new InvalidOperationException("At least one source file has to be specified for " + typeof(T).Name);
            if (StringEx.IsNullOrWhiteSpace(Destination))
                throw new InvalidOperationException("The destination has to be specified for " + typeof(T).Name);
        }
    }
}