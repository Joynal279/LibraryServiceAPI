using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LibraryServiceAPI.Data;
using LibraryServiceAPI.Helper;
using LibraryServiceAPI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace LibraryServiceAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LibraryServiceController : ControllerBase
    {

        private readonly IConfiguration _configuration;

        private readonly LibraryServiceDbContext _dbContext;
        private readonly IWebHostEnvironment environment;

        public LibraryServiceController(IConfiguration configuration, LibraryServiceDbContext dbContext, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            this.environment = environment;
        }

        //======================================= User Info start ======================================

        //MARK: - Book Room
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<UserInfo> BookRoom(UserInfo request)
        {
            RoomResponse response = new RoomResponse();
            var room = new List<RoomList>();

            if (!isValidEmailDomain(request.Email))
            {
                response.ResponseCode = 400;
                response.Errormessage = "Email is not valid";
                return BadRequest(response);
            }

            if (request.FirstName.IsNullOrEmpty() || request.LastName.IsNullOrEmpty() || request.Email.IsNullOrEmpty() || request.StudentId.IsNullOrEmpty())
            {
                response.ResponseCode = 400;
                response.Errormessage = "Please fillup all field";
                return BadRequest(response);
            }

            request.BookDate = DateTime.Today;

            var data = _dbContext.roomLists.FirstOrDefault(u => u.AvailableDate == request.BookDate && u.RoomNumber == request.RoomNumber);

            if (data == null)
            {
                response.ResponseCode = 200;
                response.Result = "Room not available this date & time";
                response.data = room;
                return BadRequest(response);
            }

            data.IsAvailable = false;

            _dbContext.roomLists.Update(data);
            _dbContext.userInfos.Add(request);

            _dbContext.SaveChanges();




            room.Add(data);

            response.ResponseCode = 200;
            response.Result = "Number Room booked successfully";
            response.data = room;

            return Ok(response);

        }

        //MARK: - Get All User
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<UserInfo>>> GetAllUser()
        {
            APIResponse response = new APIResponse();

            if (_dbContext.userInfos == null)
            {
                response.ResponseCode = 200;
                response.Result = "No data found";
                return Ok(response);
            }

            return Ok(_dbContext.userInfos.ToList());
        }

        //MARK: - User Delete
        [HttpDelete]
        public ActionResult<UserInfo> DeleteUser(int userId)
        {
            APIResponse response = new APIResponse();

            var data = _dbContext.userInfos.FirstOrDefault(u => u.id == userId);
            if (data != null)
            {
                _dbContext.userInfos.Remove(data);
                _dbContext.SaveChanges();

                response.ResponseCode = 200;
                response.Result = "Successfully deleted";
                return Ok(response);
            }

            response.ResponseCode = 400;
            response.Errormessage = "User not found";

            return NotFound(response);
        }


        //Valided Email Address
        private static bool isValidEmailDomain(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            string[] parts = email.Split('@');
            if (parts.Length != 2)
            {
                return false; // email must have exactly one @ symbol
            }

            string localPart = parts[0];
            string domainPart = parts[1];

            try
            {
                // check if domain name has a valid MX record
                var hostEntry = Dns.GetHostEntry(domainPart);
                return hostEntry.HostName.Length > 0;
            }
            catch
            {
                return false; // domain name is invalid or does not have a valid MX record
            }
        }

        //======================================= User Info End ======================================

        //======================================= Room List start ======================================

        //MARK: - Room Add
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<RoomList> RoomAdd(RoomList request)
        {
            APIResponse response = new APIResponse();

            if (request == null)
            {
                response.ResponseCode = 400;
                response.Errormessage = "Please fillup all field";
                return BadRequest(response);
            }

            request.AvailableDate = DateTime.Today.AddDays(1);

            _dbContext.roomLists.Add(request);
            _dbContext.SaveChanges();

            return Created("Room added succussfully", request);

        }

        //MARK: - Get All Room
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RoomList>>> GetAllRoom()
        {
            RoomResponse response = new RoomResponse();

            if (_dbContext.userInfos == null)
            {
                response.ResponseCode = 200;
                response.Result = "No data found";
                return Ok(response);
            }

            response.ResponseCode = 200;
            response.Result = "data found successfully";
            response.data = _dbContext.roomLists.ToList();

            return Ok(response);
        }

        //MARK: - Search room
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RoomList>>> GetAllAvailableRoom(DateTime inputDate)
        {
            RoomResponse response = new RoomResponse();
            var room = new List<RoomList>();

            if (_dbContext.userInfos == null)
            {
                response.ResponseCode = 200;
                response.Errormessage = "No data found";
                response.data = room;
                return Ok(response);
            }

            var data = _dbContext.roomLists.Where(u => u.AvailableDate == inputDate && u.IsAvailable == true);

            if (data == null)
            {
                response.ResponseCode = 200;
                response.Result = "No room available this date";
                response.data = room;
                return Ok(response);
            }

            response.ResponseCode = 200;
            response.Result = "Data found successfully";
            response.data = data.ToList();

            return Ok(response);
        }

        //MARK: - User Room
        [HttpDelete]
        public ActionResult<RoomList> DeleteRoom(int roomId)
        {
            APIResponse response = new APIResponse();

            var data = _dbContext.roomLists.FirstOrDefault(u => u.id == roomId);
            if (data != null)
            {
                _dbContext.roomLists.Remove(data);
                _dbContext.SaveChanges();

                response.ResponseCode = 200;
                response.Result = "Successfully room deleted";
                return Ok(response);
            }

            response.ResponseCode = 400;
            response.Errormessage = "Room not found";

            return NotFound(response);
        }

        //======================================= Room List End ======================================

        //======================================= Book List start ======================================

        //MARK: - Book Add
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<BookList> BookAdd(BookList request)
        {
            BookResponse response = new BookResponse();

            if (request == null)
            {
                response.ResponseCode = 400;
                response.Errormessage = "Please fillup all field";
                return BadRequest(response);
            }

            if (request.BookName.IsNullOrEmpty())
            {
                response.ResponseCode = 400;
                response.Errormessage = "Please fillup all field";
                return BadRequest(response);
            }

            _dbContext.bookLists.Add(request);
            _dbContext.SaveChanges();

            return Created("Book added succussfully", request);

        }

        //MARK: - Get All Book
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BookList>>> GetAllBook()
        {
            BookResponse response = new BookResponse();

            if (_dbContext.userInfos == null)
            {
                response.ResponseCode = 200;
                response.Result = "No data found";
                return Ok(response);
            }

            response.ResponseCode = 200;
            response.Result = "data found successfully";
            response.data = _dbContext.bookLists.ToList();

            return Ok(response);
        }

        //MARK: - User Delete
        [HttpDelete]
        public ActionResult<BookList> DeleteBook(int bookId)
        {
            APIResponse response = new APIResponse();

            var data = _dbContext.bookLists.FirstOrDefault(u => u.id == bookId);
            if (data != null)
            {
                _dbContext.bookLists.Remove(data);
                _dbContext.SaveChanges();

                response.ResponseCode = 200;
                response.Result = "Successfully deleted book";
                return Ok(response);
            }

            response.ResponseCode = 400;
            response.Errormessage = "Book not found";

            return NotFound(response);
        }

        //MARK: - Book Edit
        [HttpPut("{id}")]
        public async Task<IActionResult> EditBook(int id, [FromBody] BookList resource)
        {
            APIResponse response = new APIResponse();

            if (resource.BookName.IsNullOrEmpty() || resource == null)
            {
                response.ResponseCode = 400;
                response.Errormessage = "Please fillup all field";
                return BadRequest(response);
            }

            var data = _dbContext.bookLists.FirstOrDefault(u => u.id == resource.id);

            if (data == null)
            {
                response.ResponseCode = 200;
                response.Result = "Data not found";
                return Ok(response);
            }

            data.BookId = resource.BookId;
            data.BookName = resource.BookName;
            data.AvailableCopyNumber = resource.AvailableCopyNumber;


            _dbContext.bookLists.Update(data);
            _dbContext.SaveChanges();

            response.ResponseCode = 200;
            response.Result = "Data updated successfully";

            return Ok(response);

        }

        //MARK: - Book Edit
        [HttpGet]
        public async Task<IActionResult> BorrowBook(int bookId, DateTime date)
        {
            BookResponse response = new BookResponse();

            if (bookId == null || date == null)
            {
                response.ResponseCode = 400;
                response.Errormessage = "Please fillup all field";
                return BadRequest(response);
            }

            var data = _dbContext.bookLists.FirstOrDefault(u => u.id == bookId);

            if (data == null)
            {
                response.ResponseCode = 200;
                response.Result = "Data not found";
                return Ok(response);
            }

            data.AvailableCopyNumber = (data.AvailableCopyNumber) - 1;


            _dbContext.bookLists.Update(data);
            _dbContext.SaveChanges();

            var listOfBooks = new List<BookList>();
            listOfBooks.Add(data);

            response.ResponseCode = 200;
            response.Result = "Book borrowed successfully";
            response.data = listOfBooks;

            return Ok(response);

        }

        //MARK: - Search Online Recourse
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BookList>>> SearchBook(string name)
        {
            BookResponse response = new BookResponse();
            var resource = new List<BookList>();

            if (_dbContext.bookLists == null)
            {
                response.ResponseCode = 200;
                response.Result = "No data found";
                response.data = resource;
                return Ok(response);
            }

            var data = _dbContext.bookLists.Where(u => u.BookName.ToLower().Contains(name.ToLower()));

            if (data == null)
            {
                response.ResponseCode = 200;
                response.Result = "No data found";
                response.data = resource;
                return Ok(response);
            }

            response.ResponseCode = 200;
            response.Result = "Data found successfully";
            response.data = data.ToList();

            return Ok(response);
        }


        //======================================= Book List End ======================================

        //======================================= Online Resource List start ======================================

        //MARK: - Recource Add
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<OnlineResourceList> ResourceAdd(OnlineResourceList request)
        {
            APIResponse response = new APIResponse();

            if (request == null)
            {
                response.ResponseCode = 400;
                response.Errormessage = "Please fillup all field";
                return BadRequest(response);
            }

            if (request.ResourceName.IsNullOrEmpty())
            {
                response.ResponseCode = 400;
                response.Errormessage = "Please fillup name field";
                return BadRequest(response);
            }

            if (request.ResourceURL.IsNullOrEmpty())
            {
                response.ResponseCode = 400;
                response.Errormessage = "Please fillup Resource URL field";
                return BadRequest(response);
            }


            _dbContext.onlineResourceLists.Add(request);
            _dbContext.SaveChanges();

            return Created("Resource added succussfully", request);

        }

        //MARK: - Get All Recourse
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OnlineResourceList>>> GetAllResource()
        {
            OnlineResourceResponse response = new OnlineResourceResponse();
            var resource = new List<OnlineResourceList>();

            if (_dbContext.onlineResourceLists == null)
            {
                response.ResponseCode = 200;
                response.Result = "No data found";
                response.data = resource;
                return Ok(response);
            }

            response.ResponseCode = 200;
            response.Result = "Data found successfully";
            response.data = _dbContext.onlineResourceLists.ToList();

            return Ok(response);
        }

        //MARK: - Search Online Recourse
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OnlineResourceList>>> SearchOnlineResource(string name)
        {
            OnlineResourceResponse response = new OnlineResourceResponse();
            var resource = new List<OnlineResourceList>();

            if (_dbContext.onlineResourceLists == null)
            {
                response.ResponseCode = 200;
                response.Result = "No data found";
                response.data = resource;
                return Ok(response);
            }

            var data = _dbContext.onlineResourceLists.Where(u => u.ResourceName.ToLower().Contains(name.ToLower()));

            if (data == null)
            {
                response.ResponseCode = 200;
                response.Result = "No data found";
                response.data = resource;
                return Ok(response);
            }

            response.ResponseCode = 200;
            response.Result = "Data found successfully";
            response.data = data.ToList();

            return Ok(response);
        }

        //MARK: - Resource Delete
        [HttpDelete]
        public ActionResult<OnlineResourceList> DeleteResource(int resourceId)
        {
            APIResponse response = new APIResponse();

            var data = _dbContext.onlineResourceLists.FirstOrDefault(u => u.id == resourceId);
            if (data != null)
            {
                _dbContext.onlineResourceLists.Remove(data);
                _dbContext.SaveChanges();

                response.ResponseCode = 200;
                response.Result = "Successfully deleted";
                return Ok(response);
            }

            response.ResponseCode = 400;
            response.Errormessage = "Resource not found";

            return NotFound(response);
        }

        //MARK: - Resource Edit
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] OnlineResourceList resource)
        {
            APIResponse response = new APIResponse();

            if (resource.ResourceName.IsNullOrEmpty() || resource.ResourceURL.IsNullOrEmpty())
            {
                response.ResponseCode = 400;
                response.Errormessage = "Please fillup all field";
                return BadRequest(response);
            }

            var data = _dbContext.onlineResourceLists.FirstOrDefault(u => u.id == resource.id);

            if (data == null)
            {
                response.ResponseCode = 200;
                response.Result = "Data not found";
                return Ok(response);
            }

            data.ResourceName = resource.ResourceName;
            data.ResourceURL = resource.ResourceURL;


            _dbContext.onlineResourceLists.Update(data);
            _dbContext.SaveChanges();

            response.ResponseCode = 200;
            response.Result = "Data updated successfully";

            return Ok(response);

        }

        //======================================= Online Resource List End ======================================

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile formFile, string filecode)
        {
            APIResponse response = new APIResponse();
            try
            {
                string Filepath = GetFilepath(filecode);
                if (!System.IO.Directory.Exists(Filepath))
                {
                    System.IO.Directory.CreateDirectory(Filepath);
                }

                string imagepath = Filepath + "/" + filecode + ".pdf";
                if (System.IO.File.Exists(imagepath))
                {
                    System.IO.File.Delete(imagepath);
                }
                using (FileStream stream = System.IO.File.Create(imagepath))
                {
                    await formFile.CopyToAsync(stream);
                    response.ResponseCode = 200;
                    response.Result = "Pdf file upload successfully";
                }
            }
            catch (Exception ex)
            {
                response.Errormessage = ex.Message;
            }
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> downloadFile(string filecode)
        {
            try
            {
                string Filepath = GetFilepath(filecode);
                string imagepath = Filepath + "/" + filecode + ".pdf";
                if (System.IO.File.Exists(imagepath))
                {
                    MemoryStream stream = new MemoryStream();
                    using (FileStream fileStream = new FileStream(imagepath, FileMode.Open))
                    {
                        await fileStream.CopyToAsync(stream);
                    }
                    stream.Position = 0;
                    return File(stream, "image/png", filecode + ".pdf");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }


        }

        [NonAction]
        private string GetFilepath(string filecode)
        {
            return this.environment.WebRootPath + "/" + filecode;
        }
    }
}

