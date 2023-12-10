using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace shr.API.Models.Share
{
    public class SharePatch
    {
        [Required(AllowEmptyStrings = false)]
        [MaxLength(64)]
        public string Name { get; set; } = null!;

        [Required]
        [DefaultValue(false)]
        public bool Secure { get; set; }
    }
}
