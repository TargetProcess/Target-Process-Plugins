using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Mercurial.Attributes;

namespace Mercurial.Extensions.Churn
{
    /// <summary>
    /// This class implements the "hg churn" extension command (<see href="http://www.selenic.com/mercurial/hg.1.html#churn"/>):
    /// calculate histogram of changes to the repository.
    /// </summary>
    public sealed class ChurnCommand : IncludeExcludeCommandBase<ChurnCommand>, IMercurialCommand<IEnumerable<ChurnGroup>>
    {
        /// <summary>
        /// This is the backing field for the <see cref="Revisions"/> property.
        /// </summary>
        private readonly List<RevSpec> _Revisions = new List<RevSpec>();

        /// <summary>
        /// This is the backing field for the <see cref="GroupTemplate"/> property.
        /// </summary>
        private string _GroupTemplate = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChurnCommand"/> class.
        /// </summary>
        public ChurnCommand()
            : base("churn")
        {
            Timeout = 600;
        }

        /// <summary>
        /// Gets or sets the date to show the log for, or <c>null</c> if no filtering on date should be done.
        /// Default is <c>null</c>.
        /// </summary>
        [DateTimeArgument(NonNullOption = "--date", Format = "yyyy-MM-dd")]
        [DefaultValue(null)]
        public DateTime? Date
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the collection of <see cref="Revisions"/> to process/include.
        /// </summary>
        [RepeatableArgument(Option = "--rev")]
        public Collection<RevSpec> Revisions
        {
            get
            {
                return new Collection<RevSpec>(_Revisions);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ChurnUnit">unit</see> of measurement to use when reporting.
        /// </summary>
        [EnumArgument(ChurnUnit.Changesets, "--changesets")]
        [EnumArgument(ChurnUnit.Lines, "")]
        [DefaultValue(ChurnUnit.Lines)]
        public ChurnUnit Unit
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the formatting template to group changesets/lines by.
        /// </summary>
        [NullableArgument(NonNullOption = "--template")]
        [DefaultValue("")]
        public string GroupTemplate
        {
            get
            {
                return _GroupTemplate;
            }

            set
            {
                _GroupTemplate = (value ?? string.Empty).Trim();
            }
        }

        #region IMercurialCommand<IEnumerable<ChurnGroup>> Members

        /// <summary>
        /// Gets or sets the timeout to use when executing Mercurial commands, in
        /// seconds. Default is 600 (10 minutes) for the <see cref="ChurnCommand"/>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <para><see cref="Timeout"/> cannot be less than 0.</para>
        /// </exception>
        [DefaultValue(600)]
        public override int Timeout
        {
            get
            {
                return base.Timeout;
            }

            set
            {
                base.Timeout = value;
            }
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
                return base.Arguments.Concat(
                    new[]
                    {
                        "--diffstat"
                    });
            }
        }

        /// <summary>
        /// Gets the result from the command line execution, as an appropriately typed value.
        /// </summary>
        public IEnumerable<ChurnGroup> Result
        {
            get;
            private set;
        }

        #endregion

        /// <summary>
        /// This method should parse and store the appropriate execution result output
        /// according to the type of data the command line client would return for
        /// the command.
        /// </summary>
        /// <param name="exitCode">
        /// The exit code from executing the command line client.
        /// </param>
        /// <param name="standardOutput">
        /// The standard output from executing the command line client.
        /// </param>
        /// <remarks>
        /// Note that as long as you descend from <see cref="MercurialCommandBase{T}"/> you're not required to call
        /// the base method at all.
        /// </remarks>
        protected override void ParseStandardOutputForResults(int exitCode, string standardOutput)
        {
            var re = new Regex(@"^(?<name>.*)\s+\+(?<add>\d+)/-(?<rem>\d+)(\s+[+-]*)?$", RegexOptions.None);
            var result = new List<ChurnGroup>();
            using (var reader = new StringReader(standardOutput))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    Match ma = re.Match(line);
                    if (ma.Success)
                        result.Add(
                            new ChurnGroup(
                                ma.Groups["name"].Value, int.Parse(ma.Groups["add"].Value, CultureInfo.InvariantCulture),
                                int.Parse(ma.Groups["rem"].Value, CultureInfo.InvariantCulture)));
                }
            }
            Result = result;
        }

        /// <summary>
        /// Sets the <see cref="Date"/> property to the specified value and
        /// returns this <see cref="ChurnCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Date"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="ChurnCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public ChurnCommand WithDate(DateTime value)
        {
            Date = value;
            return this;
        }

        /// <summary>
        /// Adds the value to the <see cref="Revisions"/> collection property and
        /// returns this <see cref="ChurnCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The value to add to the <see cref="Revisions"/> collection property.
        /// </param>
        /// <returns>
        /// This <see cref="ChurnCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public ChurnCommand WithRevision(RevSpec value)
        {
            Revisions.Add(value);
            return this;
        }

        /// <summary>
        /// Sets the <see cref="GroupTemplate"/> property to the specified value and
        /// returns this <see cref="ChurnCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="GroupTemplate"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="ChurnCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public ChurnCommand WithGroupTemplate(string value)
        {
            GroupTemplate = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="Unit"/> property to the specified value and
        /// returns this <see cref="ChurnCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Unit"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="ChurnCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public ChurnCommand WithUnit(ChurnUnit value)
        {
            Unit = value;
            return this;
        }
    }
}