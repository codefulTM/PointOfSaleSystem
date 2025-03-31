using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointOfSaleSystem
{
    public static class Configuration
    {
        //public const string CONNECTION_STRING = "Host=localhost;Username=postgres;Password=123;Database=pos_management";
        public static readonly string CONNECTION_STRING = Environment.GetEnvironmentVariable("POS_DB_CONNECTION", EnvironmentVariableTarget.User);
    }
}
