using System;

namespace Mercurial
{
    /// <summary>
    /// This class is used as the result of executing the <see cref="BisectCommand"/>.
    /// </summary>
    public class BisectResult
    {
        /// <summary>
        /// This is the backing field for the <see cref="Revision"/> property.
        /// </summary>
        private readonly RevSpec _Revision;

        /// <summary>
        /// This is the backing field for the <see cref="Done"/> property.
        /// </summary>
        private readonly bool _Done;

        /// <summary>
        /// Initializes a new instance of the <see cref="BisectResult"/> class.
        /// </summary>
        public BisectResult()
        {
            _Revision = null;
            _Done = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BisectResult"/> class.
        /// </summary>
        /// <param name="revision">
        /// The revision the <see cref="BisectCommand"/> is currently at in the repository.
        /// </param>
        /// <param name="done">
        /// If <c>true</c>, then <paramref name="revision"/> specifies the first good changeset in the
        /// repository; otherwise <paramref name="revision"/> specifies the current changeset that should
        /// be tested and marked good or bad by executing another <see cref="BisectCommand"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="revision"/> is <c>null</c>.</para>
        /// </exception>
        public BisectResult(RevSpec revision, bool done)
        {
            if (revision == null)
                throw new ArgumentNullException("revision");

            _Revision = revision;
            _Done = done;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="BisectCommand"/> is done, in which case the
        /// <see cref="Revision"/> property contains the first good changeset.
        /// </summary>
        public bool Done
        {
            get
            {
                return _Done;
            }
        }

        /// <summary>
        /// Gets the <see cref="Revision"/> the <see cref="BisectCommand"/> is currently at in the repository;
        /// or if <see cref="Done"/> is <c>true</c>, the first good changeset in the repository.
        /// </summary>
        public RevSpec Revision
        {
            get
            {
                return _Revision;
            }
        }
    }
}