using System;
using EduOrgAMS.Client.Database;
using EduOrgAMS.Client.Database.Entities;

namespace EduOrgAMS.Client.Settings.Entities
{
    public class User
    {
        public string Login { get; }
        public string Token { get; }
        public int Id { get; }
        public UserStoreType StoreType { get; }

        private readonly object _roleSyncRoot = new object();
        private volatile Role _role;
        public Role Role
        {
            get
            {
                if (_role == null)
                {
                    lock (_roleSyncRoot)
                    {
                        if (_role == null)
                        {
                            var user = DatabaseManager
                                .Find<Database.Entities.User>(Id);

                            _role = user.Role;
                        }
                    }
                }

                return _role;
            }
        }


        public User(string login, string token, int id,
            UserStoreType storeType)
        {
            Login = login;
            Token = token;
            Id = id;
            StoreType = storeType;
        }

        public bool IsTemporary()
        {
            return StoreType == UserStoreType.Temporary;
        }
    }
}
