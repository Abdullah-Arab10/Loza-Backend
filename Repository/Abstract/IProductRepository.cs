using Loza.Entities;
using Loza.Models.DTO;
using Loza.Repository.Implementaion;
using Microsoft.AspNetCore.Mvc;

namespace Loza.Repository.Abstract
{
   public interface IProductRepository
    {
       List<ProductsDTO> GetProducts(int page, string sortOrder,string search);
        ProductWithPhotoD GetProductById(int id);
        List<ProductsDTO> GetProductByCat(int catygorey);
        Task<List<ProductsDTO>> GetNewest(int userId);

        List<Product> AddProduct(AddproductDTO product);
        void EditProduct(int id, AddproductDTO product);
        void DeleteProduct(List<int> ids);

    }
}
