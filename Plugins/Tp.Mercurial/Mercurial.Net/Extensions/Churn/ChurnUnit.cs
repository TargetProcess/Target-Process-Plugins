namespace Mercurial.Extensions.Churn
{
    /// <summary>
    /// This enum is used by the <see cref="ChurnCommand"/> class to specify what unit of measurement we're going to report in.
    /// </summary>
    public enum ChurnUnit
    {
        /// <summary>
        /// The churn command is reporting the number of lines added and removed by each person.
        /// </summary>
        Lines,

        /// <summary>
        /// The churn command is reporting the number of changesets added by each person.
        /// </summary>
        Changesets
    }
}