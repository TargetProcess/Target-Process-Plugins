using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Mercurial.Attributes;

namespace Mercurial
{
    /// <summary>
    /// This class implements the "hg copy" command (<see href="http://www.selenic.com/mercurial/hg.1.html#copy"/>):
    /// mark files as copied for the next commit.
    /// </summary>
    public sealed class CopyCommand : IncludeExcludeCommandBase<CopyCommand>
    {
        /// <summary>
        /// This is the backing field for the <see cref="Destination"/> property.
        /// </summary>
        private string _Destination = string.Empty;

        /// <summary>
        /// This is the backing field for the <see cref="Source"/> property.
        /// </summary>
        private string _Source = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="CopyCommand"/> class.
        /// </summary>
        public CopyCommand()
            : base("copy")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets or sets the source of the copy command, which file(s) to copy.
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
        /// Gets or sets the destination of the copy command, where to copy the files from <see cref="Source"/>.
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
        /// Gets or sets a value indicating whether to record a copy that has already occured.
        /// Default value is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "-A")]
        [DefaultValue(false)]
        public bool RecordCopiesAfterTheFact
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to force overwriting an existing managed file.
        /// Default value is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "-f")]
        [DefaultValue(false)]
        public bool ForceOverwrite
        {
            get;
            set;
        }

        /// <summary>
        /// Gets all the arguments to the <see cref="CommandBase{T}.Command"/>, or an
        /// empty array if there are none.
        /// </summary>
        /// <value></value>
        public override IEnumerable<string> Arguments
        {
            get
            {
                return base.Arguments.Concat(
                    new[]
                    {
                        "\"" + Source + "\"", "\"" + Destination + "\"",
                    });
            }
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
        /// The 'copy' command requires Destination to be specified.
        /// </exception>
        public override void Validate()
        {
            base.Validate();

            if (StringEx.IsNullOrWhiteSpace(Destination))
                throw new InvalidOperationException("The 'copy' command requires Destination to be specified");
        }
    }
}