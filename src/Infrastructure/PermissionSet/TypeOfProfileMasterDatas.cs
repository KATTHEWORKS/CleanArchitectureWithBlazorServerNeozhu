// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace CleanArchitecture.Blazor.Infrastructure.PermissionSet;

public static partial class Permissions
{
    [DisplayName("TypeOfProfileMasterDatas")]
    [Description("TypeOfProfileMasterDatas Permissions")]
    public static class TypeOfProfileMasterDatas
    {
        public const string View = "Permissions.TypeOfProfileMasterDatas.View";
        public const string Create = "Permissions.TypeOfProfileMasterDatas.Create";
        public const string Edit = "Permissions.TypeOfProfileMasterDatas.Edit";
        public const string Delete = "Permissions.TypeOfProfileMasterDatas.Delete";
        public const string Search = "Permissions.TypeOfProfileMasterDatas.Search";
        public const string Export = "Permissions.TypeOfProfileMasterDatas.Export";
        public const string Import = "Permissions.TypeOfProfileMasterDatas.Import";
    }
}

