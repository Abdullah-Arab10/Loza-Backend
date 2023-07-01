using LozaApi.Models;
using LozaApi.Models.DTO;
using LozaApi.Repository.Implementaion;
using Microsoft.AspNetCore.Mvc;

namespace LozaApi.Repository.Abstract
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
