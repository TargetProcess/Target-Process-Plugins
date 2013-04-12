using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Mercurial.Attributes;

namespace Mercurial
{
    /// <summary>
    /// This class implements the "hg clone" command (<see href="http://www.selenic.com/mercurial/hg.1.html#clone"/>):
    /// make a copy of an existing repository.
    /// </summary>
    public sealed class CloneCommand : MercurialCommandBase<CloneCommand>
    {
        /// <summary>
        /// This is the backing field for the <see cref="Branches"/> property.
        /// </summary>
        private readonly List<string> _Branches = new List<string>();

        /// <summary>
        /// This is the backing field for the <see cref="Revisions"/> property.
        /// </summary>
        private readonly List<RevSpec> _Revisions = new List<RevSpec>();

        /// <summary>
        /// This is the backing field for the <see cref="Source"/> property.
        /// </summary>
        private string _Source = string.Empty;

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
        /// Initializes a new instance of the <see cref="CloneCommand"/> class.
        /// </summary>
        public CloneCommand()
            : base("clone")
        {
            Update = true;
            CompressedTransfer = true;
        }

        /// <summary>
        /// Gets or sets the source path or Uri to clone from.
        /// </summary>
        [DefaultValue("")]
        public string Source
        {
            get
            {
                return _Source;
            }

            set
            {
                _Source = (value ?? string.Empty).Trim();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to update the clone with a working folder.
        /// Default is <c>true</c>.
        /// </summary>
        [BooleanArgument(FalseOption = "--noupdate")]
        [DefaultValue(true)]
        public bool Update
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
                RequiresVersion(new Version(1, 7, 5), "VerifyServerCertificate property of the CloneCommand class");
                _VerifyServerCertificate = value;
            }
        }

        /// <summary>
        /// Sets the <see cref="VerifyServerCertificate"/> property to the specified value and
        /// returns this <see cref="CloneCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="VerifyServerCertificate"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="CloneCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public CloneCommand WithVerifyServerCertificate(bool value)
        {
            VerifyServerCertificate = value;
            return this;
        }

        /// <summary>
        /// Gets or sets the <see cref="Revisions"/> to update the working
        /// folder to, or <c>null</c> to update to the tip. Default is <c>null</c>.
        /// </summary>
        [NullableArgument(NonNullOption = "--updaterev")]
        [DefaultValue(null)]
        public RevSpec UpdateToRevision
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use compressed transfer or not. Over LAN, uncompressed is faster, otherwise
        /// compressed is most likely faster. Default is <c>true</c>.
        /// </summary>
        [BooleanArgument(FalseOption = "--uncompressed")]
        [DefaultValue(true)]
        public bool CompressedTransfer
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the collection of revisions to include in the clone. If empty, include every changeset
        /// from the source repository. Default is empty.
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
        /// Gets the collection of branches to include in the clone. If empty, include every branch
        /// from the source repository. Default is empty.
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
        /// Gets or sets a value indicating whether to use pull protocol to copy metadata.
        /// Default value is <c>false</c>.
        /// </summary>
        [DefaultValue(false)]
        public bool UsePull
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the <see cref="UsePull"/> property to the specified value and
        /// returns this <see cref="CloneCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="UsePull"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="CloneCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public CloneCommand WithUsePull(bool value)
        {
            UsePull = value;
            return this;
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
        /// Gets all the arguments to the <see cref="CommandBase{T}.Command"/>, or an
        /// empty array if there are none.
        /// </summary>
        /// <value></value>
        public override IEnumerable<string> Arguments
        {
            get
            {
                return base.Arguments.Concat(
                    new[]
                    {
                        "\"" + Source + "\"", ".",
                    });
            }
        }

        /// <summary>
        /// Sets the <see cref="Source"/> property to the specified value and
        /// returns this <see cref="CloneCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Source"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="CloneCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public CloneCommand WithSource(string value)
        {
            Source = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="SshCommand"/> property to the specified value and
        /// returns this <see cref="CloneCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="SshCommand"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="CloneCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public CloneCommand WithSshCommand(string value)
        {
            SshCommand = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="RemoteCommand"/> property to the specified value and
        /// returns this <see cref="CloneCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="RemoteCommand"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="CloneCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public CloneCommand WithRemoteCommand(string value)
        {
            RemoteCommand = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="Update"/> property to the specified value and
        /// returns this <see cref="CloneCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Update"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="CloneCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public CloneCommand WithUpdate(bool value)
        {
            Update = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="UpdateToRevision"/> property to the specified value and
        /// returns this <see cref="CloneCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="UpdateToRevision"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="CloneCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public CloneCommand WithUpdateToRevision(RevSpec value)
        {
            UpdateToRevision = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="CompressedTransfer"/> property to the specified value and
        /// returns this <see cref="CloneCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="CompressedTransfer"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="CloneCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public CloneCommand WithCompressedTransfer(bool value)
        {
            CompressedTransfer = value;
            return this;
        }

        /// <summary>
        /// Adds the value to the <see cref="Revisions"/> collection property and
        /// returns this <see cref="CloneCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The value to add to the <see cref="Revisions"/> collection property.
        /// </param>
        /// <returns>
        /// This <see cref="CloneCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public CloneCommand WithRevision(RevSpec value)
        {
            Revisions.Add(value);
            return this;
        }

        /// <summary>
        /// Adds the value to the <see cref="Branches"/> collection property and
        /// returns this <see cref="CloneCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The value to add to the <see cref="Branches"/> collection property.
        /// </param>
        /// <returns>
        /// This <see cref="CloneCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public CloneCommand WithBranch(string value)
        {
            Branches.Add(value);
            return this;
        }

        /// <summary>
        /// Validates the command configuration. This method should throw the necessary
        /// exceptions to signal missing or incorrect configuration (like attempting to
        /// add files to the repository without specifying which files to add.)
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// <para>The 'clone' command requires <see cref="Source"/> to be specified.</para>
        /// <para>- or -</para>
        /// <para>The <see cref="VerifyServerCertificate"/> command was used with Mercurial 1.7.4 or older.</para>
        /// </exception>
        public override void Validate()
        {
            base.Validate();

            if (!VerifyServerCertificate && ClientExecutable.CurrentVersion < new Version(1, 7, 5))
                throw new InvalidOperationException("The 'VerifyServerCertificate' property is only available in Mercurial 1.7.5 and newer");
            if (StringEx.IsNullOrWhiteSpace(Source))
                throw new InvalidOperationException("The 'clone' command requires Source to be specified");
        }
    }
}