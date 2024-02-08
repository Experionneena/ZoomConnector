using Bayada.Joy.ZoomConnector.ConfigOptions;
using Bayada.Joy.ZoomConnector.Contracts;
using Bayada.Joy.ZoomConnector.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Bayada.Joy.ZoomConnector.Services
{
    public class ZoomService : IZoomService
    {
        private readonly ZoomSettings _zoomSettings;
        public ZoomService(IOptions<ZoomSettings> zoomSettingsOptions)
        {
            _zoomSettings = zoomSettingsOptions.Value;
        }

        public async Task<string> GetAccessToken()
        {
            using (var client = new HttpClient())
            {
                var base64Auth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_zoomSettings.ClientId}:{_zoomSettings.ClientSecret}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64Auth);
                string url = $"{_zoomSettings.EndPoints.Auth}?grant_type=account_credentials&account_id={_zoomSettings.AccountId}";
                var response = await client.PostAsync(url, null);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ZoomTokenModel>(responseContent).access_token;
                }
                return string.Empty;
            }
        }
        public async Task<string> CreateZoomMeeting(string accessToken)
        {
            var httpClient = new HttpClient();
            var requestData = new
            {
                topic = "My Meeting",
                type = 2, // Scheduled meeting
                start_time = DateTime.UtcNow.AddHours(1).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                duration = 60,
                timezone = "UTC",
                email_notification = true,
                registrants_confirmation_email = true,
                registrants_email_notification = true,
                alternative_hosts = "example@gmail.com",
                alternative_hosts_email_notification = true
            };
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var jsonRequestBody = JsonConvert.SerializeObject(requestData);
            var content1 = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");
            //create meeting
            HttpResponseMessage response1 = await httpClient.PostAsync(_zoomSettings.EndPoints.CreateMeeting, content1);
            if (response1.IsSuccessStatusCode)
            {
                string responseContent = await response1.Content.ReadAsStringAsync();
                var meetingId = JsonConvert.DeserializeObject<ZoomTokenModel>(responseContent).Id;
                var participantData = new
                {
                    email = "example@gmail.com",
                    first_name = "E",
                    last_name = "A"
                };
                var requestBody = JsonConvert.SerializeObject(participantData);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                string apiUrl = $"{_zoomSettings.EndPoints.CreateMeeting}{meetingId}/registrants";
                var regContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
                //add participant
                var regResponse = await httpClient.PostAsync(apiUrl, regContent);

                if (regResponse.IsSuccessStatusCode)
                {
                    string getUrl = $"{_zoomSettings.EndPoints.CreateMeeting}{meetingId}/registrants";
                    //get participants
                    var getResponse = await httpClient.GetAsync(getUrl);

                    if (getResponse.IsSuccessStatusCode)
                    {
                        string responseContent1 = await getResponse.Content.ReadAsStringAsync();
                    }
                }
                else
                {
                }
                return responseContent;
            }
            return string.Empty;
        }

        public bool IsAccessTokenExpired(string accessToken)
        {
            string[] tokenParts = accessToken.Split('.');
            string payloadBase64 = tokenParts[1];
            string payloadJson = Encoding.UTF8.GetString(Convert.FromBase64String(payloadBase64));
            var payload = JsonConvert.DeserializeObject<Dictionary<string, object>>(payloadJson);

            if (payload.TryGetValue("exp", out var expValue) && expValue is long expTimestamp)
            {
                var currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                return expTimestamp <= currentTimestamp;
            }
            return true;
        }

    }
}