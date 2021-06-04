using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stargate.WebApiServ.Data.Models
{
    public class Blog
    {
        [Column("blog_id")]
        public int BlogId { get; set; }

        [Column(TypeName = "varchar(200)")]
        [MaxLength(500)]
        public string Url { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public int Rating { get; set; }
        public List<Post> Posts { get; set; }

        [NotMapped]
        public DateTime LoadedFromDatabase { get; set; }
   }
}
