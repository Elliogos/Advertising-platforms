using Advertising_platforms.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Advertising_platforms.Tests
{
    public class AdvertisingControllerTests
    {

        private IFormFile CreateFormFile(string content, string fileName = "test.txt")
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(bytes);
            return new FormFile(stream, 0, bytes.Length, "file", fileName);
        }

        [Fact]
        public void UploadValidFile_ShouldReturnOkResult()
        {
            string fileContent =
@"Яндекс.Директ:/ru
Ревдинский рабочий:/ru/svrd/revda,/ru/svrd/pervik
Газета уральских москвичей:/ru/msk,/ru/permobl,/ru/chelobl
Крутая реклама:/ru/svrd";

            var formFile = CreateFormFile(fileContent);
            var controller = new AdvertisingController();

            var uploadResult = controller.UploadPlatforms(formFile);

            var okResult = Assert.IsType<OkObjectResult>(uploadResult);
            Assert.Equal("Платформы успешно загружены.", okResult.Value);

            var searchResult = controller.SearchPlatforms("/ru/svrd/revda");
            var okSearch = Assert.IsType<OkObjectResult>(searchResult);
            var platforms = Assert.IsType<List<string>>(okSearch.Value);


            Assert.Contains("Яндекс.Директ", platforms);
            Assert.Contains("Ревдинский рабочий", platforms);
            Assert.Contains("Крутая реклама", platforms);
            Assert.DoesNotContain("Газета уральских москвичей", platforms);
        }

        [Fact]
        public void UploadInvalidFile_ShouldReturnOkAndIgnoreInvalidLines()
        {
            string fileContent =
@"Некорректная строка
Яндекс.Директ:/ru";
            var formFile = CreateFormFile(fileContent);
            var controller = new AdvertisingController();

            var uploadResult = controller.UploadPlatforms(formFile);

            var okResult = Assert.IsType<OkObjectResult>(uploadResult);
            Assert.Equal("Платформы успешно загружены.", okResult.Value);

            var searchResult = controller.SearchPlatforms("/ru");
            var okSearch = Assert.IsType<OkObjectResult>(searchResult);
            var platforms = Assert.IsType<List<string>>(okSearch.Value);
            Assert.Single(platforms);
            Assert.Contains("Яндекс.Директ", platforms);
        }

        [Fact]
        public void SearchPlatforms_NonExistingLocation_ShouldReturnEmptyList()
        {

            string fileContent =
@"Яндекс.Директ:/ru
Ревдинский рабочий:/ru/svrd/revda,/ru/svrd/pervik";
            var formFile = CreateFormFile(fileContent);
            var controller = new AdvertisingController();
            controller.UploadPlatforms(formFile);

            var searchResult = controller.SearchPlatforms("/nonexistent");
            var okSearch = Assert.IsType<OkObjectResult>(searchResult);
            var platforms = Assert.IsType<List<string>>(okSearch.Value);

            Assert.Empty(platforms);
        }
    }
}
