using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Mercurial.Attributes;

namespace Mercurial
{
    /// <summary>
    /// This class implements the "hg add" command (<see href="http://www.selenic.com/mercurial/hg.1.html#add"/>):
    /// add the specified files on the next commit.
    /// </summary>
    public sealed class AddCommand : IncludeExcludeCommandBase<AddCommand>
    {
        /// <summary>
        /// This is the backing field for the <see cref="Paths"/> property.
        /// </summary>
        private readonly ListFile _Files = new ListFile();

        /// <summary>
        /// This is the backing field for the <see cref="RecurseSubRepositories"/> property.
        /// </summary>
        private bool _RecurseSubRepositories;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddCommand"/> class.
        /// </summary>
        /// <remarks>
        /// See <see cref="AddCommand"/> for an example.
        /// </remarks>
        public AddCommand()
            : base("add")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets the collection of path patterns to add to the repository.
        /// </summary>
        public Collection<string> Paths
        {
            get
            {
                return _Files.Collection;
            }
        }

        /// <summary>
        /// Adds all the values to the <see cref="Paths"/> collection property and
        /// returns this <see cref="AddCommand"/> instance.
        /// </summary>
        /// <param name="values">
        /// An array of values to add to the <see cref="Paths"/> collection property.
        /// </param>
        /// <returns>
        /// This <see cref="AddCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public AddCommand WithPaths(params string[] values)
        {
            if (values != null)
                foreach (string value in values)
                    Paths.Add(value);

            return this;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to recurse into subrepositories.
        /// Default is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// Note that setting this property requires Mercurial 1.6.2 or newer.
        /// </remarks>
        [BooleanArgument(TrueOption = "--subrepos")]
        [DefaultValue(false)]
        public bool RecurseSubRepositories
        {
            get
            {
                return _RecurseSubRepositories;
            }

            set
            {
                RequiresVersion(new Version(1, 6, 2), "The RecurseSubRepositories property of the AddCommand");
                _RecurseSubRepositories = value;
            }
        }

        /// <summary>
        /// Sets the <see cref="RecurseSubRepositories"/> property to the specified value and
        /// returns this <see cref="AddCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="RecurseSubRepositories"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="AddCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        /// <remarks>
        /// Note that calling this method requires Mercurial 1.6.2 or newer.
        /// </remarks>
        public AddCommand WithRecurseSubRepositories(bool value = true)
        {
            RecurseSubRepositories = value;
            return this;
        }

        /// <summary>
        /// Gets all the arguments to the <see cref="CommandBase{T}.Command"/>, or an
        /// empty array if there are none.
        /// </summary>
        public override IEnumerable<string> Arguments
        {
            get
            {
                return base.Arguments.Concat(_Files.GetArguments());
            }
        }

        /// <summary>
        /// Validates the command configuration. This method should throw the necessary
        /// exceptions to signal missing or incorrect configuration (like attempting to
        /// add files to the repository without specifying which files to add.)
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The 'add' command requires at least one path specified.
        /// </exception>
        public override void Validate()
        {
            base.Validate();

            if (Paths.Count == 0)
                throw new InvalidOperationException("The 'add' command requires at least one path specified");
        }

        /// <summary>
        /// Override this method to implement code that will execute after command
        /// line execution.
        /// </summary>
        protected override void Cleanup()
        {
            base.Cleanup();
            _Files.Cleanup();
        }
    }
}