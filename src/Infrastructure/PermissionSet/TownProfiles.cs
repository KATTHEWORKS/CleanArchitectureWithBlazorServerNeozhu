// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace CleanArchitecture.Blazor.Infrastructure.PermissionSet;

public static partial class Permissions
{
    [DisplayName("TownProfiles")]
    [Description("TownProfiles Permissions")]
    public static class TownProfiles
    {
        public const string View = "Permissions.TownProfiles.View";
        public const string Create = "Permissions.TownProfiles.Create";
        public const string Edit = "Permissions.TownProfiles.Edit";
        public const string Delete = "Permissions.TownProfiles.Delete";
        public const string Search = "Permissions.TownProfiles.Search";
        public const string Export = "Permissions.TownProfiles.Export";
        public const string Import = "Permissions.TownProfiles.Import";
    }
}

