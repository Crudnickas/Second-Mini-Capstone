using RestSharp;
using System.Collections.Generic;
using TenmoClient.Models;

using System.Net.Http;

namespace TenmoClient.Services
{
    public class TenmoApiService : AuthenticatedApiService
    {
        public readonly string ApiUrl;

        public TenmoApiService(string apiUrl) : base(apiUrl) { }

        // Add methods to call api here...

        public Account RetrieveAccount(int userId)
        {
            RestRequest rest = new RestRequest($"account/{userId}");
            IRestResponse<Account> response = client.Get<Account>(rest);

            CheckForError(response);
            return response.Data;
        }
        
        public List<Transfer> GetTransfersByAccountId(int accountId)
        {
            RestRequest rest = new RestRequest($"transfer/accountid/{accountId}");
            IRestResponse<List<Transfer>> response = client.Get<List<Transfer>>(rest);

            CheckForError(response);
            return response.Data;
        }

        public Transfer CreateTransfer(Transfer newTransfer)
        {
            RestRequest rest = new RestRequest("transfer");
            rest.AddJsonBody(newTransfer);
            IRestResponse<Transfer> response = client.Post<Transfer>(rest);
            CheckForError(response);
            return response.Data;
        }
        public Account UpdateAccount(Account updatedAccount)
        {
            RestRequest rest = new RestRequest($"account/{updatedAccount.AccountId}");
            rest.AddJsonBody(updatedAccount);
            IRestResponse<Account> response = client.Put<Account>(rest);
            CheckForError(response);
            return response.Data;
        }
        public List<User> GetListOfUsers()
        {
            RestRequest rest = new RestRequest("user");
            IRestResponse<List<User>> response = client.Get<List<User>>(rest);
            CheckForError(response);
            return response.Data;
        }
        public Transfer UpdateTransferStatus(Transfer updatedTransfer)
        {
            RestRequest rest = new RestRequest($"transfer/{updatedTransfer.TransferId}");
            rest.AddJsonBody(updatedTransfer);
            IRestResponse<Transfer> response = client.Put<Transfer>(rest);
            CheckForError(response);
            return response.Data;
        }
        public Transfer GetTransferByTransferId(int id)
        {
            RestRequest rest = new RestRequest($"transfer/{id}");
            IRestResponse<Transfer> response = client.Get<Transfer>(rest);
            CheckForError(response);
            return response.Data;
        }
        public Account GetAccountByAccountId(int id)
        {
            RestRequest rest = new RestRequest($"account/accountid/{id}");
            IRestResponse<Account> response = client.Get<Account>(rest);
            CheckForError(response);
            return response.Data;
        }
    }
}
