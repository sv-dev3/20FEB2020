﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;

namespace BR904WIP.Helpers
{
    public class DatabaseHelper
    {

        private static readonly string br_db_host_name;
        private static readonly string br_db_port;
        private static readonly string br_db_user_name;
        private static readonly string br_db_password;
        private static readonly string br_db_name;

        static DatabaseHelper()
        {
            br_db_host_name = System.Environment.GetEnvironmentVariable("BR_DB_HOST_NAME");
            if (string.IsNullOrWhiteSpace(br_db_host_name))
            {
                throw new Exception("Missing environment variable: 'BR_DB_HOST_NAME'");
            }

            br_db_port = System.Environment.GetEnvironmentVariable("BR_DB_PORT");
            if (string.IsNullOrWhiteSpace(br_db_port))
            {
                throw new Exception("Missing environment variable: 'BR_DB_PORT'");
            }

            br_db_user_name = System.Environment.GetEnvironmentVariable("BR_DB_USER_NAME");
            if (string.IsNullOrWhiteSpace(br_db_user_name))
            {
                throw new Exception("Missing environment variable: 'BR_DB_USER_NAME'");
            }

            br_db_password = System.Environment.GetEnvironmentVariable("BR_DB_PASSWORD");
            if (string.IsNullOrWhiteSpace(br_db_password))
            {
                throw new Exception("Missing environment variable: 'BR_DB_PASSWORD'");
            }

            br_db_name = System.Environment.GetEnvironmentVariable("BR_DB_NAME");
            if (string.IsNullOrWhiteSpace(br_db_name))
            {
                throw new Exception("Missing environment variable: 'BR_DB_NAME'");
            }
        }

        private readonly string connString = String.Format("Server={0};Port={1};Username={2};Password={3};Database={4};" +
                "Pooling=true;MinPoolSize=0;MaxPoolSize=50;Timeout=60;ConnectionIdleLifetime=5;ConnectionPruningInterval=1;",
                br_db_host_name,
                br_db_port,
                br_db_user_name,
                br_db_password,
                br_db_name);

        private NpgsqlConnection connection;

        public DatabaseHelper()
        {
            connection = new NpgsqlConnection(connString);
            connection.Open();
        }

        public void CloseConnection()
        {
            connection.Close();
            connection.Dispose();
        }

        public NpgsqlCommand SpawnCommand()
        {
            var command = new NpgsqlCommand();
            command.Connection = connection;
            return command;
        }
    }
}
