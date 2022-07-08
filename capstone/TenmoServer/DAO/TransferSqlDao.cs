using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using TenmoServer.Models;


namespace TenmoServer.DAO
{
    public class TransferSqlDao : ITransferDao
    {
     
            private readonly string connectionString;

            public TransferSqlDao(string dbConnectionString)
            {
                connectionString = dbConnectionString;
            }

            public Transfer GetTransfer(int TransferId)
            {
                Transfer returnTransfer = null;

                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        SqlCommand cmd = new SqlCommand("SELECT * FROM transfer WHERE transfer_id = @transferId;", conn);
                        cmd.Parameters.AddWithValue("@transferId", TransferId);
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            returnTransfer = GetTransferFromReader(reader);
                        }
                    }
                }
                catch (SqlException)
                {
                    throw;
                }

                return returnTransfer;
            }

        public List<Transfer> GetListOfTransfers(int accountId)
        {
            List<Transfer> transfers = new List<Transfer>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {

                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM transfer WHERE account_from = @accountid OR account_to = @accountid;", conn);

                cmd.Parameters.AddWithValue("@accountid", accountId);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Transfer transfer = GetTransferFromReader(reader);
                    transfers.Add(transfer);
                }
            }
            return transfers;
        }

        public Transfer CreateTransfer(Transfer newTransfer)
        {
            Transfer returnTransfer = null;
            int newTransferId = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {

                conn.Open();
                SqlCommand cmd = new SqlCommand(" INSERT INTO transfer (transfer_type_id, transfer_status_id, account_from, account_to, amount) OUTPUT INSERTED.transfer_id VALUES(@transferTypeId, @transferStatusId, @accountFrom, @accountTo, @amount)", conn);

                cmd.Parameters.AddWithValue("@transferTypeId", newTransfer.TransferTypeId);
                cmd.Parameters.AddWithValue("@transferStatusId", newTransfer.TransferStatusId);
                cmd.Parameters.AddWithValue("@accountFrom", newTransfer.AccountFrom);
                cmd.Parameters.AddWithValue("@accountTo", newTransfer.AccountTo);
                cmd.Parameters.AddWithValue("@amount", newTransfer.Amount);

                newTransferId = Convert.ToInt32(cmd.ExecuteScalar());


               
            }
            returnTransfer = GetTransfer(newTransferId);
            return returnTransfer;
        }



        public Transfer UpdateTransfer(Transfer newTransfer)
        {
            Transfer returnTransfer = null;
           

            using (SqlConnection conn = new SqlConnection(connectionString))
            {

                conn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE transfer SET transfer_status_id = @transferStatusId WHERE transfer_id = @transferid", conn);

                cmd.Parameters.AddWithValue("@transferid", newTransfer.TransferId);
                cmd.Parameters.AddWithValue("@transferStatusId", newTransfer.TransferStatusId);


                cmd.ExecuteNonQuery();



            }
            returnTransfer = GetTransfer(newTransfer.TransferId);
            return returnTransfer;
        }

        private Transfer GetTransferFromReader(SqlDataReader reader)
            {
                Transfer t = new Transfer()
                {
                    TransferId = Convert.ToInt32(reader["transfer_id"]),
                    TransferTypeId = Convert.ToInt32(reader["transfer_type_id"]),
                    TransferStatusId = Convert.ToInt32(reader["transfer_status_id"]),
                    AccountFrom = Convert.ToInt32(reader["account_from"]),
                    AccountTo = Convert.ToInt32(reader["account_to"]),
                    Amount = Convert.ToDecimal(reader["amount"])
                };

                return t;
            }
        }
    }

