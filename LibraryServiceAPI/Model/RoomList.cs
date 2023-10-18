using System;
namespace LibraryServiceAPI.Model
{
    public class RoomList
    {
        public int id { get; set; }
        public int RoomNumber { get; set; }
        public DateTime AvailableDate { get; set; }
        public bool IsAvailable { get; set; }
    }
}

