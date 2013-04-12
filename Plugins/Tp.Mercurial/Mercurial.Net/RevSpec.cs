using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Mercurial
{
    /// <summary>
    /// Specifies a set of revisions, typically used to extract only
    /// a portion of the log, or specifying diff ranges
    /// </summary>
    public sealed class RevSpec : IEquatable<RevSpec>
    {
        /// <summary>
        /// This field holds the <see cref="Regex"/> for identifying a valid changeset hash.
        /// </summary>
        private static readonly Regex _HashRegex = new Regex(@"^[a-f0-9]{1,40}$", RegexOptions.IgnoreCase);

        /// <summary>
        /// This is the backing field for the <see cref="Null"/> property.
        /// </summary>
        private static readonly RevSpec _Null = new RevSpec("null");

        /// <summary>
        /// This is the backing field for the <see cref="Closed"/> property.
        /// </summary>
        private static readonly RevSpec _Closed = new RevSpec("closed()");

        /// <summary>
        /// This is the backing field for the <see cref="WorkingDirectoryParent"/> property.
        /// </summary>
        private static readonly RevSpec _WorkingDirectoryParent = new RevSpec(".");

        /// <summary>
        /// This is the backing field for the <see cref="All"/> property.
        /// </summary>
        private static readonly RevSpec _All = new RevSpec("all()");

        /// <summary>
        /// This is the backing field for the <see cref="Heads"/> property.
        /// </summary>
        private static readonly RevSpec _Heads = new RevSpec("head()");

        /// <summary>
        /// This is the backing field for the <see cref="Merges"/> property.
        /// </summary>
        private static readonly RevSpec _Merges = new RevSpec("merge()");

        /// <summary>
        /// This is the backing field for this <see cref="RevSpec"/> object, returned from
        /// <see cref="ToString"/>.
        /// </summary>
        private readonly string _Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="RevSpec"/> class.
        /// </summary>
        /// <param name="value">
        /// The value of this <see cref="RevSpec"/> value, can be both a hash and an expression.
        /// </param>
        public RevSpec(string value)
        {
            Debug.Assert(!StringEx.IsNullOrWhiteSpace(value), "value cannot be null or empty here");

            _Value = value.Trim();
        }

        /// <summary>
        /// Gets a <see cref="RevSpec"/> object that select the null revision, the revision that is
        /// the initial, empty repository, revision, the parent of revision 0.
        /// </summary>
        /// <returns>
        /// The revision specification for the empty repository revision.
        /// </returns>
        public static RevSpec Null
        {
            get
            {
                return _Null;
            }
        }

        /// <summary>
        /// Gets a <see cref="RevSpec"/> object that select the parent revision of the working directory. If an
        /// uncommitted merge is in progress, pick the first parent.
        /// </summary>
        /// <returns>
        /// The revision specification for the parent of the working directory.
        /// </returns>
        public static RevSpec WorkingDirectoryParent
        {
            get
            {
                return _WorkingDirectoryParent;
            }
        }

        /// <summary>
        /// Gets a <see cref="RevSpec"/> object that selects all the changesets in the repository.
        /// </summary>
        /// <value>
        /// The revision specification for all the changesets in the repository.
        /// </value>
        public static RevSpec All
        {
            get
            {
                return _All;
            }
        }

        /// <summary>
        /// Gets a <see cref="RevSpec"/> that selects all changesets that is a named branch head.
        /// </summary>
        public static RevSpec Heads
        {
            get
            {
                return _Heads;
            }
        }

        /// <summary>
        /// Gets a <see cref="RevSpec"/> that selects all changesets that are merges.
        /// </summary>
        public static RevSpec Merges
        {
            get
            {
                return _Merges;
            }
        }

        /// <summary>
        /// Gets a <see cref="RevSpec"/> object that selects all changesets that
        /// belongs to branches found in this <see cref="RevSpec"/>.
        /// </summary>
        public RevSpec Branches
        {
            get
            {
                return new RevSpec(string.Format(CultureInfo.InvariantCulture, "branch({0})", this));
            }
        }

        /// <summary>
        /// Gets a <see cref="RevSpec"/> object that selects all changesets that are child
        /// changesets of changesets in this <see cref="RevSpec"/>.
        /// </summary>
        public RevSpec Children
        {
            get
            {
                return new RevSpec(string.Format(CultureInfo.InvariantCulture, "children({0})", this));
            }
        }

        /// <summary>
        /// Gets a <see cref="RevSpec"/> object that selects all changesets that is a parent
        /// changesets of changesets in this <see cref="RevSpec"/>.
        /// </summary>
        public RevSpec Parents
        {
            get
            {
                return new RevSpec(string.Format(CultureInfo.InvariantCulture, "parents({0})", this));
            }
        }

        /// <summary>
        /// Gets a <see cref="RevSpec"/> object that selects all changesets that has no parents in the set.
        /// </summary>
        public RevSpec Roots
        {
            get
            {
                return new RevSpec(string.Format(CultureInfo.InvariantCulture, "roots({0})", this));
            }
        }

        /// <summary>
        /// Gets a <see cref="RevSpec"/> that selects all the left parents of changesets
        /// in this set.
        /// </summary>
        public RevSpec LeftParent
        {
            get
            {
                return new RevSpec(string.Format(CultureInfo.InvariantCulture, "p1({0})", this));
            }
        }

        /// <summary>
        /// Gets a <see cref="RevSpec"/> that selects all the right parents of changesets
        /// in this set.
        /// </summary>
        public RevSpec RightParent
        {
            get
            {
                return new RevSpec(string.Format(CultureInfo.InvariantCulture, "p2({0})", this));
            }
        }

        /// <summary>
        /// Gets a <see cref="RevSpec"/> that will include all changesets in this set, only if
        /// all changesets specified are present, otherwise it will be an empty set.
        /// </summary>
        public RevSpec AllOrNothing
        {
            get
            {
                return new RevSpec(string.Format(CultureInfo.InvariantCulture, "present({0})", this));
            }
        }

        /// <summary>
        /// Returns a <see cref="RevSpec"/> that selects the first "n" changesets of the set.
        /// </summary>
        /// <param name="amount">
        /// The number of changesets to select.
        /// </param>
        /// <returns>
        /// A <see cref="RevSpec"/> that selects the first N changesets of this set.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <para><paramref name="amount"/> is less than 1.</para>
        /// </exception>
        public RevSpec Limit(int amount)
        {
            if (amount < 1)
                throw new ArgumentOutOfRangeException("amount", amount, "amount must be at least 1");

            return new RevSpec(string.Format(CultureInfo.InvariantCulture, "limit({0}, {1})", this, amount));
        }

        /// <summary>
        /// Gets a new <see cref="RevSpec"/> that selects the changeset with the highest revision
        /// number of this set.
        /// </summary>
        public RevSpec Max
        {
            get
            {
                return new RevSpec(string.Format(CultureInfo.InvariantCulture, "max({0})", this));
            }
        }

        /// <summary>
        /// Gets a new <see cref="RevSpec"/> that selects the changeset with the lowest revision
        /// number of this set.
        /// </summary>
        public RevSpec Min
        {
            get
            {
                return new RevSpec(string.Format(CultureInfo.InvariantCulture, "min({0})", this));
            }
        }

        /// <summary>
        /// Gets a <see cref="RevSpec"/> object that selects all changesets that are not
        /// part of the current <see cref="RevSpec"/>.
        /// </summary>
        public RevSpec Not
        {
            get
            {
                return !this;
            }
        }

        /// <summary>
        /// Gets a <see cref="RevSpec"/> object that selects all changesets that close a branch.
        /// </summary>
        /// <value>
        /// The revision specification for all changesets that close a branch.
        /// </value>
        public static RevSpec Closed
        {
            get
            {
                return _Closed;
            }
        }

        #region IEquatable<RevSpec> Members

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        public bool Equals(RevSpec other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Equals(other._Value, _Value);
        }

        #endregion

        /// <summary>
        /// Returns a <see cref="RevSpec"/> that will include all changesets that add a file 
        /// with a name that matches the specified pattern.
        /// </summary>
        /// <param name="pattern">
        /// The pattern to match filenames against.
        /// </param>
        /// <returns>
        /// A new <see cref="RevSpec"/> that matches changesets that add a file with a name that
        /// matches the specified pattern.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="pattern"/> is <c>null</c> or empty.</para>
        /// </exception>
        public static RevSpec Adds(string pattern)
        {
            if (StringEx.IsNullOrWhiteSpace(pattern))
                throw new ArgumentNullException("pattern");

            return new RevSpec(string.Format(CultureInfo.InvariantCulture, "adds({0})", pattern));
        }

        /// <summary>
        /// Returns a <see cref="RevSpec"/> that will include all changesets that contains a file 
        /// with a name that matches the specified pattern.
        /// </summary>
        /// <param name="pattern">
        /// The pattern to match filenames against.
        /// </param>
        /// <returns>
        /// A new <see cref="RevSpec"/> that matches changeset that contains a file with a name that
        /// matches the specified pattern.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="pattern"/> is <c>null</c> or empty.</para>
        /// </exception>
        public static RevSpec Contains(string pattern)
        {
            if (StringEx.IsNullOrWhiteSpace(pattern))
                throw new ArgumentNullException("pattern");

            return new RevSpec(string.Format(CultureInfo.InvariantCulture, "contains({0})", pattern));
        }

        /// <summary>
        /// Returns a <see cref="RevSpec"/> that will include all changesets that affects a file 
        /// with a name that matches the specified pattern.
        /// </summary>
        /// <param name="pattern">
        /// The pattern to match filenames against.
        /// </param>
        /// <returns>
        /// A new <see cref="RevSpec"/> that matches changesets that contains affects a file with a name that
        /// matches the specified pattern.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="pattern"/> is <c>null</c> or empty.</para>
        /// </exception>
        public static RevSpec Affects(string pattern)
        {
            if (StringEx.IsNullOrWhiteSpace(pattern))
                throw new ArgumentNullException("pattern");

            return new RevSpec(string.Format(CultureInfo.InvariantCulture, "file({0})", pattern));
        }

        /// <summary>
        /// Returns a <see cref="RevSpec"/> that will include all changesets that modifies a file 
        /// with a name that matches the specified pattern.
        /// </summary>
        /// <param name="pattern">
        /// The pattern to match filenames against.
        /// </param>
        /// <returns>
        /// A new <see cref="RevSpec"/> that matches changesets that contains modifies a file with a name that
        /// matches the specified pattern.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="pattern"/> is <c>null</c> or empty.</para>
        /// </exception>
        public static RevSpec Modifies(string pattern)
        {
            if (StringEx.IsNullOrWhiteSpace(pattern))
                throw new ArgumentNullException("pattern");

            return new RevSpec(string.Format(CultureInfo.InvariantCulture, "modifies({0})", pattern));
        }

        /// <summary>
        /// Returns a <see cref="RevSpec"/> that selects all changesets not found in the specified
        /// destination repository, or if no path is specified, not found in the default push location.
        /// </summary>
        /// <param name="path">
        /// The path or url to the repository to compare against.
        /// If <c>null</c> or empty string, will use the default push location.
        /// </param>
        /// <returns>
        /// A <see cref="RevSpec"/> that selects all changesets not found in the specified
        /// destination repository, or if no path is specified, not found in the default push location.
        /// </returns>
        public static RevSpec Outgoing(string path)
        {
            path = path ?? string.Empty;

            return new RevSpec(string.Format(CultureInfo.InvariantCulture, "outgoing({0})", path));
        }

        /// <summary>
        /// Returns a <see cref="RevSpec"/> that will include all changesets that has a commit message,
        /// author name or names of changed files that matches the pattern.
        /// </summary>
        /// <param name="pattern">
        /// The pattern to match filenames against.
        /// </param>
        /// <returns>
        /// A new <see cref="RevSpec"/> that matches changesets that has a commit message,
        /// author name or names of changed files that matches the pattern.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="pattern"/> is <c>null</c> or empty.</para>
        /// </exception>
        public static RevSpec Keyword(string pattern)
        {
            if (StringEx.IsNullOrWhiteSpace(pattern))
                throw new ArgumentNullException("pattern");

            return new RevSpec(string.Format(CultureInfo.InvariantCulture, "keyword({0})", pattern));
        }

        /// <summary>
        /// Returns a <see cref="RevSpec"/> that will include all changesets that has a commit message,
        /// author name or names of changed files that matches the regular expression pattern.
        /// </summary>
        /// <param name="pattern">
        /// The pattern to match filenames against.
        /// </param>
        /// <returns>
        /// A new <see cref="RevSpec"/> that matches changesets that has a commit message,
        /// author name or names of changed files that matches the regular expression pattern.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="pattern"/> is <c>null</c> or empty.</para>
        /// </exception>
        public static RevSpec Grep(string pattern)
        {
            if (StringEx.IsNullOrWhiteSpace(pattern))
                throw new ArgumentNullException("pattern");

            return new RevSpec(string.Format(CultureInfo.InvariantCulture, "grep(r'{0}')", pattern));
        }

        /// <summary>
        /// Create a <see cref="RevSpec"/> that includes a range
        /// of revisions, by simply selecting all changesets that has
        /// a revision number in the specified range.
        /// </summary>
        /// <param name="from">
        /// The first <see cref="RevSpec"/> to include.
        /// </param>
        /// <param name="to">
        /// The last <see cref="RevSpec"/> to include.
        /// </param>
        /// <returns>
        /// A <see cref="RevSpec"/> with the specified range.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="from"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="to"/> is <c>null</c>.</para>
        /// </exception>
        public static RevSpec Range(RevSpec from, RevSpec to)
        {
            if (from == null)
                throw new ArgumentNullException("from");
            if (to == null)
                throw new ArgumentNullException("to");

            return new RevSpec(string.Format(CultureInfo.InvariantCulture, "{0}:{1}", from, to));
        }

        /// <summary>
        /// Implements the operator ! by creating a <see cref="RevSpec"/>
        /// that includes all revisions except the ones in the specified set.
        /// </summary>
        /// <param name="set">
        /// The <see cref="RevSpec"/> set to negate.
        /// </param>
        /// <returns>The result of the operator.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="set" /> is <c>null</c>.</exception>
        public static RevSpec operator !(RevSpec set)
        {
            if (set == null)
                throw new ArgumentNullException("set");

            return new RevSpec(string.Format(CultureInfo.InvariantCulture, "not {0}", ApplyParenthesis(set)));
        }

        /// <summary>
        /// Create a <see cref="RevSpec"/> that includes a range
        /// of revisions, starting with the specified
        /// <see cref="RevSpec"/> and runs all the way up to and including
        /// the tip.
        /// </summary>
        /// <param name="revSpec">
        /// The first <see cref="RevSpec"/> to include.
        /// </param>
        /// <returns>
        /// A <see cref="RevSpec"/> with the specified range.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="revSpec"/> is <c>null</c>.</para>
        /// </exception>
        public static RevSpec From(RevSpec revSpec)
        {
            if (revSpec == null)
                throw new ArgumentNullException("revSpec");

            return new RevSpec(string.Format(CultureInfo.InvariantCulture, "{0}:", revSpec));
        }

        /// <summary>
        /// Create a <see cref="RevSpec"/> that includes a range
        /// of revisions, starting with the first revision in the
        /// repository, and ending with the specified
        /// <see cref="RevSpec"/>.
        /// the tip.
        /// </summary>
        /// <param name="revSpec">
        /// The last <see cref="RevSpec"/> to include.
        /// </param>
        /// <returns>
        /// A <see cref="RevSpec"/> with the specified range.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="revSpec"/> is <c>null</c>.</para>
        /// </exception>
        public static RevSpec To(RevSpec revSpec)
        {
            if (revSpec == null)
                throw new ArgumentNullException("revSpec");

            return new RevSpec(string.Format(CultureInfo.InvariantCulture, ":{0}", revSpec));
        }

        /// <summary>
        /// Create a <see cref="RevSpec"/> that includes the revision
        /// specified and all descendant revisions.
        /// </summary>
        /// <param name="revSpec">
        /// The <see cref="RevSpec"/> to start from.
        /// </param>
        /// <returns>
        /// A <see cref="RevSpec"/> with the specified range.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="revSpec"/> is <c>null</c>.</para>
        /// </exception>
        public static RevSpec DescendantsOf(RevSpec revSpec)
        {
            if (revSpec == null)
                throw new ArgumentNullException("revSpec");

            return new RevSpec(string.Format(CultureInfo.InvariantCulture, "{0}::", revSpec));
        }

        /// <summary>
        /// Create a <see cref="RevSpec"/> that includes the changeset that is the common
        /// ancestor of the two single changeset specifications.
        /// </summary>
        /// <param name="single1">
        /// A <see cref="RevSpec"/> identifying a single changeset that is one of the two ancestors
        /// of the changeset to select, <paramref name="single2"/> is the other.
        /// </param>
        /// <param name="single2">
        /// A <see cref="RevSpec"/> identifying a single changeset that is one of the two ancestors
        /// of the changeset to select, <paramref name="single1"/> is the other.
        /// </param>
        /// <returns>
        /// A <see cref="RevSpec"/> that includes the changeset that is the common
        /// ancestor of both <paramref name="single1"/> and <paramref name="single2"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="single1"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="single2"/> is <c>null</c>.</para>
        /// </exception>
        public static RevSpec CommonAncestorOf(RevSpec single1, RevSpec single2)
        {
            if (single1 == null)
                throw new ArgumentNullException("single1");
            if (single2 == null)
                throw new ArgumentNullException("single2");

            return new RevSpec(string.Format(CultureInfo.InvariantCulture, "ancestor({0}, {1})", single1, single2));
        }

        /// <summary>
        /// Create a <see cref="RevSpec"/> that includes the revision
        /// specified and all ancestor revisions.
        /// </summary>
        /// <param name="revSpec">
        /// The <see cref="RevSpec"/> to end with.
        /// </param>
        /// <returns>
        /// A <see cref="RevSpec"/> with the specified range.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="revSpec"/> is <c>null</c>.</para>
        /// </exception>
        public static RevSpec AncestorsOf(RevSpec revSpec)
        {
            if (revSpec == null)
                throw new ArgumentNullException("revSpec");

            return new RevSpec(string.Format(CultureInfo.InvariantCulture, "::{0}", revSpec));
        }

        /// <summary>
        /// Create a <see cref="RevSpec"/> that includes the greatest common
        /// ancestor of the two changesets.
        /// </summary>
        /// <param name="revision1">
        /// The first revision of which to find the ancestor of.
        /// </param>
        /// <param name="revision2">
        /// The second revision of which to find the ancestor of.
        /// </param>
        /// <returns>
        /// The <see cref="RevSpec"/> that includes the greatest common
        /// ancestor of the two changesets.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="revision1"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="revision2"/> is <c>null</c>.</para>
        /// </exception>
        public static RevSpec GreatestCommonAncestorOf(RevSpec revision1, RevSpec revision2)
        {
            if (revision1 == null)
                throw new ArgumentNullException("revision1");
            if (revision2 == null)
                throw new ArgumentNullException("revision2");

            return new RevSpec(string.Format(CultureInfo.InvariantCulture, "ancestors({0}, {1})", revision1, revision2));
        }

        /// <summary>
        /// Gets a <see cref="RevSpec"/> that selects all changesets that are ancestors of a changeset
        /// in this set.
        /// </summary>
        public RevSpec Ancestors
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "ancestors({0})", this);
            }
        }

        /// <summary>
        /// Gets a <see cref="RevSpec"/> that selects all changesets that are descendants of a changeset
        /// in this set.
        /// </summary>
        public RevSpec Descendants
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "descendants({0})", this);
            }
        }

        /// <summary>
        /// Create a <see cref="RevSpec"/> that includes the revisions
        /// specified, and all revisions that are both descendants of
        /// <paramref name="from"/> and ancestors of <paramref name="to"/>
        /// </summary>
        /// <param name="from">
        /// The <see cref="RevSpec"/> to start from.
        /// </param>
        /// <param name="to">
        /// The <see cref="RevSpec"/> to end with.
        /// </param>
        /// <returns>
        /// A <see cref="RevSpec"/> with the specified range.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="from"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="to"/> is <c>null</c>.</para>
        /// </exception>
        public static RevSpec Bracketed(RevSpec from, RevSpec to)
        {
            if (from == null)
                throw new ArgumentNullException("from");
            if (to == null)
                throw new ArgumentNullException("to");

            return new RevSpec(string.Format(CultureInfo.InvariantCulture, "{0}::{1}", from, to));
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return _Value;
        }

        /// <summary>
        /// Select a revision based on its locally unique revision
        /// number.
        /// </summary>
        /// <param name="revision">
        /// The locally unique revision number.
        /// </param>
        /// <returns>
        /// The revision specification for a revision selected by
        /// its locally unique revision number.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <para><paramref name="revision"/> is less than 0.</para>
        /// </exception>
        public static RevSpec Single(int revision)
        {
            if (revision < 0)
                throw new ArgumentOutOfRangeException("revision", revision, "revision cannot be negative");

            return new RevSpec(revision.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Select a revision based on its globally unique hash.
        /// </summary>
        /// <param name="hash">
        /// The globally unique hash, a 1-40 digit hexadecimal number.
        /// </param>
        /// <returns>
        /// The revision specification for a revision selected by
        /// its globally unique hash.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="hash"/> is <c>null</c> or empty.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><paramref name="hash"/> does not contain a valid hexadecimal number of maximum 40 digits.</para>
        /// </exception>
        public static RevSpec Single(string hash)
        {
            if (StringEx.IsNullOrWhiteSpace(hash))
                throw new ArgumentNullException("hash");
            if (!_HashRegex.Match(hash.Trim()).Success)
                throw new ArgumentException("The hash parameter does not contain a valid hexadecimal hash");

            return new RevSpec(hash.Trim().ToLowerInvariant());
        }

        /// <summary>
        /// Select a revision based on identifying hash prefix.
        /// </summary>
        /// <param name="id">
        /// The prefix or complete hash of the changeset to select.
        /// </param>
        /// <returns>
        /// The revision specification for the changeset with the given identifying
        /// id.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="id"/> is <c>null</c> or empty.</para>
        /// </exception>
        public static RevSpec ById(string id)
        {
            if (StringEx.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException("id");

            return new RevSpec(string.Format(CultureInfo.InvariantCulture, "id({0})", id));
        }

        /// <summary>
        /// Select a revision based on its revision number.
        /// </summary>
        /// <param name="revisionNumber">
        /// The revision number to select.
        /// </param>
        /// <returns>
        /// The revision specification for the changeset with the given revision number.
        /// </returns>
        public static RevSpec ByRevisionNumber(int revisionNumber)
        {
            return new RevSpec(string.Format(CultureInfo.InvariantCulture, "rev({0})", revisionNumber));
        }

        /// <summary>
        /// Select a revision based on its branch name, will select the
        /// tipmost revision that belongs to the named branch.
        /// </summary>
        /// <param name="name">
        /// The name of the branch to select the tipmost revision of.
        /// </param>
        /// <returns>
        /// The revision specification for the tipmost revision that
        /// belongs to the named branch.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="name"/> is <c>null</c> or empty.</para>
        /// </exception>
        public static RevSpec ByBranch(string name)
        {
            if (StringEx.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            return new RevSpec(string.Format(CultureInfo.InvariantCulture, "branch('{0}')", name.Trim()));
        }

        /// <summary>
        /// Select all changesets in the specified branch.
        /// </summary>
        /// <param name="name">
        /// The name of the branch to select all changesets in.
        /// </param>
        /// <returns>
        /// A <see cref="RevSpec"/> that selects all changesets in the specified branch.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="name"/> is <c>null</c> or empty.</para>
        /// </exception>
        public static RevSpec InBranch(string name)
        {
            if (StringEx.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            return new RevSpec(string.Format(CultureInfo.InvariantCulture, "branch({0})", ByBranch(name)));
        }

        /// <summary>
        /// Select a revision based on tag.
        /// </summary>
        /// <param name="name">
        /// The name of the tag to select.
        /// </param>
        /// <returns>
        /// The revision specification for the revision that has
        /// the specified tag.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="name"/> is <c>null</c> or empty.</para>
        /// </exception>
        public static RevSpec ByTag(string name)
        {
            if (StringEx.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            return new RevSpec(name.Trim());
        }

        /// <summary>
        /// Select all changesets committed by the specified user.
        /// </summary>
        /// <param name="name">
        /// The name of the user to select changesets from.
        /// </param>
        /// <returns>
        /// The revision specification for all changesets committed by the specified user.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="name"/> is <c>null</c> or empty.</para>
        /// </exception>
        public static RevSpec ByUser(string name)
        {
            if (StringEx.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            return new RevSpec(string.Format(CultureInfo.InvariantCulture, "user('{0}')", name));
        }

        /// <summary>
        /// Returns a <see cref="RevSpec"/> that selects all tagged changesets.
        /// </summary>
        /// <returns>
        /// A <see cref="RevSpec"/> that selects all tagged changesets.
        /// </returns>
        public static RevSpec Tagged()
        {
            return new RevSpec("tag()");
        }

        /// <summary>
        /// Returns a <see cref="RevSpec"/> that selects the changeset with the specified tag.
        /// </summary>
        /// <param name="name">
        /// The name of the tag to find the changeset of.
        /// </param>
        /// <returns>
        /// A <see cref="RevSpec"/> that selects the changeset with the specified tag.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="name"/> is <c>null</c> or empty.</para>
        /// </exception>
        public static RevSpec Tagged(string name)
        {
            if (StringEx.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            return new RevSpec(string.Format(CultureInfo.InvariantCulture, "tag('{0}')", name));
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Int32"/> to <see cref="RevSpec"/>
        /// by using the number as the revision number.
        /// </summary>
        /// <param name="revisionNumber">The revision number.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator RevSpec(int revisionNumber)
        {
            return Single(revisionNumber);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="RevSpec"/>
        /// by using the string as the revision hash.
        /// </summary>
        /// <param name="hash">
        /// The changeset hash <see cref="String"/> to convert to a <see cref="RevSpec"/>.
        /// </param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator RevSpec(string hash)
        {
            return new RevSpec(hash);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="RevSpec"/> to <see cref="System.String"/>
        /// by calling the <see cref="ToString"/> method.
        /// </summary>
        /// <param name="revSpec">The revisions.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator string(RevSpec revSpec)
        {
            if (revSpec == null)
                return string.Empty;

            return revSpec.ToString();
        }

        /// <summary>
        /// Implements the operator &amp;.
        /// </summary>
        /// <param name="revision1">The revision1.</param>
        /// <param name="revision2">The revision2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        /// <exception cref="ArgumentException">At least one of the revisions in a &amp; expression must be non-null</exception>
        public static RevSpec operator &(RevSpec revision1, RevSpec revision2)
        {
            if (revision1 == null && revision2 == null)
                throw new ArgumentException("At least one of the revisions in a & expression must be non-null");

            if (revision1 == null)
                return revision2;
            if (revision2 == null)
                return revision1;
            return revision1.And(revision2);
        }

        /// <summary>
        /// Implements the operator |.
        /// </summary>
        /// <param name="revision1">The revision1.</param>
        /// <param name="revision2">The revision2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        /// <exception cref="ArgumentException">At least one of the revisions in a | expression must be non-null</exception>
        public static RevSpec operator |(RevSpec revision1, RevSpec revision2)
        {
            if (revision1 == null && revision2 == null)
                throw new ArgumentException("At least one of the revisions in a | expression must be non-null");

            if (revision1 == null)
                return revision2;
            if (revision2 == null)
                return revision1;
            return revision1.Or(revision2);
        }

        /// <summary>
        /// Select all changesets in this <see cref="RevSpec"/> specification, but
        /// not in <paramref name="revSpec"/>.
        /// </summary>
        /// <param name="revSpec">
        /// The revisions of the changesets to exclude.
        /// </param>
        /// <returns>
        /// A revision specification that selects all the changesets in this
        /// <see cref="RevSpec"/>, but not in <paramref name="revSpec"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="revSpec"/> is <c>null</c>.</para>
        /// </exception>
        public RevSpec Except(RevSpec revSpec)
        {
            if (revSpec == null)
                throw new ArgumentNullException("revSpec");

            return new RevSpec(string.Format(CultureInfo.InvariantCulture, "{0} - {1}", ApplyParenthesis(this), ApplyParenthesis(revSpec)));
        }

        /// <summary>
        /// Selects all changesets that are both in this <see cref="RevSpec"/>
        /// and also in <paramref name="revSpec"/>.
        /// </summary>
        /// <param name="revSpec">
        /// The 2nd operand to the <c>and</c> operator.
        /// </param>
        /// <returns>
        /// A revision specification that selects all changesets that are both in this <see cref="RevSpec"/>
        /// and also in <paramref name="revSpec"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="revSpec"/> is <c>null</c>.</para>
        /// </exception>
        public RevSpec And(RevSpec revSpec)
        {
            if (revSpec == null)
                throw new ArgumentNullException("revSpec");

            return new RevSpec(string.Format(CultureInfo.InvariantCulture, "{0} and {1}", ApplyParenthesis(this), ApplyParenthesis(revSpec)));
        }

        /// <summary>
        /// Check if the given <see cref="RevSpec"/> requires parenthesis before being combined with another
        /// <see cref="RevSpec"/>.
        /// </summary>
        /// <param name="revSpec">
        /// The <see cref="RevSpec"/> to check if needs parenthesis.
        /// </param>
        /// <returns>
        /// The formatted <see cref="RevSpec"/> value, possibly with added parenthesis around it.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="revSpec"/> is <c>null</c>.</para>
        /// </exception>
        private static string ApplyParenthesis(RevSpec revSpec)
        {
            if (revSpec == null)
                throw new ArgumentNullException("revSpec");

            int level = 0;
            char quote = '\0';
            foreach (char c in revSpec.ToString())
            {
                if (quote != '\0')
                {
                    if (c == quote)
                    {
                        quote = '\0';
                    }
                }
                else
                {
                    switch (c)
                    {
                        case '"':
                        case '\'':
                            quote = c;
                            break;

                        case '(':
                        case '[':
                        case '{':
                            level++;
                            break;

                        case ')':
                        case ']':
                        case '}':
                            if (level > 0)
                                level--;
                            break;

                        case ' ':
                        case '\t':
                        case '\r':
                        case '\n':
                            if (level == 0)
                                return string.Format(CultureInfo.InvariantCulture, "({0})", revSpec);
                            break;
                    }
                }
            }

            return revSpec.ToString();
        }

        /// <summary>
        /// Selects all changesets that either in this <see cref="RevSpec"/>
        /// or in <paramref name="revSpec"/>.
        /// </summary>
        /// <param name="revSpec">
        /// The 2nd operand to the <c>or</c> operator.
        /// </param>
        /// <returns>
        /// A revision specification that selects all changesets that are either in this <see cref="RevSpec"/>
        /// or in <paramref name="revSpec"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="revSpec"/> is <c>null</c>.</para>
        /// </exception>
        public RevSpec Or(RevSpec revSpec)
        {
            if (revSpec == null)
                throw new ArgumentNullException("revSpec");

            return new RevSpec(string.Format(CultureInfo.InvariantCulture, "{0} or {1}", ApplyParenthesis(this), ApplyParenthesis(revSpec)));
        }

        /// <summary>
        /// Selects all changesets committed by the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">
        /// The name of the author to select changesets for.
        /// </param>
        /// <returns>
        /// A revision specification that selects all changesets committed
        /// by the specified <paramref name="name"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="name"/> is <c>null</c> or empty.</para>
        /// </exception>
        public static RevSpec Author(string name)
        {
            if (StringEx.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            return new RevSpec(string.Format(CultureInfo.InvariantCulture, "author('{0}')", name));
        }

        /// <summary>
        /// Creates a new <see cref="RevSpec"/> that will include the changeset idenfitied by
        /// the specified bookmark name.
        /// </summary>
        /// <param name="name">
        /// The name of the bookmark to include.
        /// </param>
        /// <returns>
        /// A <see cref="RevSpec"/> including the changeset with the specified bookmark.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="name"/> is <c>null</c> or empty.</para>
        /// </exception>
        public static RevSpec Bookmark(string name)
        {
            if (StringEx.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            return new RevSpec(string.Format(CultureInfo.InvariantCulture, "bookmark({0})", name));
        }

        /// <summary>
        /// Gets a new <see cref="RevSpec"/> that includes all changesets identified by
        /// bookmarks.
        /// </summary>
        public static RevSpec Bookmarks
        {
            get
            {
                return new RevSpec("bookmark()");
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">
        /// The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. 
        /// </param>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != typeof(RevSpec))
                return false;
            return Equals((RevSpec)obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return _Value != null ? _Value.GetHashCode() : 0;
        }

        /// <summary>
        /// Implements the operator true.
        /// </summary>
        /// <param name="revision">The revision.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "revision", Justification = "The true/false operators both return false in order to make && and || behave like & and |")]
        public static bool operator true(RevSpec revision)
        {
            return false;
        }

        /// <summary>
        /// Implements the operator false.
        /// </summary>
        /// <param name="revision">The revision.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "revision", Justification = "The true/false operators both return false in order to make && and || behave like & and |")]
        public static bool operator false(RevSpec revision)
        {
            return false;
        }
    }
}