using System;
using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity.Data.Entities
{
    public abstract class BaseEntity
    {
        public int ID { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}