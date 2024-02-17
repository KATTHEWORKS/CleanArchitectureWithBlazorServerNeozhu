//#define HOSPITAL_SYSTEM
#define VOTING_SYSTEM

//#undef HOSPITAL_SYSTEM
//#undef VOTING_SYSTEM
using System.ComponentModel;
using static System.Net.Mime.MediaTypeNames;

namespace CleanArchitecture.Blazor.Domain.Enums;

public enum TenantTypeDefaultEnum : byte
{//mORE IS HIGHEST PREVILEGE... always maintain Unique values
    //[Description(nameof(Next))]
    //Next = 3,
    [Description("NEXT")]
    Default = 3,//=Next,
    //above are for Next-country-state

    //below is internal common for all
    Internal = 50,
    RejectedOrBlocked = 0,
    [Description(nameof(Guest))]
    Guest = 1,
}

#if HOSPITAL_SYSTEM
public enum TenantTypeEnum : byte
{//mORE IS HIGHEST PREVILEGE... always maintain Unique values
    
    //below are for Hospital

    //[Description(nameof(Patient))]
    //Patient = 4,
    [Description("Patient")]
    Default=4,//=Patient,

    [Description("New Request for creating HOSPITAL/PHARMACY/DIAGNOSTICCENTER")]
    NewTenant = 6,

    [Description("PHARMACY & Staff related")]
    PharmacyAndStaff = 15,

    [Description("DIAGNOSTICIAN center & Staff related")]
    DiagnosticsAndStaff = 20,

    [Description("HOSPITAL & All Staff related")]
    HospitalAndStaff = 30,
    //above are for Hospital

    //below is internal common for all
    Internal = 50,
    RejectedOrBlocked = 0,
    [Description(nameof(Guest))]
    Guest = 1,
}
#elif VOTING_SYSTEM
public enum TenantTypeEnum : byte
{//mORE IS HIGHEST PREVILEGE... always maintain Unique values
    //[Description(nameof(Next))]
    //Next = 3,
    [Description("NEXT")]
    Default=3,//=Next,
    //above are for Next-country-state

    //below is internal common for all
    Internal = 50,
    RejectedOrBlocked = 0,
    [Description(nameof(Guest))]
    Guest = 1,
}
#else
public enum TenantTypeEnum : byte
{//mORE IS HIGHEST PREVILEGE... always maintain Unique values
    //[Description(nameof(Next))]
    //Next = 3,
    [Description("NEXT")]
    Default = TenantTypeDefaultEnum.Default,//=Next,
    //above are for Next-country-state

    //below is internal common for all
    Internal = TenantTypeDefaultEnum.Internal,
    RejectedOrBlocked = TenantTypeDefaultEnum.RejectedOrBlocked,
    [Description(nameof(Guest))]
    Guest = TenantTypeDefaultEnum.Guest,
}
#endif
public static class TenantType
{
    public static List<TenantTypeEnum> GetAll()
    {
        return PublicCommon.EnumExtensions.GetEnumValues<TenantTypeEnum>();
        //        return [TenantTypeEnum.RejectedOrBlocked, TenantTypeEnum.Guest, TenantTypeEnum.Patient, TenantTypeEnum.NewTenant,
        //        TenantTypeEnum.PharmacyAndStaff, TenantTypeEnum.DiagnosticsAndStaff, TenantTypeEnum.HospitalAndStaff,
        //TenantTypeEnum.Internal];
    }

}
