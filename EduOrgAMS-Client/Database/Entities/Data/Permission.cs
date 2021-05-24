using System;

namespace EduOrgAMS.Client.Database.Entities.Data
{
    public class Permission
    {
        public string TableName { get; set; }
        public bool AddAccess { get; set; }
        public bool EditAccess { get; set; }
        public bool RemoveAccess { get; set; }
    }
}
