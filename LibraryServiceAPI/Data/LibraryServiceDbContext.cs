using System;
using LibraryServiceAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace LibraryServiceAPI.Data
{
    public class LibraryServiceDbContext : DbContext
    {
        public LibraryServiceDbContext(DbContextOptions<LibraryServiceDbContext> options) : base(options)
        {
        }

        public DbSet<UserInfo> userInfos { get; set; }
        public DbSet<RoomList> roomLists { get; set; }
        public DbSet<BookList> bookLists { get; set; }
        public DbSet<OnlineResourceList> onlineResourceLists { get; set; }

    }
}

