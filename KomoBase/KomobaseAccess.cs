using KomoBase.Models;
using SQLite;
using System;
using System.Collections;
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
            //connection.CreateTable(typeof(AuthToken));
            connection.CreateTable(typeof(User));
            connection.CreateTable(typeof(Subscription));
            connection.CreateTable(typeof(Wow));
            connection.CreateTable(typeof(ClashRoyale));
            connection.CreateTable(typeof(Points));

            //connection.Insert(new AuthToken());
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

        public void AddPoints(string username, int points)
        {
            long currentPoints = 0;
            int id = GetUserID(username);

            if(UserExistsInPointsTable(id,out currentPoints))
            {
                connection.Update(new Points()
                {
                    Balance = currentPoints + points,
                    UserID = id
                });
            }
            else
            {
                connection.Insert(new Points()
                {
                    Balance = points,
                    UserID = id
                });
            }
        }

        public long GetPoints(string username)
        {
            int id = GetUserID(username);

            if(UserExistsInPointsTable(id))
            {
                return connection.Get<Points>(id).Balance;
            }
            else
            {
                return 0;
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

        public string GetWoWRealmName(string username)
        {
            int id = GetUserID(username);

            if (UserExistsInWoWTable(id))
            {
                return GetWoWRealmName(id);
            }
            else return string.Empty;
        }

        public string GetWoWRealmName(int id)
        {
            return GetUserWoWRealm(id);
        }

        public string GetWoWCharName(string username)
        {
            int id = GetUserID(username);

            if (UserExistsInWoWTable(id))
            {
                return GetWoWCharName(id);
            }
            else return string.Empty;
        }

        public string GetWoWCharName(int id)
        {
            return GetUserWoWCharName(id);
        }

        public void UpdateWowRealm(string username, string realm)
        {
            int id = GetUserID(username);

            Wow wow = new Wow()
            {
                Realm = realm,
                UserID = id,
                WowCharName = GetWoWCharName(id)
            };

            if (UserExistsInWoWTable(id))
            {
                connection.Update(wow, typeof(Wow));
            }
            else
            {
                connection.Insert(wow, typeof(Wow));
            }
        }

        public void UpdateWowCharName(string username, string charname)
        {
            int id = GetUserID(username);

            Wow wow = new Wow()
            {
                UserID = id,
                Realm = GetUserWoWRealm(id),
                WowCharName = charname
            };

            if (UserExistsInWoWTable(id))
            {
                connection.Update(wow, typeof(Wow));
            }
            else
            {
                connection.Insert(wow, typeof(Wow));
            }
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

        private bool UserExistsInPointsTable(int id)
        {
            var row = connection.Get<Points>(id);

            if (row == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Dictionary<string, long> GetLeaderboard()
        {
            Dictionary<string, long> dict = new Dictionary<string, long>(3);
            var result = connection.Query<Points>(@"SELECT UserID, Balance FROM Points ORDER BY Balance desc");
            
            if (result != null)
            {
                result.ForEach(x => dict.Add(GetUserByID(x.UserID).UserName, x.Balance));
                return dict;
            }
            throw new Exception("Nincs egyelőre pont bejegyzés!");
        }

        private bool UserExistsInPointsTable(int id, out long currentPoints)
        {
            currentPoints = 0;

            var result = connection.ExecuteScalar<int>(@"SELECT UserID FROM Points WHERE UserID = " + id);

            if (result == 0)
            {
                return false;
            }
            else
            {
                currentPoints = connection.Get<Points>(id).Balance;
                return true;
            }
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

        private bool UserExistsInWoWTable(int id)
        {
            var result = connection.ExecuteScalar<int>(@"SELECT UserID FROM Wow WHERE UserID = " + id);

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

        private string GetUserWoWRealm(int id)
        {
            var result = connection.ExecuteScalar<string>(@"SELECT Realm FROM Wow WHERE UserID = " + id);

            return result;
        }

        private string GetUserWoWCharName(int id)
        {
            var result = connection.ExecuteScalar<string>(@"SELECT Wowcharname FROM Wow WHERE UserID = " + id);

            return result;
        }
    }
}
