using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALo.Addresses.Data.Models
{
    [Table("Houses", Schema = "dbo")]
    public class House : IHasId<Guid>
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        [Required]
        public Guid HouseId { get; set; }
        [Required]
        public Guid AddressId { get; set; }

        [StringLength(20)]
        public string HouseNumber { get; set; }
        [StringLength(10)]
        public string BuildNumber { get; set; }
        [StringLength(10)]
        public string StructureNumber { get; set; }

        [Required]
        public byte HouseType { get; set; }
        [Required]
        public byte HouseState { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
    }
}
