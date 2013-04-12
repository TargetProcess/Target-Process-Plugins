using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Mercurial
{
    /// <summary>
    /// This class encapsulates a single file as part of a <see cref="MergeJob"/>;
    /// basically a file that had a merge conflict when the merge job started.
    /// </summary>
    public class MergeJobConflict : IEquatable<MergeJobConflict>
    {
        /// <summary>
        /// This is the backing field for the <see cref="MergeJob"/> property.
        /// </summary>
        private readonly MergeJob _MergeJob;

        /// <summary>
        /// This is the backing field for the <see cref="Path"/> property.
        /// </summary>
        private readonly string _Path;

        /// <summary>
        /// Initializes a new instance of the <see cref="MergeJobConflict"/> class.
        /// </summary>
        /// <param name="mergeJob">
        /// The <see cref="MergeJob"/> that manages this <see cref="MergeJobConflict"/>.
        /// </param>
        /// <param name="path">
        /// The name of the file this <see cref="MergeJobConflict"/> relates to.
        /// </param>
        /// <param name="state">
        /// The initial state of the file.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="mergeJob"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="path"/> is <c>null</c> or empty.</para>
        /// </exception>
        public MergeJobConflict(MergeJob mergeJob, string path, MergeConflictState state)
        {
            if (mergeJob == null)
                throw new ArgumentNullException("mergeJob");
            if (StringEx.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException("path");

            _MergeJob = mergeJob;
            _Path = path;
            State = state;
        }

        /// <summary>
        /// Gets the <see cref="MergeJob"/> that manages this <see cref="MergeJobConflict"/>.
        /// </summary>
        public MergeJob MergeJob
        {
            get
            {
                return _MergeJob;
            }
        }

        /// <summary>
        /// Gets the name of the file this <see cref="MergeJobConflict"/> relates to.
        /// </summary>
        public string Path
        {
            get
            {
                return _Path;
            }
        }

        /// <summary>
        /// Gets the path (local to the repository) of the sub-file present in the working directory
        /// during the merge, for files with merge-conflicts.
        /// </summary>
        /// <param name="subFile">
        /// Which particular <see cref="MergeJobConflictSubFile"/> to get the path to.
        /// </param>
        /// <returns>
        /// The path (local to the repository) of the sub-file.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <para>An invalid <paramref name="subFile"/> value was specified.</para>
        /// </exception>
        public string GetMergeSubFilePath(MergeJobConflictSubFile subFile)
        {
            switch (subFile)
            {
                case MergeJobConflictSubFile.Base:
                    return string.Format(CultureInfo.InvariantCulture, "{0}.base", Path);

                case MergeJobConflictSubFile.Local:
                    return string.Format(CultureInfo.InvariantCulture, "{0}.local", Path);

                case MergeJobConflictSubFile.Other:
                    return string.Format(CultureInfo.InvariantCulture, "{0}.other", Path);

                case MergeJobConflictSubFile.Current:
                    return Path;

                default:
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid subFile parameter passed to GetMergeSubFilePath: {0}", subFile));
            }
        }

        /// <summary>
        /// Gets the path (local to the repository) of the sub-file present in the working directory
        /// during the merge, for files with merge-conflicts.
        /// </summary>
        /// <param name="subFile">
        /// Which particular <see cref="MergeJobConflictSubFile"/> to get the path to.
        /// </param>
        /// <returns>
        /// The path (local to the repository) of the sub-file.
        /// </returns>
        public string GetFullMergeSubFilePath(MergeJobConflictSubFile subFile)
        {
            return System.IO.Path.Combine(MergeJob.Repository.Path, GetMergeSubFilePath(subFile));
        }

        /// <summary>
        /// Gets the contents of the sub-file, as a <see cref="String"/>, using the
        /// specified <paramref name="encoding"/> to decode the contents.
        /// </summary>
        /// <param name="subFile">
        /// Which particular <see cref="MergeJobConflictSubFile"/> to get the contents of.
        /// </param>
        /// <param name="encoding">
        /// The <see cref="Encoding"/> to use when reading the file contents;
        /// or <c>null</c> to use <see cref="Encoding.Default"/>.
        /// </param>
        /// <returns>
        /// The contents of the specified <paramref name="subFile"/>, as a <see cref="String"/>.
        /// </returns>
        public string GetMergeSubFileContentsAsText(MergeJobConflictSubFile subFile, Encoding encoding)
        {
            encoding = encoding ?? Encoding.Default;
            return File.ReadAllText(GetFullMergeSubFilePath(subFile), encoding);
        }

        /// <summary>
        /// Gets the contents of the sub-file, as a <see cref="String"/>,
        /// using <see cref="Encoding.Default"/> to decode the contents.
        /// </summary>
        /// <param name="subFile">
        /// Which particular <see cref="MergeJobConflictSubFile"/> to get the contents of.
        /// </param>
        /// <returns>
        /// The contents of the specified <paramref name="subFile"/>, as a <see cref="String"/>.
        /// </returns>
        public string GetMergeSubFileContentsAsText(MergeJobConflictSubFile subFile)
        {
            return GetMergeSubFileContentsAsText(subFile, Encoding.Default);
        }

        /// <summary>
        /// Gets the contents of the sub-file, as an array of <see cref="Byte"/>s.
        /// </summary>
        /// <param name="subFile">
        /// Which particular <see cref="MergeJobConflictSubFile"/> to get the contents of.
        /// </param>
        /// <returns>
        /// The contents of the specified <paramref name="subFile"/>, as an array of <see cref="Byte"/>s.
        /// </returns>
        public byte[] GetMergeSubFileContentsAsBytes(MergeJobConflictSubFile subFile)
        {
            return File.ReadAllBytes(GetFullMergeSubFilePath(subFile));
        }

        /// <summary>
        /// Gets the current state of the file this <see cref="MergeJobConflict"/> relates to.
        /// </summary>
        public MergeConflictState State
        {
            get;
            internal set;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        public bool Equals(MergeJobConflict other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Equals(other._Path, _Path) && Equals(other.State, State);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="Object"/> is equal to the current <see cref="Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">
        /// The <see cref="Object"/> to compare with the current <see cref="Object"/>. 
        /// </param>
        /// <exception cref="NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != typeof(MergeJobConflict))
                return false;
            return Equals((MergeJobConflict)obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((_Path != null ? _Path.GetHashCode() : 0) * 397) ^ State.GetHashCode();
            }
        }

        /// <summary>
        /// Returns a <see cref="String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "MergeJobFile (Path={0}, State={1})", _Path, State);
        }

        /// <summary>
        /// Mark the file as resolved, unresolved, or attempt to merge the file again. Afterwards,
        /// update the <see cref="State"/>.
        /// </summary>
        /// <param name="command">
        /// Any extra options to the resolve method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="ResolveAction.List"/> action is invalid for this method.
        /// </exception>
        /// <remarks>
        /// Note that the <see cref="ResolveCommand.MergeTool"/> property is not overridden here,
        /// which means that a visual, third-party, merge tool might pop up and ask the user
        /// for help. To avoid this, override the merge tool yourself.
        /// </remarks>
        public void Resolve(ResolveCommand command = null)
        {
            command = (command ?? new ResolveCommand())
                .WithFile(_Path);

            if (command.Action == ResolveAction.List)
                throw new InvalidOperationException("The ResolveAction.List action is invalid for this method");

            try
            {
                MergeJob.Repository.Resolve(command);
            }
            catch (MercurialExecutionException)
            {
                return;
            }

            IEnumerable<MergeConflict> conflictsAfterResolve = MergeJob.Repository.Resolve(new ResolveCommand().WithAction(ResolveAction.List));
            State =
                (from conflict in conflictsAfterResolve
                 where conflict.Path == _Path
                 select conflict.State).First();
        }

        /// <summary>
        /// Cleans up temporary files left by the initial merge command (the .base, .local, .orig and .other files.)
        /// </summary>
        public void Cleanup()
        {
            var extensions = new[]
            {
                ".base",
                ".local",
                ".orig",
                ".other",
            };

            foreach (string physicalPath in extensions.Select(extension => System.IO.Path.Combine(MergeJob.Repository.Path, Path) + extension).Where(File.Exists))
            {
                File.Delete(physicalPath);
            }
        }
    }
}