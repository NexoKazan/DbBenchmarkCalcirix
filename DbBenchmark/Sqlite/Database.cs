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
using System.Data;
using System.Data.SQLite;
using System.Text;

namespace DbBenchmark.Sqlite
{
    public class Database
    {
        private SQLiteConnection _connection;

        public Database(string connectionString)
        {
            ConnectionString = connectionString;
        }


        private SQLiteConnection Connection
        {
            get
            {
                //if (_connection != null) return _connection;

                //всегда новое подключение
                _connection = new SQLiteConnection(ConnectionString);
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
            SelectBlocksManaged(query, blockSize);
        }

        private void SelectBlocksManaged(string query, int blockSize)
        {
            SQLiteDataReader rdr = null;

            try
            {
                var cmd = new SQLiteCommand(query, Connection);
                rdr = cmd.ExecuteReader(CommandBehavior.SingleResult);
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
                            sb.Append(values[i]);
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

            catch (Exception ex)
            {
            }
            finally
            {
                rdr?.Close();
            }
        }


        #region Events


        protected void OnBlockReaded(byte[] rows, bool isLast = false, int orderNumber = 0)
        {
        }

        #endregion

    }
}