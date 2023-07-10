using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace NotesApi.Shared.Models;

[Table("Roles")]
public class Role
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }

    [JsonIgnore]
    public ICollection<User> Users { get; set; }
}