using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TenmoServer.DAO;
using TenmoServer.Models;
using TenmoServer.Security;



namespace TenmoServer.Controllers
{
    
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserDao userDao;

        public UserController(IUserDao _userDao)
        {
            userDao = _userDao;
        }

        [HttpGet()]
        public ActionResult<List<User>> GetUsers()
        {
            List<User> listOfUsers = null;
            listOfUsers =  userDao.GetUsers();
            if (listOfUsers != null)
            {
                return listOfUsers;
            }
            else
            {
                return NotFound();
            }
        }
    }
}
