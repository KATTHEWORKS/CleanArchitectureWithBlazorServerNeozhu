using Blazorise.DeepCloner;
using Presentation.Shared;
using Microsoft.AspNetCore.Components;
using PublicCommon;
using System.Diagnostics;

namespace Presentation.BaseRazorClassLibrary
{
    public static class NavigationManagerExtension
        {
        public static AppSettings AppSettings(this NavigationManager navigationManager)
            {

            var baseUri = navigationManager.BaseUri;
            //if (Debugger.IsAttached)
            //    baseUri = "bdvt.in/";// change here for locally running

            //var appType = UrlDomainBasedHandling.GetApplicationTypeOfUrlDomain(baseUri);
            var settingngsToModifyForRequest = AppConfigurations.AppSettings.DeepClone();
            switch (UrlDomainBasedHandling.GetApplicationTypeOfUrlDomain(baseUri))
                {
                case ApplicationTypeEnum.MyVote:
                    settingngsToModifyForRequest.PublicDomain = "next-mp.in";
                    settingngsToModifyForRequest.PublicDomainAbsoluteUrl = "https://www.Next-MP.in";
                    settingngsToModifyForRequest.PublicDomainAbsoluteUrlSecond = "https://www.MP24.in";
                    settingngsToModifyForRequest.Title = "MP DareDevil Transparent Secure Feedback Voting System & Survey";
                    settingngsToModifyForRequest.Description = "Know about your Member of Paliament MP \n Share FearLessly current situation of your Constituency Problem Corruption Dictatorship Illegal Actions Corrections Needed";
                    settingngsToModifyForRequest.AppVideoUrl = "https://youtu.be/Ktc8GLW3QZo";
                    settingngsToModifyForRequest.FaviconImage = "_content/BlazorRazorClassLibraryMyVote/images/faviconIndiaFlag.png";
                    break;
                //return settingngsToModifyForRequest;
                case ApplicationTypeEnum.MyProducts:
                    if (baseUri.EndsWith("hennu.in/"))
                        {
                        settingngsToModifyForRequest.PublicDomain = "Hennu.in";
                        settingngsToModifyForRequest.PublicDomainAbsoluteUrl = "https://www.Hennu.in";
                        //settingngsToModifyForRequest.PublicDomainAbsoluteUrlSecond = "https://www.MP24.in";
                        settingngsToModifyForRequest.Title = "Hennu.in ಹೆಣ್ಣು";
                        settingngsToModifyForRequest.Description = "Hennu.in ಹೆಣ್ಣು ತಾಯಿ The Source of Origin";
                        //settingngsToModifyForRequest.AppVideoUrl = "https://youtu.be/Ktc8GLW3QZo";

                        settingngsToModifyForRequest.FaviconImage = "_content/BlazorRazorClassLibraryMyProducts/images/faviconHennu.png";
                        }
                    else if (baseUri.EndsWith("holige.in/"))
                        {
                        settingngsToModifyForRequest.PublicDomain = "Holige.in";
                        settingngsToModifyForRequest.PublicDomainAbsoluteUrl = "https://www.Holige.in";
                        //settingngsToModifyForRequest.PublicDomainAbsoluteUrlSecond = "https://www.Holige.in";
                        settingngsToModifyForRequest.Title = "Holige.in ಹೋಳಿಗೆ ಹೂರಣ ಒಬ್ಬಟ್ಟು";
                        settingngsToModifyForRequest.Description = "Karnataka's Culinary Heritage";
                        //settingngsToModifyForRequest.AppVideoUrl = "https://youtu.be/Ktc8GLW3QZo";

                        settingngsToModifyForRequest.FaviconImage = "_content/BlazorRazorClassLibraryMyProducts/images/faviconHolige.png";
                        }
                    break;

                case ApplicationTypeEnum.MyTown:
                    if (baseUri.EndsWith("bhadravathi.com/") || baseUri.EndsWith("bdvt.in/"))
                        {
                        settingngsToModifyForRequest.PublicDomain = "Bdvt.in";
                        settingngsToModifyForRequest.PublicDomainAbsoluteUrl = "https://www.Bdvt.in";
                        settingngsToModifyForRequest.PublicDomainAbsoluteUrlSecond = "https://www.Bhadravathi.com";
                        settingngsToModifyForRequest.Title = "Bdvt.in ಭದ್ರಾವತಿ";
                        settingngsToModifyForRequest.Description = "RUSTING OF BHADRA ಬೆಂಕಿಪುರಕ್ಕೆ ತುಕ್ಕು";
                        //settingngsToModifyForRequest.AppVideoUrl = "https://youtu.be/Ktc8GLW3QZo";

                        //settingngsToModifyForRequest.FaviconImage = "_content/BlazorRazorClassLibraryMyTown/images/faviconHolige.png";
                        }
                    break;
                case ApplicationTypeEnum.Katthe://default from appsettings.json
                default: break;
                }

            return settingngsToModifyForRequest;
            }
        }
    }
