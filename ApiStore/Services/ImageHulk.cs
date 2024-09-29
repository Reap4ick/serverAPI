using ApiStore.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ApiStore.Services
{
    public class ImageHulk : IImageHulk
    {
        private readonly IConfiguration _configuration;

        public ImageHulk(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Синхронний метод Delete для підтримки старої логіки
        public bool Delete(string fileName)
        {
            try
            {
                var dir = _configuration["ImagesDir"];
                var sizes = _configuration["ImageSizes"].Split(",")
                    .Select(x => int.Parse(x));

                foreach (var size in sizes)
                {
                    string dirSave = Path.Combine(Directory.GetCurrentDirectory(),
                        dir, $"{size}_{fileName}");

                    if (File.Exists(dirSave))
                    {
                        File.Delete(dirSave); // Синхронне видалення
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        // Асинхронний метод DeleteAsync для продуктів
        public async Task<bool> DeleteAsync(string fileName)
        {
            try
            {
                var dir = _configuration["ImagesDir"];
                var sizes = _configuration["ImageSizes"].Split(",")
                    .Select(x => int.Parse(x));

                foreach (var size in sizes)
                {
                    string dirSave = Path.Combine(Directory.GetCurrentDirectory(),
                        dir, $"{size}_{fileName}");

                    if (File.Exists(dirSave))
                    {
                        await Task.Run(() => File.Delete(dirSave)); // Асинхронне видалення
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> Save(IFormFile image)
        {
            string imageName = Guid.NewGuid().ToString() + ".webp";

            using (MemoryStream ms = new())
            {
                await image.CopyToAsync(ms);
                var bytes = ms.ToArray();
                await SaveByteArrayAsync(bytes, imageName);
            }

            return imageName;
        }

        private async Task SaveByteArrayAsync(byte[] bytes, string imageName)
        {
            var dir = _configuration["ImagesDir"];
            var sizes = _configuration["ImageSizes"].Split(",")
                    .Select(x => int.Parse(x));

            foreach (var size in sizes)
            {
                string dirSave = Path.Combine(Directory.GetCurrentDirectory(),
                    dir, $"{size}_{imageName}");

                using (var imageLoad = Image.Load(bytes))
                {
                    imageLoad.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(size, size),
                        Mode = ResizeMode.Max
                    }));

                    await Task.Run(() => imageLoad.Save(dirSave, new WebpEncoder())); // Асинхронне збереження
                }
            }
        }

        public async Task<string> Save(string urlImage)
        {
            string imageName = String.Empty;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(urlImage);

                    if (response.IsSuccessStatusCode)
                    {
                        byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();
                        imageName = Guid.NewGuid().ToString() + ".webp";
                        await SaveByteArrayAsync(imageBytes, imageName);
                    }
                }
            }
            catch
            {
                return imageName;
            }

            return imageName;
        }
    }
}
