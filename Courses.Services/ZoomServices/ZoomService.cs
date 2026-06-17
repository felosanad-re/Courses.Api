using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Zoom;
using Courses.Core.ModelsDTO.ResponseDTO.Zoom;
using Courses.Core.Options;
using Courses.Core.Services.Contract.ZoomServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Courses.Services.ZoomServices
{
    public class ZoomService : IZoomService
    {
        protected readonly ZoomOptions _zoomPotions;
        protected readonly HttpClient _httpClient;
        protected readonly ILogger<ZoomService> _logger;

        public ZoomService(IOptions<ZoomOptions> zoomPotions, HttpClient httpClient, ILogger<ZoomService> logger)
        {
            _zoomPotions = zoomPotions.Value;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<string> GetAccessToken()
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, "https://zoom.us/oauth/token");

            var authHeader = Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{_zoomPotions.ClientId}:{_zoomPotions.ClientSecret}")
                );

            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader);

            request.Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type",  "account_credentials"),
                new KeyValuePair<string, string>("account_id", _zoomPotions.AccountId)
            });

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Zoom Token Error: {error}");
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(json);

            return (string)result!.access_token;
        }

        public async Task<ApplicationServiceResult<ZoomResponse>> CreateMeetingAsync(ZoomRequest req)
        {
            var token = await GetAccessToken();

            // Meeting Data
            var meetingData = new
            {
                topic = req.Topic,
                type = 2,  // Scheduled Meeting
                start_time = req.StartTime
                    .ToUniversalTime()
                    .ToString("yyyy-MM-ddTHH:mm:ssZ"),
                duration = req.Duration,
                timezone = "UTC",
                settings = new
                {
                    host_video = true,
                    participant_video = false,
                    waiting_room = false,
                    join_before_host = false,
                    mute_upon_entry = true
                }
            };

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://api.zoom.us/v2/users/me/meetings"
            );
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent(
                JsonConvert.SerializeObject(meetingData),
                Encoding.UTF8,
                "application/json"
            );
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                return ApplicationServiceResult<ZoomResponse>
                    .Fail($"Zoom Error: {error}");
            }

            var json = await response.Content.ReadAsStringAsync();
            var zoomResponse = JsonConvert.DeserializeObject<ZoomResponse>(json);
            return ApplicationServiceResult<ZoomResponse>.Success(zoomResponse, "Meeting Created Succeeded");
        }
    }
}
