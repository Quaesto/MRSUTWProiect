using MRSTWEb.BuisnessLogic.BuisnessModels;
using MRSTWEb.BuisnessLogic.Interfaces;
using MRSTWEb.BusinessLogic.DTO;
using MRSTWEb.Domain.Interfaces;
using MRSTWEb.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MRSTWEb.BuisnessLogic.Services
{
    public class WishListService : IWishListService
    {
        private IUnitOfWork Database;
        public WishListService(IUnitOfWork Database) { this.Database = Database; }
        public WishListService() { this.Database = new EFUnitOfWork(); }

        public void AddToWishList(BookDTO bookDTO)
        {
            List<WishList> wishList = GetWishList();

            int index = IsInList(wishList, bookDTO.Id);
            if (index == -1)
            {
                var item = new WishList { BookDTO = bookDTO };
                wishList.Add(item);
            }
            UpdateList(wishList);
        }

        public List<WishList> GetWishList()
        {
            if (HttpContext.Current.Session["wish"] == null)
            {
                HttpContext.Current.Session["wish"] = new List<WishList>();
            }
            return (List<WishList>)HttpContext.Current.Session["wish"];
        }

        public void RemoveFromTheList(int BookId)
        {
            List<WishList> wishList = GetWishList();
            int index = IsInList(wishList, BookId);
            wishList.RemoveAt(index);
            UpdateList(wishList);
        }
        private void UpdateList(List<WishList> wishList)
        {
            HttpContext.Current.Session["wish"] = wishList;
        }
        private int IsInList(List<WishList> wishList, int bookId)
        {
            for (int i = 0; i < wishList.Count; i++)
            {
                if (wishList[i].BookDTO.Id == bookId) { return i; }

            }
            return -1;
        }
        public void Dispose()
        {
            Database.Dispose();
        }

    }
}
