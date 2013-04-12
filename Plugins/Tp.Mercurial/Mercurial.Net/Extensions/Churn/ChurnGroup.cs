namespace Mercurial.Extensions.Churn
{
    /// <summary>
    /// This class is returned by the <see cref="ChurnCommand"/>.
    /// </summary>
    public sealed class ChurnGroup
    {
        /// <summary>
        /// This is the backing field for the <see cref="Additions"/> property.
        /// </summary>
        private readonly int _Additions;

        /// <summary>
        /// This is the backing field for the <see cref="GroupName"/> property.
        /// </summary>
        private readonly string _GroupName;

        /// <summary>
        /// This is the backing field for the <see cref="Removals"/> property.
        /// </summary>
        private readonly int _Removals;

        /// <summary>
        /// This is the backing field for the <see cref="Unit"/> property.
        /// </summary>
        private readonly ChurnUnit _Unit;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChurnGroup"/> class.
        /// </summary>
        /// <param name="groupName">
        /// The name of the group we're reporting for.
        /// </param>
        /// <param name="addedLines">
        /// The number of lines added by the group we're reporting for.
        /// </param>
        /// <param name="removedLines">
        /// The number of lines removed by the group we're reporting for.
        /// </param>
        public ChurnGroup(string groupName, int addedLines, int removedLines)
        {
            _GroupName = (groupName ?? string.Empty).Trim();
            _Additions = addedLines;
            _Removals = removedLines;
            _Unit = ChurnUnit.Lines;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChurnGroup"/> class.
        /// </summary>
        /// <param name="groupName">
        /// The name of the group we're reporting for.
        /// </param>
        /// <param name="addedChangesets">
        /// The number of changesets committed by the group we're reporting for.
        /// </param>
        public ChurnGroup(string groupName, int addedChangesets)
        {
            _GroupName = (groupName ?? string.Empty).Trim();
            _Additions = addedChangesets;
            _Unit = ChurnUnit.Changesets;
        }

        /// <summary>
        /// Gets the number of changes (lines or changesets, see <see cref="Unit"/>) reported for the <see cref="GroupName"/> we're reporting for.
        /// </summary>
        public int Total
        {
            get
            {
                return _Additions + _Removals;
            }
        }

        /// <summary>
        /// Gets the number of lines or changesets (see <see cref="Unit"/>) added by the <see cref="GroupName"/> we're reporting for.
        /// </summary>
        public int Additions
        {
            get
            {
                return _Additions;
            }
        }

        /// <summary>
        /// Gets the number of lines removed by the <see cref="GroupName"/> we're reporting for.
        /// </summary>
        public int Removals
        {
            get
            {
                return _Removals;
            }
        }

        /// <summary>
        /// Gets the name of the group we're reporting for.
        /// </summary>
        public string GroupName
        {
            get
            {
                return _GroupName;
            }
        }

        /// <summary>
        /// Gets the <see cref="ChurnUnit"/> we're reporting in.
        /// </summary>
        public ChurnUnit Unit
        {
            get
            {
                return _Unit;
            }
        }
    }
}