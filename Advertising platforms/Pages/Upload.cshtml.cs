using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;

namespace Advertising_platforms.Padges
{
    public class UploadModel : PageModel
    {
        [BindProperty]
        public IFormFile File { get; set; }

        public string ResultMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (File == null || File.Length == 0)
            {
                ResultMessage = "Файл не выбран или пуст.";
                return Page();
            }

            try
            {
                using var client = new HttpClient();
                using var content = new MultipartFormDataContent();
                using var stream = File.OpenReadStream();
                var streamContent = new StreamContent(stream);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
                content.Add(streamContent, "file", File.FileName);
                var response = await client.PostAsync("https://localhost:7067/api/advertising/upload", content);


                if (response.IsSuccessStatusCode)
                {
                    ResultMessage = "Файл успешно загружен.";
                }
                else
                {
                    ResultMessage = $"Ошибка загрузки файла: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                ResultMessage = "Произошла ошибка: " + ex.Message;
            }

            return Page();
        }
    }
}
