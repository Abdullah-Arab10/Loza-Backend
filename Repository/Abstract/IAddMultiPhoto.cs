using Loza.Entities;

namespace Loza.Repository.Abstract
{
    public interface IAddMultiPhoto
    {

        public List<Photo> SaveMultiplePhotos(List<IFormFile> files);
       

    }
}
