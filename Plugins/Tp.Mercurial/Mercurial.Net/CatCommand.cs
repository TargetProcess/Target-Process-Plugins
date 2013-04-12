using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Mercurial.Attributes;

namespace Mercurial
{
    /// <summary>
    /// This class implements the "hg cat" command (<see href="http://www.selenic.com/mercurial/hg.1.html#cat"/>):
    /// Retrieve the current or given revision of files.
    /// </summary>
    public sealed class CatCommand : IncludeExcludeCommandBase<CatCommand>
    {
        /// <summary>
        /// This is the backing field for the <see cref="Files"/> property.
        /// </summary>
        private readonly ListFile _Files = new ListFile();

        /// <summary>
        /// This is the backing field for the <see cref="OutputFormat"/> property.
        /// </summary>
        private string _OutputFormat = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="CatCommand"/> class.
        /// </summary>
        public CatCommand()
            : base("cat")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets the collection of files to process.
        /// </summary>
        public Collection<string> Files
        {
            get
            {
                return _Files.Collection;
            }
        }

        /// <summary>
        /// Adds the value to the <see cref="Files"/> collection property and
        /// returns this <see cref="CatCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The value to add to the <see cref="Files"/> collection property.
        /// </param>
        /// <returns>
        /// This <see cref="CatCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="value"/> is <c>null</c> or empty.</para>
        /// </exception>
        public CatCommand WithFile(string value)
        {
            if (StringEx.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException("value");

            Files.Add(value);
            return this;
        }

        /// <summary>
        /// Gets or sets the format to use if print to files.
        /// Default value is <see cref="string.Empty"/>.
        /// </summary>
        [NullableArgument(NonNullOption = "--output")]
        [DefaultValue("")]
        public string OutputFormat
        {
            get
            {
                return _OutputFormat;
            }

            set
            {
                _OutputFormat = (value ?? string.Empty).Trim();
            }
        }

        /// <summary>
        /// Sets the <see cref="OutputFormat"/> property to the specified value and
        /// returns this <see cref="CatCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="OutputFormat"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="CatCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public CatCommand WithOutputFormat(string value)
        {
            OutputFormat = value;
            return this;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to apply any matching decode filter.
        /// Default value is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--decode")]
        [DefaultValue(false)]
        public bool Decode
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the <see cref="Decode"/> property to the specified value and
        /// returns this <see cref="CatCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Decode"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="CatCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public CatCommand WithDecode(bool value = true)
        {
            Decode = value;
            return this;
        }

        /// <summary>
        /// Gets or sets the <see cref="RevSpec"/> from which to retrieve the file contents.
        /// </summary>
        [NullableArgument]
        [DefaultValue(null)]
        public RevSpec Revision
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the <see cref="Revision"/> property to the specified value and
        /// returns this <see cref="CatCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Revision"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="CatCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public CatCommand WithRevision(RevSpec value)
        {
            Revision = value;
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