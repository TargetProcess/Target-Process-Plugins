// 
// Copyright (c) 2005-2008 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;

namespace Tp.Integration.Common
{
    /// <summary>
    /// It is a marker for DTO relation name properties. Only for system usage.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    [Serializable]
    public class RelationNameAttribute : Attribute
    {}
}