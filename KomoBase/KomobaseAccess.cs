using KomoBase.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KomoBase
{
    public class KomoBaseAccess : IDisposable
    {
        private SQLiteConnection connection;

        public KomoBaseAccess()
        {
            connection = new SQLiteConnection(Path.Combine(Environment.CurrentDirectory, @"Komobase", @"Komobase.sqlite"));
        }

        public void Initialize()
        {
            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, @"Komobase"));
            connection.CreateTable(typeof(AuthToken));
            connection.CreateTable(typeof(User));
            connection.CreateTable(typeof(Subscription));
            connection.CreateTable(typeof(Wow));
            connection.CreateTable(typeof(ClashRoyale));
        }

        public void SyncUsers(List<string> userList)
        {
            List<User> allUsers = GetAllUsers();

            foreach(string user in userList)
            {
                if(!allUsers.Exists(x => x.UserName == user))
                {
                    connection.Insert(new User() { UserName = user });
                }
            }
        }

        public string GetAuthToken()
        {
            return connection.ExecuteScalar<string>("SELECT BlizzardToken FROM AuthToken");
        }

        public void SetAuthToken(string token)
        {
            connection.Execute(@"UPDATE AuthToken SET BlizzardToken = '" + token + @"'");
        }

        public bool SubStatus(string username)
        {
            if(UserExistsInSubTable(username))
            {
                return GetUserSubStatus(username);
            }
            else
            {
                return false;
            }
        }

        public void UpdateSubscription(string username, bool update)
        {
            int id = GetUserID(username);

            Subscription sub = new Subscription()
            {
                IsSubscribed = update,
                UserID = id
            };

            if (UserExistsInSubTable(username))
            {
                connection.Update(sub, typeof(Subscription));
            }
            else
            {
                connection.Insert(sub, typeof(Subscription));
            }
        }

        public void UpdateCRTag(string username, string tag)
        {
            int id = GetUserID(username);

            ClashRoyale cr = new ClashRoyale()
            {
                CRTag = tag,
                UserID = id
            };

            if(UserExistsInCRTable(id))
            {
                connection.Update(cr, typeof(ClashRoyale));
            }
            else
            {
                connection.Insert(cr, typeof(ClashRoyale));
            }
        }

        public string GetCRTag(string username)
        {
            int id = GetUserID(username);

            if (UserExistsInCRTable(id))
            {
                return GetCRTag(id);
            }
            else return string.Empty;
        }

        public string GetCRTag(int id)
        {
            return GetUserCRTag(id);
        }

        public void Dispose()
        {
            connection.Close();
        }

        private List<User> GetAllUsers()
        {
            return connection.Query<User>("SELECT * FROM USER");
        }

        private User GetUserByID(int userID)
        {
            var result = connection.Get<User>(userID);

            if(result != null)
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        private int GetUserID(string username)
        {
            var result = connection.ExecuteScalar<int>(@"SELECT ID FROM User WHERE UserName = '" + username+@"'");

            if(result == 0)
            {
                throw new ArgumentException("Username is not present in the DB.");
            }
            else
            {
                return result;
            }
        }

        private bool UserExistsInSubTable(int userID)
        {
            var result = connection.ExecuteScalar<int>(@"SELECT UserID FROM Subscription WHERE UserID = " + userID);

            if(result == 0)
            {
                return false;
            }
            else 
            return true;
        }

        private bool UserExistsInSubTable(string userName)
        {
            var result = connection.ExecuteScalar<int>(@"SELECT ID FROM User WHERE UserName = '" + userName+@"'");

            return UserExistsInSubTable(result);
        }

        private bool GetUserSubStatus(string username)
        {
            int userid = GetUserID(username);

            return connection.ExecuteScalar<bool>(@"SELECT IsSubscribed FROM Subscription WHERE UserID = " + userid);


        }

        private bool UserExistsInCRTable(int id)
        {
            var result = connection.ExecuteScalar<int>(@"SELECT UserID FROM ClashRoyale WHERE UserID = " + id);

            if (result == 0)
            {
                return false;
            }
            else
                return true;
        }

        private string GetUserCRTag(int id)
        {
            var result = connection.ExecuteScalar<string>(@"SELECT CRTag FROM ClashRoyale WHERE UserID = "+id);

            return result;
        }
    }
}
