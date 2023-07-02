using Loza.Data;
using Loza.Entities;
using Loza.Repository.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Loza.Repository.Implementaion
{
    public class AddMultiPhoto:IAddMultiPhoto
    {

        readonly private DataContext _context;
        readonly private IPhotoService _photoService;
        public AddMultiPhoto(IPhotoService photoService,DataContext dataContext)
        {
            _photoService = photoService;
            _context = dataContext;


        }

        public List<Photo> SaveMultiplePhotos(List<IFormFile> files)
        {

            var photoUrls = new List<string>();

            foreach (var file in files)
            {
                var photoUrl =  _photoService.SaveImage(file);
                var photo = new Photo { Url = photoUrl };
                _context.Photos.Add(photo);

                _context.SaveChangesAsync();
                photoUrls.Add(photoUrl);
            }

            
            return _context.Photos.ToList();

        }
    }
}
