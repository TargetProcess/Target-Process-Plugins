using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Mercurial.Attributes;

namespace Mercurial
{
    /// <summary>
    /// This class implements the "hg bundle" command (<see href="http://www.selenic.com/mercurial/hg.1.html#bundle"/>):
    /// create a changegroup file.
    /// </summary>
    public sealed class BundleCommand : MercurialCommandBase<BundleCommand>
    {
        /// <summary>
        /// This is the backing field for the <see cref="BaseRevisions"/> property.
        /// </summary>
        private readonly List<RevSpec> _BaseRevisions = new List<RevSpec>();

        /// <summary>
        /// This is the backing field for the <see cref="Revisions"/> property.
        /// </summary>
        private readonly List<RevSpec> _Revisions = new List<RevSpec>();

        /// <summary>
        /// This is the backing field for the <see cref="Branches"/> property.
        /// </summary>
        private readonly List<string> _Branches = new List<string>();

        /// <summary>
        /// This is the backing field for the <see cref="Destination"/> property.
        /// </summary>
        private string _Destination = string.Empty;

        /// <summary>
        /// This is the backing field for the <see cref="FileName"/> property.
        /// </summary>
        private string _FileName = string.Empty;

        /// <summary>
        /// This is the backing field for the <see cref="SshCommand"/> property.
        /// </summary>
        private string _SshCommand = string.Empty;

        /// <summary>
        /// This is the backing field for the <see cref="RemoteCommand"/> property.
        /// </summary>
        private string _RemoteCommand = string.Empty;

        /// <summary>
        /// This is the backing field for the <see cref="VerifyServerCertificate"/> property.
        /// </summary>
        private bool _VerifyServerCertificate = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="BundleCommand"/> class.
        /// </summary>
        public BundleCommand()
            : base("bundle")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets or sets a value indicating whether to run even when the destination is unrelated.
        /// Default is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--force")]
        [DefaultValue(false)]
        public bool Force
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to verify the server certificate. If set to <c>false</c>, will ignore web.cacerts configuration.
        /// Default value is <c>true</c>.
        /// </summary>
        [BooleanArgument(FalseOption = "--insecure")]
        [DefaultValue(true)]
        public bool VerifyServerCertificate
        {
            get
            {
                return _VerifyServerCertificate;
            }

            set
            {
                RequiresVersion(new Version(1, 7, 5), "VerifyServerCertificate property of the BundleCommand class");
                _VerifyServerCertificate = value;
            }
        }

        /// <summary>
        /// Sets the <see cref="SshCommand"/> property to the specified value and
        /// returns this <see cref="BundleCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="SshCommand"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="BundleCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public BundleCommand WithSshCommand(string value)
        {
            SshCommand = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="RemoteCommand"/> property to the specified value and
        /// returns this <see cref="BundleCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="RemoteCommand"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="BundleCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public BundleCommand WithRemoteCommand(string value)
        {
            RemoteCommand = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="VerifyServerCertificate"/> property to the specified value and
        /// returns this <see cref="BundleCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="VerifyServerCertificate"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="BundleCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public BundleCommand WithVerifyServerCertificate(bool value)
        {
            VerifyServerCertificate = value;
            return this;
        }

        /// <summary>
        /// Gets the collection of revisions to be added to the destination, and thus include in the bundle.
        /// If empty, include every changeset from the source repository that isn't in the destination.
        /// Default is empty.
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
        /// Gets the collection of branches to bundle.
        /// Default is empty.
        /// </summary>
        [RepeatableArgument(Option = "--branch")]
        public Collection<string> Branches
        {
            get
            {
                return new Collection<string>(_Branches);
            }
        }

        /// <summary>
        /// Gets the collection of revisions assumed to be available at the destination, and thus not
        /// included in the bundle. Mostly used when leaving <see cref="Destination"/> empty,
        /// to calculate an "assumed" bundle for a target repository that may not be available for
        /// comparison.
        /// Default is empty.
        /// </summary>
        [RepeatableArgument(Option = "--base")]
        public Collection<RevSpec> BaseRevisions
        {
            get
            {
                return new Collection<RevSpec>(_BaseRevisions);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to bundle all the changesets in the repository.
        /// Default is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--all")]
        [DefaultValue(false)]
        public bool All
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="MercurialCompressionType">compression type</see> to use
        /// for the bundle file. Default is <c>MercurialCompressionType.BZip2</c>.
        /// </summary>
        [DefaultValue(MercurialCompressionType.BZip2)]
        [EnumArgument(MercurialCompressionType.None, "--type", "none")]
        [EnumArgument(MercurialCompressionType.GZip, "--type", "gzip")]
        public MercurialCompressionType Compression
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the full path to and name of the file to save the bundle to.
        /// Default is <see cref="string.Empty"/>.
        /// </summary>
        [NullableArgument]
        [DefaultValue("")]
        public string FileName
        {
            get
            {
                return _FileName;
            }

            set
            {
                _FileName = (value ?? string.Empty).Trim();
            }
        }

        /// <summary>
        /// Gets or sets the path or URL to the destination repository that the bundled will be
        /// built for. The repository will be used to figure out which changesets to add to the
        /// bundle.
        /// Default is <see cref="string.Empty"/>.
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
        /// Gets or sets the ssh command to use when cloning.
        /// Default is <see cref="string.Empty"/>.
        /// </summary>
        [NullableArgument(NonNullOption = "--ssh")]
        [DefaultValue("")]
        public string SshCommand
        {
            get
            {
                return _SshCommand;
            }

            set
            {
                _SshCommand = (value ?? string.Empty).Trim();
            }
        }

        /// <summary>
        /// Gets or sets the hg command to run on the remote side.
        /// Default is <see cref="string.Empty"/>.
        /// </summary>
        [NullableArgument(NonNullOption = "--remotecmd")]
        [DefaultValue("")]
        public string RemoteCommand
        {
            get
            {
                return _RemoteCommand;
            }

            set
            {
                _RemoteCommand = (value ?? string.Empty).Trim();
            }
        }

        /// <summary>
        /// Validates the command configuration. This method should throw the necessary
        /// exceptions to signal missing or incorrect configuration (like attempting to
        /// add files to the repository without specifying which files to add.)
        /// </summary>
        /// <remarks>
        /// Note that as long as you descend from <see cref="MercurialCommandBase{T}"/> you're not required to call
        /// the base method at all.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// <para>The 'bundle' command requires FileName to be specified.</para>
        /// <para>- or -</para>
        /// <para>The <see cref="VerifyServerCertificate"/> command was used with Mercurial 1.7.4 or older.</para>
        /// </exception>
        public override void Validate()
        {
            base.Validate();

            if (!VerifyServerCertificate && ClientExecutable.CurrentVersion < new Version(1, 7, 5))
                throw new InvalidOperationException("The 'VerifyServerCertificate' property is only available in Mercurial 1.7.5 and newer");
            if (StringEx.IsNullOrWhiteSpace(FileName))
                throw new InvalidOperationException("The 'bundle' command requires FileName to be specified");
        }

        /// <summary>
        /// Sets the <see cref="Force"/> property to the specified value and
        /// returns this <see cref="BundleCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Force"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="BundleCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public BundleCommand WithForce(bool value = true)
        {
            Force = value;
            return this;
        }

        /// <summary>
        /// Adds the value to the <see cref="Revisions"/> collection property and
        /// returns this <see cref="BundleCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The value to add to the <see cref="Revisions"/> collection property.
        /// </param>
        /// <returns>
        /// This <see cref="BundleCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public BundleCommand WithRevision(RevSpec value)
        {
            Revisions.Add(value);
            return this;
        }

        /// <summary>
        /// Adds the value to the <see cref="Branches"/> collection property and
        /// returns this <see cref="BundleCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The value to add to the <see cref="Branches"/> collection property.
        /// </param>
        /// <returns>
        /// This <see cref="BundleCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public BundleCommand WithBranch(string value)
        {
            Branches.Add(value);
            return this;
        }

        /// <summary>
        /// Adds the value to the <see cref="BaseRevisions"/> collection property and
        /// returns this <see cref="BundleCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The value to add to the <see cref="BaseRevisions"/> collection property.
        /// </param>
        /// <returns>
        /// This <see cref="BundleCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public BundleCommand WithBaseRevision(RevSpec value)
        {
            BaseRevisions.Add(value);
            return this;
        }

        /// <summary>
        /// Sets the <see cref="All"/> property to the specified value and
        /// returns this <see cref="BundleCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="All"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="BundleCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public BundleCommand WithAll(bool value = true)
        {
            All = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="Compression"/> property to the specified value and
        /// returns this <see cref="BundleCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Compression"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="BundleCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public BundleCommand WithCompression(MercurialCompressionType value)
        {
            Compression = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="FileName"/> property to the specified value and
        /// returns this <see cref="BundleCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="FileName"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="BundleCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public BundleCommand WithFileName(string value)
        {
            FileName = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="Destination"/> property to the specified value and
        /// returns this <see cref="BundleCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Destination"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="BundleCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public BundleCommand WithDestination(string value)
        {
            Destination = value;
            return this;
        }

        /// <summary>
        /// This method should throw the appropriate exception depending on the contents of
        /// the <paramref name="exitCode"/> and <paramref name="standardErrorOutput"/>
        /// parameters, or simply return if the execution is considered successful.
        /// </summary>
        /// <param name="exitCode">
        /// The exit code from executing the command line client.
        /// </param>
        /// <param name="standardErrorOutput">
        /// The standard error output from executing the command client.
        /// </param>
        /// <remarks>
        /// Note that as long as you descend from <see cref="MercurialCommandBase{T}"/> you're not required to call
        /// the base method at all. The default behavior is to throw a <see cref="MercurialExecutionException"/>
        /// if <paramref name="exitCode"/> is not zero. If you require different behavior, don't call the base
        /// method.
        /// </remarks>
        /// <exception cref="NoChangesFoundMercurialExecutionException">
        /// <para><paramref name="exitCode"/> is <c>1</c>.</para>
        /// </exception>
        /// <exception cref="MercurialExecutionException">
        /// <para><paramref name="exitCode"/> is not <c>0</c> or <c>1</c>.</para>
        /// </exception>
        protected override void ThrowOnUnsuccessfulExecution(int exitCode, string standardErrorOutput)
        {
            switch (exitCode)
            {
                case 0:
                    break;

                case 1:
                    throw new NoChangesFoundMercurialExecutionException(exitCode, standardErrorOutput);

                default:
                    base.ThrowOnUnsuccessfulExecution(exitCode, standardErrorOutput);
                    break;
            }
        }
    }
}