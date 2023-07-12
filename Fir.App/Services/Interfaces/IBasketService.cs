using Fir.App.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Fir.App.Services.Interfaces
{
    public interface IBasketService
    {
        public Task AddBasket(int id, int? count);
        public Task<List<BasketItemVM>> GetAll();
        public Task Remove(int id);
    }
}
