using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Mercurial.Attributes;

namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "clone" command (<see href="http://tortoisehg.bitbucket.org/manual/2.0/clone.html#from-command-line"/>):
    /// Clone a repository.
    /// </summary>
    public sealed class CloneGuiCommand : GuiCommandBase<CloneGuiCommand>
    {
        /// <summary>
        /// This is the backing field for the <see cref="UpdateToRevision"/> property.
        /// </summary>
        private RevSpec _UpdateToRevision;

        /// <summary>
        /// This is the backing field for the <see cref="IncludeRevision"/> property.
        /// </summary>
        private RevSpec _IncludeRevision;

        /// <summary>
        /// This is the backing field for the <see cref="Source"/> property.
        /// </summary>
        private string _Source = string.Empty;

        /// <summary>
        /// This is the backing field for the <see cref="Branch"/> property.
        /// </summary>
        private string _Branch = string.Empty;

        /// <summary>
        /// This is the backing field for the <see cref="CompressedTransfer"/> property.
        /// </summary>
        private bool _CompressedTransfer = true;

        /// <summary>
        /// This is the backing field for the <see cref="Update"/> property.
        /// </summary>
        private bool _Update = true;

        /// <summary>
        /// This is the backing field for the <see cref="UsePullProtocol"/> property.
        /// </summary>
        private bool _UsePullProtocol;

        /// <summary>
        /// Initializes a new instance of the <see cref="CloneGuiCommand"/> class.
        /// </summary>
        public CloneGuiCommand()
            : base("clone")
        {
            // Do nothing here
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
        /// <remarks>
        /// This property is only available for the <see cref="GuiClientType.PyQT"/> client type.
        /// </remarks>
        [BooleanArgument(FalseOption = "--noupdate")]
        [DefaultValue(true)]
        public bool Update
        {
            get
            {
                return _Update;
            }

            set
            {
                if (_Update == value)
                    return;
                
                EnsurePropertyAvailability("Update", GuiClientType.PyQT);
                _Update = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use the pull protocol to copy metadata.
        /// Default is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// This property is only available for the <see cref="GuiClientType.PyQT"/> client type.
        /// </remarks>
        [BooleanArgument(TrueOption = "--pull")]
        [DefaultValue(false)]
        public bool UsePullProtocol
        {
            get
            {
                return _UsePullProtocol;
            }

            set
            {
                if (_UsePullProtocol == value)
                    return;
                
                EnsurePropertyAvailability("UsePullProtocol", GuiClientType.PyQT);
                _UsePullProtocol = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="UpdateToRevision"/> to update the working
        /// folder to, or <c>null</c> to update to the tip. Default is <c>null</c>.
        /// </summary>
        /// <remarks>
        /// This property is only available for the <see cref="GuiClientType.PyQT"/> client type.
        /// </remarks>
        [NullableArgument(NonNullOption = "--updaterev")]
        [DefaultValue(null)]
        public RevSpec UpdateToRevision
        {
            get
            {
                return _UpdateToRevision;
            }

            set
            {
                if (_UpdateToRevision == value)
                    return;
                
                EnsurePropertyAvailability("UpdateToRevision", GuiClientType.PyQT);
                _UpdateToRevision = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use compressed transfer or not. Over LAN, uncompressed is faster, otherwise
        /// compressed is most likely faster. Default is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// This property is only available for the <see cref="GuiClientType.PyQT"/> client type.
        /// </remarks>
        [BooleanArgument(FalseOption = "--uncompressed")]
        [DefaultValue(true)]
        public bool CompressedTransfer
        {
            get
            {
                return _CompressedTransfer;
            }

            set
            {
                if (_CompressedTransfer == value)
                    return;
                
                EnsurePropertyAvailability("CompressedTransfer", GuiClientType.PyQT);
                _CompressedTransfer = value;
            }
        }

        /// <summary>
        /// Gets or sets the revision to include in the clone. If <c>null</c>, include every changeset
        /// from the source repository.
        /// Default is <c>null</c>.
        /// </summary>
        /// <remarks>
        /// This property is only available for the <see cref="GuiClientType.PyQT"/> client type.
        /// </remarks>
        [NullableArgument(NonNullOption = "--rev")]
        [DefaultValue(null)]
        public RevSpec IncludeRevision
        {
            get
            {
                return _IncludeRevision;
            }

            set
            {
                if (_IncludeRevision == value)
                    return;
                
                EnsurePropertyAvailability("IncludeRevision", GuiClientType.PyQT);
                _IncludeRevision = value;
            }
        }

        /// <summary>
        /// Gets or sets the branch to include in the clone. If empty, include every branch
        /// from the source repository. Default is empty.
        /// </summary>
        /// <remarks>
        /// This property is only available for the <see cref="GuiClientType.PyQT"/> client type.
        /// </remarks>
        [RepeatableArgument(Option = "--branch")]
        [DefaultValue("")]
        public string Branch
        {
            get
            {
                return _Branch;
            }

            set
            {
                if (_Branch == value)
                    return;
                
                EnsurePropertyAvailability("Branch", GuiClientType.PyQT);
                _Branch = (value ?? string.Empty).Trim();
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
        /// returns this <see cref="CloneGuiCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Source"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="CloneGuiCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public CloneGuiCommand WithSource(string value)
        {
            Source = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="Update"/> property to the specified value and
        /// returns this <see cref="CloneGuiCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Update"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="CloneGuiCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public CloneGuiCommand WithUpdate(bool value)
        {
            Update = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="UpdateToRevision"/> property to the specified value and
        /// returns this <see cref="CloneGuiCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="UpdateToRevision"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="CloneGuiCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public CloneGuiCommand WithUpdateToRevision(RevSpec value)
        {
            UpdateToRevision = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="CompressedTransfer"/> property to the specified value and
        /// returns this <see cref="CloneGuiCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="CompressedTransfer"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="CloneGuiCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public CloneGuiCommand WithCompressedTransfer(bool value)
        {
            CompressedTransfer = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="IncludeRevision"/> property to the specified value and
        /// returns this <see cref="CloneGuiCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="IncludeRevision"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="CloneGuiCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public CloneGuiCommand WithIncludeRevision(RevSpec value)
        {
            IncludeRevision = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="Branch"/> property to the specified value and
        /// returns this <see cref="CloneGuiCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Branch"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="CloneGuiCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public CloneGuiCommand WithBranch(string value)
        {
            Branch = value;
            return this;
        }

        /// <summary>
        /// Validates the command configuration. This method should throw the necessary
        /// exceptions to signal missing or incorrect configuration (like attempting to
        /// add files to the repository without specifying which files to add.)
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The 'clone' command requires <see cref="Source"/> to be specified.
        /// </exception>
        public override void Validate()
        {
            base.Validate();

            if (StringEx.IsNullOrWhiteSpace(Source))
                throw new InvalidOperationException("The 'clone' command requires Source to be specified");
        }
    }
}