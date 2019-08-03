using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using SQLitePCL;

namespace KomoBase.Models
{
    [Table("AuthToken")]
    public class AuthToken
    {
        [Column("BlizzardToken")]
        public string BlizzardToken { get; set; }
    }
}
