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
                    Transfer tranfer = GetTransferFromReader(reader);
                    transfers.Add(tranfer);
                }
            }
            return transfers;
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

