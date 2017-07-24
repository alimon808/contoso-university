using System;
using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity.Data.Entities
{
    public abstract class BaseEntity
    {
        public int ID { get; set; }
        public DateTime AddedDate { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}