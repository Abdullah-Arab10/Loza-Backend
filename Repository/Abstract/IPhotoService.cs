namespace LozaApi.Repository.Abstract
{
    public interface IPhotoService
    {
        public string SaveImage(IFormFile imageFile);
    }
}
