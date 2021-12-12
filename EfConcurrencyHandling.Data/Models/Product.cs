using System.ComponentModel.DataAnnotations;

namespace EfConcurrencyHandling.Data.Models
{
    public class Product
    {
        [Key] 
        public int Id { get; set; }

        // Check for the value in the DB, is it changed or not etc.
        // [ConcurrencyCheck] 
        public string Name { get; set; }

        // Check for the row version in the DB  - so applied for all entity properties
        [Timestamp]
        public byte[] RowVersion { get; set; }
        
        // Check for the row version in the DB - so applied for all entity properties
        // In this way, the developer needs to ensure that it increments with any row change
        // [ConcurrencyCheck] 
        // public int RowVersion { get; set; }
        
    }
}