using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace RUCClassCatcher
{
    class DatabaseManager
    {
        private static readonly DatabaseManager single = new DatabaseManager();

        public static string connStr = "server=" + "ruc0data.mysql.rds.aliyuncs.com"
                 + ";user=" + "testor"
                 + ";database=" + "rucclasses"
                 + ";port=" + "3306"
                 + ";password=" + "19941201"
                 + ";";


        private DatabaseManager()
        {

        }

        public void excuteNonQuery(string sql)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                lock (conn)
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public int excuteNumQuery(string sql)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                lock (conn)
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    string result = reader[0].ToString();
                    reader.Close();
                    return int.Parse(result);
                }
            }
        }

        /// <summary>
        /// 欲执行查询类SQL，应当在BLL层自己写一层
        /// using(MySqlConnection conn = new MySqlConnection(MySqlManager.connStr))
        /// </summary>
        public void excuteQuery()
        {
            throw new NotImplementedException();
        }


        public static DatabaseManager getSinglton()
        {
            return single;
        }

    }
}
