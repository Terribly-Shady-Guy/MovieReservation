using ApplicationLogic.ValidationAttributes;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Text;

namespace Tests.Unit
{
    public class MoviePosterFileAttributeTests
    {
        [Theory]
        [InlineData("validjpegfile.jpeg", 30 * 1024, MediaTypeNames.Image.Jpeg, "FFD8", TestDisplayName = "Valid with jpeg")]
        [InlineData("validpngfile.png", 10 * 1024, MediaTypeNames.Image.Png, "89504E470D0A1A0A", TestDisplayName = "Valid with png")]
        [InlineData("validjpgfile.jpg", 10 * 1024 * 1024, MediaTypeNames.Image.Jpeg, "FFD8", TestDisplayName = "Valid with jpg and max file size")]
        public void IsValid_WithFormFile_ReturnsNull(string fileName, int fileSize, string contentType, string signature)
        {
            byte[] fileSignature = Convert.FromHexString(signature);
            IFormFile testFile = CreateTestFile(fileName, fileSize, contentType, fileSignature);
            var context = new ValidationContext(new { PosterImage = testFile });

            var validationAttribute = new MoviePosterFileAttribute();
            ValidationResult? result = validationAttribute.GetValidationResult(testFile, context);
            
            Assert.Null(result);
        }

        [Theory]
        [InlineData("invalidfileextension.txt", 30 * 1024, MediaTypeNames.Text.Plain, "", TestDisplayName = "Invalid file extension and content type")]
        [InlineData("filetoolarge.jpg", (10 * 1024 * 1024) + 1, MediaTypeNames.Image.Jpeg, "FFD8", TestDisplayName = "Invalid file size")]
        [InlineData("wrongcontenttype.jpg", 11 * 1024, MediaTypeNames.Text.Plain, "FFD8", TestDisplayName = "Invalid content type")]
        [InlineData("mismatchedcontenttype.jpeg", 10 * 1024, MediaTypeNames.Image.Png, "FFD8", TestDisplayName = "Invalid with mismatched content type and extension")]
        [InlineData("doubleexstension.jpg.exe", 30 * 1024, MediaTypeNames.Image.Jpeg, "FFD8", TestDisplayName = "Invalid with double extension")]
        [InlineData("invalidsignature.png", 10 * 1024, MediaTypeNames.Image.Png, "4D5A", TestDisplayName = "Invalid file signature")]
        public void IsValid_WithFormFile_ReturnsValidationResult(string fileName, int fileSize, string contentType, string signature)
        {
            byte[] fileSignature = Convert.FromHexString(signature);
            IFormFile testFile = CreateTestFile(fileName, fileSize, contentType, fileSignature);
            var context = new ValidationContext(new { PosterImage = testFile });

            var validationAttribute = new MoviePosterFileAttribute();
            ValidationResult? result = validationAttribute.GetValidationResult(testFile, context);
            
            Assert.NotNull(result);
        }
        
        [Fact]
        public void IsValid_WithInvalidType_ReturnsNull()
        {
            int invalidTypeTestValue = 0;
            var context = new ValidationContext(invalidTypeTestValue);

            var validationAttribute = new MoviePosterFileAttribute();
            ValidationResult? result = validationAttribute.GetValidationResult(invalidTypeTestValue, context);

            Assert.Null(result);
        }

        [Fact]
        public void IsValid_WithNull_ReturnsNull()
        {
            IFormFile? fakeUploadedFile = null;
            var context = new ValidationContext(new { fakeUploadedFile });
            
            var validationAttribute = new MoviePosterFileAttribute();
            ValidationResult? result = validationAttribute.GetValidationResult(fakeUploadedFile, context);

            Assert.Null(result);
        }

        private static IFormFile CreateTestFile(string fileName, int fileSize, string contentType, byte[] signature)
        {
            const int StreamOffset = 0;
            const string FormInputName = "posterImage";

            byte[] testFileBuffer = [..signature, ..Encoding.UTF8.GetBytes(fileName)];
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

            return fakeUploadedFile;
        }
    }
}
