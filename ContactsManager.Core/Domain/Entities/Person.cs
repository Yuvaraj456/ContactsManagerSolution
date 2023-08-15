using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactsManager.Core.Domain.Entities
{
    /// <summary>
    /// Person Domain model Class
    /// </summary>
    public class Person
    {
        [Key]
        public Guid PersonId { get; set; }

        [StringLength(40)] //nvarchar(40) default its create nvarchar(max).
        public string? PersonName { get; set; }

        [StringLength(40)]
        public string? Email { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }


        //uniqueidentifier
        public Guid? CountryId { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        //bit
        public bool ReceiveNewsLetters { get; set; }

        [ForeignKey("CountryId")]
        public virtual Country? Country { get; set; }

        public override string ToString()
        {
            return $"PersonId : {PersonId}, PersonName : {PersonName}, Email : {Email}, DateOfBirth : {DateOfBirth?.ToString("dd/MM/yyyy")}, Gender : {Gender},CountryId :{CountryId} ,Address : {Address}, ReceiveNewsLetters : {ReceiveNewsLetters} , Country : {Country?.CountryName} ";
        }

    }
}
