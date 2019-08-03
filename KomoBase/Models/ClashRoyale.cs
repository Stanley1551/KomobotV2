using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace KomoBase.Models
{
    [Table("ClashRoyale")]
    public class ClashRoyale
    {
        [PrimaryKey]
        [Column("UserID")]
        public int UserID { get; set; }
        [Column("CRTag")]
        public string CRTag { get; set; }
    }
}
