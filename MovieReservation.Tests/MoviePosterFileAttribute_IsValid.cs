using Microsoft.AspNetCore.Http;
using MovieReservation.ViewModels;
using System.Text;

namespace MovieReservation.Tests
{
    public class MoviePosterFileAttribute_IsValid
    {
        [Theory]
        [InlineData("valid-test-file-1.jpeg", 30 * 1024, true)]
        [InlineData("valid-test-file-2.png", 30 * 1024, true)]
        [InlineData("invalid-test-file-1.txt", 30 * 1024, false)]
        [InlineData("invalid-test-file-2.jpg", 11 * (1024 * 1024), false)]
        public void IsValid_WithFormFile_ReturnsExpected(string fileName, int fileSize, bool expected)
        {
            byte[] fileBytes = Encoding.UTF8.GetBytes(fileName);
            using MemoryStream fakeFileStream = new(fileBytes);
            const int StreamOffset = 0;
            const string FormInputName = "posterImage";

            IFormFile fakeUploadedFile = new FormFile(baseStream: fakeFileStream, 
                baseStreamOffset: StreamOffset, 
                name: FormInputName, 
                fileName: fileName, 
                length: fileSize); 
            
            MoviePosterFileAttribute attribute = new();
            bool result = attribute.IsValid(fakeUploadedFile);

            Assert.Equal(expected, result);
        }
        
        [Fact]
        public void IsValid_WithInvalidType_ReturnsFalse()
        {
            int invalidTypeTestValue = 0;
            MoviePosterFileAttribute attribute = new();

            bool result = attribute.IsValid(invalidTypeTestValue);

            Assert.False(result);
        }

        [Fact]
        public void IsValid_WithNull_ReturnsFalse()
        {
            IFormFile? fakeUploadedFile = null;

            MoviePosterFileAttribute attribute = new();
            bool result = attribute.IsValid(fakeUploadedFile);

            Assert.False(result);
        }
    }
}
