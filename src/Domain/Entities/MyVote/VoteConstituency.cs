using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using PublicCommon;

namespace Domain.MyVote;
//https://sansad.in/

//public class StateWithConstituencies
//{
//    //todo better to add stateid
//    public string StateName { get; set; }
//    public List<V_Constituency> Constituencies { get; set; }
//    public bool Expanded { get; set; }//for Ui purposes
//}
//dbtable1
public class VoteConstituency(string stateName, string constituencyName) : BaseAuditableEntity //target Topic/Focus details // for small voting case this can be enum but for V_Constituency
{
    public VoteConstituency() : this(default, default)
    {

    }
    public VoteConstituency(string stateName, string constituencyName, string currentMemberName)
        : this(stateName, constituencyName)
    {
        CurrentMemberName = currentMemberName;
    }
    public VoteConstituency(string stateName, string constituencyName, string currentMemberName, string description)
       : this(stateName, constituencyName, currentMemberName)
    {
        CurrentMemberName = currentMemberName;
        Description = description;
    }
    //[Key]
    ////[DatabaseGenerated(DatabaseGeneratedOption.Computed)]//this gives error so using fluentnt API .ValueGeneratedOnAdd();
    //public int Id { get; set; }

    //todo better to add stateid
    [Required]
    public string State { get; set; } = stateName;

    [Required]
    public string Name { get; set; } = constituencyName;

    public string? Description { get; set; }//extra message


    public string? CurrentMemberName { get; set; }
    public string? CurrentMemberParty { get; set; }
    public string? CurrentMemberTerms { get; set; }


    //later make this as json of //name+party+terms
    public string? MemberNamesEarlierOthers { get; set; }
    //name+party+terms



    public int ReadsCount { get; set; } = 0;//users read count of constituency,add frm cache

    public virtual VoteSummary? Summary { get; set; }//; = new();

}
