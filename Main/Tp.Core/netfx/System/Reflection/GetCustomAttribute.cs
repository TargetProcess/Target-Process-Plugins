#region BSD License
/* 
Copyright (c) 2011, NETFx
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, 
are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this list 
  of conditions and the following disclaimer.

* Redistributions in binary form must reproduce the above copyright notice, this 
  list of conditions and the following disclaimer in the documentation and/or other 
  materials provided with the distribution.

* Neither the name of Clarius Consulting nor the names of its contributors may be 
  used to endorse or promote products derived from this software without specific 
  prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY 
EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES 
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT 
SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, 
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED 
TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR 
BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH 
DAMAGE.
*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Tp.Core;

/// <summary>
/// Allows retrieving custom attributes from assemblies, types, methods, properties, etc. using a generic method.
/// </summary>
///	<nuget id="netfx-System.Reflection.GetCustomAttribute" />
public static partial class GetCustomAttributeExtension
{
	/// <summary>
	/// Retrieves the first defined attribute of the given type from the provider if any.
	/// </summary>
	/// <typeparam name="TAttribute">Type of the attribute, which must inherit from <see cref="Attribute"/>.</typeparam>
	/// <param name="provider" this="true">Any type implementing the interface, which can be an assembly, type, 
	/// property, method, etc.</param>
	/// <param name="inherit">Optionally, whether the attribute will be looked in base classes.</param>
	/// <returns>The attribute instance if defined; <see langword="null"/> otherwise.</returns>
	public static Maybe<TAttribute> GetCustomAttribute<TAttribute>(this ICustomAttributeProvider provider, bool inherit = true)
		where TAttribute : Attribute
	{
		return GetCustomAttributes<TAttribute>(provider, inherit).FirstOrNothing();
	}

	/// <summary>
	/// Retrieves the first defined attribute of the given type from the provider if any.
	/// </summary>
	/// <typeparam name="TAttribute">Type of the attribute, which must inherit from <see cref="Attribute"/>.</typeparam>
	/// <param name="provider" this="true">Any type implementing the interface, which can be an assembly, type, 
	/// property, method, etc.</param>
	/// <param name="inherit">Optionally, whether the attribute will be looked in base classes.</param>
	/// <returns>The attribute instance if defined; <see langword="null"/> otherwise.</returns>
	public static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(this ICustomAttributeProvider provider, bool inherit = true)
		where TAttribute : Attribute
	{
		Guard.NotNull(() => provider, provider);

		return provider
			.GetCustomAttributes(typeof(TAttribute), inherit)
			.Cast<TAttribute>();
	}


	public static Maybe<TAttribute> GetAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
	{
		return GetAttributes<TAttribute>(value).FirstOrNothing();
	}

	public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this Enum value) where TAttribute : Attribute
	{
		var type = value.GetType();
		var name = Enum.GetName(type, value);
		return type.GetField(name) // I prefer to get attributes this way
			.GetCustomAttributes(true)
			.OfType<TAttribute>();
	}

	public static TAttribute GetCustomAttributeOrThrow<TAttribute>(this Type type, bool inherit = true)
		where TAttribute : Attribute
	{
		return GetCustomAttributes<TAttribute>(type, inherit).FirstOrNothing().GetOrThrow(() => new InvalidOperationException("There is no {0} attribute on {1}".Fmt(typeof(TAttribute).Name, type.Name)));
	}
}
