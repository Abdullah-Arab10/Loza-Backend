using Loza.Entities;
using Loza.Models.DTO;
using Loza.Repository.Implementaion;
using Microsoft.AspNetCore.Mvc;

namespace Loza.Repository.Abstract
{
   public interface IProductRepository
    {
        ActionResult<IEnumerable<ProductsDTO>> GetProducts(int page, string sortOrder,string search);
        ProductWithPhotoD GetProductById(int id);
        ActionResult<IEnumerable<ProductsDTO>> GetProductByCat(int catygorey);
        Task<List<ProductsDTO>> GetNewest();

        List<Product> AddProduct(AddproductDTO product);
        void EditProduct(int id, AddproductDTO product);
        void DeleteProduct(List<int> ids);

    }
}
