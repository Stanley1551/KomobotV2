using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace KomoBase.Models
{
    [Table("User")]
    public class User
    {
        [PrimaryKey]
        [AutoIncrement]
        [Column("ID")]
        public int ID { get; set; }
        [Column("UserName")]
        public string UserName { get; set; }
    }
}
