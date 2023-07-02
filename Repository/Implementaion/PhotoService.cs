using Loza.Repository.Abstract;

namespace Loza.Repository.Implementaion
{
    public class PhotoService : IPhotoService
    {
        private IWebHostEnvironment environment;
        public PhotoService(IWebHostEnvironment env)
        {
            this.environment = env;

        }


        private string GetUniqueFileName(string fileName)
        {
            var guid = Guid.NewGuid().ToString();
            var extension = Path.GetExtension(fileName).ToLower();
            return guid + extension;
        }


        public string SaveImage(IFormFile imageFile)
        {

            var uniqueFileName = GetUniqueFileName(imageFile.FileName);
            var filePath = Path.Combine(environment.ContentRootPath, "Uploads");


            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            var fileWithPath = Path.Combine(filePath, uniqueFileName);
            var stream = new FileStream(fileWithPath, FileMode.Create);
            imageFile.CopyTo(stream);
            stream.Close();
            var name = "/Uploads/" + Path.GetFileName(uniqueFileName);

            return name ;


        }




       
    }
    }
