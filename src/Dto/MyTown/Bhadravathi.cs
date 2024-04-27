using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.MyTown
    {
    public class Bhadravathi
        {
        public static readonly TownDto Data = new TownDto()
            {
            Title = "Bhadravathi",
            UrlName1 = "bhadravathi.com",
            UrlName2 = "bdvt.in",
            SubTitle = "Steel & Iron Rusted City",
            ImageUrl = "https://mytown.blob.core.windows.net/files-of-town/bhadravathi.com/bdvtProblems.gif",

            TownItems = [(TownItemDto)
               new TownBaseDto(ItemTypeCardMasterDataDto.GetFirst("doctor") ,"Kallesh")
                   {SubTitle="Pediatric",Address="Old Town,Bhadravathi",Description="Kids specialist",
                   ImageUrl="https://lh5.googleusercontent.com/p/AF1QipPSgv8ZzUDUx-9sPW-joj-iV7ijS1UyU3-J0kNs=w284-h160-k-no",
                   GoogleMapAddressUrl="https://g.co/kgs/78o8hhL",
                   InstagramUrl="https://www.instagram.com/dr.anilkallesh/",
                    TwitterUrl="https://twitter.com/anilkallesh?lang=en",
                   OtherReferenceUrl="https://www.researchgate.net/profile/Anil-Kallesh"
                   },

                (TownItemDto)
               new TownBaseDto(ItemTypeCardMasterDataDto.GetFirst("doctor"),"Aswath Narayana")
                {SubTitle="Physician",Address="Sujatha Clinic,New Bridge,Bhadravathi",Description="General Medicine",
                    ImageUrl="https://mytown.blob.core.windows.net/files-of-town/bhadravathi.com/UserFiles/DrAshwathNarayanHd.png",
                    GoogleMapAddressUrl="https://maps.app.goo.gl/rtMJyszi9ATfwFUC9",InstagramUrl="https://www.instagram.com/dr.anilkallesh/",
                    //TwitterUrl="https://twitter.com/anilkallesh?lang=en",
                    //OtherReferenceUrl="https://www.ask4healthcare.com/healthcaresolutions/DesktopModules/SearchCommon/ResDoctordetails.aspx?docid=DR/NEM001428"
                    },
                (TownItemDto)
               new TownBaseDto(ItemTypeCardMasterDataDto.GetFirst("doctor"),"Mallikarjun")
                {SubTitle="Pediatric",Address="Old Town,Bhadravathi",Description="Kids specialist",
                    //ImageUrl="https://mytown.blob.core.windows.net/files-of-town/bhadravathi.com/Iron_City.jpg",
                    //GoogleMapAddressUrl="https://g.co/kgs/78o8hhL",
                    //InstagramUrl="https://www.instagram.com/dr.anilkallesh/",
                    //TwitterUrl="https://twitter.com/anilkallesh?lang=en",
                    //OtherReferenceUrl="https://www.ask4healthcare.com/healthcaresolutions/DesktopModules/SearchCommon/ResDoctordetails.aspx?docid=DR/NEM001428"
                    },
                (TownItemDto)
               new TownBaseDto(ItemTypeCardMasterDataDto.GetFirst("doctor"),"Meena Nursing Home")
                {SubTitle="Hospital",Address="Old Town,Bhadravathi",Description="Hospital",
                    ImageUrl="https://ourdoctors.in/meenanursinghome/images/hospital-1.jpg",
                    //GoogleMapAddressUrl="https://g.co/kgs/78o8hhL",InstagramUrl="https://www.instagram.com/dr.anilkallesh/",
                    //TwitterUrl="https://twitter.com/anilkallesh?lang=en",OtherReferenceUrl="https://www.ask4healthcare.com/healthcaresolutions/DesktopModules/SearchCommon/ResDoctordetails.aspx?docid=DR/NEM001428",
                    },
                (TownItemDto)
               new TownBaseDto(ItemTypeCardMasterDataDto.GetFirst("doctor"),"ShivaShankar")
                {SubTitle="Pediatric",Address="Old Town,Bhadravathi",Description="Kids specialist",
                    GoogleMapAddressUrl="https://maps.app.goo.gl/4KvKGxmiDQWWJDbt5",
                    //InstagramUrl="https://www.instagram.com/dr.anilkallesh/",
                    //TwitterUrl="https://twitter.com/anilkallesh?lang=en",OtherReferenceUrl="https://www.ask4healthcare.com/healthcaresolutions/DesktopModules/SearchCommon/ResDoctordetails.aspx?docid=DR/NEM001428",
                    //ImageUrl="https://lh5.googleusercontent.com/p/AF1QipPSgv8ZzUDUx-9sPW-joj-iV7ijS1UyU3-J0kNs=w284-h160-k-no"
                    },
                //new(EnumTownItemType.Doctor_Clinic_Hospital,"Hussain")
                //{SubTitle="Pediatric",Address="Old Town,Bhadravathi",Description="Kids specialist",
                //    GoogleMapAddressUrl="https://g.co/kgs/78o8hhL",InstagramUrl="https://www.instagram.com/dr.anilkallesh/",
                //    TwitterUrl="https://twitter.com/anilkallesh?lang=en",OtherReferenceUrl="https://www.ask4healthcare.com/healthcaresolutions/DesktopModules/SearchCommon/ResDoctordetails.aspx?docid=DR/NEM001428",ImageUrl="https://lh5.googleusercontent.com/p/AF1QipPSgv8ZzUDUx-9sPW-joj-iV7ijS1UyU3-J0kNs=w284-h160-k-no"},

                (TownItemDto)
               new TownBaseDto(ItemTypeCardMasterDataDto.GetFirst("event"),"Kabaddi"),
                ]
            };

        }
    }
