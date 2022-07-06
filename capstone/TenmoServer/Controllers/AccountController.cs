using Microsoft.AspNetCore.Mvc;
using TenmoServer.DAO;
using TenmoServer.Models;
using TenmoServer.Security;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountDao accountDao;

        public AccountController(IAccountDao _accountDao)
        {
            accountDao = _accountDao;
        }

        [HttpGet("{id}")]
        public ActionResult<Account> GetAccountByUserId(int id)
        {
            Account account = accountDao.GetAccount(id);

            if (account != null)
            {
                return account;
            }
            else
            {
                return NotFound();
            }
        }

    }
}
