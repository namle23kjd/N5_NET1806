﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetSpa.CustomActionFilter;
using PetSpa.Models.Domain;
using PetSpa.Models.DTO.Image;
using PetSpa.Repositories.ImageRepository;

namespace PetSpa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImagesRepository imagesRepository;
        private readonly ApiResponseService apiResponseService;

        public ImagesController(IImagesRepository imagesRepository, ApiResponseService apiResponseService) 
        {
            this.imagesRepository = imagesRepository;
            this.apiResponseService = apiResponseService;
        }

        //Post : /api/Images/Upload
        [HttpPost]
        [Route("Upload")]
        //[Authorize(Roles = "Admin,Customer,Manager")]
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
                return Ok(apiResponseService.CreateSuccessResponse(imageDomainModel));
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
