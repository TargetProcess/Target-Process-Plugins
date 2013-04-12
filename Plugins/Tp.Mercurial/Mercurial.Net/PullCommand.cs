using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Mercurial.Attributes;

namespace Mercurial
{
    /// <summary>
    /// This class implements the "hg pull" command (<see href="http://www.selenic.com/mercurial/hg.1.html#pull"/>):
    /// pull changes from the specified source.
    /// </summary>
    public sealed class PullCommand : MercurialCommandBase<PullCommand>
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
        /// Initializes a new instance of the <see cref="PullCommand"/> class.
        /// </summary>
        public PullCommand()
            : base("pull")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets or sets a value indicating whether to update to new branch head if changes were pulled.
        /// Default is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--update")]
        [DefaultValue(false)]
        public bool Update
        {
            get;
            set;
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
        /// Sets the <see cref="SshCommand"/> property to the specified value and
        /// returns this <see cref="PullCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="SshCommand"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="PullCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public PullCommand WithSshCommand(string value)
        {
            SshCommand = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="RemoteCommand"/> property to the specified value and
        /// returns this <see cref="PullCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="RemoteCommand"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="PullCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public PullCommand WithRemoteCommand(string value)
        {
            RemoteCommand = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="VerifyServerCertificate"/> property to the specified value and
        /// returns this <see cref="PullCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="VerifyServerCertificate"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="PullCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public PullCommand WithVerifyServerCertificate(bool value)
        {
            VerifyServerCertificate = value;
            return this;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to force pulling from the source, even if the
        /// source repository is unrelated.
        /// </summary>
        [BooleanArgument(TrueOption = "--force")]
        [DefaultValue(false)]
        public bool Force
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the source to pull from. If <see cref="string.Empty"/>, pull from the
        /// default source. Default is <see cref="string.Empty"/>.
        /// </summary>
        [NullableArgument]
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
        /// Gets the collection of branches to pull. If empty, pull all branches.
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
        /// Gets the collection of revisions to include from the <see cref="Source"/>.
        /// If empty, pull all changes. Default is empty.
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
        /// Sets the <see cref="Update"/> property to the specified value and
        /// returns this <see cref="PullCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Update"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="PullCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public PullCommand WithUpdate(bool value = true)
        {
            Update = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="Force"/> property to the specified value and
        /// returns this <see cref="PullCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Force"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="PullCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public PullCommand WithForce(bool value = true)
        {
            Force = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="Source"/> property to the specified value and
        /// returns this <see cref="PullCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Source"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="PullCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public PullCommand WithSource(string value)
        {
            Source = value;
            return this;
        }

        /// <summary>
        /// Adds the value to the <see cref="Branches"/> collection property and
        /// returns this <see cref="PullCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The value to add to the <see cref="Branches"/> collection property.
        /// </param>
        /// <returns>
        /// This <see cref="PullCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public PullCommand WithBranch(string value)
        {
            Branches.Add(value);
            return this;
        }

        /// <summary>
        /// Adds the value to the <see cref="Revisions"/> collection property and
        /// returns this <see cref="PullCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The value to add to the <see cref="Revisions"/> collection property.
        /// </param>
        /// <returns>
        /// This <see cref="PullCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public PullCommand WithRevision(RevSpec value)
        {
            Revisions.Add(value);
            return this;
        }

        /// <summary>
        /// Validates the command configuration. This method should throw the necessary
        /// exceptions to signal missing or incorrect configuration (like attempting to
        /// add files to the repository without specifying which files to add.)
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// <para>The <see cref="VerifyServerCertificate"/> command was used with Mercurial 1.7.4 or older.</para>
        /// </exception>
        public override void Validate()
        {
            base.Validate();

            if (!VerifyServerCertificate && ClientExecutable.CurrentVersion < new Version(1, 7, 5))
                throw new InvalidOperationException("The 'VerifyServerCertificate' property is only available in Mercurial 1.7.5 and newer");
        }
    }
}