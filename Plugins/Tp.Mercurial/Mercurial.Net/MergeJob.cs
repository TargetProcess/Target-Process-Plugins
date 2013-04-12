using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Mercurial
{
    /// <summary>
    /// This helper class is used to control a merge, providing help and data structures for the job.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This class is first and foremost a helper class, and not a collection, leaving name as-is.")]
    public class MergeJob : IEnumerable<MergeJobConflict>
    {
        /// <summary>
        /// This is the backing field for the <see cref="Repository"/> property.
        /// </summary>
        private readonly Repository _Repository;

        /// <summary>
        /// This is the backing field for the <see cref="MergeCommand"/> property.
        /// </summary>
        private readonly MergeCommand _MergeCommand;

        /// <summary>
        /// This is the backing field for the this <see cref="MergeJob"/>.
        /// </summary>
        private readonly List<MergeJobConflict> _Conflicts = new List<MergeJobConflict>();

        /// <summary>
        /// This is the backing field for the this <see cref="State"/> property.
        /// </summary>
        private MergeJobState _InternalState;

        /// <summary>
        /// This is the backing field for the <see cref="LocalParent"/> property.
        /// </summary>
        private Changeset _LocalParent;

        /// <summary>
        /// This is the backing field for the <see cref="OtherParent"/> property.
        /// </summary>
        private Changeset _OtherParent;

        /// <summary>
        /// Initializes a new instance of the <see cref="MergeJob"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository the merge was initiated in.
        /// </param>
        /// <param name="mergeCommand">
        /// The <see cref="MergeCommand"/> that initiated the merge.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="mergeCommand"/> is <c>null</c>.</para>
        /// </exception>
        public MergeJob(Repository repository, MergeCommand mergeCommand)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            if (mergeCommand == null)
                throw new ArgumentNullException("mergeCommand");

            _Repository = repository;
            _MergeCommand = mergeCommand;

            if (_MergeCommand.Result == MergeResult.UnresolvedFiles)
                InitialRefresh();
        }

        /// <summary>
        /// Gets the <see cref="MergeCommand"/> that initiated the merge.
        /// </summary>
        public MergeCommand MergeCommand
        {
            get
            {
                return _MergeCommand;
            }
        }

        /// <summary>
        /// Gets the <see cref="Repository"/> this <see cref="MergeJob"/> is operating in.
        /// </summary>
        public Repository Repository
        {
            get
            {
                return _Repository;
            }
        }

        /// <summary>
        /// Gets the local parent revision number, ie. the "left" parent of the merge.
        /// </summary>
        /// <remarks>
        /// This is what the working directory was at before the merge was initiated.
        /// </remarks>
        public Changeset LocalParent
        {
            get
            {
                EnsureParentsLoaded();
                return _LocalParent;
            }
        }

        /// <summary>
        /// Ensures that the two <see cref="LocalParent"/> and <see cref="OtherParent"/> properties
        /// have values by issuing a <c>hg summary</c> command and retrieving the parents,
        /// if needed.
        /// </summary>
        private void EnsureParentsLoaded()
        {
            if (_LocalParent != null)
                return;
            
            Changeset[] parents = Repository.Parents().ToArray();
            _LocalParent = parents[0];
            _OtherParent = parents[1];
        }

        /// <summary>
        /// Gets the other parent revision number, ie. the "right" parent of the merge.
        /// </summary>
        /// <remarks>
        /// This is the revision the working directory was merged with.
        /// </remarks>
        public Changeset OtherParent
        {
            get
            {
                EnsureParentsLoaded();
                return _OtherParent;
            }
        }

        /// <summary>
        /// Gets a value indicating the current state of the merge job.
        /// </summary>
        public MergeJobState State
        {
            get
            {
                if (_InternalState == MergeJobState.Canceled || _InternalState == MergeJobState.Committed)
                    return _InternalState;

                if (UnresolvedConflicts.Any())
                    return MergeJobState.HasUnresolvedConflicts;

                return MergeJobState.ReadyToCommit;
            }
        }

        /// <summary>
        /// Does the initial refresh of the list of files that has merge conflicts.
        /// </summary>
        private void InitialRefresh()
        {
            IEnumerable<MergeConflict> conflicts = _Repository.Resolve(new ResolveCommand()
                .WithAction(ResolveAction.List));

            _Conflicts.AddRange(
                from conflict in conflicts
                select new MergeJobConflict(this, conflict.Path, conflict.State));
        }

        /// <summary>
        /// Refreshes the resolution status of the files that had merge conflicts when the
        /// merge started.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Internal error, new file with conflict detected after initial merge.
        /// </exception>
        public void Refresh()
        {
            ThrowIfCancelled();

            IEnumerable<MergeConflict> conflicts = _Repository.Resolve(new ResolveCommand()
                .WithAction(ResolveAction.List));

            foreach (MergeConflict currentConflict in conflicts)
            {
                MergeJobConflict existingConflict = _Conflicts.FirstOrDefault(c => c.Path == currentConflict.Path);
                if (existingConflict == null)
                    throw new InvalidOperationException("Internal error, new file with conflict detected after initial merge");

                existingConflict.State = currentConflict.State;
            }
        }

        /// <summary>
        /// If this <see cref="MergeJob"/> has been cancelled this method will throw an
        /// appropriate <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="MergeJob"/> has been cancelled, it can no longer be used to manage or query the merge.
        /// </exception>
        private void ThrowIfCancelled()
        {
            if (State == MergeJobState.Canceled)
                throw new InvalidOperationException("The MergeJob has been cancelled, it can no longer be used to manage or query the merge");
        }

        /// <summary>
        /// Gets the <see cref="MergeJobConflict"/> at the specified index.
        /// </summary>
        /// <param name="index">
        /// The index of the <see cref="MergeJobConflict"/> to get.
        /// </param>
        /// <returns>
        /// The <see cref="MergeJobConflict"/> at the given <paramref name="index"/>.
        /// </returns>
        public MergeJobConflict this[int index]
        {
            get
            {
                ThrowIfCancelled();
                return _Conflicts[index];
            }
        }

        /// <summary>
        /// Gets the <see cref="MergeJobConflict"/> with the specified filename;
        /// or <c>null</c> if there is no such <see cref="MergeJobConflict"/>.
        /// </summary>
        /// <param name="path">
        /// The path to the <see cref="MergeJobConflict"/> to get.
        /// </param>
        /// <returns>
        /// The <see cref="MergeJobConflict"/> object for the given <paramref name="path"/>.
        /// </returns>
        public MergeJobConflict this[string path]
        {
            get
            {
                ThrowIfCancelled();
                return _Conflicts.FirstOrDefault(file => file.Path == path);
            }
        }

        /// <summary>
        /// Gets the number of files in this <see cref="MergeJob"/> that had merge conflicts when
        /// the merge started.
        /// </summary>
        public int Count
        {
            get
            {
                ThrowIfCancelled();
                return _Conflicts.Count;
            }
        }

        /// <summary>
        /// Cancels the merge by updating back to the initial parent.
        /// </summary>
        /// <remarks>
        /// WARNING! This will revert and undo any local changes in the working directory.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// Cannot cancel this <see cref="MergeJob"/>, it has already been committed.
        /// </exception>
        public void CancelMerge()
        {
            switch (State)
            {
                case MergeJobState.Canceled:
                    break;

                case MergeJobState.Committed:
                    throw new InvalidOperationException("Cannot cancel this MergeJob, it has already been committed");

                case MergeJobState.HasUnresolvedConflicts:
                case MergeJobState.ReadyToCommit:
                    _Repository.Update(".", new UpdateCommand().WithClean());
                    Cleanup();
                    _InternalState = MergeJobState.Canceled;
                    break;

                default:
                    throw new InvalidOperationException("Internal error, unknown MergeJobState for this MergeJob");
            }
        }

        /// <summary>
        /// Commits the changes introduced by this <see cref="MergeJob"/>.
        /// </summary>
        /// <param name="command">
        /// Any extra options to the commit method.
        /// </param>
        /// <returns>
        /// The <see cref="RevSpec"/> with the hash of the new changeset.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="command"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <para>This MergeJob has already been committed, cannot commit again.</para>
        /// <para>- or -</para>
        /// <para>This MergeJob has been cancelled, cannot commit</para>
        /// <para>- or -</para>
        /// <para>There are unresolved merge conflicts in this MergeJob, cannot commit</para>
        /// </exception>
        public RevSpec Commit(CommitCommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            switch (State)
            {
                case MergeJobState.Committed:
                    throw new InvalidOperationException("This MergeJob has already been committed, cannot commit again");

                case MergeJobState.Canceled:
                    throw new InvalidOperationException("This MergeJob has been cancelled, cannot commit");

                case MergeJobState.HasUnresolvedConflicts:
                    throw new InvalidOperationException("There are unresolved merge conflicts in this MergeJob, cannot commit");

                case MergeJobState.ReadyToCommit:
                    Repository.Commit(command);
                    _InternalState = MergeJobState.Committed;
                    return command.Result;

                default:
                    throw new InvalidOperationException("Internal error, unknown MergeJobState for this MergeJob");
            }
        }

        /// <summary>
        /// Commits the changes introduced by this <see cref="MergeJob"/>.
        /// </summary>
        /// <param name="message">
        /// The commit message to use.
        /// </param>
        /// <param name="command">
        /// Any extra options to the commit method, or <c>null</c> for default options.
        /// </param>
        /// <returns>
        /// The <see cref="RevSpec"/> with the hash of the new changeset.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="message"/> is <c>null</c> or empty.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <para>This MergeJob has already been committed, cannot commit again.</para>
        /// <para>- or -</para>
        /// <para>This MergeJob has been cancelled, cannot commit</para>
        /// <para>- or -</para>
        /// <para>There are unresolved merge conflicts in this MergeJob, cannot commit</para>
        /// </exception>
        public RevSpec Commit(string message, CommitCommand command = null)
        {
            if (StringEx.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException("message");

            command = (command ?? new CommitCommand())
                .WithMessage(message);

            return Commit(command);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="IEnumerator{T}"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<MergeJobConflict> GetEnumerator()
        {
            ThrowIfCancelled();
            return _Conflicts.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            ThrowIfCancelled();
            return GetEnumerator();
        }

        /// <summary>
        /// Gets a collection of <see cref="MergeJobConflict"/> objects that are still marked as
        /// <see cref="MergeConflictState.Unresolved"/>.
        /// </summary>
        /// <remarks>
        /// Note that this collection is built from in-memory data, if you're unsure about the
        /// current state of the working folder, call <see cref="Refresh"/> before retrieving
        /// the collection.
        /// </remarks>
        public IEnumerable<MergeJobConflict> UnresolvedConflicts
        {
            get
            {
                return
                    from file in _Conflicts
                    where file.State == MergeConflictState.Unresolved
                    select file;
            }
        }

        /// <summary>
        /// Cleans up temporary files left by the initial merge command (the .base, .local, .orig and .other files.)
        /// </summary>
        public void Cleanup()
        {
            foreach (MergeJobConflict conflict in _Conflicts)
                conflict.Cleanup();
        }
    }
}