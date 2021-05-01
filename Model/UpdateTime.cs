using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealtorObjects.Model
{
    [Table("UpdateTime")]
    public class UpdateTime
    {
        private String name = "LastUpdateTime";

        [Key]
        public String Name
        {
            get => name;
            private set => name = value;
        }
        public DateTime DateTime { get; set; }
    }
}
