using System;
namespace LibraryServiceAPI.Model
{
    public class BookList
    {
        public int id { get; set; }
        public int BookId { get; set; }
        public string BookName { get; set; }
        public int AvailableCopyNumber { get; set; }
    }
}

