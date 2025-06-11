using System.Net;
using System.Text.Json;
using ExternalUserService.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace ExternalUserService.Clients
{
    public class ReqresClient : IReqresClient
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ReqresClient> _logger;

        public ReqresClient(HttpClient httpClient, IMemoryCache cache, ILogger<ReqresClient> logger)
        {
            _httpClient = httpClient;
            _cache = cache;
            _logger = logger;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var users = new List<User>();
            int page = 1;

            try
            {
                while (true)
                {
                    var response = await _httpClient.GetAsync($"users?page={page}");
                    response.EnsureSuccessStatusCode();

                    var json = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<UserListResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    users.AddRange(apiResponse.Data.Select(d => new User
                    {
                        Id = d.Id,
                        Email = d.Email,
                        FirstName = d.First_Name,
                        LastName = d.Last_Name
                    }));

                    if (page >= apiResponse.Total_Pages) break;
                    page++;
                }

                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all users.");
                throw;
            }
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            string cacheKey = $"user_{userId}";
            if (_cache.TryGetValue(cacheKey, out User user)) return user;

            try
            {
                var response = await _httpClient.GetAsync($"users/{userId}");
                if (response.StatusCode == HttpStatusCode.NotFound) return null;
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<UserResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                user = new User
                {
                    Id = apiResponse.Data.Id,
                    Email = apiResponse.Data.Email,
                    FirstName = apiResponse.Data.First_Name,
                    LastName = apiResponse.Data.Last_Name
                };

                _cache.Set(cacheKey, user, TimeSpan.FromMinutes(5));
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user.");
                throw;
            }
        }
    }
}
