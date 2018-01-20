using Kontur.ImageTransformer.Tests.Fixtures;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Kontur.ImageTransformer.Tests.Controllers
{
    [Collection("REST Collection")]
    public class ImageProcessingController
    {
        private readonly ITestOutputHelper _output;
        private readonly ServerFixture _serverFixture;

        private const string DEFAULT_IMAGE = "Resources/Images/32bit/1000x1000.png";
        private const int CONTROL_WIDTH_PIXELS = 10;
        private const int CONTROL_HEIGHT_PIXELS = 10;

        private HttpClient restClient { get { return _serverFixture.Client; } }

        public ImageProcessingController(ServerFixture serverFixture, ITestOutputHelper output)
        {
            _serverFixture = serverFixture;
            _output = output;
        }

        [Theory(DisplayName = "Apply valid threshold filter via POST request")]
        [InlineData("32bit", 0)]
        [InlineData("32bit", 25)]
        [InlineData("32bit", 50)]
        [InlineData("32bit", 75)]
        [InlineData("32bit", 100)]
        [InlineData("8bit", 0)]
        [InlineData("8bit", 25)]
        [InlineData("8bit", 50)]
        [InlineData("8bit", 75)]
        [InlineData("8bit", 100)]
        [Trait("Controller", "ImageProcessing")]
        public async void POST_ValidThresholdFilter_200OK(string bit, int multiplier)
        {
            string[] filenames = Directory.GetFiles($"Resources/Images/{bit}");

            foreach (var filename in filenames)
                using (var stream = File.OpenRead(filename))
                using (var content = new StreamContent(File.OpenRead(filename)))
                {
                    _output.WriteLine($"Trying {Path.GetFileNameWithoutExtension(filename)} ({bit})");
                    var response = await restClient.PostAsync($"process/threshold({multiplier})/0,0,{int.MaxValue},{int.MaxValue}", content);
                    _output.WriteLine("POST request return a {0} status code. Is success status code: {1}", (int)response.StatusCode, response.IsSuccessStatusCode);
                    Assert.True(response.IsSuccessStatusCode);
                    Assert.Equal(200, (int)response.StatusCode);

                    using (var original = new Bitmap(stream))
                    using (var result = new Bitmap(await response.Content.ReadAsStreamAsync()))
                        for (int y = 0; y < original.Height; y += original.Height / CONTROL_HEIGHT_PIXELS)
                            for (int x = 0; x < original.Width; x += original.Width / CONTROL_WIDTH_PIXELS)
                                Assert.Equal(ApplyThresholdFilter(original.GetPixel(x, y), multiplier), result.GetPixel(x, y));

                    _output.WriteLine("Successful");
                }
        }

        [Theory(DisplayName = "Apply valid sepia filter via POST request")]
        [InlineData("32bit")]
        [InlineData("8bit")]
        [Trait("Controller", "ImageProcessing")]
        public async void POST_ValidSepiaFilter_200OK(string bit)
        {
            string[] filenames = Directory.GetFiles($"Resources/Images/{bit}");

            foreach (var filename in filenames)
                using (var stream = File.OpenRead(filename))
                using (var content = new StreamContent(File.OpenRead(filename)))
                {
                    _output.WriteLine($"Trying {Path.GetFileNameWithoutExtension(filename)} ({bit})");
                    var response = await restClient.PostAsync($"process/sepia/0,0,{int.MaxValue},{int.MaxValue}", content);
                    _output.WriteLine("POST request return a {0} status code. Is success status code: {1}", (int)response.StatusCode, response.IsSuccessStatusCode);
                    Assert.True(response.IsSuccessStatusCode);
                    Assert.Equal(200, (int)response.StatusCode);

                    using (var original = new Bitmap(stream))
                    using (var result = new Bitmap(await response.Content.ReadAsStreamAsync()))
                        for (int y = 0; y < original.Height; y += original.Height / CONTROL_HEIGHT_PIXELS)
                            for (int x = 0; x < original.Width; x += original.Width / CONTROL_WIDTH_PIXELS)
                                Assert.Equal(ApplySepiaFilter(original.GetPixel(x, y)), result.GetPixel(x, y));

                    _output.WriteLine("Successful");
                }
        }

        [Theory(DisplayName = "Apply valid grayscale filter via POST request")]
        [InlineData("32bit")]
        [InlineData("8bit")]
        [Trait("Controller", "ImageProcessing")]
        public async void POST_ValidGrayscaleFilter_200OK(string bit)
        {
            string[] filenames = Directory.GetFiles($"Resources/Images/{bit}");

            foreach (var filename in filenames)
                using (var stream = File.OpenRead(filename))
                using (var content = new StreamContent(File.OpenRead(filename)))
                {
                    _output.WriteLine($"Trying {Path.GetFileNameWithoutExtension(filename)} ({bit})");
                    var response = await restClient.PostAsync($"process/grayscale/0,0,{int.MaxValue},{int.MaxValue}", content);
                    _output.WriteLine("POST request return a {0} status code. Is success status code: {1}", (int)response.StatusCode, response.IsSuccessStatusCode);
                    Assert.True(response.IsSuccessStatusCode);
                    Assert.Equal(200, (int)response.StatusCode);

                    using (var original = new Bitmap(stream))
                    using (var result = new Bitmap(await response.Content.ReadAsStreamAsync()))
                        for (int y = 0; y < original.Height; y += original.Height / CONTROL_HEIGHT_PIXELS)
                            for (int x = 0; x < original.Width; x += original.Width / CONTROL_WIDTH_PIXELS)
                                Assert.Equal(ApplyGrayscaleFilter(original.GetPixel(x, y)), result.GetPixel(x, y));

                    _output.WriteLine("Successful");
                }
        }

        [Fact(DisplayName = "Empty region")]
        [Trait("Controller", "ImageProcessing")]
        public async void POST_EmptyRegion_204()
        {
            using (var content = new StreamContent(File.OpenRead(DEFAULT_IMAGE)))
            {
                var response = await restClient.PostAsync($"process/grayscale/0,0,{int.MinValue},{int.MinValue}", content);
                _output.WriteLine("POST request return a {0} status code. Is success status code: {1}", (int)response.StatusCode, response.IsSuccessStatusCode);
                Assert.Equal(204, (int)response.StatusCode);
                Assert.True(response.IsSuccessStatusCode);
            }
        }

        [Fact(DisplayName = "Crop image")]
        [Trait("Controller", "ImageProcessing")]
        public async void POST_CropImage_200()
        {
            using (var content = new StreamContent(File.OpenRead(DEFAULT_IMAGE)))
            {
                var response = await restClient.PostAsync($"process/grayscale/25,25,25,25", content);
                _output.WriteLine("POST request return a {0} status code. Is success status code: {1}", (int)response.StatusCode, response.IsSuccessStatusCode);
                Assert.Equal(200, (int)response.StatusCode);
                Assert.True(response.IsSuccessStatusCode);

                using (var result = new Bitmap(await response.Content.ReadAsStreamAsync()))
                {
                    _output.WriteLine($"{result.Width}x{result.Height} image received");
                    Assert.Equal(25, result.Width);
                    Assert.Equal(25, result.Height);
                }
            }
        }

        public static Color ApplySepiaFilter(Color color)
        {
            float r = color.R * 0.393f + color.G * 0.769f + color.B * 0.189f;
            r = r > 255 ? 255 : r;
            float g = color.R * 0.349f + color.G * 0.686f + color.B * 0.168f;
            g = g > 255 ? 255 : g;
            float b = color.R * 0.272f + color.G * 0.534f + color.B * 0.131f;
            b = b > 255 ? 255 : b;

            return Color.FromArgb(color.A, Convert.ToInt32(r), Convert.ToInt32(g), Convert.ToInt32(b));
        }

        public static Color ApplyGrayscaleFilter(Color color)
        {
            int intensity = (color.R + color.G + color.B) / 3;
            return Color.FromArgb(color.A, intensity, intensity, intensity);
        }

        public static Color ApplyThresholdFilter(Color color, int multiplier)
        {
            int intensity = (color.R + color.G + color.B) / 3;
            if (intensity >= 255 * multiplier / 100)
                return Color.FromArgb(color.A, 255, 255, 255);
            else
                return Color.FromArgb(color.A, 0, 0, 0);
        }
    }
}