using PetSpa.Data;
using PetSpa.Models.Domain;

namespace PetSpa.Repositories.ImageRepository
{
    public class LocalImageRepository : IImagesRepository
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly PetSpaContext dbContext;

        public LocalImageRepository(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor, PetSpaContext dbContext)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.httpContextAccessor = httpContextAccessor;
            this.dbContext = dbContext;
        }
        public async Task<Images> Upload(Images images)
        {
            var localFilePath = Path.Combine(webHostEnvironment.ContentRootPath, "Images", $"{images.FileName}{images.FileExtension}");

            //Upload Image to Local Path
            using var stream = new FileStream(localFilePath, FileMode.Create);
            await images.File.CopyToAsync(stream);

            var urlFilePath = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}{httpContextAccessor.HttpContext.Request.PathBase}/Images/{images.FileName}{images.FileExtension}";

            images.FilePath = urlFilePath;
            //Add Image to The Images to database
            await dbContext.Images.AddAsync(images);
            await dbContext.SaveChangesAsync();
            return images;
        }
    }
}
