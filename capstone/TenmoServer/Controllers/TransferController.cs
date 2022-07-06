using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TenmoServer.DAO;
using TenmoServer.Models;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TransferController :ControllerBase
    {
        private readonly ITransferDao transferDao;

        public TransferController(ITransferDao _transferDao)
        {
            transferDao = _transferDao;
        }
        [HttpGet("{id}")]
        public ActionResult<Transfer> GetTransferByTransferId(int id)
        {
            Transfer transfer = transferDao.GetTransfer(id);

            if (transfer != null)
            {
                return transfer;
            }
            else
            {
                return NotFound();
            }
        }
        [HttpGet("accountid/{id}")]
        public ActionResult<List<Transfer>> GetListOfTransferByAccountId(int id)
        {
            List<Transfer> transfer = transferDao.GetListOfTransfers(id);
            if (transfer != null)
            {
                return transfer;
            }
            else
            {
                return NotFound();
            }

        }

        [HttpPost()]
        public ActionResult<Transfer> AddTransfer(Transfer transfer)
        {
            Transfer newTransfer = transferDao.CreateTransfer(transfer);

            if (newTransfer != null)
            {
                return newTransfer;
            }
            else
            {
                return NotFound();
            }
        }

    }
}
