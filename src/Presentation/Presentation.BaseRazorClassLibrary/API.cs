using Dto;
using Presentation.Shared;
using PublicCommon.MyVote;
using System.Net.Http.Json;

namespace Presentation.BaseRazorClassLibrary;

public static class API
    {
    const string prefix = ApiEndPoints.Prefix + "/";
    public static ConfigurationsForWasmClient? Settings { get; set; }
    public static async Task<ConfigurationsForWasmClient> LoadSettings(this HttpClient client)
        {
        if (Settings == null || DateTime.Now.Subtract(Settings.LoadedOn).TotalDays > 1)
            {
            var settingsResult = await client.GetFromJsonAsync<ConfigurationsForWasmClient>(prefix + ApiEndPoints.Config);
            if (settingsResult != null)
                Settings = settingsResult;
            else throw new Exception("Configurations are not loading properly, please try after sometime");
            }
        return Settings;
        }

    /*
    //await Http.GetFromJsonAsync<IEnumerable<ConstituencyDto>>(ApiEndPoints.ConstituencyGetAll)
    public static async Task<List<ConstituencyDto>> ConstituencyGetAll(this HttpClient client)
        {
        var response = await client.GetAsync(prefix + ApiEndPoints.Constituency);
        if (response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NoContent)
            {
            return (await response.Content.ReadFromJsonAsync<IEnumerable<ConstituencyDto>>())?.ToList();
            }
        return null;
        }


    public static async Task<VoteDto?> MyVoteGet(this HttpClient client, Guid? UserId)
        {
        if (UserId == null) return null;
        var response = await client.GetAsync(prefix + ApiEndPoints.Vote + "/" + UserId.ToString());
        if (response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NoContent)
            {
            return await response.Content.ReadFromJsonAsync<VoteDto?>();
            }
        return null;
        }

    //var result = await Http.PostAsJsonAsync(ApiEndPoints.VoteSupportOpposePost, new VoteSupportOppose(constituencyId, voteId, UserId ?? new Guid(), support));
    public static async Task<int?> VoteSupportOpposePost(this HttpClient client, int constituencyId, int voteId, Guid? userId, bool? support)
        {
        if (userId == null) return null;
        var response = await client.PostAsJsonAsync(prefix + ApiEndPoints.VoteSupportOppose, new VoteSupportOppose(constituencyId, voteId, userId ?? new Guid(), support));
        if (response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NoContent)
            {
            return await response.Content.ReadFromJsonAsync<int>();
            }
        return null;
        }

    public static async Task<int> VotePost(this HttpClient client, VoteDto Model, Guid userId)
        {
        var toSendModel = new VoteDto()
            {
            Id = Model.Id,
            ConstituencyId = Model.ConstituencyId,
            WorkDoneQuality = Model.WorkDoneQuality,
            KPIMessages = Model.KPIRatingMessages.ValidRatingsNullifyMessageAndExtractMessagesToPostApi(),
            KPIRatingMessages = Model.KPIRatingMessages,
            UserId = userId,//AuthenticationState.User.Identity.GetUserId(),
                            //below had to be removed for security 
            Created = Model.Created,
            LastModified = Model.LastModified,
            Rating = Model.Rating
            };

        var response = await client.PostAsJsonAsync(prefix + ApiEndPoints.Vote, toSendModel);
        //show loading symbol
        if (response.IsSuccessStatusCode)
            {
            return await response.Content.ReadFromJsonAsync<int>();
            }
        return 0;
        }
    */
    }

