#region Copyright
/*
 * Copyright 2017 Roman Klassen
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy
 * of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 *
 */
#endregion

namespace DbBenchmark.Mysql
{
    public class ConnectionStringParser : ConnectionStringParserBase
    {
        static readonly string[] Address = { "address", "addr", "Server" };
        static readonly string[] Port = { "port" };
        static readonly string[] User = { "Uid" };
        static readonly string[] Password = { "Pwd", "pass" };
        static readonly string[] Database = { "Database" };

        public static string GetAddress(string connectionString)
        {
            return GetValue(connectionString, Address);
        }

        public static string GetUser(string connectionString)
        {
            return GetValue(connectionString, User);
        }

        public static string GetPassword(string connectionString)
        {
            return GetValue(connectionString, Password);
        }

        public static string GetDatabase(string connectionString)
        {
            return GetValue(connectionString, Database);
        }

        public static int GetPort(string connectionString)
        {
            int port;
            return int.TryParse(GetValue(connectionString, Port), out port) ? port : 3306;
        }
    }
}
