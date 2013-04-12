using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Mercurial.Attributes;

namespace Mercurial
{
    /// <summary>
    /// This class serves as a base class for the <see cref="MoveCommand"/> and <see cref="RenameCommand"/> classes.
    /// </summary>
    /// <typeparam name="T">
    /// The actual type descending from <see cref="MoveRenameCommandBase{T}"/>, used to generate type-correct
    /// methods in this base class.
    /// </typeparam>
    public abstract class MoveRenameCommandBase<T> : IncludeExcludeCommandBase<T>
        where T : MoveRenameCommandBase<T>
    {
        /// <summary>
        /// This is the backing field for the <see cref="Source"/> property.
        /// </summary>
        private string _Source = string.Empty;

        /// <summary>
        /// This is the backing field for the <see cref="Destination"/> property.
        /// </summary>
        private string _Destination = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveRenameCommandBase{T}"/> class.
        /// </summary>
        /// <param name="command">
        /// The name of the command that will be passed to the Mercurial command line client.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="command"/> is <c>null</c> or empty.</para>
        /// </exception>
        protected MoveRenameCommandBase(string command)
            : base(command)
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets or sets a value indicating whether to record an action that has already occured.
        /// Default value is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// If set to <c>true</c>, only record a move/rename that has already occured, do not try to actually rename or
        /// move an existing file on disk, just manipulate the repository to record that it has
        /// already happened.
        /// </remarks>
        [BooleanArgument(TrueOption = "--after")]
        [DefaultValue(false)]
        public bool RecordOnly
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the value of the <see cref="RecordOnly"/> property to the
        /// specified value and returns this <see cref="RenameCommand"/>  or <see cref="MoveCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The value to set the <see cref="RecordOnly"/> property to.
        /// </param>
        /// <returns>
        /// This <see cref="RenameCommand"/> or <see cref="MoveCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public T WithRecordOnly(bool value)
        {
            RecordOnly = value;
            return (T)this;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to forcibly copy over an existing managed file.
        /// Default value is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--force")]
        [DefaultValue(false)]
        public bool Force
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the value of the <see cref="Force"/> property to the
        /// specified value and returns this <see cref="RenameCommand"/>  or <see cref="MoveCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The value to set the <see cref="Force"/> property to.
        /// </param>
        /// <returns>
        /// This <see cref="RenameCommand"/> or <see cref="MoveCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public T WithForce(bool value)
        {
            Force = value;
            return (T)this;
        }

        /// <summary>
        /// Gets or sets the source of the move/rename command.
        /// Default value is <see cref="string.Empty"/>.
        /// </summary>
        /// <remarks>
        /// This property must be set before the command is executed.
        /// </remarks>
        [DefaultValue("")]
        public string Source
        {
            get
            {
                return _Source;
            }

            set
            {
                _Source = value;
            }
        }

        /// <summary>
        /// Sets the value of the <see cref="Source"/> property to the
        /// specified value and returns this <see cref="RenameCommand"/>  or <see cref="MoveCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The value to set the <see cref="Source"/> property to.
        /// </param>
        /// <returns>
        /// This <see cref="RenameCommand"/> or <see cref="MoveCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public T WithSource(string value)
        {
            Source = value;
            return (T)this;
        }

        /// <summary>
        /// Gets or sets the destination of the move/rename command.
        /// Default value is <see cref="string.Empty"/>.
        /// </summary>
        /// <remarks>
        /// This property must be set before the command is executed.
        /// </remarks>
        [DefaultValue("")]
        public string Destination
        {
            get
            {
                return _Destination;
            }

            set
            {
                _Destination = value;
            }
        }

        /// <summary>
        /// Sets the value of the <see cref="Destination"/> property to the
        /// specified value and returns this <see cref="RenameCommand"/>  or <see cref="MoveCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The value to set the <see cref="Destination"/> property to.
        /// </param>
        /// <returns>
        /// This <see cref="RenameCommand"/> or <see cref="MoveCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public T WithDestination(string value)
        {
            Destination = value;
            return (T)this;
        }

        /// <summary>
        /// Gets all the arguments to the <see cref="CommandBase{T}.Command"/>, or an
        /// empty array if there are none.
        /// </summary>
        public override IEnumerable<string> Arguments
        {
            get
            {
                foreach (string arg in GetBaseArguments())
                    yield return arg;

                yield return Source;
                yield return Destination;
            }
        }

        /// <summary>
        /// Gets the base arguments.
        /// </summary>
        /// <returns>
        /// The contents of the base arguments property, to avoide unverifiable code in <see cref="Arguments"/>.
        /// </returns>
        private IEnumerable<string> GetBaseArguments()
        {
            return base.Arguments;
        }

        /// <summary>
        /// Validates the command configuration. This method should throw the necessary
        /// exceptions to signal missing or incorrect configuration (like attempting to
        /// add files to the repository without specifying which files to add.)
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// <para><see cref="Source"/> is empty.</para>
        /// <para>- or -</para>
        /// <para><see cref="Destination"/> is empty.</para>
        /// </exception>
        public override void Validate()
        {
            base.Validate();

            if (StringEx.IsNullOrWhiteSpace(Source))
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "The Source property must be set before executing a {0}", GetType().Name));
            if (StringEx.IsNullOrWhiteSpace(Destination))
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "The Destination property must be set before executing a {0}", GetType().Name));
        }
    }
}