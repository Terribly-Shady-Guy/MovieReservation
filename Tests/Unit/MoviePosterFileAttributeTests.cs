using ApplicationLogic.ValidationAttributes;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;

namespace Tests.Unit
{
    public class MoviePosterFileAttributeTests
    {
        private const string WrongFileErrorMessage = "This is not a valid file type. File type must be one of the following: .jpg, .jpeg, .png.";
        private const string InvalidContentTypeErrorMessage = "The provided file type or content type is not valid. File type must be one of the following: .jpg, .jpeg, .png.";
        private const string FileTooLargeErrorMessage = "The uploaded file must be 10mb or smaller.";

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
        [InlineData("invalidfileextension.txt", 30 * 1024, MediaTypeNames.Text.Plain, "", InvalidContentTypeErrorMessage, TestDisplayName = "Invalid file extension and content type")]
        [InlineData("filetoolarge.jpg", (10 * 1024 * 1024) + 1, MediaTypeNames.Image.Jpeg, "FFD8", FileTooLargeErrorMessage, TestDisplayName = "Invalid file size")]
        [InlineData("wrongcontenttype.jpg", 11 * 1024, MediaTypeNames.Text.Plain, "FFD8", InvalidContentTypeErrorMessage, TestDisplayName = "Invalid content type")]
        [InlineData("mismatchedcontenttype.jpeg", 10 * 1024, MediaTypeNames.Image.Png, "FFD8", InvalidContentTypeErrorMessage, TestDisplayName = "Invalid with mismatched content type and extension")]
        [InlineData("doubleexstension.jpg.exe", 30 * 1024, MediaTypeNames.Image.Jpeg, "FFD8", InvalidContentTypeErrorMessage, TestDisplayName = "Invalid with double extension")]
        [InlineData("invalidsignature.png", 10 * 1024, MediaTypeNames.Image.Png, "4D5A", WrongFileErrorMessage, TestDisplayName = "Invalid file signature")]
        public void IsValid_WithFormFile_ReturnsValidationResult(string fileName, int fileSize, string contentType, string signature, string expectedErrorMessage)
        {
            byte[] fileSignature = Convert.FromHexString(signature);
            IFormFile testFile = CreateTestFile(fileName, fileSize, contentType, fileSignature);
            var context = new ValidationContext(new { PosterImage = testFile });

            var validationAttribute = new MoviePosterFileAttribute();
            ValidationResult? result = validationAttribute.GetValidationResult(testFile, context);
            
            Assert.NotNull(result);
            Assert.Equal(expectedErrorMessage, result.ErrorMessage);
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

            var fakeFileStream = new MemoryStream(signature);

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
