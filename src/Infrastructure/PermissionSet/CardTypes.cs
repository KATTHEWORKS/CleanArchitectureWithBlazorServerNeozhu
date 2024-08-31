// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace CleanArchitecture.Blazor.Infrastructure.PermissionSet;

public static partial class Permissions
{
    [DisplayName("CardTypes")]
    [Description("CardTypes Permissions")]
    public static class CardTypes
    {
        public const string View = "Permissions.CardTypes.View";
        public const string Create = "Permissions.CardTypes.Create";
        public const string Edit = "Permissions.CardTypes.Edit";
        public const string Delete = "Permissions.CardTypes.Delete";
        public const string Search = "Permissions.CardTypes.Search";
        public const string Export = "Permissions.CardTypes.Export";
        public const string Import = "Permissions.CardTypes.Import";
    }
}

