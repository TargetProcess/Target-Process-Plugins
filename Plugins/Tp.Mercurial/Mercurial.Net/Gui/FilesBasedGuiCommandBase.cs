using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Mercurial.Gui.Clients;

namespace Mercurial.Gui
{
    /// <summary>
    /// This is the base class for option classes for various commands for
    /// the TortoiseHg client, that have a Files collection property
    /// </summary>
    /// <typeparam name="T">
    /// The actual type descending from <see cref="GuiCommandBase{T}"/>, used to generate type-correct
    /// methods in this base class.
    /// </typeparam>
    public abstract class FilesBasedGuiCommandBase<T> : GuiCommandBase<T>
        where T : FilesBasedGuiCommandBase<T>
    {
        /// <summary>
        /// This is the backing field for the <see cref="Files"/> property.
        /// </summary>
        private readonly Collection<string> _Files = new Collection<string>(new List<string>());

        /// <summary>
        /// This field holds the full path to and name of the temporary file where the list of filenames have been put.
        /// </summary>
        private string _FilesListTempFile = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesBasedGuiCommandBase{T}"/> class.
        /// </summary>
        /// <param name="command">
        /// The name of the command that will be passed to the TortoiseHg command line client.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="command"/> is <c>null</c> or empty.</para>
        /// </exception>
        protected FilesBasedGuiCommandBase(string command)
            : base(command)
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets the collection of files to pass to the command.
        /// </summary>
        public Collection<string> Files
        {
            get
            {
                return _Files;
            }
        }

        /// <summary>
        /// Adds all the values to the <see cref="Files"/> collection property and
        /// returns this command instance.
        /// </summary>
        /// <param name="values">
        /// An array of values to add to the <see cref="Files"/> collection property.
        /// </param>
        /// <returns>
        /// This command instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public T WithFiles(params string[] values)
        {
            if (values != null)
                foreach (string value in values)
                    Files.Add(value);

            return (T)this;
        }

        /// <summary>
        /// Gets all the arguments to the <see cref="CommandBase{T}.Command"/>, or an
        /// empty array if there are none.
        /// </summary>
        /// <remarks>
        /// Note that as long as you descend from <see cref="MercurialCommandBase{T}"/> you're not required to access
        /// the base property at all, but you are required to return a non-<c>null</c> array reference,
        /// even for an empty array.
        /// </remarks>
        public override IEnumerable<string> Arguments
        {
            get
            {
                foreach (string argument in GetBaseArguments())
                    yield return argument;

                foreach (string argument in TortoiseHgClient.Current.GetFileListArguments(Files, out _FilesListTempFile))
                    yield return argument;
            }
        }

        /// <summary>
        /// Gets the base arguments.
        /// </summary>
        /// <returns>
        /// The contents of the base arguments property, to avoide unverifiable code in <see cref="Arguments"/>.
        /// </returns>
        private IEnumerable<string> GetBaseArguments()
        {
            return base.Arguments;
        }

        /// <summary>
        /// Override this method to implement code that will execute after command
        /// line execution.
        /// </summary>
        protected override void Cleanup()
        {
            base.Cleanup();
            if (!StringEx.IsNullOrWhiteSpace(_FilesListTempFile) && File.Exists(_FilesListTempFile))
                File.Delete(_FilesListTempFile);
        }
    }
}