using ApplicationLogic.ValidationAttributes;
using Microsoft.AspNetCore.Http;
using System.Net.Mime;
using System.Text;

namespace Tests.Unit
{
    public class MoviePosterFileAttribute_IsValid
    {
        [Theory]
        [InlineData("valid-test-file-1.jpeg", 30 * 1024, MediaTypeNames.Image.Jpeg, true)]
        [InlineData("valid-test-file-2.png", 30 * 1024, MediaTypeNames.Image.Png, true)]
        [InlineData("invalid-test-file-1.txt", 30 * 1024, MediaTypeNames.Text.Plain, false)]
        [InlineData("invalid-test-file-2.jpg", 11 * 1024 * 1024, MediaTypeNames.Image.Jpeg, false)]
        [InlineData("invalid-test-file-mime.jpg", 11 * 1024, MediaTypeNames.Text.Plain, false)]
        [InlineData("invalid-test-file.jpg.exe", 30 * 1024, MediaTypeNames.Image.Jpeg, false)]
        public void IsValid_WithFormFile_ReturnsExpected(string fileName, int fileSize, string contentType, bool expected)
        {
            byte[] fileBytes = Encoding.UTF8.GetBytes(fileName);
            using var fakeFileStream = new MemoryStream(fileBytes);
            const int StreamOffset = 0;
            const string FormInputName = "posterImage";

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

            var validationAttribute = new MoviePosterFileAttribute();
            bool result = validationAttribute.IsValid(fakeUploadedFile);

            Assert.Equal(expected, result);
        }
        
        [Fact]
        public void IsValid_WithInvalidType_ReturnsFalse()
        {
            int invalidTypeTestValue = 0;

            var validationAttribute = new MoviePosterFileAttribute();
            bool result = validationAttribute.IsValid(invalidTypeTestValue);

            Assert.False(result);
        }

        [Fact]
        public void IsValid_WithNull_ReturnsFalse()
        {
            IFormFile? fakeUploadedFile = null;
            
            var validationAttribute = new MoviePosterFileAttribute();
            bool result = validationAttribute.IsValid(fakeUploadedFile);

            Assert.False(result);
        }
    }
}
