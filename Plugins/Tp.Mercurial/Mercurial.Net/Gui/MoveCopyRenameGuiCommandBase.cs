using System;
using System.ComponentModel;

namespace Mercurial.Gui
{
    /// <summary>
    /// This is the base class for <see cref="MoveGuiCommand"/>, <see cref="CopyGuiCommand"/> and <see cref="RenameGuiCommand"/>:
    /// </summary>
    /// <typeparam name="T">
    /// The actual type descending from <see cref="MoveCopyRenameGuiCommandBase{T}"/>, used to generate type-correct
    /// methods in this base class.
    /// </typeparam>
    public abstract class MoveCopyRenameGuiCommandBase<T> : GuiCommandBase<T>
        where T : MoveCopyRenameGuiCommandBase<T>
    {
        /// <summary>
        /// This is the backing field for the <see cref="Source"/> property.
        /// </summary>
        private string _Source = string.Empty;

        /// <summary>
        /// This is the backing field for the <see cref="Destination"/> property.
        /// </summary>
        private string _Destination = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveCopyRenameGuiCommandBase{T}"/> class.
        /// </summary>
        /// <param name="command">
        /// The name of the command that will be passed to the TortoiseHg command line client.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="command"/> is <c>null</c> or empty.</para>
        /// </exception>
        protected MoveCopyRenameGuiCommandBase(string command)
            : base(command)
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets or sets the source file to rename, copy, or move.
        /// Default is <see cref="string.Empty"/>.
        /// </summary>
        [DefaultValue("")]
        public string Source
        {
            get
            {
                return _Source;
            }

            set
            {
                _Source = (value ?? string.Empty).Trim();
            }
        }

        /// <summary>
        /// Gets or sets the destination of the rename, copy, or move.
        /// Default is <see cref="string.Empty"/>.
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
                _Destination = (value ?? string.Empty).Trim();
            }
        }

        /// <summary>
        /// Sets the <see cref="Source"/> property to the specified value and
        /// returns this command instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Source"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This command instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public T WithSource(string value)
        {
            Source = value;
            return (T)this;
        }

        /// <summary>
        /// Sets the <see cref="Destination"/> property to the specified value and
        /// returns this command instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Destination"/> property,
        /// defaults to <c>true</c>.
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
    }
}