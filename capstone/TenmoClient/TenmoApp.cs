using System;
using System.Collections.Generic;
using TenmoClient.Models;
using TenmoClient.Services;
using System.Data.SqlClient;


namespace TenmoClient
{
    public class TenmoApp
    {
        private readonly TenmoConsoleService console = new TenmoConsoleService();
        private readonly TenmoApiService tenmoApiService;

        public TenmoApp(string apiUrl)
        {
            tenmoApiService = new TenmoApiService(apiUrl);
        }

        public void Run()
        {
            bool keepGoing = true;
            while (keepGoing)
            {
                // The menu changes depending on whether the user is logged in or not
                if (tenmoApiService.IsLoggedIn)
                {
                    keepGoing = RunAuthenticated();
                }
                else // User is not yet logged in
                {
                    keepGoing = RunUnauthenticated();
                }
            }
        }

        private bool RunUnauthenticated()
        {
            console.PrintLoginMenu();
            int menuSelection = console.PromptForInteger("Please choose an option", 0, 2, 1);
            while (true)
            {
                if (menuSelection == 0)
                {
                    return false;   // Exit the main menu loop
                }

                if (menuSelection == 1)
                {
                    // Log in
                    Login();
                    return true;    // Keep the main menu loop going
                }

                if (menuSelection == 2)
                {
                    // Register a new user
                    Register();
                    return true;    // Keep the main menu loop going
                }
                console.PrintError("Invalid selection. Please choose an option.");
                console.Pause();
            }
        }

        private bool RunAuthenticated()
        {
            console.PrintMainMenu(tenmoApiService.Username);
            int menuSelection = console.PromptForInteger("Please choose an option", 0, 6);
            if (menuSelection == 0)
            {
                // Exit the loop
                return false;
            }

            if (menuSelection == 1)
            {
                ViewBalance();
            }

            if (menuSelection == 2)
            {
                // View your past transfers
                GetTransfers();
            }

            if (menuSelection == 3)
            {
                // View your pending requests
                GetPendingTransfers();
            }

            if (menuSelection == 4)
            {
                // Send TE bucks
                TransferTEBucks();
            }

            if (menuSelection == 5)
            {
                // Request TE bucks
                RequestTEBucks();
            }

            if (menuSelection == 6)
            {
                // Log out
                tenmoApiService.Logout();
                console.PrintSuccess("You are now logged out");
            }

            return true;    // Keep the main menu loop going
        }

        private void Login()
        {
            LoginUser loginUser = console.PromptForLogin();
            if (loginUser == null)
            {
                return;
            }

            try
            {
                ApiUser user = tenmoApiService.Login(loginUser);
                if (user == null)
                {
                    console.PrintError("Login failed.");
                }
                else
                {
                    console.PrintSuccess("You are now logged in");
                }
            }
            catch (Exception)
            {
                console.PrintError("Login failed.");
            }
            console.Pause();
        }

        private void Register()
        {
            LoginUser registerUser = console.PromptForLogin();
            if (registerUser == null)
            {
                return;
            }
            try
            {
                bool isRegistered = tenmoApiService.Register(registerUser);
                if (isRegistered)
                {
                    console.PrintSuccess("Registration was successful. Please log in.");
                }
                else
                {
                    console.PrintError("Registration was unsuccessful.");
                }
            }
            catch (Exception)
            {
                console.PrintError("Registration was unsuccessful.");
            }
            console.Pause();
        }

        public void ViewBalance()
        {
            int currentUserId = tenmoApiService.UserId;
            Account currentAccount = tenmoApiService.RetrieveAccount(currentUserId);
            decimal currentBalance = currentAccount.Balance;
            console.PrintSuccess($"Here is your current balance: {currentBalance}");
            console.Pause();
        }

        public void GetTransfers()
        {
            int currentUserId = tenmoApiService.UserId;
            Account currentAccount = tenmoApiService.RetrieveAccount(currentUserId);
            List<Transfer> transfers = new List<Transfer>();
            int currentAccountId = currentAccount.AccountId;
            transfers = tenmoApiService.GetTransfersByAccountId(currentAccountId);
            Console.WriteLine("|transfer id|account id|amount|");
            Console.WriteLine("*******************************");
            foreach (Transfer item in transfers)
            {
                

                if (item.TransferStatusId == 2)
                {
                    if(currentAccount.AccountId == item.AccountFrom)
                    {
                        //if we are the account thats sending money, show the account that is recieving it.
                        Console.WriteLine(string.Format($"|{item.TransferId,11}|to: {item.AccountTo,6}|{item.Amount,6}|"));
                    }
                    else
                    {
                        Console.WriteLine(string.Format($"|{item.TransferId,11}|from: {item.AccountFrom,3}|{item.Amount,6}|"));
                    }
                }
                
            }
            Console.WriteLine("*******************************");
            int transferId = console.PromptForInteger("Please enter transfer id to view details. (0 to exit)");
            if (transferId == 0)
            {
                console.PrintSuccess("Exiting back to main menu");
                console.Pause();
                return;
            }
            Transfer currentTransfer = null;
            try 
            { 
            currentTransfer = tenmoApiService.GetTransferByTransferId(transferId);
            }
            catch (Exception exception)
            {
                console.PrintError("Invalid transfer id.");
                console.Pause();
                return;
            }
            console.PrintSuccess($"Transfer id: {currentTransfer.TransferId}, from account: {currentTransfer.AccountFrom}, to account: {currentTransfer.AccountTo}, with the amount of: {currentTransfer.Amount}.");
            console.Pause();
        }

        public void TransferTEBucks()
        {
            List<User> listOfUsers = tenmoApiService.GetListOfUsers();
            Console.WriteLine("| user id | user name |");
            Console.WriteLine("***********************");
            foreach(User item in listOfUsers)
            {
                if (tenmoApiService.UserId != item.UserId)
                {
                    Console.WriteLine(string.Format($"|{item.UserId,10}|{item.Username,10}|"));
                }
                
            }
            Console.WriteLine("***********************");


            int recievingUserId = console.PromptForInteger("Please enter the id of the user you want to send money to. (0 to exit)");
            if(recievingUserId == tenmoApiService.UserId)
            {
                console.PrintError("Make sure you do not try to send money to yourself.");
                console.Pause();
                return;
            }
            if (recievingUserId == 0)
            {
                console.PrintSuccess("Exiting back to main menu");
                console.Pause();
                return;
            }
            decimal amountToSend = console.PromptForDecimal("Please enter the amount of money you want to send.");
            if(amountToSend <= 0)
            {
                console.PrintError("The amount of money you want to send needs to be more than 0.");
                console.Pause();
                return;
            }

            Account sendingAccount = tenmoApiService.RetrieveAccount(tenmoApiService.UserId);

            Account recievingAccount = null;
            try
            {
                recievingAccount = tenmoApiService.RetrieveAccount(recievingUserId);
            }
            catch(Exception exception)
            {
                console.PrintError("Invalid user id.");
                console.Pause();
                return;
            }
            if(amountToSend > sendingAccount.Balance)
            {
                console.PrintError("You don't have the necessary funds for that transaction.");
                console.Pause();
                return;
            }

            Transfer newTransfer = null;
            try
            {
                newTransfer = new Transfer(2, 2, sendingAccount.AccountId, recievingAccount.AccountId, amountToSend);
            }
            catch(Exception exception)
            {
                console.PrintError($"Invalid user id.{exception.Message}{exception.GetType()}");
                console.Pause();
                return;
            }
            sendingAccount.Balance -= amountToSend;
            recievingAccount.Balance += amountToSend;
            
          
            tenmoApiService.UpdateAccount(sendingAccount);
            tenmoApiService.UpdateAccount(recievingAccount);

            try
            {
                tenmoApiService.CreateTransfer(newTransfer);
            }
            catch (SqlException exception)
            {
                console.PrintError($"Invalid user id.{exception.Message}{exception.GetType()}");
                console.Pause();
                return;
            }
            console.PrintSuccess($"You have sent {amountToSend} to {recievingUserId}!");
            console.Pause();


        }

        public void RequestTEBucks()
        {
            List<User> listOfUsers = tenmoApiService.GetListOfUsers();

            Console.WriteLine("| user id | user name |");
            Console.WriteLine("***********************");
            foreach (User item in listOfUsers)
            {
                if (tenmoApiService.UserId != item.UserId)
                {
                    Console.WriteLine(string.Format($"|{item.UserId,10}|{item.Username,10}|"));
                }

            }
            Console.WriteLine("***********************");
            int requestedUserId = console.PromptForInteger("Please enter the id of the user you want to request money from. (0 to exit)");
            if (requestedUserId == 0)
            {
                console.PrintSuccess("Exiting back to main menu");
                console.Pause();
                return;
            }
            if (requestedUserId == tenmoApiService.UserId)
            {
                console.PrintError("Make sure you do not try to send money to yourself.");
                console.Pause();
                return;
            }
            decimal amountToRecieve = console.PromptForDecimal("Please enter the amount of money you wish to recieve.");
            if (amountToRecieve <= 0)
            {
                console.PrintError("The amount of money you want to recieve needs to be more than 0.");
                console.Pause();
                return;
            }
            Account recievingAccount = tenmoApiService.RetrieveAccount(tenmoApiService.UserId);

            Account sendingAccount = null;
            try
            {
                sendingAccount = tenmoApiService.RetrieveAccount(requestedUserId);
            }
            catch (Exception exception)
            {
                console.PrintError("Invalid user id.");
                console.Pause();
                return;
            }
            if (amountToRecieve > sendingAccount.Balance)
            {
                console.PrintError("The person does not have the necessary funds for that transaction.");
                console.Pause();
                return;
            }

            Transfer newTransfer = null;
            try
            {
                newTransfer = new Transfer(1, 1, sendingAccount.AccountId, recievingAccount.AccountId, amountToRecieve);
            }
            catch (Exception exception)
            {
                console.PrintError($"Invalid user id.{exception.Message}{exception.GetType()}");
                console.Pause();
                return;
            }
            try
            {
                tenmoApiService.CreateTransfer(newTransfer);
            }
            catch (SqlException exception)
            {
                console.PrintError($"Invalid user id.{exception.Message}{exception.GetType()}");
                console.Pause();
                return;
            }
            console.PrintSuccess($"You have rquested {amountToRecieve} from {requestedUserId}!");
            console.Pause();
        }

        public void GetPendingTransfers()
        {
            int currentUserId = tenmoApiService.UserId;
            Account currentAccount = tenmoApiService.RetrieveAccount(currentUserId);
            List<Transfer> transfers = new List<Transfer>();
            int currentAccountId = currentAccount.AccountId;
            transfers = tenmoApiService.GetTransfersByAccountId(currentAccountId);
            Console.WriteLine("|transfer id|account id|amount|");
            Console.WriteLine("*******************************");

            foreach (Transfer item in transfers)
            {
                if (item.TransferStatusId == 1)
                {
                    Console.WriteLine(string.Format($"|{item.TransferId,11}|to: {item.AccountTo,6}|{item.Amount,6}|"));
                    //console.PrintSuccess($"Transfer id: {item.TransferId}, from account: {item.AccountFrom}, to account: {item.AccountTo}, with the amount of: {item.Amount}.");
                }
            }
            int transferId = console.PromptForInteger("Please enter transfer id to view details. (0 to exit)");
            if (transferId == 0)
            {
                console.PrintSuccess("Exiting back to main menu");
                console.Pause();
                return;
            }
            Transfer currentTransfer = null;
            try
            {
                currentTransfer = tenmoApiService.GetTransferByTransferId(transferId);
            }
            catch (Exception exception)
            {
                console.PrintError("Invalid transfer id.");
                console.Pause();
                return;
            }
            console.PrintSuccess($"Transfer id: {currentTransfer.TransferId}, from account: {currentTransfer.AccountFrom}, to account: {currentTransfer.AccountTo}, with the amount of: {currentTransfer.Amount}.");
            Account recievingAccount = tenmoApiService.GetAccountByAccountId(currentTransfer.AccountTo);
            if(recievingAccount.AccountId == currentAccount.AccountId)
            {
                console.PrintError("You cannot change the status of your own request.");
                console.Pause();
                return;
            }
            int selectionInt = console.PromptForInteger("select 1 for approve and 2 for reject");            
            if(selectionInt == 1)
            {
                Transfer updatedTransfer = currentTransfer;
                updatedTransfer.TransferStatusId = 2;
                Account updatedSendingAccount = currentAccount;
                Account updatedRecievingAccount = recievingAccount;
                updatedSendingAccount.Balance -= currentTransfer.Amount;
                updatedRecievingAccount.Balance += currentTransfer.Amount;
                tenmoApiService.UpdateAccount(updatedSendingAccount);
                tenmoApiService.UpdateAccount(updatedRecievingAccount);
                tenmoApiService.UpdateTransferStatus(updatedTransfer);
                console.PrintSuccess("You have accepted the transfer, money is being sent now.");
                console.Pause();
                return;
            }
            else if(selectionInt == 2)
            {
                Transfer updatedTransfer = currentTransfer;
                updatedTransfer.TransferStatusId = 3;
                tenmoApiService.UpdateTransferStatus(updatedTransfer);
                console.PrintSuccess("You have rejected the transfer.");
                console.Pause();
                return;
            }
            else
            {
                console.PrintError("invalid selection.");
                console.Pause();
                return;
            }
        }
    }
}
