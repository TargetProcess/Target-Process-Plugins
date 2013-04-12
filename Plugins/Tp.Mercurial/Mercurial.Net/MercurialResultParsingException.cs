using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Mercurial
{
    /// <summary>
    /// Represents a problem with the results returned from executing the Mercurial command, in the sense
    /// that the output text did not match any known format that was expected.
    /// </summary>
    [Serializable]
    public class MercurialResultParsingException : MercurialExecutionException
    {
        /// <summary>
        /// This is the backing field for the <see cref="Output"/> property.
        /// </summary>
        private readonly string _Output = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="MercurialResultParsingException"/> class.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="SerializationException">
        /// The class name is null or <see cref="Exception.HResult"/> is zero (0).
        /// </exception>
        protected MercurialResultParsingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _Output = info.GetString("_Output");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref = "MercurialResultParsingException" /> class
        /// s with a specified error message and a reference to the inner exception
        /// that is the cause of this exception.
        /// </summary>
        /// <param name="exitCode">
        /// The exit code from executing the Mercurial command line client executable.
        /// </param>
        /// <param name="message">
        /// The error message that explains the reason for the exception. 
        /// </param>
        /// <param name="output">
        /// The output from the mercurial executable, unparsed/unformatted.
        /// </param>
        public MercurialResultParsingException(int exitCode, string message, string output)
            : base(exitCode, message)
        {
            _Output = output;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref = "MercurialResultParsingException" /> class
        /// s with a specified error message and a reference to the inner exception
        /// that is the cause of this exception.
        /// </summary>
        /// <param name="exitCode">
        /// The exit code from executing the Mercurial command line client executable.
        /// </param>
        /// <param name="message">
        /// The error message that explains the reason for the exception. 
        /// </param>
        /// <param name="output">
        /// The output from the mercurial executable, unparsed/unformatted.
        /// </param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception,
        /// or a <c>null</c> reference (<c>Nothing</c> in Visual Basic)
        /// if no inner exception is specified. 
        /// </param>
        public MercurialResultParsingException(int exitCode, string message, string output, Exception innerException)
            : base(exitCode, message, innerException)
        {
            _Output = output;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MercurialResultParsingException"/> class.
        /// </summary>
        public MercurialResultParsingException()
        {
            // Do nothing here
        }

        /// <summary>
        /// Initializes a new instance of the <see cref = "MercurialResultParsingException" /> class
        /// with a specific error message.
        /// </summary>
        /// <param name="message">The message.</param>
        public MercurialResultParsingException(string message)
            : base(message)
        {
            // Do nothing here
        }

        /// <summary>
        /// Initializes a new instance of the <see cref = "MercurialResultParsingException" /> class
        /// with a specific error message.
        /// </summary>
        /// <param name="exitCode">
        /// The exit code from executing the Mercurial command line client executable.
        /// </param>
        /// <param name="message">The message.</param>
        public MercurialResultParsingException(int exitCode, string message)
            : base(exitCode, message)
        {
            // Do nothing here
        }

        /// <summary>
        /// Initializes a new instance of the <see cref = "MercurialResultParsingException" /> class
        /// s with a specified error message and a reference to the inner exception
        /// that is the cause of this exception.
        /// </summary>
        /// <param name="message">
        /// The error message that explains the reason for the exception. 
        /// </param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception,
        /// or a <c>null</c> reference (<c>Nothing</c> in Visual Basic)
        /// if no inner exception is specified. 
        /// </param>
        public MercurialResultParsingException(string message, Exception innerException)
            : base(message, innerException)
        {
            // Do nothing here
        }

        /// <summary>
        /// Initializes a new instance of the <see cref = "MercurialResultParsingException" /> class
        /// s with a specified error message and a reference to the inner exception
        /// that is the cause of this exception.
        /// </summary>
        /// <param name="exitCode">
        /// The exit code from executing the Mercurial command line client executable.
        /// </param>
        /// <param name="message">
        /// The error message that explains the reason for the exception. 
        /// </param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception,
        /// or a <c>null</c> reference (<c>Nothing</c> in Visual Basic)
        /// if no inner exception is specified. 
        /// </param>
        public MercurialResultParsingException(int exitCode, string message, Exception innerException)
            : base(exitCode, message, innerException)
        {
            // Do nothing here
        }

        /// <summary>
        /// When overridden in a derived class, sets the <see cref = "T:System.Runtime.Serialization.SerializationInfo" /> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref = "T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref = "T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        /// <exception cref = "T:System.ArgumentNullException">The <paramref name = "info" /> parameter is a null reference (Nothing in Visual Basic). </exception>
        /// <PermissionSet>
        /// <IPermission class = "System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version = "1" Read = "*AllFiles*" PathDiscovery = "*AllFiles*" />
        /// <IPermission class = "System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version = "1" Flags = "SerializationFormatter" />
        /// </PermissionSet>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("_Output", _Output);
        }

        /// <summary>
        /// Gets the output from executing the mercurial command, the text that was in an unknown format.
        /// </summary>
        public string Output
        {
            get
            {
                return _Output;
            }
        }
    }
}