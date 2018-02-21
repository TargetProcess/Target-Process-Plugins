using System;
using System.Collections;
using System.Collections.Generic;
using Tp.Core.Annotations;

namespace Tp.Core
{
    /// <summary>
    /// Provides methods for verification of argument preconditions.
    /// </summary>
    public static class Argument
    {
        /// <summary>
        /// Verifies that a given argument value is not <c>null</c> and returns the value provided.
        /// </summary>
        /// <typeparam name="T">Type of the <paramref name="name" />.</typeparam>
        /// <param name="name">Argument name.</param>
        /// <param name="value">Argument value.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is <c>null</c>.</exception>
        /// <returns><paramref name="value"/> if it is not <c>null</c>.</returns>
        [NotNull]
        public static T NotNull<T>([InvokerParameterName] string name, T value) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(name);
            }

            return value;
        }

        /// <summary>
        /// Verifies that a given argument value is not <c>null</c> and returns the value provided.
        /// </summary>
        /// <typeparam name="T">Type of the <paramref name="name" />.</typeparam>
        /// <param name="name">Argument name.</param>
        /// <param name="value">Argument value.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is <c>null</c>.</exception>
        /// <returns><paramref name="value"/> if it is not <c>null</c>.</returns>
        public static T NotNull<T>([InvokerParameterName] string name, T? value) where T : struct
        {
            if (value == null)
            {
                throw new ArgumentNullException(name);
            }

            return value.Value;
        }

        /// <summary>
        /// Verifies that a given argument value is not <c>null</c> or empty and returns the value provided.
        /// </summary>
        /// <param name="name">Argument name.</param>
        /// <param name="value">Argument value.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is empty.</exception>
        /// <returns><paramref name="value"/> if it is not <c>null</c> or empty.</returns>
        [NotNull]
        public static string NotNullOrEmpty([InvokerParameterName] string name, string value)
        {
            Argument.NotNull(name, value);

            if (value.Length == 0)
            {
                throw new ArgumentException("Value can not be empty.", name);
            }

            return value;
        }

        /// <summary>
        /// Verifies that a given argument value is not <c>null</c> or empty and returns the value provided.
        /// </summary>
        /// <param name="name">Argument name.</param>
        /// <param name="value">Argument value.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is empty.</exception>
        /// <returns><paramref name="value"/> if it is not <c>null</c> or empty.</returns>
        [NotNull]
        public static T[] NotNullOrEmpty<T>([InvokerParameterName] string name, T[] value)
        {
            Argument.NotNull(name, value);

            if (value.Length == 0)
            {
                throw new ArgumentException("Value can not be empty.", name);
            }

            return value;
        }

        /// <summary>
        /// Verifies that a given argument value is not <c>null</c> or empty and returns the value provided.
        /// </summary>
        /// <param name="name">Argument name.</param>
        /// <param name="value">Argument value.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is empty.</exception>
        /// <returns><paramref name="value"/> if it is not <c>null</c> or empty.</returns>
        [NotNull]
        public static IReadOnlyCollection<T> NotNullOrEmpty<T>([InvokerParameterName] string name, IReadOnlyCollection<T> value)
        {
            Argument.NotNull(name, value);

            if (value.Count == 0)
            {
                throw new ArgumentException("Value can not be empty.", name);
            }

            return value;
        }


        /// <summary>
        /// Verifies that a given argument value is not <c>null</c> or empty and returns the value provided.
        /// </summary>
        /// <param name="name">Argument name.</param>
        /// <param name="value">Argument value.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is empty.</exception>
        /// <returns><paramref name="value"/> if it is not <c>null</c> or empty.</returns>
        [NotNull]
        public static IReadOnlyList<T> NotNullOrEmpty<T>([InvokerParameterName] string name, IReadOnlyList<T> value)
        {
            Argument.NotNull(name, value);

            if (value.Count == 0)
            {
                throw new ArgumentException("Value can not be empty.", name);
            }

            return value;
        }

        private const string PotentialDoubleEnumeration =
            "Using NotNullOrEmpty with plain IEnumerable may cause double enumeration. Please use a collection instead.";

        /// <summary>
        /// (DO NOT USE) Ensures that NotNullOrEmpty can not be used with plain <see cref="IEnumerable"/>,
        /// as this may cause double enumeration.
        /// </summary>
        [Obsolete(PotentialDoubleEnumeration, true)]
        public static void NotNullOrEmpty([InvokerParameterName] string name, IEnumerable value)
        {
            throw new Exception(PotentialDoubleEnumeration);
        }

        /// <summary>
        /// (DO NOT USE) Ensures that NotNullOrEmpty can not be used with plain <see cref="IEnumerable{T}" />,
        /// as this may cause double enumeration.
        /// </summary>
        [Obsolete(PotentialDoubleEnumeration, true)]
        public static void NotNullOrEmpty<T>([InvokerParameterName] string name, IEnumerable<T> value)
        {
            throw new Exception(PotentialDoubleEnumeration);
        }
    }
}
