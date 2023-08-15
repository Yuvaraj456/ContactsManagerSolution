using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Core.Domain.Entities
{
    //Documentation Commands or Xml commands
    /// <summary>
    /// Domain model for storing a country details
    /// </summary>
    public class Country
    {
        [Key]
        public Guid CountryId { get; set; }

        public string? CountryName { get; set; }

        public virtual ICollection<Person>? Persons { get; set; }

    }
}