using Microsoft.AspNetCore.Mvc;
using TenmoServer.DAO;
using TenmoServer.Models;
using TenmoServer.Security;
using Microsoft.AspNetCore.Authorization;




namespace TenmoServer.Controllers
{
    [Authorize]
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

        [HttpPut("{id}")]
        public ActionResult<Account> UpdateAccount(Account account)
        {
            bool isUpdated = accountDao.UpdateAccount(account);
            if(isUpdated == true)
            {
                return account;
            }
            else
            {
                return NotFound();
            }
        }
        [HttpGet("accountid/{id}")]
        public ActionResult<Account> RecieveAccountByAccountId(int id)
        {
            Account account = accountDao.GetAccountByAccountId(id);

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
