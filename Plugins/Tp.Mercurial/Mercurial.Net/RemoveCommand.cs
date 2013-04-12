using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Mercurial.Attributes;

namespace Mercurial
{
    /// <summary>
    /// This class implements the "hg remove" command (<see href="http://www.selenic.com/mercurial/hg.1.html#remove"/>):
    /// remove the specified files on the next commit.
    /// </summary>
    public sealed class RemoveCommand : IncludeExcludeCommandBase<RemoveCommand>
    {
        /// <summary>
        /// This is the backing field for the <see cref="Paths"/> property.
        /// </summary>
        private readonly ListFile _Paths = new ListFile();

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveCommand"/> class.
        /// </summary>
        public RemoveCommand()
            : base("remove")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets the collection of path patterns to remove from the repository.
        /// </summary>
        public Collection<string> Paths
        {
            get
            {
                return _Paths.Collection;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to remove (and delete) file even if
        /// current status is added or modified.
        /// Default is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--force")]
        [DefaultValue(false)]
        public bool ForceRemoval
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to record deletes for missing files after
        /// the files has been deleted.
        /// Default is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--after")]
        [DefaultValue(false)]
        public bool RecordDeletes
        {
            get;
            set;
        }

        /// <summary>
        /// Adds the value to the <see cref="Paths"/> collection property and
        /// returns this <see cref="RemoveCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The value to add to the <see cref="Paths"/> collection property.
        /// </param>
        /// <returns>
        /// This <see cref="RemoveCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public RemoveCommand WithPath(string value)
        {
            Paths.Add(value);
            return this;
        }

        /// <summary>
        /// Sets the <see cref="ForceRemoval"/> property to the specified value and
        /// returns this <see cref="RemoveCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="ForceRemoval"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="RemoveCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public RemoveCommand WithForceRemoval(bool value = true)
        {
            ForceRemoval = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="RecordDeletes"/> property to the specified value and
        /// returns this <see cref="RemoveCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="RecordDeletes"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="RemoveCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public RemoveCommand WithRecordDeletes(bool value = true)
        {
            RecordDeletes = value;
            return this;
        }

        /// <summary>
        /// Validates the command configuration. This method should throw the necessary
        /// exceptions to signal missing or incorrect configuration (like attempting to
        /// add files to the repository without specifying which files to add.)
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The 'remove' command requires at least one path specified in <see cref="Paths"/>, unless <see cref="RecordDeletes"/> is true.
        /// </exception>
        public override void Validate()
        {
            base.Validate();

            if (!RecordDeletes && Paths.Count == 0)
                throw new InvalidOperationException("The 'remove' command requires at least one path specified, unless RecordDeletes is true");
        }

        /// <summary>
        /// Gets all the arguments to the <see cref="CommandBase{T}.Command"/>, or an
        /// empty array if there are none.
        /// </summary>
        public override IEnumerable<string> Arguments
        {
            get
            {
                return base.Arguments.Concat(_Paths.GetArguments());
            }
        }

        /// <summary>
        /// Override this method to implement code that will execute after command
        /// line execution.
        /// </summary>
        protected override void Cleanup()
        {
            base.Cleanup();
            _Paths.Cleanup();
        }
    }
}