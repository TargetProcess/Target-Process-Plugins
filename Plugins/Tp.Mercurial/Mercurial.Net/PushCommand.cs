using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Mercurial.Attributes;

namespace Mercurial
{
    /// <summary>
    /// This class implements the "hg push" command (<see href="http://www.selenic.com/mercurial/hg.1.html#push"/>):
    /// push changes to the specified destination.
    /// </summary>
    public sealed class PushCommand : MercurialCommandBase<PushCommand>
    {
        /// <summary>
        /// This is the backing field for the <see cref="Revisions"/> property.
        /// </summary>
        private readonly List<RevSpec> _Revisions = new List<RevSpec>();

        /// <summary>
        /// This is the backing field for the <see cref="Bookmarks"/> property.
        /// </summary>
        private readonly List<string> _Bookmarks = new List<string>();

        /// <summary>
        /// This is the backing field for the <see cref="Destination"/> property.
        /// </summary>
        private string _Destination = string.Empty;

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
        /// Initializes a new instance of the <see cref="PushCommand"/> class.
        /// </summary>
        public PushCommand()
            : base("push")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets the collection of bookmarks to push.
        /// Default is empty.
        /// </summary>
        [RepeatableArgument(Option = "--bookmark")]
        public Collection<string> Bookmarks
        {
            get
            {
                return new Collection<string>(_Bookmarks);
            }
        }

        /// <summary>
        /// Gets or sets the ssh command to use when pushing.
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
                RequiresVersion(new Version(1, 7, 5), "VerifyServerCertificate property of the PushCommand class");
                _VerifyServerCertificate = value;
            }
        }

        /// <summary>
        /// Sets the <see cref="VerifyServerCertificate"/> property to the specified value and
        /// returns this <see cref="PushCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="VerifyServerCertificate"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="PushCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public PushCommand WithVerifyServerCertificate(bool value)
        {
            VerifyServerCertificate = value;
            return this;
        }

        /// <summary>
        /// Gets or sets the destination to pull from. If <see cref="string.Empty"/>, push to the
        /// default destination. Default is <see cref="string.Empty"/>.
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
        /// Gets or sets a value indicating whether to force push to the destination, even if
        /// the repositories are unrelated, or pushing would create new heads in the
        /// destination repository. Default is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--force")]
        [DefaultValue(false)]
        public bool Force
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to allow creating a new branch in the destination
        /// repository. Default is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--new-branch")]
        [DefaultValue(false)]
        public bool AllowCreatingNewBranch
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to push large repositories in chunks.
        /// Default is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--chunked")]
        [DefaultValue(false)]
        public bool ChunkedTransfer
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the collection of revisions to include when pushing.
        /// If empty, push all changes. Default is empty.
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
        /// Sets the <see cref="Destination"/> property to the specified value and
        /// returns this <see cref="PushCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Destination"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="PushCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public PushCommand WithDestination(string value)
        {
            Destination = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="Force"/> property to the specified value and
        /// returns this <see cref="PushCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Force"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="PushCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public PushCommand WithForce(bool value = true)
        {
            Force = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="AllowCreatingNewBranch"/> property to the specified value and
        /// returns this <see cref="PushCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="AllowCreatingNewBranch"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="PushCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public PushCommand WithAllowCreatingNewBranch(bool value = true)
        {
            AllowCreatingNewBranch = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="ChunkedTransfer"/> property to the specified value and
        /// returns this <see cref="PushCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="ChunkedTransfer"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="PushCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public PushCommand WithChunkedTransfer(bool value)
        {
            ChunkedTransfer = value;
            return this;
        }

        /// <summary>
        /// Adds the value to the <see cref="Bookmarks"/> collection property and
        /// returns this <see cref="PushCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The value to add to the <see cref="Bookmarks"/> collection property.
        /// </param>
        /// <returns>
        /// This <see cref="PushCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public PushCommand WithBookmark(string value)
        {
            Bookmarks.Add(value);
            return this;
        }

        /// <summary>
        /// Adds the value to the <see cref="Revisions"/> collection property and
        /// returns this <see cref="PushCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The value to add to the <see cref="Revisions"/> collection property.
        /// </param>
        /// <returns>
        /// This <see cref="PushCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public PushCommand WithRevision(RevSpec value)
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

            if (Bookmarks.Count > 0)
                RequiresVersion(new Version(1, 8), "Bookmarks property of the PushCommand class");

            if (!VerifyServerCertificate && ClientExecutable.CurrentVersion < new Version(1, 7, 5))
                throw new InvalidOperationException("The 'VerifyServerCertificate' property is only available in Mercurial 1.7.5 and newer");
        }
    }
}