// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace CleanArchitecture.Blazor.Infrastructure.PermissionSet;

public static partial class Permissions
{
    [DisplayName("Votes")]
    [Description("Votes Permissions")]
    public static class Votes
    {
        public const string View = "Permissions.Votes.View";
        public const string Create = "Permissions.Votes.Create";
        public const string Edit = "Permissions.Votes.Edit";
        public const string Delete = "Permissions.Votes.Delete";
        public const string Search = "Permissions.Votes.Search";
        public const string Export = "Permissions.Votes.Export";
        public const string Import = "Permissions.Votes.Import";
    }
}

