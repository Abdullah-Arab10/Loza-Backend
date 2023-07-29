﻿using Loza.Data;
using Loza.Entities;
using Loza.Models.DTO;
using Loza.Models.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Loza.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {

        private readonly DataContext _dataContext;
        public CartController(DataContext dataContext)
        {
            _dataContext = dataContext;

        }


        [HttpPost]
        public async Task<ActionResult> AddProductToCart(int userId, string name, string color, int colorno, int quan) {

         

            var prodctId = await _dataContext.Product.Where(p => p.Name == name && p.Color == color && p.ColorNo == colorno).Select(p => new {id=p.Id,quant=p.Quantity }).FirstAsync();
            //var prodquant = await _dataContext.Product.Where(p => p.Id == prodctId.id).FirstAsync();
            if (prodctId.quant<quan)
            {
                return Ok("no enought products in stock");
            }
            else
            {
                var find = await _dataContext.ShoppingCarts.FirstOrDefaultAsync(p => p.ProductId == prodctId.id && p.UserId == userId);

                if (find != null)
                {
                    find.Quant = find.Quant + quan;
                    await _dataContext.SaveChangesAsync();
                    return Ok();
                }
                var pro = await _dataContext.Product.FirstOrDefaultAsync(P => P.Id == prodctId.id);
                var cart = new ShoppingCart
                {
                    ProductId = prodctId.id,
                    ProductName = pro.Name,
                    price = pro.Price,
                    UserId = userId,
                    Quant = quan

                };

                await _dataContext.ShoppingCarts.AddAsync(cart);
                await _dataContext.SaveChangesAsync();
                return Ok();

            }
        }

        [HttpGet]
        public async Task<ActionResult> GetUserCart(int userId)
        {
            //  var cartitems = from c in _dataContext.ShoppingCarts join p in _dataContext.Product on c.ProductId equals p.Id select p;
            var car = await _dataContext.ShoppingCarts.Where(p => p.UserId == userId)
            .Select(s => new CartDTO
            {
                price = s.Total,
                Qunatity = s.Quant,
                ProductName = s.ProductName,
                id = (int)s.ProductId,
                Photo =  _dataContext.Product.Where(p => p.Id == s.ProductId).Select( p => p.ProductImage).First(),
                color = _dataContext.Product.Where(p => p.Id == s.ProductId).Select(p => p.Color).First(),
                colorNo = _dataContext.Product.Where(p => p.Id == s.ProductId).Select(p => p.ColorNo).First()
            })
            .ToListAsync();

            return Ok(car);

        }

        [HttpDelete]
        public async Task<ActionResult> RmoveFromCart(int userid, int productud)
        {

            var pro = await _dataContext.ShoppingCarts.Where(o => o.UserId == userid && o.ProductId == productud).ToListAsync();
            _dataContext.ShoppingCarts.RemoveRange(pro);
           await _dataContext.SaveChangesAsync();

            return Ok();
        }


       /* [Route("api/Cart/Clear")]
        [HttpDelete]
        public async Task<ActionResult> ClearCart(int userid) 
        {
            var items = await _dataContext.ShoppingCarts.Where(p => p.UserId == userid).ToListAsync();
            _dataContext.RemoveRange(items);   
            await _dataContext.SaveChangesAsync();
            return Ok();
        }*/



    }
}
