#region Copyright
/*
 * Copyright 2019 Roman Klassen
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

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using MySql.Data.MySqlClient;

namespace DbBenchmark.Mysql
{
    public class Database
    {
        private MySqlConnection _connection;
        public bool IsRunningOnMono { get; set; } = true;

        public Database(string connectionString)
        {
            ConnectionString = connectionString;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void BlockReadCallback(IntPtr block);

        [DllImport("FastMysqlSelect.dll", EntryPoint = "FastSelectString")]
        protected static extern IntPtr FastSelect(IntPtr handle, string host, string user, string passwd, string db,
            string query, ref int lenght, ref int rowCount);

        [DllImport("FastMysqlSelect.dll", EntryPoint = "FastSelectBloksString")]
        protected static extern IntPtr FastBlocksMysqlSelect(IntPtr handle, string host, string user, string passwd, string db,
            string query, ref int lenght, int rowCount);

        [DllImport("FastMysqlSelect.dll", EntryPoint = "INIT")]
        protected static extern IntPtr Init();

        [DllImport("FastMysqlSelect.dll", EntryPoint = "SetBlockReadCallback")]
        protected static extern void SetBlockReadCallback(IntPtr ptr, BlockReadCallback callback);

        [DllImport("FastMysqlSelect.dll", EntryPoint = "DESTROY")]
        protected static extern void DESTROY(IntPtr ptr);

        [DllImport("FastMysqlSelect.dll", EntryPoint = "GetErrorMessage")]
        protected static extern IntPtr GetErrorMessage(IntPtr ptr, ref int lenght);

        [StructLayout(LayoutKind.Sequential)]
        struct DataBlock
        {
            public IntPtr Data;
            public int Length;
        }

        private MySqlConnection Connection
        {
            get
            {
                //if (_connection != null) return _connection;
                
                //всегда новое подключение
                _connection = new MySqlConnection(ConnectionString);
                try
                {
                    _connection.Open();
                }
                catch (Exception e)
                {
                    _connection = null;
                }
                return _connection;
            }
        }

        public string ConnectionString { get; set; }


        /// <summary>
        ///     Выборка по блокам из БД
        /// </summary>
        /// <param name="query">Запрос к БД</param>
        /// <param name="blockSize">Размер блока в строках</param>
        public void SelectBlocks(string query, int blockSize)
        {
            if (IsRunningOnMono)
            {
                SelectBlocksManaged(query, blockSize);
            }
            else
            {
                SelectBlocksNative(query, blockSize);
            }
        }

        private void SelectBlocksManaged(string query, int blockSize)
        {
            MySqlDataReader rdr = null;

            try
            {
                var types = GetColumnTypes(Connection, query);

                var cmd = new MySqlCommand(query, Connection);
                rdr = cmd.ExecuteReader(CommandBehavior.SingleResult);
                var colNames = GetColumnNmaes(rdr);
                uint count = 0;
                var sendcount = 0;
                var sb = new StringBuilder();
                byte[] sendBuffer = null;

                while (rdr.Read())
                {

                    var values = new object[rdr.FieldCount];
                    var fieldCount = rdr.GetValues(values);
                    var maxFieldCount = fieldCount - 1;
                    for (var i = 0; i < fieldCount; i++)
                    {
                        if (values[i] is DBNull)
                        {
                            sb.Append("NULL");
                        }
                        else
                        {
                            sb.Append("\"");
                            sb.Append(ValueToStr(types, colNames, i, values));
                            sb.Append("\"");
                        }

                        sb.Append(i != maxFieldCount ? "|" : "\n");
                    }

                    count++;

                    if (count % blockSize == 0) //отправка блока
                    {
                        if (sendBuffer != null)
                        {
                            OnBlockReaded(sendBuffer, orderNumber: sendcount++);
                        }

                        var dest = new char[sb.Length];
                        sb.CopyTo(0, dest, 0, sb.Length);
                        sendBuffer = Encoding.UTF8.GetBytes(dest);
                        sb = new StringBuilder();
                    }
                }

                //отправка последнего блока
                if (sb.Length > 0)
                {
                    if (sendBuffer != null)
                    {
                        OnBlockReaded(sendBuffer, orderNumber: sendcount++);
                    }

                    OnBlockReaded(Encoding.UTF8.GetBytes(sb.Remove(sb.Length - 1, 1).ToString()), true, sendcount);
                }
                else
                {
                    OnBlockReaded(sendBuffer, true, sendcount);
                }
            }

            catch (MySqlException ex)
            {
                Console.WriteLine("Error:" + ex.ToString());
            }
            finally
            {
                rdr?.Close();
            }
        }

        private void SelectBlocksNative(string query, int blockSize)
        {
            try
            {
                var rowCount = 0;
                var page = 0;
                query = query.Replace(";", "");
                do
                {

                    var selQuery = query + $" LIMIT {blockSize * page}, {blockSize};";
                    var lenght = 0;
                    var handle = Init();
                    var buf = FastSelect(handle,
                        ConnectionStringParser.GetAddress(ConnectionString) + ":" +
                        ConnectionStringParser.GetPort(ConnectionString),
                        ConnectionStringParser.GetUser(ConnectionString),
                        ConnectionStringParser.GetPassword(ConnectionString),
                        ConnectionStringParser.GetDatabase(ConnectionString),
                        selQuery, ref lenght, ref rowCount);
                    var buffer = new byte[lenght];

                    if (buf == IntPtr.Zero)
                    {
                        var messageLenght = 0;
                        var msgBuf = GetErrorMessage(handle, ref messageLenght);
                        var msgBuffer = new byte[messageLenght];
                        Marshal.Copy(msgBuf, msgBuffer, 0, msgBuffer.Length);
                        var error = Encoding.ASCII.GetString(msgBuffer);
                    }
                    else
                    {
                        Marshal.Copy(buf, buffer, 0, buffer.Length);
                        Marshal.FreeHGlobal(buf);
                    }

                    DESTROY(handle);

                    OnBlockReaded(buffer, rowCount != blockSize, orderNumber: page);

                    page++;
                } while (rowCount == blockSize);
            }
            catch (Exception e)
            {
            }
        }
         /// <summary>
        /// Получение данных из БД по блокам
        /// </summary>
        /// <param name="query">Запрос</param>
        /// <param name="blockSize">Размер блока в строках</param>
        public List<byte[]> Select(string query, int blockSize)
        {
            return IsRunningOnMono ? SelectManaged(query) : SelectNative(query, blockSize);
        }

        private List<byte[]> SelectManaged(string query)
        {
            MySqlDataReader rdr = null;

            try
            {
                var types = GetColumnTypes(Connection, query);

                var cmd = new MySqlCommand(query, Connection);
                rdr = cmd.ExecuteReader(CommandBehavior.SingleResult);
                var colNames = GetColumnNmaes(rdr);
                var sb = new StringBuilder();

                while (rdr.Read())
                {
                    var values = new object[rdr.FieldCount];
                    var fieldCount = rdr.GetValues(values);
                    var maxFieldCount = fieldCount - 1;
                    for (var i = 0; i < fieldCount; i++)
                    {
                        sb.Append(values[i] is DBNull ? "NULL" : ValueToStr(types, colNames, i, values));
                        sb.Append(i != maxFieldCount ? "|" : "\n");
                    }
                }

                return new List<byte[]>() {Encoding.UTF8.GetBytes(sb.ToString())};
            }

            catch (MySqlException ex)
            {
            }
            finally
            {
                rdr?.Close();
            }

            return new List<byte[]>();
        }

        private List<byte[]> SelectNative(string query, int blockSize)
        {
            var useCallback = true;
            var resultData = new List<byte[]>();
            try
            {

                var selQuery = query;
                var lenght = 0;
                var handle = Init();
                

                var datablocks = FastBlocksMysqlSelect(handle,
                    ConnectionStringParser.GetAddress(ConnectionString) + ":" +
                    ConnectionStringParser.GetPort(ConnectionString),
                    ConnectionStringParser.GetUser(ConnectionString),
                    ConnectionStringParser.GetPassword(ConnectionString),
                    ConnectionStringParser.GetDatabase(ConnectionString),
                    selQuery, ref lenght, blockSize);

                if (datablocks == IntPtr.Zero)
                {
                    var messageLenght = 0;
                    var buf = GetErrorMessage(handle, ref messageLenght);
                    var buffer = new byte[messageLenght];
                    Marshal.Copy(buf, buffer, 0, buffer.Length);
                    var error = Encoding.ASCII.GetString(buffer);
                    DESTROY(handle);
                    return new List<byte[]>();
                }


                DESTROY(handle);
            }
            catch (Exception e)
            {
            }
            return resultData;
        }


        private string ValueToStr(Dictionary<string, int> types, string[] colNames, int i, object[] values)
        {
            if (!types.ContainsKey(colNames[i])) return values[i].ToString();

            var type = types[colNames[i]];
            var str = string.Empty;
            var val = values[i];

            switch (type)
            {
                case 0:
                    str = val.ToString();
                    break;
                case 1:
                    str = ((DateTime)val).ToString("yyyy-MM-dd H:mm:ss");
                    break;
                case 2:
                    str = ((decimal)val).ToString(CultureInfo.InvariantCulture);
                    break;
            }
            return str;
        }
        
        private string[] GetColumnNmaes(MySqlDataReader rdr)
        {
            var names = new string[rdr.FieldCount];
            for (var i = 0; i < rdr.FieldCount; i++)
            {
                names[i] = rdr.GetName(i);
            }

            return names;
        }

        private Dictionary<string,int> GetColumnTypes(MySqlConnection conn, string query)
        {
            var types = new Dictionary<string, int>();
            var tableName = FindTableName(query);
            MySqlDataReader rdr = null;
            try
            {
                var cmd = new MySqlCommand("show columns from " + tableName, conn);
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    var values = new object[rdr.FieldCount];
                    rdr.GetValues(values);
                    var typeStr = values[1].ToString().ToLowerInvariant();
                    int type = 0;
                    if (typeStr.Contains("date")) type = 1;
                    else if (typeStr.Contains("decimal")) type = 2;

                    types.Add(values[0].ToString(), type);
                }
            }
            catch (Exception e)
            {
            }
            finally
            {
                rdr?.Close();
            }
            return types;
        }

        private string FindTableName(string query)
        {
            int start = query.IndexOf("from", StringComparison.InvariantCultureIgnoreCase) + 4;
            int end = query.IndexOf("where", StringComparison.InvariantCultureIgnoreCase);
            int lenght = end < start ? query.Length - start : end - start;
            return query.Substring(start, lenght).Trim();
        }
        

        #region Events

        
        protected void OnBlockReaded(byte[] rows, bool isLast = false, int orderNumber = 0)
        {
        }

        #endregion

    }
}