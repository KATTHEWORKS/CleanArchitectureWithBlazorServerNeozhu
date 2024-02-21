// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace CleanArchitecture.Blazor.Infrastructure.PermissionSet;

public static partial class Permissions
{
    [DisplayName("Constituencies")]
    [Description("Constituencies Permissions")]
    public static class Constituencies
    {
        public const string View = "Permissions.Constituencies.View";
        public const string Create = "Permissions.Constituencies.Create";
        public const string Edit = "Permissions.Constituencies.Edit";
        public const string Delete = "Permissions.Constituencies.Delete";
        public const string Search = "Permissions.Constituencies.Search";
        public const string Export = "Permissions.Constituencies.Export";
        public const string Import = "Permissions.Constituencies.Import";
    }
}

