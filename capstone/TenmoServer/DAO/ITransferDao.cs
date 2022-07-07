using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;
namespace TenmoServer.DAO
{
    public interface ITransferDao
    {
        Transfer GetTransfer(int transferId);
        List<Transfer> GetListOfTransfers(int accountId);
        Transfer CreateTransfer(Transfer newTransfer);
    }
}
