using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetSpa.Models.Domain;
using PetSpa.Models.DTO;
using PetSpa.Repositories;

namespace PetSpa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImagesRepository imagesRepository;

        public ImagesController(IImagesRepository imagesRepository) 
        {
            this.imagesRepository = imagesRepository;
        }

        //Post : /api/Images/Upload
        [HttpPost]
        [Route("Upload")]
        public async Task<IActionResult> Upload([FromForm] ImageUploadRequestDTO request)
        {
            ValidateFileUpload(request);

            if (ModelState.IsValid)
            {
                //User repository to uplaod image
                var imageDomainModel = new Images
                {
                    File = request.File,
                    FileExtension = Path.GetExtension(request.File.FileName),
                    FileSizeInBytes = request.File.Length,
                    FileName = request.FileName,
                    FileDescription = request.FileDescription,
                };

                //User repository to upload image
                await imagesRepository.Upload(imageDomainModel);
                return Ok(imageDomainModel);
            }

            return BadRequest(ModelState);
        }

        private void ValidateFileUpload(ImageUploadRequestDTO request)
        {
            var allowdExtentsions = new string[] { ".jpg", ".jpeg", ".png" };
            if (!allowdExtentsions.Contains(Path.GetExtension(request.File.FileName)))
            {
                ModelState.AddModelError("file", "Unsupported file extension");
            }
            if( request.File.Length > 10485760)
            {
                ModelState.AddModelError("file", "File size more then 10MB, Please upload a smaller size file.");
            }
        }

    }
}
