using Loza.Data;
using Loza.Repository.Abstract;

namespace Loza.Repository.Implementaion
{
    public class ClearCart:IClearCart
    {

        private readonly DataContext _dataContext;

        public ClearCart(DataContext dataContext)
        {
            
            _dataContext = dataContext;
        }
        public async Task clearCart(int userid) 
        {
            var items = await _dataContext.ShoppingCarts.Where(p => p.UserId == userid).ToListAsync();
            _dataContext.RemoveRange(items);
            await _dataContext.SaveChangesAsync();
            return ;
            
        }

    }
}
