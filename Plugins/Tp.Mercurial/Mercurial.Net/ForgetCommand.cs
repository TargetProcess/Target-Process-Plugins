using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Mercurial
{
    /// <summary>
    /// This class implements the "hg forget" command (<see href="http://www.selenic.com/mercurial/hg.1.html#forget"/>):
    /// forget the specified files on the next commit.
    /// </summary>
    public sealed class ForgetCommand : IncludeExcludeCommandBase<ForgetCommand>
    {
        /// <summary>
        /// This is the backing field for the <see cref="Paths"/> property.
        /// </summary>
        private readonly ListFile _Paths = new ListFile();

        /// <summary>
        /// Initializes a new instance of the <see cref="ForgetCommand"/> class.
        /// </summary>
        public ForgetCommand()
            : base("forget")
        {
        }

        /// <summary>
        /// Gets the collection of path patterns to add to the repository.
        /// </summary>
        public Collection<string> Paths
        {
            get
            {
                return _Paths.Collection;
            }
        }

        /// <summary>
        /// Adds the value to the <see cref="Paths"/> collection property and
        /// returns this <see cref="ForgetCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The value to add to the <see cref="Paths"/> collection property.
        /// </param>
        /// <returns>
        /// This <see cref="ForgetCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="value"/> is <c>null</c> or empty.</para>
        /// </exception>
        public ForgetCommand WithPath(string value)
        {
            if (StringEx.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException("value");

            Paths.Add(value.Trim());
            return this;
        }

        /// <summary>
        /// Validates the command configuration. This method should throw the necessary
        /// exceptions to signal missing or incorrect configuration (like attempting to
        /// add files to the repository without specifying which files to add.)
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The 'forget' command requires at least one path specified.
        /// </exception>
        public override void Validate()
        {
            base.Validate();

            if (Paths.Count == 0)
                throw new InvalidOperationException("The 'forget' command requires at least one path specified");
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