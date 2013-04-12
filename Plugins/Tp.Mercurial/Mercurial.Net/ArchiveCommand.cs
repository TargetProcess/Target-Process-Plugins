using System;
using System.ComponentModel;
using Mercurial.Attributes;

namespace Mercurial
{
    /// <summary>
    /// This class implements the "hg archive" command (<see href="http://www.selenic.com/mercurial/hg.1.html#archive"/>):
    /// create an unversioned archive of a repository revision.
    /// </summary>
    public sealed class ArchiveCommand : IncludeExcludeCommandBase<ArchiveCommand>
    {
        /// <summary>
        /// This is the backing field for the <see cref="DirectoryPrefix"/> property.
        /// </summary>
        private string _DirectoryPrefix = string.Empty;

        /// <summary>
        /// This is the backing field for the <see cref="PassThroughDecoders"/> property.
        /// </summary>
        private bool _PassThroughDecoders = true;

        /// <summary>
        /// This is the backing field for the <see cref="ArchiveType"/> property.
        /// </summary>
        private ArchiveType _ArchiveType = ArchiveType.Automatic;

        /// <summary>
        /// This is the backing field for the <see cref="Destination"/> property.
        /// </summary>
        private string _Destination = string.Empty;

        /// <summary>
        /// This is the backing field for the <see cref="RecurseSubRepositories"/> property.
        /// </summary>
        private bool _RecurseSubRepositories;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveCommand"/> class.
        /// </summary>
        public ArchiveCommand()
            : base("archive")
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether to pass files through decoders before archival.
        /// Default is <c>true</c>.
        /// </summary>
        [BooleanArgument(FalseOption = "--no-decode")]
        [DefaultValue(true)]
        public bool PassThroughDecoders
        {
            get
            {
                return _PassThroughDecoders;
            }

            set
            {
                _PassThroughDecoders = value;
            }
        }

        /// <summary>
        /// Gets or sets a directory prefix for files in the archive.
        /// Default is <see cref="string.Empty"/>.
        /// </summary>
        [NullableArgument(NonNullOption = "--prefix")]
        [DefaultValue("")]
        public string DirectoryPrefix
        {
            get
            {
                return _DirectoryPrefix;
            }

            set
            {
                _DirectoryPrefix = (value ?? string.Empty).Trim();
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="RevSpec"/> to archive.
        /// </summary>
        [NullableArgument]
        [DefaultValue(null)]
        public RevSpec Revision
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of archive to produce.
        /// Default is <see cref="Mercurial.ArchiveType.Automatic"/>.
        /// </summary>
        [EnumArgument(ArchiveType.Automatic, "")]
        [EnumArgument(ArchiveType.DirectoryWithFiles, "--type", "files")]
        [EnumArgument(ArchiveType.TarUncompressed, "--type", "tar")]
        [EnumArgument(ArchiveType.TarBZip2Compressed, "--type", "tbz2")]
        [EnumArgument(ArchiveType.TarGZipCompressed, "--type", "tgz")]
        [EnumArgument(ArchiveType.ZipUncompressed, "--type", "uzip")]
        [EnumArgument(ArchiveType.ZipDeflateCompressed, "--type", "zip")]
        [DefaultValue(ArchiveType.Automatic)]
        public ArchiveType ArchiveType
        {
            get
            {
                return _ArchiveType;
            }

            set
            {
                _ArchiveType = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to recurse into subrepositories.
        /// Default is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// This property requires Mercurial 1.7 or newer.
        /// </remarks>
        [BooleanArgument(TrueOption = "--subrepos")]
        [DefaultValue(false)]
        public bool RecurseSubRepositories
        {
            get
            {
                return _RecurseSubRepositories;
            }

            set
            {
                RequiresVersion(new Version(1, 7, 0), "RecurseSubRepositories property of the ArchiveCommand class");
                _RecurseSubRepositories = value;
            }
        }

        /// <summary>
        /// Gets or sets the destination to archive typ, either the directory (if <see cref="ArchiveType"/> is
        /// <see cref="Mercurial.ArchiveType.DirectoryWithFiles"/>) or the full path to and name of the archive file
        /// (for all other <see cref="ArchiveType"/>s of archives.)
        /// Default value is <see cref="string.Empty"/>.
        /// </summary>
        [NullableArgument]
        [DefaultValue("")]
        public string Destination
        {
            get
            {
                return _Destination;
            }

            set
            {
                _Destination = (value ?? string.Empty).Trim();
            }
        }

        /// <summary>
        /// Sets the <see cref="PassThroughDecoders"/> property to the specified value and
        /// returns this <see cref="ArchiveCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="PassThroughDecoders"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="ArchiveCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public ArchiveCommand WithPassThroughDecoders(bool value)
        {
            PassThroughDecoders = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="DirectoryPrefix"/> property to the specified value and
        /// returns this <see cref="ArchiveCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="DirectoryPrefix"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="ArchiveCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public ArchiveCommand WithDirectoryPrefix(string value)
        {
            DirectoryPrefix = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="Revision"/> property to the specified value and
        /// returns this <see cref="ArchiveCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Revision"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="ArchiveCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public ArchiveCommand WithRevision(RevSpec value)
        {
            Revision = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="ArchiveType"/> property to the specified value and
        /// returns this <see cref="ArchiveCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="ArchiveType"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="ArchiveCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public ArchiveCommand WithArchiveType(ArchiveType value)
        {
            ArchiveType = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="RecurseSubRepositories"/> property to the specified value and
        /// returns this <see cref="ArchiveCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="RecurseSubRepositories"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="ArchiveCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public ArchiveCommand WithRecurseSubRepositories(bool value)
        {
            RecurseSubRepositories = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="Destination"/> property to the specified value and
        /// returns this <see cref="ArchiveCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Destination"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="ArchiveCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public ArchiveCommand WithDestination(string value)
        {
            Destination = value;
            return this;
        }

        /// <summary>
        /// Validates the command configuration. This method should throw the necessary
        /// exceptions to signal missing or incorrect configuration (like attempting to
        /// add files to the repository without specifying which files to add.)
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// <para><see cref="Destination"/> is <c>null</c> or empty.</para>
        /// </exception>
        public override void Validate()
        {
            base.Validate();
            if (StringEx.IsNullOrWhiteSpace(Destination))
                throw new InvalidOperationException("Must specify the destination for the archive");
        }
    }
}