using Loza.Repository.Abstract;
using Loza.Entities;
using Loza.Data;
using Microsoft.AspNetCore.Mvc;
using Loza.Controllers;
using Microsoft.AspNetCore.Http.HttpResults;
using Loza.Models.DTO;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Azure;

namespace Loza.Repository.Implementaion
{
    public class ProductRepostory : IProductRepository
    {
        private readonly DataContext _context;
        private readonly IPhotoService _photoservice;
        private readonly IAddMultiPhoto _addMultiPhoto;

        public ProductRepostory(DataContext context, IPhotoService photoservice,IAddMultiPhoto addMultiPhoto)
        {
            this._context = context;
            _photoservice = photoservice;
            this._addMultiPhoto = addMultiPhoto;
        }


        public List<ProductsDTO> GetProducts(int page, string sortorder="",string searchstring = null)
        {
            string nameSort = String.IsNullOrEmpty(sortorder)?"name_desc":"";
            string priceSort = sortorder == "Price" ? "Price_desc" : "Price";
            string searchString = searchstring;



            if (!String.IsNullOrEmpty(searchString))
            {

                var pageResult = 10f;
                var products = _context.Product.Where(s => s.Name.Contains(searchString)
                                        || s.Color.Contains(searchString))
                                         .Skip((page - 1) * (int)pageResult)
                                         .Take((int)pageResult)
                                          .Select(s => new ProductsDTO
                                          {
                                              Id = s.Id,
                                              Name = s.Name,
                                              Description = s.Description,
                                              Price = s.Price,
                                              Category = s.Category,
                                              Color = s.Color,
                                              ColorNo = s.ColorNo,
                                              Quantity = s.Quantity,
                                              ProductImage = s.ProductImage,
                               
                                          })
                                         .ToList();

                return products;
              }

            else
            {
                var pageResult = 10f;
                switch (sortorder)
                {
                    case "name_desc":
                        var products1 =  _context.Product.OrderByDescending(s => s.Name)
                            .Skip((page - 1) * (int)pageResult)
                            .Take((int)pageResult)
                             .Select(s => new ProductsDTO
                             {
                                 Id = s.Id,
                                 Name = s.Name,
                                 Description = s.Description,
                                 Price = s.Price,
                                 Category = s.Category,
                                 Color = s.Color,
                                 ColorNo = s.ColorNo,
                                 Quantity = s.Quantity,
                                 ProductImage = s.ProductImage,
                   
                             })
                            .ToList();



                        return products1;



                    case "Price":
                        var products2 =  _context.Product
                            .OrderBy(s => s.Price)
                            .Skip((page - 1) * (int)pageResult)
                            .Take((int)pageResult)
                             .Select(s => new ProductsDTO
                             {
                                 Id = s.Id,
                                 Name = s.Name,
                                 Description = s.Description,
                                 Price = s.Price,
                                 Category = s.Category,
                                 Color = s.Color,
                                 ColorNo = s.ColorNo,
                                 Quantity = s.Quantity,
                                 ProductImage = s.ProductImage,
                       
                             })
                            .ToList();

                  

                        return products2;




                    case "Price_desc":
                        var products3 =  _context.Product
                           .OrderByDescending(s => s.Price)
                           .Skip((page - 1) * (int)pageResult)
                           .Take((int)pageResult)
                            .Select(s => new ProductsDTO
                            {
                                Id = s.Id,
                                Name = s.Name,
                                Description = s.Description,
                                Price = s.Price,
                                Category = s.Category,
                                Color = s.Color,
                                ColorNo = s.ColorNo,
                                Quantity = s.Quantity,
                                ProductImage = s.ProductImage,
                     
                            })
                           .ToList();

               
                        return products3;




                    default:

                        var products =  _context.Product
                           
                       
                         .Skip((page - 1) * (int)pageResult)
                         .Take((int)pageResult)
                         .Select(s =>new ProductsDTO
                         {
                             Id = s.Id,
                             Name = s.Name, 
                             Description = s.Description,
                             Price = s.Price,
                             Category = s.Category,
                             Color = s.Color,
                                ColorNo = s.ColorNo,
                             Quantity = s.Quantity,
                             ProductImage = s.ProductImage,

                         })
                         .ToList();

                   
                        return products;

                }

                //var pageCount = Math.Ceiling(_context.Product.Count()/pageResult);

                /* var products = await _context.Product

                       .Skip((page - 1) * (int)pageResult)
                       .Take((int)pageResult)
                       .ToListAsync();

                 /*var response = new Paging
                 {
                     Products = products,
                     CurrentPages=page,
                     Pages=(int)pageCount
                 };*/

                // return products;
            }
        }
        public List<Product> AddProduct(AddproductDTO product)
        {
             

            if(product.ImageFile!= null)
            {
                // _photosevice.SaveImage(model.ImageFile)
                var fileResult = _photoservice.SaveImage(product.ImageFile);

                product.ProductImage = fileResult;
            }

            // if(product.ImageFiles!= null) { 

            // var files = _addMultiPhoto.SaveMultiplePhotos(product.ImageFiles);
            //}



            var pro = new Product
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Category = product.Category,
                Color = product.Color,
                ColorNo = product.ColorNo,
                Quantity = product.Quantity,
                ProductImage = product.ProductImage,
                Photos = new List<Photo>()

            };

            if (product.ImageFiles != null)
            {
              
                
         
              foreach (var fileResult in product.ImageFiles)
                {
                    var photoUrl = _photoservice.SaveImage(fileResult);
                    var photo = new Photo { Url = photoUrl };
                    pro.Photos.Add(photo);
                }
            }
            

            _context.Product.Add(pro);
            _context.SaveChanges();
            return _context.Product.ToList();

        }


        public  List<ProductsDTO> GetProductByCat(int catygorey)
        {


            var product = _context.Product.Where(x => x.Category == catygorey)
             .Select(s => new ProductsDTO
             {
                 Id = s.Id,
                 Name = s.Name,
                 Description = s.Description,
                 Price = s.Price,
                 Category = s.Category,
                 Color = s.Color,
                 Quantity = s.Quantity,
                 ProductImage = s.ProductImage,
                 // CreateDateTime = s.CrateDateTime,
                 //  UpdateDateTime = s.UpdateDateTime


             })
                      .ToList();

            return product;


        }

         public async Task<List<ProductsDTO>> GetNewest()
        {
            int page = 1;
            var pageResult = 10f;
            var products =await _context.Product.OrderByDescending(p=>p.Id).Skip((page-1)*(int)pageResult).Take((int)pageResult)

                        .Select(s => new ProductsDTO
                        {
                            Id = s.Id,
                            Name = s.Name,
                            Description = s.Description,
                            Price = s.Price,
                            Category = s.Category,
                            Color = s.Color,
                            ColorNo = s.ColorNo,
                            Quantity = s.Quantity,
                            ProductImage = s.ProductImage,
                            // CreateDateTime = s.CrateDateTime,
                            //UpdateDateTime = s.UpdateDateTime


                        })
                        .ToListAsync();


            return products;

        }

        public ProductWithPhotoD GetProductById(int id)
        {

            var product = _context.Product
          .Include(p => p.Photos) // Include the associated photos
          .FirstOrDefault(p => p.Id == id);

            var pro = new ProductWithPhotoD
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Category = product.Category,
                Color = product.Color,
                Quantity = product.Quantity,
                ProductImage = product.ProductImage,
                Photos = product.Photos.Select(photo => new PhotosDTO
                {
                    PhotoUrl = photo.Url
                }).ToList()
            };
            decimal TotalRate(int productId)
            {
                var total = _context.Ratings.Where(p => p.ProductId == productId).Select(p => p.Rate).ToList();


                decimal totalRate = (decimal)0;

                if (total.Count == 0)
                {
                    return totalRate;
 
                }
                foreach (var user in total)
                {
                    totalRate = totalRate + user;
                }
                totalRate = totalRate / total.Count;
                totalRate = Math.Round(totalRate, 2);
                return totalRate;


            }
            
            
            decimal totalrate = TotalRate(id);
            pro.Totalrate = totalrate;


            return pro;


            
            /* var product = 
                 _context.Product

                  .Where(x=>x.Id == id)
                  .Select(s => new ProductsDTO
                 {
                     Id = s.Id,
                     Name = s.Name,
                     Description = s.Description,
                     Price = s.Price,
                     Category = s.Category,
                     Color = s.Color,
                     Quantity = s.Quantity,
                     ProductImage = s.ProductImage,
                    // CreateDateTime = s.CrateDateTime,
                   //  UpdateDateTime = s.UpdateDateTime


                 })
                          .ToList(); 

             return product;*/

        }
        public void EditProduct(int id, AddproductDTO request)
        {


            if (request.ImageFile != null)
            {
                // _photosevice.SaveImage(model.ImageFile)
                var fileResult = _photoservice.SaveImage(request.ImageFile);

                request.ProductImage = fileResult;
            }
            var product = _context.Product.Find(id);
            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.Category = request.Category;
            product.Color = request.Color;
            product.Quantity = request.Quantity;
            product.ProductImage = request.ProductImage;
           
            
            _context.SaveChanges();
        

        }
        public void DeleteProduct(List<int> ids)
        {

            foreach(int id in ids)
            {
                var product = _context.Product.Find(id);
                _context.Product.Remove(product);
                _context.SaveChanges();
            }
             
        }
    }
     
}
