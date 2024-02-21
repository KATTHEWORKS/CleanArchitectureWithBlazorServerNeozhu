using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Domain.Common.Entities;
using CleanArchitecture.Blazor.Domain.Identity;
using PublicCommon;

namespace CleanArchitecture.Blazor.Domain.Entities.VotingSystem;
//https://sansad.in/

//public class StateWithConstituencies
//{
//    //todo better to add stateid
//    public string StateName { get; set; }
//    public List<V_Constituency> Constituencies { get; set; }
//    public bool Expanded { get; set; }//for Ui purposes
//}
//dbtable1
public class Constituency(string stateName, string constituency):BaseAuditableEntity //target Topic/Focus details // for small voting case this can be enum but for V_Constituency
{
    public Constituency() : this(default, default)
    {

    }
    public Constituency(string stateName, string constituency, string existingMpName)
        : this(stateName, constituency)
    {
        ExistingMpName = existingMpName;
    }
    public Constituency(string stateName, string constituency, string existingMpName, string description)
       : this(stateName, constituency, existingMpName)
    {
        ExistingMpName = existingMpName;
        Description = description;
    }
    //[Key]
    ////[DatabaseGenerated(DatabaseGeneratedOption.Computed)]//this gives error so using fluentnt API .ValueGeneratedOnAdd();
    //public int Id { get; set; }

    //todo better to add stateid
    [Required]
    public string StateName { get; set; } = stateName;

    [Required]
    public string Name { get; set; } = constituency;
    public string? ExistingMpName { get; set; }
    public string? AlternateMpNames { get; set; }

    public string? Description { get; set; }//extra comment

    public int ReadCount { get; set; } = 0;
    public int WriteCount { get; set; } = 0;

    public virtual V_VoteSummary? Summary { get; set; }//; = new();

}
