// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace CleanArchitecture.Blazor.Infrastructure.PermissionSet;

public static partial class Permissions
{
    [DisplayName("Towns")]
    [Description("Towns Permissions")]
    public static class Towns
    {
        public const string View = "Permissions.Towns.View";
        public const string Create = "Permissions.Towns.Create";
        public const string Edit = "Permissions.Towns.Edit";
        public const string Delete = "Permissions.Towns.Delete";
        public const string Search = "Permissions.Towns.Search";
        public const string Export = "Permissions.Towns.Export";
        public const string Import = "Permissions.Towns.Import";
    }
}

