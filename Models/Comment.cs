using AmadeusBlog.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AmadeusBlog.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string AuthorId { get; set; }
        public string ModeratorId { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at most {1} characters.", MinimumLength = 2)]

        public string Body { get; set; }
        public DateTime Created { get; set; }

        public DateTime? Moderated { get; set; }
        public string ModeratedBody { get; set; }
        public ModerationType? ModerationType { get; set; }

        public virtual Post Post { get; set; }
        public virtual BlogUser Author { get; set; }
        public virtual BlogUser Moderator { get; set; }
    }
}
