using Microsoft.AspNetCore.Mvc;
using Advertising_platforms.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advertising_platforms.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdvertisingController : ControllerBase
    {

        private static List<AdvertisingPlatform> _platforms = new List<AdvertisingPlatform>();

        [HttpPost("upload")]
        public IActionResult UploadPlatforms([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Файл не выбран или пуст.");

            try
            {
                var platforms = new List<AdvertisingPlatform>();

                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        var parts = line.Split(':');
                        if (parts.Length != 2)
                        {
                            continue;
                        }

                        var platformName = parts[0].Trim();
                        var locations = parts[1]
                                        .Split(',')
                                        .Select(l => l.Trim())
                                        .Where(l => !string.IsNullOrEmpty(l))
                                        .ToList();

                        platforms.Add(new AdvertisingPlatform
                        {
                            Name = platformName,
                            Locations = locations
                        });
                    }
                }

                _platforms = platforms;

                return Ok("Платформы успешно загружены.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ошибка при обработке файла: " + ex.Message);
            }
        }

        [HttpGet("search")]
        public IActionResult SearchPlatforms([FromQuery] string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                return BadRequest("Параметр location обязателен.");

            var result = _platforms
                            .Where(p => p.Locations.Any(l => location.StartsWith(l, StringComparison.OrdinalIgnoreCase)))
                            .Select(p => p.Name)
                            .ToList();

            return Ok(result);
        }
    }
}
