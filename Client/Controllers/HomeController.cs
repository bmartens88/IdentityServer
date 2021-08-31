using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Client.Models;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> WeatherForecasts()
        {
            var client = _httpClientFactory.CreateClient("APIClient");
            var response = await client.GetAsync("WeatherForecast");

            if (response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
                var weatherForecastString = await response.Content.ReadAsStringAsync();
                var weatherForecasts = JsonSerializer.Deserialize<List<WeatherForecast>>(weatherForecastString,
                    new JsonSerializerOptions {PropertyNameCaseInsensitive = true});
                return View(weatherForecasts);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized ||
                response.StatusCode == HttpStatusCode.Forbidden)
            {
                return RedirectToAction("AccessDenied", "Auth");
            }

            throw new Exception("There is a problem accessing the API.");
        }

        [Authorize(Policy = "CanCreateAndModifyData")]
        public async Task<IActionResult> Privacy()
        {
            var idpClient = _httpClientFactory.CreateClient("IDPClient");
            var metaDataResponse = await idpClient.GetDiscoveryDocumentAsync();

            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            var response = await idpClient.GetUserInfoAsync(new UserInfoRequest
            {
                Address = metaDataResponse.UserInfoEndpoint,
                Token = accessToken
            });

            if (response.IsError)
                throw new Exception("Problem while fetching data from the UserInfo endpoint", response.Exception);

            var addressClaim = response.Claims.FirstOrDefault(c => c.Type.Equals("address"));

            User.AddIdentity(new ClaimsIdentity(new[] {addressClaim}));

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}