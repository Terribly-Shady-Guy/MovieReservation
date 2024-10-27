using Microsoft.AspNetCore.Http;
using MovieReservation.ViewModels;
using System.Text;

namespace MovieReservation.Tests
{
    public class MoviePosterFileValidationTests
    {
        [Theory]
        [InlineData("valid-test-file-1.jpeg", 30 * 1024, true)]
        [InlineData("valid-test-file-2.png", 30 * 1024, true)]
        [InlineData("invalid-test-file-1.txt", 30 * 1024, false)]
        [InlineData("invalid-test-file-2.jpg", 11 * (1024 * 1024), false)]
        public void TestWithFormFile(string fileName, int fileSize, bool expected)
        {
            byte[] fileBytes = Encoding.UTF8.GetBytes(fileName);
            IFormFile file = new FormFile(baseStream: new MemoryStream(fileBytes), 
                baseStreamOffset: 0, 
                name: "posterImage", 
                fileName: fileName, 
                length: fileSize);

            MoviePosterFileAttribute attribute = new();
            bool result = attribute.IsValid(file);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void TestWithInvalidType()
        {
            int test = 0;
            MoviePosterFileAttribute attribute = new();

            bool result = attribute.IsValid(test);

            Assert.False(result);
        }

        [Fact]
        public void TestWithNull()
        {
            MoviePosterFileAttribute attribute = new();
            bool result = attribute.IsValid(null);

            Assert.False(result);
        }
    }
}
