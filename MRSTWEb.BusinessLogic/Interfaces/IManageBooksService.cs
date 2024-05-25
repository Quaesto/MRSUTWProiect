using MRSTWEb.BusinessLogic.DTO;

namespace MRSTWEb.BuisnessLogic.Interfaces
{
    public interface IManageBooksService
    {
        void UpdateProduct(BookDTO bookDTO);
        void AddBook(BookDTO bookDTO);
        void DeleteBook(int BookId);
        void Dispose();
    }
}
