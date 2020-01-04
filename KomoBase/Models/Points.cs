using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace KomoBase.Models
{
    [Table("Points")]
    internal class Points
    {
        [PrimaryKey]
        [Column("UserID")]
        public int UserID { get; set; }

        [Column("Balance")]
        public long Balance { get; set; }
    }
}
