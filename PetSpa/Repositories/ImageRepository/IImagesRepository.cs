using PetSpa.Models.Domain;

namespace PetSpa.Repositories.ImageRepository
{
    public interface IImagesRepository
    {
        Task<Images> Upload(Images images);
    }
}
