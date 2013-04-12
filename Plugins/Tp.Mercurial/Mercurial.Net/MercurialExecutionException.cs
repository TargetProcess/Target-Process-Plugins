using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Mercurial
{
    /// <summary>
    /// Represents errors related to Mercurial.
    /// </summary>
    [Serializable]
    public class MercurialExecutionException : MercurialException
    {
        /// <summary>
        /// This is the backing field for the <see cref="ExitCode"/> property.
        /// </summary>
        private readonly int _ExitCode;

        /// <summary>
        /// Initializes a new instance of the <see cref = "MercurialExecutionException" /> class.
        /// </summary>
        /// <param name="info">The <see cref = "T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref = "T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        /// <exception cref = "T:System.ArgumentNullException">
        /// The <paramref name = "info" /> parameter is null.
        /// </exception>
        /// <exception cref = "T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref = "P:System.Exception.HResult" /> is zero (0).
        /// </exception>
        protected MercurialExecutionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _ExitCode = info.GetInt32("_ExitCode");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref = "MercurialExecutionException" /> class.
        /// </summary>
        public MercurialExecutionException()
        {
            // Do nothing here
        }

        /// <summary>
        /// Initializes a new instance of the <see cref = "MercurialExecutionException" /> class
        /// with a specific error message.
        /// </summary>
        /// <param name="message">The message.</param>
        public MercurialExecutionException(string message)
            : base(message)
        {
            // Do nothing here
        }

        /// <summary>
        /// Initializes a new instance of the <see cref = "MercurialExecutionException" /> class
        /// with a specific error message.
        /// </summary>
        /// <param name="exitCode">
        /// The exit code from executing the Mercurial command line client executable.
        /// </param>
        /// <param name="message">The message.</param>
        public MercurialExecutionException(int exitCode, string message)
            : base(message)
        {
            _ExitCode = exitCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref = "MercurialExecutionException" /> class
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
        public MercurialExecutionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref = "MercurialExecutionException" /> class
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
        public MercurialExecutionException(int exitCode, string message, Exception innerException)
            : base(message, innerException)
        {
            _ExitCode = exitCode;
        }

        /// <summary>
        /// Gets the exit code of the process that was executed and failed.
        /// </summary>
        public int ExitCode
        {
            get
            {
                return _ExitCode;
            }
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
            info.AddValue("_ExitCode", ExitCode);
        }
    }
}