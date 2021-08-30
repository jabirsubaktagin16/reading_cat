using ReadingCat.Models;

namespace ReadingCat.ViewModel
{
    public class LoginAndBookList
    {
        public LoginAndBookList()
        {
            loginModel = new LoginModel();
            booksAndDatabase = new BooksAndDatabase();
        }
        public LoginModel loginModel { get; set; }
        public BooksAndDatabase booksAndDatabase { get; set; }
    }
}