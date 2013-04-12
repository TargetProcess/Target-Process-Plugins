using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Mercurial.Attributes;

namespace Mercurial
{
    /// <summary>
    /// This class implements the "hg unbundle" command (<see href="http://www.selenic.com/mercurial/hg.1.html#unbundle"/>):
    /// apply one or more changegroup files.
    /// </summary>
    public sealed class UnbundleCommand : MercurialCommandBase<UnbundleCommand>
    {
        /// <summary>
        /// This is the backing field for the <see cref="FileNames"/> property.
        /// </summary>
        private readonly List<string> _FileNames = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="UnbundleCommand"/> class.
        /// </summary>
        public UnbundleCommand()
            : base("unbundle")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets the collection of filenames of bundle files to apply, in the order to apply them.
        /// Default is empty.
        /// </summary>
        [RepeatableArgument]
        public Collection<string> FileNames
        {
            get
            {
                return new Collection<string>(_FileNames);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to update to new branch head if changesets were unbundled.
        /// Default is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--update")]
        [DefaultValue(false)]
        public bool Update
        {
            get;
            set;
        }

        /// <summary>
        /// Adds the value to the <see cref="FileNames"/> collection property and
        /// returns this <see cref="UnbundleCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The value to add to the <see cref="FileNames"/> collection property.
        /// </param>
        /// <returns>
        /// This <see cref="UnbundleCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="value"/> is <c>null</c> or empty.</para>
        /// </exception>
        public UnbundleCommand WithFileName(string value)
        {
            if (StringEx.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException("value");

            FileNames.Add(value);
            return this;
        }

        /// <summary>
        /// Sets the <see cref="Update"/> property to the specified value and
        /// returns this <see cref="UnbundleCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Update"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="UnbundleCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public UnbundleCommand WithUpdate(bool value = true)
        {
            Update = value;
            return this;
        }

        /// <summary>
        /// Validates the command configuration. This method should throw the necessary
        /// exceptions to signal missing or incorrect configuration (like attempting to
        /// add files to the repository without specifying which files to add.)
        /// </summary>
        /// <remarks>
        /// Note that as long as you descend from <see cref="MercurialCommandBase{T}"/> you're not required to call
        /// the base method at all.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// <see cref="FileNames"/> cannot be an empty collection.
        /// </exception>
        public override void Validate()
        {
            base.Validate();

            if (FileNames.Count == 0)
                throw new InvalidOperationException("UnbundleCommand.FileNames cannot be an empty collection");
        }

        /// <summary>
        /// This method should throw the appropriate exception depending on the contents of
        /// the <paramref name="exitCode"/> and <paramref name="standardErrorOutput"/>
        /// parameters, or simply return if the execution is considered successful.
        /// </summary>
        /// <param name="exitCode">
        /// The exit code from executing the command line client.
        /// </param>
        /// <param name="standardErrorOutput">
        /// The standard error output from executing the command client.
        /// </param>
        /// <remarks>
        /// Note that as long as you descend from <see cref="MercurialCommandBase{T}"/> you're not required to call
        /// the base method at all. The default behavior is to throw a <see cref="MercurialExecutionException"/>
        /// if <paramref name="exitCode"/> is not zero. If you require different behavior, don't call the base
        /// method.
        /// </remarks>
        /// <exception cref="MercurialExecutionException">
        /// <para><paramref name="exitCode"/> is not <c>0</c>.</para>
        /// </exception>
        /// <exception cref="UnresolvedFilesAfterUnbundleMercurialExecutionException">
        /// One or more files was left in an unresolved state after merging during an 'unbundle', these
        /// must be resolved manually.
        /// </exception>
        protected override void ThrowOnUnsuccessfulExecution(int exitCode, string standardErrorOutput)
        {
            switch (exitCode)
            {
                case 0:
                    break;

                case 1:
                    throw new UnresolvedFilesAfterUnbundleMercurialExecutionException(exitCode, standardErrorOutput);

                default:
                    base.ThrowOnUnsuccessfulExecution(exitCode, standardErrorOutput);
                    break;
            }
        }
    }
}