using System;
namespace LibraryServiceAPI.Model
{
	public class UserInfo
	{
		public int id { get; set; }
		public string StudentId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public int RoomNumber { get; set; }
		public DateTime BookDate { get; set; }
	}

}

