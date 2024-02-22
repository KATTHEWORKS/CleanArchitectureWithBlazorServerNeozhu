// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace CleanArchitecture.Blazor.Infrastructure.PermissionSet;

public static partial class Permissions
{
    [DisplayName("VoteSummaries")]
    [Description("VoteSummaries Permissions")]
    public static class VoteSummaries
    {
        public const string View = "Permissions.VoteSummaries.View";
        public const string Create = "Permissions.VoteSummaries.Create";
        public const string Edit = "Permissions.VoteSummaries.Edit";
        public const string Delete = "Permissions.VoteSummaries.Delete";
        public const string Search = "Permissions.VoteSummaries.Search";
        public const string Export = "Permissions.VoteSummaries.Export";
        public const string Import = "Permissions.VoteSummaries.Import";
    }
}

