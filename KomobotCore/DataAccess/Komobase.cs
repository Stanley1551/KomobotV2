//using DSharpPlus.Entities;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;

//namespace KomobotV2.DataAccess
//{
//    public class Komobase
//    {
//        #region constructors
//        public Komobase()
//        {
//            MissingFile = false;

//            if(!File.Exists("Komobase\\Komobase.sqlite"))
//            {
//                MissingFile = true;

//                SQLiteConnection.CreateFile("Komobase\\Komobase.sqlite");
//            }

//            DbConnection = new SQLiteConnection("Data Source=Komobase\\Komobase.sqlite;Version=3;");
//            DbConnection.Open();

//            if(MissingFile)
//            {
//                string sql = "create table Users (name nvarchar(25) NULL, subscribed bit NULL, CRTag nvarchar(25) NULL, PUBGID nvarchar(50) NULL, isAlive bit NULL, Realm nvarchar(25) NULL, WoWCharName nvarchar(25) NULL)";
//                string sql2 = "create table AuthToken (token nvarchar(50) NULL)";
//                string sql3 = @"insert into AuthToken values(NULL)";

//                List<string> sqlCommandList = new List<string>() { sql, sql2, sql3 };
//                SQLiteCommand cmd = new SQLiteCommand(DbConnection);
//                foreach (string sqlCommand in sqlCommandList)
//                {
//                    cmd.CommandText = sqlCommand;
//                    cmd.ExecuteNonQuery();
//                }

//                //SQLiteCommand cmd = new SQLiteCommand(sql, DbConnection);
//                //cmd.ExecuteNonQuery();
//                //cmd.CommandText = sql2;
//                //cmd.ExecuteNonQuery();
//                //cmd.CommandText = sql3;
//                //cmd.ExecuteNonQuery();
//            }

//        }
//        #endregion

//        #region fields
//        private static SQLiteConnection m_dbConnection;
//        private bool m_missingFile;
//        #endregion

//        #region properties
//        public static SQLiteConnection DbConnection {
//            get { return m_dbConnection; }
//            set { m_dbConnection = value; }
//        }

//        public bool MissingFile {
//            get { return m_missingFile; }
//            set { m_missingFile = value; }
//        }
//        #endregion

//        #region public methods
//        public void SyncUsers(List<DiscordMember> members)
//        {
//            foreach(var member in members)
//            {
//                string sql = "insert into Users(name) values('" + member.Username + "') except select name from users";
//                Execute(sql);

//                sql = @"update Users set isAlive = 1 where name = '" + member.Username + "'";
//                Execute(sql);
//            }
//            //string membersInDb =ReadDatas("select name from users");
//        }

//        public static void SubscribeUser(string userName)
//        {
//            string sql = "update users set subscribed = 1 where name = '" + userName + "'";

//            Execute(sql);
//        }

//        public static void UnsubscribeUser(string userName)
//        {
//            string sql = "update users set subscribed = 0 where name = '" + userName + "'";

//            Execute(sql);
//        }

//        public static bool IsSubscribed(string userName)
//        {
//            string sql = "select subscribed from users where name = '" + userName + "'";

//            SQLiteCommand cmd = new SQLiteCommand(sql, DbConnection);

//            var retVal = cmd.ExecuteScalar();
//            bool result = false;
//            if(bool.TryParse(retVal.ToString(),out result))
//            {
//                return result;
//            }
//            return false;
//        }

//        public static void SetPUBGID(string userName, string id)
//        {
//            string sql = "update users set PUBGID = '" + id + "' where name = '" + userName+"'";

//            Execute(sql);
//        }

//        public static void SetCRTag(string userName, string tag)
//        {
//            string sql = "update users set CRTag = '" + tag + "' where name = '" + userName+"'";

//            Execute(sql);
//        }

//        public static string GetCRTag(string username)
//        {
//            string sql = "select CRTag from users where name = '" + username + "'";

//            return ReadData(sql).ToString();
//        }

//        public static string GetPUBGID(string username)
//        {
//            string sql = "select PUBGID from users where name = '" + username + "'";

//            return ReadData(sql).ToString();
//        }

//        public static string GetAuthToken()
//        {
//            string sql = "select token from AuthToken";

//            var result = ReadData(sql);

//            if (result == null)
//            {
//                return "";
//            }
//            else return result.ToString();
//        }

//        public static void SetAuthToken(string token)
//        {
//            string sql = @"update AuthToken set token = '" + token+@"'";

//            Execute(sql);
//        }

//        public static void SetWowRealmAndName(string username, string realm, string name)
//        {
//            string sql = @"update users set Realm = '" + realm + @"', WoWCharName = '" + name + @"' where name = '" + username + @"'";

//            Execute(sql);
//        }

//        public static string GetWowRealmAndName(string username)
//        {
//            string sql = @"select Realm, WoWCharName from Users where name = '" + username + @"'";

//            var result = ReadData(sql);
//            return result.ToString();
//        }
//        #endregion

//        #region private methods
//        private static void Execute(string query)
//        {
//            SQLiteCommand cmd = new SQLiteCommand(query, DbConnection);

//            cmd.ExecuteNonQuery();
//        }

//        private static object ReadData(string query)
//        {
//            SQLiteCommand cmd = new SQLiteCommand(query, DbConnection);

//            var retVal = cmd.ExecuteScalar();

//            return retVal;
//        }

//        private static string ReadDatas(string query)
//        {
//            SQLiteCommand cmd = new SQLiteCommand(query, DbConnection);

//            var retVal = cmd.ExecuteReader(System.Data.CommandBehavior.Default);

//            string result= string.Empty;
//            if(retVal.HasRows)
//            {
//                for(int i=0; ;)
//                {
//                    result += retVal.GetString(i);
//                    if(retVal.NextResult())
//                    {
//                        continue;
//                    }
//                    break;
//                }
//            }

//            return result;
//        }
//        #endregion
//    }
//}
