//#define HOSPITAL_SYSTEM
#define VOTING_SYSTEM
//#undef HOSPITAL_SYSTEM
//#undef VOTING_SYSTEM
using System;
using System.Collections.Generic;
using System.ComponentModel;

//using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Domain.Enums;

//TODO had to change to class in whcih seaparate for  VoterRoles 
/*
 CommonRoles
 InternRoles
HospitalRoles will have Internal+PublicCommon
VoterRoles will have Internal+PublicCommon
StitchRoles will have Internal+PublicCommon
RepairRoles will have Internal+PublicCommon
  */
#if HOSPITAL_SYSTEM
public enum RoleNamesEnum : byte
{//mORE IS HIGHEST PREVILEGE
    [Description("Patient")]
    Default = 4,//PATIENT
                //On UI while displaying show as PATIENT
                //PATIENT = 4,
    ROOTADMIN = 50,
    ELEVATEADMINGROUP = 45,
    ELEVATEADMINVIEWER = 41,

    //below is for hospitals
    HOSPITAL = 30,
    HOSPITALADMIN = 29,
    DOCTORHOD = 28,
    DOCTOR = 27,
    DOCTORASSISTANT = 26,
    NURSE = 25,
    VIEWERHOSPITAL = 21,

    DIAGNOSTICCENTER = 20,
    DIAGNOSTICIAN = 16,

    PHARMACY = 15,
    PHARMACIST = 11,


    //above is for hospitals

    //below are common for all
    [Description(nameof(GUEST))]
    GUEST = 1,
    REJECTED_BLOCKED = 0,
    // public const string GUEST = nameof(GUEST);//vmadhu2023 =>requesting for HOSPITAL registration , vmadhu203=>patient   ,   vmadhu2022 =>req for doctor 
    // DefaultRole1 = PATIENT
    //public const string Basic =nameof(Basic);
    //public const string Users =nameof(Users);
}
#elif VOTING_SYSTEM
public enum RoleNamesEnum : byte
{//mORE IS HIGHEST PREVILEGE
    //On UI while displaying show as Voter
    [Description("Voter")]
    Default =3 ,//VOTER
    //FOR NEXT tenant
    //VOTER = 3,
    ROOTADMIN = 50,
    ELEVATEADMINGROUP = 45,
    ELEVATEADMINVIEWER = 41,



    

    //below are common for all
    [System.ComponentModel.Description(nameof(GUEST))]
    GUEST = 1,
    REJECTED_BLOCKED = 0,
    // public const string GUEST = nameof(GUEST);//vmadhu2023 =>requesting for HOSPITAL registration , vmadhu203=>patient   ,   vmadhu2022 =>req for doctor 
    // DefaultRole1 = PATIENT
    //public const string Basic =nameof(Basic);
    //public const string Users =nameof(Users);
}
#else
public enum RoleNamesEnum : byte
{//mORE IS HIGHEST PREVILEGE
    //On UI while displaying show as Voter
    [Description("Voter")]
    Default =3 ,//VOTER
    //FOR NEXT tenant
    //VOTER = 3,
    ROOTADMIN = 50,
    ELEVATEADMINGROUP = 45,
    ELEVATEADMINVIEWER = 41,



    

    //below are common for all
    [System.ComponentModel.Description(nameof(GUEST))]
    GUEST = 1,
    REJECTED_BLOCKED = 0,
    // public const string GUEST = nameof(GUEST);//vmadhu2023 =>requesting for HOSPITAL registration , vmadhu203=>patient   ,   vmadhu2022 =>req for doctor 
    // DefaultRole1 = PATIENT
    //public const string Basic =nameof(Basic);
    //public const string Users =nameof(Users);
}
#endif

public static class RoleType
{
    public static List<RoleNamesEnum> GetAll()
    {
        return PublicCommon.EnumExtensions.GetEnumValues<RoleNamesEnum>();
    }
#if HOSPITAL_SYSTEM
    public static List<RoleNamesEnum> GetAllAdmins()
    {
        return [RoleNamesEnum.ROOTADMIN, RoleNamesEnum.HOSPITALADMIN, RoleNamesEnum.ELEVATEADMINGROUP, RoleNamesEnum.ELEVATEADMINVIEWER];
    }
#elif VOTING_SYSTEM
    public static List<RoleNamesEnum> GetAllAdmins()
    {
        return [RoleNamesEnum.ROOTADMIN, RoleNamesEnum.ELEVATEADMINGROUP, RoleNamesEnum.ELEVATEADMINVIEWER];
    }
   
#else
    public static List<RoleNamesEnum> GetAllAdmins()
    {
        return [RoleNamesEnum.ROOTADMIN, RoleNamesEnum.ELEVATEADMINGROUP, RoleNamesEnum.ELEVATEADMINVIEWER];
    }
   
#endif

    public static string[] GetAllAdminsAsStringArray()
    {
        return GetAllAdmins().Select(x => x.ToString()).ToArray();
    }
}
public enum PermissionsEnum : byte
{
    Assign,
    UnAssign,
    Read,
    Create,
    Update,
    Delete,
    ReadRestricted,
    CreateRestricted,
    UpdateRestricted,
    DeleteRestricted
}
