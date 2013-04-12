using System;
using System.ComponentModel;
using Mercurial.Attributes;

namespace Mercurial
{
    /// <summary>
    /// This class implements the "hg init" command (<see href="http://www.selenic.com/mercurial/hg.1.html#init"/>):
    /// create a new repository at the given remote location.
    /// </summary>
    public sealed class RemoteInitCommand : MercurialCommandBase<RemoteInitCommand>
    {
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
        /// This is the backing field for the <see cref="Location"/> property.
        /// </summary>
        private string _Location = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteInitCommand"/> class.
        /// </summary>
        public RemoteInitCommand()
            : base("init")
        {
            // Do nothing here
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
                RequiresVersion(new Version(1, 7, 5), "VerifyServerCertificate property of the PullCommand class");
                _VerifyServerCertificate = value;
            }
        }

        /// <summary>
        /// Gets or sets the Url of the remote location to initialize a repository.
        /// Default value is <see cref="string.Empty"/>.
        /// </summary>
        [NullableArgument]
        [DefaultValue("")]
        public string Location
        {
            get
            {
                return _Location;
            }

            set
            {
                _Location = (value ?? string.Empty).Trim();
            }
        }

        /// <summary>
        /// Sets the <see cref="SshCommand"/> property to the specified value and
        /// returns this <see cref="RemoteInitCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="SshCommand"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="RemoteInitCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public RemoteInitCommand WithSshCommand(string value)
        {
            SshCommand = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="RemoteCommand"/> property to the specified value and
        /// returns this <see cref="RemoteInitCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="RemoteCommand"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="RemoteInitCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public RemoteInitCommand WithRemoteCommand(string value)
        {
            RemoteCommand = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="VerifyServerCertificate"/> property to the specified value and
        /// returns this <see cref="RemoteInitCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="VerifyServerCertificate"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="RemoteInitCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public RemoteInitCommand WithVerifyServerCertificate(bool value)
        {
            VerifyServerCertificate = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="VerifyServerCertificate"/> property to the specified value and
        /// returns this <see cref="RemoteInitCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="VerifyServerCertificate"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="RemoteInitCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public RemoteInitCommand WithLocation(string value)
        {
            Location = value;
            return this;
        }

        /// <summary>
        /// Validates the command configuration. This method should throw the necessary
        /// exceptions to signal missing or incorrect configuration (like attempting to
        /// add files to the repository without specifying which files to add.)
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// <para>The <see cref="VerifyServerCertificate"/> command was used with Mercurial 1.7.4 or older.</para>
        /// <para>- or -</para>
        /// <para>The <see cref="Location"/> property was blank.</para>
        /// </exception>
        public override void Validate()
        {
            base.Validate();

            if (StringEx.IsNullOrWhiteSpace(Location))
                throw new InvalidOperationException("The Location property must be set before executing a RemoteInitCommand");
            if (!VerifyServerCertificate && ClientExecutable.CurrentVersion < new Version(1, 7, 5))
                throw new InvalidOperationException("The 'VerifyServerCertificate' property is only available in Mercurial 1.7.5 and newer");
        }
    }
}
