using LozaApi.Models;

namespace LozaApi.Repository.Abstract
{
    public interface IAddMultiPhoto
    {

        public List<Photo> SaveMultiplePhotos(List<IFormFile> files);
       

    }
}
