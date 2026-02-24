using ApplicationLogic.ValidationAttributes;
using ApplicationLogic.ViewModels;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Text;

namespace Tests.Unit
{
    public class MoviePosterFileAttributeTests
    {
        [Theory]
        [InlineData("testfilename.jpeg", 30 * 1024, MediaTypeNames.Image.Jpeg, TestDisplayName = "Valid with jpeg")]
        [InlineData("testfilename.png", 10 * 1024, MediaTypeNames.Image.Png, TestDisplayName = "Valid with png")]
        [InlineData("testfilename.jpg", 10 * 1024 * 1024, MediaTypeNames.Image.Jpeg, TestDisplayName = "Valid with jpg and max file size")]
        public void IsValid_WithFormFile_ReturnsValid(string fileName, int fileSize, string contentType)
        {
            MovieFormDataBody testModel = CreateTestModelInstance(fileName, fileSize, contentType);
            var context = new ValidationContext(testModel);

            var validationAttribute = new MoviePosterFileAttribute();
            ValidationResult? result = validationAttribute.GetValidationResult(testModel.PosterImage, context);

            Assert.Null(result);
        }

        [Theory]
        [InlineData("testfilename.txt", 30 * 1024, MediaTypeNames.Text.Plain, TestDisplayName = "Invalid extension and content type")]
        [InlineData("testfilename.jpg", (10 * 1024 * 1024) + 1, MediaTypeNames.Image.Jpeg, TestDisplayName = "Invalid file size")]
        [InlineData("testfilename.jpg", 11 * 1024, MediaTypeNames.Text.Plain, TestDisplayName = "Invalid with mismatched content type")]
        [InlineData("testfilename.jpg.exe", 30 * 1024, MediaTypeNames.Image.Jpeg, TestDisplayName = "Invalid with double extension")]
        public void IsValid_WithFormFile_ReturnsInvalid(string fileName, int fileSize, string contentType)
        {
            MovieFormDataBody testModel = CreateTestModelInstance(fileName, fileSize, contentType);
            var context = new ValidationContext(testModel);

            var validationAttribute = new MoviePosterFileAttribute();
            ValidationResult? result = validationAttribute.GetValidationResult(testModel.PosterImage, context);

            Assert.NotNull(result);
        }
        
        [Fact]
        public void IsValid_WithInvalidType_ReturnsInvalid()
        {
            int invalidTypeTestValue = 0;
            var context = new ValidationContext(invalidTypeTestValue);

            var validationAttribute = new MoviePosterFileAttribute();
            ValidationResult? result = validationAttribute.GetValidationResult(invalidTypeTestValue, context);

            Assert.NotNull(result);
        }

        [Fact]
        public void IsValid_WithNull_ReturnsInvalid()
        {
            IFormFile? fakeUploadedFile = null;
            var context = new ValidationContext(new { fakeUploadedFile });
            
            var validationAttribute = new MoviePosterFileAttribute();
            ValidationResult? result = validationAttribute.GetValidationResult(fakeUploadedFile, context);

            Assert.NotNull(result);
        }

        private static MovieFormDataBody CreateTestModelInstance(string fileName, int fileSize, string contentType)
        {
            const int StreamOffset = 0;
            const string FormInputName = "posterImage";

            byte[] testFileBuffer = Encoding.UTF8.GetBytes(fileName);
            var fakeFileStream = new MemoryStream(testFileBuffer);

            IFormFile fakeUploadedFile = new FormFile(
                baseStream: fakeFileStream,
                baseStreamOffset: StreamOffset,
                name: FormInputName,
                fileName: fileName,
                length: fileSize)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType,
                ContentDisposition = $"form-data; name=\"{FormInputName}\"; filename=\"{fileName}\""
            };

            return new MovieFormDataBody()
            {
                Description = string.Empty,
                Genres = [],
                PosterImage = fakeUploadedFile,
                Title = string.Empty
            };
        }
    }
}
