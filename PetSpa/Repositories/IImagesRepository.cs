using PetSpa.Models.Domain;

namespace PetSpa.Repositories
{
    public interface IImagesRepository
    {
        Task<Images> Upload(Images images);
    }
}
