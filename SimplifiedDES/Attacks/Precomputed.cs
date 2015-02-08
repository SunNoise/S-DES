using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplifiedDES.Attacks
{
    class Precomputed
    {
        internal static List<string> Attack(BitArray[] plainTextArray, BitArray[] cipherTextArray)
        {
            var KeyList = new List<string>();
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["SimplifiedDES.Properties.Settings.PrecomputedConnectionString"].ConnectionString;
                conn.Open();

                StringBuilder query = new StringBuilder("SELECT [key] FROM Combinations WHERE ");
                for (int x = 0; x < cipherTextArray.Length; x++)
                {
                    query.Append("plain = ");
                    query.Append(Tools.BitArrayToString(plainTextArray[x]));
                    query.Append(" AND ");
                    query.Append("cipher = ");
                    query.Append(Tools.BitArrayToString(cipherTextArray[x]));
                    query.Append(" OR ");
                }
                query.Remove(query.Length - 4, 4);
                SqlCommand command = new SqlCommand(query.ToString(), conn);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (!KeyList.Contains(reader[0]))
                            KeyList.Add(String.Format("{0}", reader[0]));
                    }
                }
            }
            return KeyList;
        }
    }
}
