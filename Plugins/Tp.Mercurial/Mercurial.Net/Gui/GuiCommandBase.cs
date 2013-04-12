using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Mercurial.Attributes;

namespace Mercurial.Gui
{
    /// <summary>
    /// This is the base class for option classes for various commands for
    /// the TortoiseHg client.
    /// </summary>
    /// <typeparam name="T">
    /// The actual type descending from <see cref="GuiCommandBase{T}"/>, used to generate type-correct
    /// methods in this base class.
    /// </typeparam>
    public abstract class GuiCommandBase<T> : CommandBase<T>, IGuiCommand
        where T : GuiCommandBase<T>
    {
        /// <summary>
        /// This is the backing field for the <see cref="WaitForGuiToClose"/> property.
        /// </summary>
        private bool _WaitForGuiToClose = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuiCommandBase{T}"/> class.
        /// </summary>
        /// <param name="command">
        /// The name of the command that will be passed to the TortoiseHg command line client.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="command"/> is <c>null</c> or empty.</para>
        /// </exception>
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "This property is safe to set from this point.")]
        protected GuiCommandBase(string command)
            : base(command)
        {
// ReSharper disable DoNotCallOverridableMethodsInConstructor
            Timeout = 0;
// ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        /// <summary>
        /// Gets or sets the timeout to use when executing Mercurial commands, in seconds.
        /// Default value for all Gui commands is <c>0</c>, which translates to infinite timeout.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <para><see cref="CommandBase{T}.Timeout"/> cannot be less than 0.</para>
        /// </exception>
        [DefaultValue(0)]
        public override int Timeout
        {
            get
            {
                return base.Timeout;
            }

            set
            {
                base.Timeout = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the given command is available.
        /// </summary>
        /// <remarks>
        /// The command might not be available if the "wrong" type of client is installed (PyGTK vs. PyQT version),
        /// or the "wrong" version is installed (for instance if a command was introduce in a version, and an older
        /// version is currently installed.)
        /// </remarks>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "Intended usage through descendants, which already has the correct generic type specified.")]
        public static bool IsAvailable
        {
            get
            {
                // if no attribute applied, it is available on all clients
                if (!typeof(T).IsDefined(typeof(GuiClientRequirementAttribute), true))
                    return true;

                return typeof(T)
                    .GetCustomAttributes(typeof(GuiClientRequirementAttribute), true)
                    .Cast<GuiClientRequirementAttribute>()
                    .Any(attr => attr.AreRequirementsMet);
            }
        }

        /// <summary>
        /// This method will throw an <see cref="InvalidOperationException"/> if the command is not available
        /// in the installed client.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The command is not available in the installed client.
        /// </exception>
        protected static void EnsureCommandAvailability()
        {
            if (!IsAvailable)
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "The command {0} is not available in the installed client", typeof(T).Name));
        }

        /// <summary>
        /// This method will throw a <see cref="InvalidOperationException"/> if the "wrong" client type is installed, to
        /// signal that a given property on a command object can only be used from a specific client type.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property.
        /// </param>
        /// <param name="requiredClientType">
        /// The required client type.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// The property is not available in the installed client.
        /// </exception>
        protected static void EnsurePropertyAvailability(string propertyName, GuiClientType requiredClientType)
        {
            if (GuiClient.ClientType != requiredClientType)
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "The property {0} is not available in the installed client", propertyName));
        }

        /// <summary>
        /// Validates the command configuration. This method should throw the necessary
        /// exceptions to signal missing or incorrect configuration (like attempting to
        /// add files to the repository without specifying which files to add.)
        /// </summary>
        public override void Validate()
        {
            base.Validate();
            EnsureCommandAvailability();
        }

        /// <summary>
        /// Gets or sets a value indicating whether to wait for the Gui window to close before returning from the execution method.
        /// Default value is <c>true</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--nofork")]
        [DefaultValue(true)]
        public bool WaitForGuiToClose
        {
            get
            {
                return _WaitForGuiToClose;
            }

            set
            {
                _WaitForGuiToClose = value;
            }
        }

        /// <summary>
        /// Sets the <see cref="WaitForGuiToClose"/> property to the specified value and
        /// returns this command instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="WaitForGuiToClose"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This command instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public T WithWaitForGuiToClose(bool value)
        {
            WaitForGuiToClose = value;
            return (T)this;
        }
    }
}