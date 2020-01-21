using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALo.Addresses.Data.Models
{
    [Table("Addresses", Schema = "dbo")]
    public class Address
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        [Required]
        public Guid AddressId { get; set; }
        public Guid? ParentAddressId { get; set; }

        [StringLength(120), Required]
        public string Name { get; set; }
        [StringLength(10), Required]
        public string TypeShortName { get; set; }

        [Required]
        public byte ActualityStatus { get; set; }
        [Required]
        public byte Level { get; set; }
        [Required]
        public byte DivisionType { get; set; }
    }
}
