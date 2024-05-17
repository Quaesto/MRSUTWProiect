using MRSTWEb.BuisnessLogic.BuisnessModels;
using MRSTWEb.BusinessLogic.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRSTWEb.BuisnessLogic.Interfaces
{
    public interface IWishListService
    {
        void AddToWishList(BookDTO bookDTO);
        List<WishList> GetWishList();
        void RemoveFromTheList(int BookId);
        void Dispose();
    }
}
