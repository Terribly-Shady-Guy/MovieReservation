using ApplicationLogic.ValidationAttributes;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Tests.Unit
{
    public class MoviePosterFileAttributeTests
    {
        private const string WrongFileErrorMessage = "This is not a valid file type. File type must be one of the following: .jpg, .jpeg, .png.";
        private const string FileTooLargeErrorMessage = "The uploaded file must be 10mb or smaller.";
        private const string FileNameIllegalCharactersErrorMessage = "The file name is too long or contains illegal characters. The filename must be between 5 and 100 characters and only contain alphanumeric and the following special characters: -, _, ., and whitespace.";

        [Theory]
        [InlineData("validjpegfile.jpeg", 30 * 1024, "FFD8", TestDisplayName = "Valid with jpeg")]
        [InlineData("valid-pngfile.png", 10 * 1024, "89504E470D0A1A0A", TestDisplayName = "Valid with png")]
        [InlineData("valid jpg file.jpg", 10 * 1024 * 1024, "FFD8", TestDisplayName = "Valid with jpg and max file size")]
        public void IsValid_WithFormFile_ReturnsNull(string fileName, int fileSize, string signature)
        {
            byte[] fileSignature = Convert.FromHexString(signature);
            IFormFile testFile = CreateTestFile(fileName, fileSize, fileSignature);
            var context = new ValidationContext(new { PosterImage = testFile });

            var validationAttribute = new MoviePosterFileAttribute();
            ValidationResult? result = validationAttribute.GetValidationResult(testFile, context);
            
            Assert.Null(result);
        }

        [Theory]
        [InlineData("invalidfileextension.txt", 30 * 1024, "", WrongFileErrorMessage, TestDisplayName = "Invalid file extension")]
        [InlineData("filetoolarge.jpg", (10 * 1024 * 1024) + 1, "FFD8", FileTooLargeErrorMessage, TestDisplayName = "Invalid file size")]
        [InlineData("doubleexstension.jpg.exe", 30 * 1024, "FFD8", WrongFileErrorMessage, TestDisplayName = "Invalid with double extension")]
        [InlineData("invalidsignature.png", 10 * 1024, "4D5A", WrongFileErrorMessage, TestDisplayName = "Invalid file signature")]
        [InlineData("invalidcharacters.php%00.jpg", 10 * 1024, "FFD8", FileNameIllegalCharactersErrorMessage, TestDisplayName = "Invalid with null byte")]
        [InlineData("invalidwithtoolongnametoolongtoolongtoolongtoolongtoolongwaytoolongwaywaytoolongyougetthepointyet.jpg", 10 * 1024, "FFD8", FileNameIllegalCharactersErrorMessage, TestDisplayName = "Invalid with file name too long")]
        public void IsValid_WithFormFile_ReturnsValidationResult(string fileName, int fileSize, string signature, string expectedErrorMessage)
        {
            byte[] fileSignature = Convert.FromHexString(signature);
            IFormFile testFile = CreateTestFile(fileName, fileSize, fileSignature);
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

        private static IFormFile CreateTestFile(string fileName, int fileSize, byte[] signature)
        {
            const int StreamOffset = 0;
            const string FormInputName = "posterImage";

            var fakeFileStream = new MemoryStream(signature);

            IFormFile fakeUploadedFile = new FormFile(
                baseStream: fakeFileStream,
                baseStreamOffset: StreamOffset,
                name: FormInputName,
                fileName: fileName,
                length: fileSize);

            return fakeUploadedFile;
        }
    }
}
