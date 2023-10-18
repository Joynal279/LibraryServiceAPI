using System;
namespace LibraryServiceAPI.Model
{
    public class OnlineResourceList
    {
        public int id { get; set; }
        public string ResourceName { get; set; }
        public bool IsAvailable { get; set; }
        public string ResourceURL { get; set; }
    }
}

