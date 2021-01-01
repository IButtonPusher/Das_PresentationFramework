﻿// <auto-generated>
//   This code file has automatically been added by the "Nullable" NuGet package (https://www.nuget.org/packages/Nullable).
//   Please see https://github.com/manuelroemer/Nullable for more information.
//
//   IMPORTANT:
//   DO NOT DELETE THIS FILE if you are using a "packages.config" file to manage your NuGet references.
//   Consider migrating to PackageReferences instead:
//   https://docs.microsoft.com/en-us/nuget/consume-packages/migrate-packages-config-to-package-reference
//   Migrating brings the following benefits:
//   * The "Nullable" folder and the "NullableAttributes.cs" files don't appear in your project.
//   * The added files are immutable and can therefore not be modified by coincidence.
//   * Updating/Uninstalling the package will work flawlessly.
// </auto-generated>

#region License
// MIT License
// 
// Copyright (c) 2019 Manuel Römer
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

#if !NULLABLE_ATTRIBUTES_DISABLE
#nullable enable
#pragma warning disable

namespace System.Diagnostics.CodeAnalysis
{
    using global::System;

#if DEBUG
    /// <summary>
    ///     Specifies that <see langword="null"/> is allowed as an input even if the
    ///     corresponding type disallows it.
    /// </summary>
#endif
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, 
        Inherited = false
    )]
#if !NULLABLE_ATTRIBUTES_INCLUDE_IN_CODE_COVERAGE
    [DebuggerNonUserCode]
#endif
    internal sealed class AllowNullAttribute : Attribute
    {
#if DEBUG
        /// <summary>
        ///     Initializes a new instance of the <see cref="AllowNullAttribute"/> class.
        /// </summary>
#endif
        public AllowNullAttribute() { }
    }

#if DEBUG
    /// <summary>
    ///     Specifies that <see langword="null"/> is disallowed as an input even if the
    ///     corresponding type allows it.
    /// </summary>
#endif
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, 
        Inherited = false
    )]
#if !NULLABLE_ATTRIBUTES_INCLUDE_IN_CODE_COVERAGE
    [DebuggerNonUserCode]
#endif
    internal sealed class DisallowNullAttribute : Attribute
    {
#if DEBUG
        /// <summary>
        ///     Initializes a new instance of the <see cref="DisallowNullAttribute"/> class.
        /// </summary>
#endif
        public DisallowNullAttribute() { }
    }

#if DEBUG
    /// <summary>
    ///     Specifies that a method that will never return under any circumstance.
    /// </summary>
#endif
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
#if !NULLABLE_ATTRIBUTES_INCLUDE_IN_CODE_COVERAGE
    [DebuggerNonUserCode]
#endif
    internal sealed class DoesNotReturnAttribute : Attribute
    {
#if DEBUG
        /// <summary>
        ///     Initializes a new instance of the <see cref="DoesNotReturnAttribute"/> class.
        /// </summary>
        ///
#endif
        public DoesNotReturnAttribute() { }
    }

#if DEBUG
    /// <summary>
    ///     Specifies that the method will not return if the associated <see cref="Boolean"/>
    ///     parameter is passed the specified value.
    /// </summary>
#endif
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
#if !NULLABLE_ATTRIBUTES_INCLUDE_IN_CODE_COVERAGE
    [DebuggerNonUserCode]
#endif
    internal sealed class DoesNotReturnIfAttribute : Attribute
    {
#if DEBUG
        /// <summary>
        ///     Gets the condition parameter value.
        ///     Code after the method is considered unreachable by diagnostics if the argument
        ///     to the associated parameter matches this value.
        /// </summary>
#endif
        public bool ParameterValue { get; }

#if DEBUG
        /// <summary>
        ///     Initializes a new instance of the <see cref="DoesNotReturnIfAttribute"/>
        ///     class with the specified parameter value.
        /// </summary>
        /// <param name="parameterValue">
        ///     The condition parameter value.
        ///     Code after the method is considered unreachable by diagnostics if the argument
        ///     to the associated parameter matches this value.
        /// </param>
#endif
        public DoesNotReturnIfAttribute(bool parameterValue)
        {
            ParameterValue = parameterValue;
        }
    }

#if DEBUG
    /// <summary>
    ///     Specifies that an output may be <see langword="null"/> even if the
    ///     corresponding type disallows it.
    /// </summary>
#endif
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Parameter | 
        AttributeTargets.Property | AttributeTargets.ReturnValue, 
        Inherited = false
    )]
#if !NULLABLE_ATTRIBUTES_INCLUDE_IN_CODE_COVERAGE
    [DebuggerNonUserCode]
#endif
    internal sealed class MaybeNullAttribute : Attribute
    {
#if DEBUG
        /// <summary>
        ///     Initializes a new instance of the <see cref="MaybeNullAttribute"/> class.
        /// </summary>
#endif
        public MaybeNullAttribute() { }
    }

#if DEBUG
    /// <summary>
    ///     Specifies that when a method returns <see cref="ReturnValue"/>, 
    ///     the parameter may be <see langword="null"/> even if the corresponding type disallows it.
    /// </summary>
#endif
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
#if !NULLABLE_ATTRIBUTES_INCLUDE_IN_CODE_COVERAGE
    [DebuggerNonUserCode]
#endif
    internal sealed class MaybeNullWhenAttribute : Attribute
    {
#if DEBUG
        /// <summary>
        ///     Gets the return value condition.
        ///     If the method returns this value, the associated parameter may be <see langword="null"/>.
        /// </summary>
#endif
        public bool ReturnValue { get; }

#if DEBUG
        /// <summary>
        ///      Initializes the attribute with the specified return value condition.
        /// </summary>
        /// <param name="returnValue">
        ///     The return value condition.
        ///     If the method returns this value, the associated parameter may be <see langword="null"/>.
        /// </param>
#endif
        public MaybeNullWhenAttribute(bool returnValue)
        {
            ReturnValue = returnValue;
        }
    }

#if DEBUG
    /// <summary>
    ///     Specifies that an output is not <see langword="null"/> even if the
    ///     corresponding type allows it.
    /// </summary>
#endif
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Parameter | 
        AttributeTargets.Property | AttributeTargets.ReturnValue, 
        Inherited = false
    )]
#if !NULLABLE_ATTRIBUTES_INCLUDE_IN_CODE_COVERAGE
    [DebuggerNonUserCode]
#endif
    internal sealed class NotNullAttribute : Attribute
    {
#if DEBUG
        /// <summary>
        ///     Initializes a new instance of the <see cref="NotNullAttribute"/> class.
        /// </summary>
#endif
        public NotNullAttribute() { }
    }

#if DEBUG
    /// <summary>
    ///     Specifies that the output will be non-<see langword="null"/> if the
    ///     named parameter is non-<see langword="null"/>.
    /// </summary>
#endif
    [AttributeUsage(
        AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, 
        AllowMultiple = true, 
        Inherited = false
    )]
#if !NULLABLE_ATTRIBUTES_INCLUDE_IN_CODE_COVERAGE
    [DebuggerNonUserCode]
#endif
    internal sealed class NotNullIfNotNullAttribute : Attribute
    {
#if DEBUG
        /// <summary>
        ///     Gets the associated parameter name.
        ///     The output will be non-<see langword="null"/> if the argument to the
        ///     parameter specified is non-<see langword="null"/>.
        /// </summary>
#endif
        public string ParameterName { get; }

#if DEBUG
        /// <summary>
        ///     Initializes the attribute with the associated parameter name.
        /// </summary>
        /// <param name="parameterName">
        ///     The associated parameter name.
        ///     The output will be non-<see langword="null"/> if the argument to the
        ///     parameter specified is non-<see langword="null"/>.
        /// </param>
#endif
        public NotNullIfNotNullAttribute(string parameterName)
        {
            // .NET Core 3.0 doesn't throw an ArgumentNullException, even though this is
            // tagged as non-null.
            // Follow this behavior here.
            ParameterName = parameterName;
        }
    }

#if DEBUG
    /// <summary>
    ///     Specifies that when a method returns <see cref="ReturnValue"/>,
    ///     the parameter will not be <see langword="null"/> even if the corresponding type allows it.
    /// </summary>
#endif
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
#if !NULLABLE_ATTRIBUTES_INCLUDE_IN_CODE_COVERAGE
    [DebuggerNonUserCode]
#endif
    internal sealed class NotNullWhenAttribute : Attribute
    {
#if DEBUG
        /// <summary>
        ///     Gets the return value condition.
        ///     If the method returns this value, the associated parameter will not be <see langword="null"/>.
        /// </summary>
#endif
        public bool ReturnValue { get; }

#if DEBUG
        /// <summary>
        ///     Initializes the attribute with the specified return value condition.
        /// </summary>
        /// <param name="returnValue">
        ///     The return value condition.
        ///     If the method returns this value, the associated parameter will not be <see langword="null"/>.
        /// </param>
#endif
        public NotNullWhenAttribute(bool returnValue)
        {
            ReturnValue = returnValue;
        }
    }
}

#pragma warning enable
#nullable restore
#endif // NULLABLE_ATTRIBUTES_DISABLE