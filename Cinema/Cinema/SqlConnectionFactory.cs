using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema
{
    public class SqlConnectionFactory
    {
        private string ConnectionString;

        public SqlConnectionFactory(string ConnectionString)
        {
            this.ConnectionString = ConnectionString;
        }

        public SqlConnection Create()
        {
            return new SqlConnection
            {
                ConnectionString = ConnectionString
            };
        }
    }
}
