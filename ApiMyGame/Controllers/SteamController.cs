using ApiMyGame.Classes;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;

namespace SteamApiExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SteamController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public SteamController()
        {
            _httpClient = new HttpClient();
        }

        [HttpGet("appdetails/{appId}")]
        [SwaggerOperation(Summary = "Detalhes de um App da Steam por ID")]
        [SwaggerResponse(200, "Success")]
        [SwaggerResponse(400, "Bad request")]
        public async Task<IActionResult> GetAppDetails(string appId)
        {
            try
            {
                string url = $"https://store.steampowered.com/api/appdetails?appids={appId}";
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();

                    NossaBase nossaBase = new NossaBase();
                    nossaBase.ProcessGameDetails(result, appId);

                    return Ok(result);
                }
                else
                {
                    return StatusCode((int)response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ocorreu um erro ao processar a solicitação.");
            }
        }

        [HttpGet("applist")]
        [SwaggerOperation(Summary = "Lista dos Apps da Steam")]
        [SwaggerResponse(200, "Success")]
        [SwaggerResponse(400, "Bad request")]
        public async Task<IActionResult> GetAppList([FromQuery] string steamKey)
        {
            try
            {
                string url = $"http://api.steampowered.com/ISteamApps/GetAppList/v0002/?key={steamKey}&format=json";
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();

                    NossaBase nossaBase = new NossaBase();
                    nossaBase.ProcessAppList(result);

                    return Ok(result);
                }
                else
                {
                    return StatusCode((int)response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ocorreu um erro ao processar a solicitação.");
            }
        }

        [HttpGet("appimage/{appId}")]
        [SwaggerOperation(Summary = "Imagem de um App da Steam por ID")]
        [SwaggerResponse(200, "Success")]
        [SwaggerResponse(400, "Bad request")]
        public async Task<IActionResult> GetAppImage(string appId)
        {
            try
            {
                string imageUrl = $"https://media.steampowered.com/steamcommunity/public/images/apps/400/{appId}.jpg";
                HttpResponseMessage response = await _httpClient.GetAsync(imageUrl);

                if (response.IsSuccessStatusCode)
                {
                    var imageBytes = await response.Content.ReadAsByteArrayAsync();

                    return File(imageBytes, "image/jpeg");
                }
                else
                {
                    return StatusCode((int)response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ocorreu um erro ao processar a solicitação.");
            }
        }
    }
}
