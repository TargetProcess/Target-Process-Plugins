using System;
using System.ComponentModel;
using Mercurial.Attributes;

namespace Mercurial
{
    /// <summary>
    /// This class implements the "hg addremove" command (<see href="http://www.selenic.com/mercurial/hg.1.html#addremove"/>):
    /// add all new files, delete all missing files.
    /// </summary>
    public sealed class AddRemoveCommand : IncludeExcludeCommandBase<AddRemoveCommand>
    {
        /// <summary>
        /// This is the backing field for the <see cref="Similarity"/> property.
        /// </summary>
        private int? _Similarity;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddRemoveCommand"/> class.
        /// </summary>
        public AddRemoveCommand()
            : base("addremove")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets or sets the similarity ratio to use when guessing renamed or
        /// moved files. Must be in the range of 0 to 100, inclusive, or <c>null</c> to not pass this
        /// option to the addremove command.
        /// Default is <c>null</c>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <para>Attempted set to less than 0 or greater than 100.</para>
        /// </exception>
        [NullableArgument(NonNullOption = "--similarity")]
        [DefaultValue(null)]
        public int? Similarity
        {
            get
            {
                return _Similarity;
            }

            set
            {
                if (value != null)
                {
                    if (value < 0)
                        throw new ArgumentOutOfRangeException("value", value, "Similarity cannot be less than 0");
                    if (value > 100)
                        throw new ArgumentOutOfRangeException("value", value, "Similarity cannot be greater than 100");
                }

                _Similarity = value;
            }
        }

        /// <summary>
        /// Sets the <see cref="Similarity"/> property to the specified value and
        /// returns this <see cref="AddRemoveCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Similarity"/> property,
        /// defaults to <c>100</c>.
        /// </param>
        /// <returns>
        /// This <see cref="AddRemoveCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public AddRemoveCommand WithSimilarity(int value = 100)
        {
            Similarity = value;
            return this;
        }
    }
}