using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace KomoBase.Models
{
    [Table("Wow")]
    public class Wow
    {
        [PrimaryKey]
        [Column("UserID")]
        public int UserID { get; set; }
        [Column("Realm")]
        public string Realm { get; set; }
        [Column("Wowcharname")]
        public string WowCharName { get; set; }
    }
}
