using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Mercurial.Attributes;
using Mercurial.Versions;

namespace Mercurial
{
    /// <summary>
    /// This class implements the "hg backout" command (<see href="http://www.selenic.com/mercurial/hg.1.html#backout"/>):
    /// reverse effect of earlier changeset.
    /// </summary>
    public sealed class BackoutCommand : IncludeExcludeCommandBase<BackoutCommand>
    {
        /// <summary>
        /// This field is used internally to temporarily hold the filename of the file
        /// that the <see cref="Message"/> was stored into, during command execution.
        /// </summary>
        private string _MessageFilePath;

        /// <summary>
        /// This is the backing field for the <see cref="MergeTool"/> property.
        /// </summary>
        private string _MergeTool = string.Empty;

        /// <summary>
        /// This is the backing field for the <see cref="Message"/> property.
        /// </summary>
        private string _Message = string.Empty;

        /// <summary>
        /// This is the backing field for the <see cref="OverrideAuthor"/> property.
        /// </summary>
        private string _OverrideAuthor = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackoutCommand"/> class.
        /// </summary>
        public BackoutCommand()
            : base("backout")
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether to merge with the old dirstate parent after backout.
        /// Default value is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--merge")]
        [DefaultValue(false)]
        public bool Merge
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the merge tool to use.
        /// Default value is <see cref="string.Empty"/>, which means the default merge tool is to
        /// be used.
        /// </summary>
        /// <remarks>
        /// This property requires Mercurial 1.7 or newer.
        /// </remarks>
        [DefaultValue("")]
        public string MergeTool
        {
            get
            {
                return _MergeTool;
            }

            set
            {
                RequiresVersion(new Version(1, 7, 0), "MergeTool property of the BackoutCommand class");
                _MergeTool = (value ?? string.Empty).Trim();
            }
        }

        /// <summary>
        /// Gets or sets the parent <see cref="RevSpec"/> to choose when backing out merge.
        /// Default value is <c>null</c>.
        /// </summary>
        [NullableArgument(NonNullOption = "--parent")]
        [DefaultValue(null)]
        public RevSpec ParentRevision
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="RevSpec"/> to backout.
        /// Default value is <c>null</c>.
        /// </summary>
        [NullableArgument]
        [DefaultValue(null)]
        public RevSpec Revision
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the commit message to use when committing.
        /// </summary>
        [DefaultValue("")]
        public string Message
        {
            get
            {
                return _Message;
            }

            set
            {
                _Message = (value ?? string.Empty).Trim();
            }
        }

        /// <summary>
        /// Gets or sets the username to use when committing;
        /// or <see cref="string.Empty"/> to use the username configured in the repository or by
        /// the current user. Default is <see cref="string.Empty"/>.
        /// </summary>
        [NullableArgument(NonNullOption = "--user")]
        [DefaultValue("")]
        public string OverrideAuthor
        {
            get
            {
                return _OverrideAuthor;
            }

            set
            {
                _OverrideAuthor = (value ?? string.Empty).Trim();
            }
        }

        /// <summary>
        /// Gets or sets the timestamp <see cref="DateTime"/> to use when committing;
        /// or <c>null</c> which means use the current date and time. Default is <c>null</c>.
        /// </summary>
        [DateTimeArgument(NonNullOption = "--date")]
        [DefaultValue(null)]
        public DateTime? OverrideTimestamp
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the <see cref="MergeTool"/> property to the specified value and
        /// returns this <see cref="BackoutCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="MergeTool"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="BackoutCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public BackoutCommand WithMergeTool(string value)
        {
            MergeTool = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="Merge"/> property to the specified value and
        /// returns this <see cref="BackoutCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Merge"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="BackoutCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public BackoutCommand WithMerge(bool value)
        {
            Merge = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="ParentRevision"/> property to the specified value and
        /// returns this <see cref="BackoutCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="ParentRevision"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="BackoutCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public BackoutCommand WithParentRevision(RevSpec value)
        {
            ParentRevision = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="Revision"/> property to the specified value and
        /// returns this <see cref="BackoutCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Revision"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="BackoutCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public BackoutCommand WithRevision(RevSpec value)
        {
            Revision = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="Message"/> property to the specified value and
        /// returns this <see cref="BackoutCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Message"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="BackoutCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public BackoutCommand WithMessage(string value)
        {
            Message = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="OverrideAuthor"/> property to the specified value and
        /// returns this <see cref="BackoutCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="OverrideAuthor"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="BackoutCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public BackoutCommand WithOverrideAuthor(string value)
        {
            OverrideAuthor = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="OverrideTimestamp"/> property to the specified value and
        /// returns this <see cref="BackoutCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="OverrideTimestamp"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="BackoutCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public BackoutCommand WithOverrideTimestamp(DateTime value)
        {
            OverrideTimestamp = value;
            return this;
        }

        /// <summary>
        /// Override this method to implement code that will execute before command
        /// line execution.
        /// </summary>
        protected override void Prepare()
        {
            _MessageFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString().Replace("-", string.Empty).ToLowerInvariant() + ".txt");
            File.WriteAllText(_MessageFilePath, Message);
        }

        /// <summary>
        /// Override this method to implement code that will execute after command
        /// line execution.
        /// </summary>
        protected override void Cleanup()
        {
            if (_MessageFilePath != null && File.Exists(_MessageFilePath))
                File.Delete(_MessageFilePath);
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
                foreach (string argument in GetBaseArguments())
                    yield return argument;

                if (!StringEx.IsNullOrWhiteSpace(Message))
                {
                    yield return "--logfile";
                    yield return StringEx.EncapsulateInQuotesIfWhitespace(_MessageFilePath);
                }

                foreach (string argument in MercurialVersionBase.Current.MergeToolOption(MergeTool))
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
    }
}