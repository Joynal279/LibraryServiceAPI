using System;
using LibraryServiceAPI.Model;

namespace LibraryServiceAPI.Helper
{
    public class APIResponse
    {
        public int ResponseCode { get; set; }
        public string Result { get; set; }
        public string Errormessage { get; set; }
    }

    public class BookResponse
    {
        public int ResponseCode { get; set; }
        public string Result { get; set; }
        public string Errormessage { get; set; }
        public ICollection<BookList> data { get; set; }
    }

    public class RoomResponse
    {
        public int ResponseCode { get; set; }
        public string Result { get; set; }
        public string Errormessage { get; set; }
        public ICollection<RoomList> data { get; set; }
    }

    public class OnlineResourceResponse
    {
        public int ResponseCode { get; set; }
        public string Result { get; set; }
        public string Errormessage { get; set; }
        public ICollection<OnlineResourceList> data { get; set; }
    }
}

