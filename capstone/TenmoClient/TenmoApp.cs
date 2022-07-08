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
            }

            if (menuSelection == 4)
            {
                // Send TE bucks
                TransferTEBucks();
            }

            if (menuSelection == 5)
            {
                // Request TE bucks
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
            console.PrintSuccess($"yo insane! you have so much money! Look! {currentBalance}");
            console.Pause();
        }

        public void GetTransfers()
        {
            int currentUserId = tenmoApiService.UserId;
            Account currentAccount = tenmoApiService.RetrieveAccount(currentUserId);
            List<Transfer> transfers = new List<Transfer>();
            int currentAccountId = currentAccount.AccountId;
            transfers = tenmoApiService.GetTransfersByAccountId(currentAccountId);

            foreach(Transfer item in transfers)
            {
                console.PrintSuccess($"Transfer id: {item.TransferId}, from account: {item.AccountFrom}, to account: {item.AccountTo}, with the amount of: {item.Amount}.");

            }
            console.Pause();
        }

        public void TransferTEBucks()
        {
            List<User> listOfUsers = tenmoApiService.GetListOfUsers();
            
            foreach(User item in listOfUsers)
            {
                if (tenmoApiService.UserId != item.UserId)
                {
                    console.PrintSuccess($"User id: {item.UserId}, user name: {item.Username}");
                }
                
            }
            
            //console.Pause();
            int recievingUserId = console.PromptForInteger("Please enter the id of the user you want to send money to!");
            if(recievingUserId == tenmoApiService.UserId)
            {
                console.PrintError("STOP TRYING TO SEND MONEY TO YOURSELF");
                console.Pause();
                return;
            }
            decimal amountToSend = console.PromptForDecimal("Please enter the amount of money you want to send:)");
            if(amountToSend <= 0)
            {
                console.PrintError("Hey, the amount you want to send needs to be more than 0 dummy!");
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
                console.PrintError("Hey, you dont have the funds for this playa'.");
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


    }
}
