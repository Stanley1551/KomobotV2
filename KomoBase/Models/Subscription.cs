using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace KomoBase.Models
{
    [Table("Subscription")]
    public class Subscription
    {
        //próba hogy jó-e aztán rárakni a többire is
        [PrimaryKey]
        [Column("UserID")]
        public int UserID { get; set; }
        [Column("IsSubscribed")]
        public bool IsSubscribed { get; set; }
    }
}
