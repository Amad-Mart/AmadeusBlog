using Microsoft.AspNetCore.Http;
using AmadeusBlog.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using AmadeusBlog.Models;

namespace AmadeusBlog.Models
{
    public class Post
    {
        /// <summary>
        /// The Primary Key of the Post
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The Foreign key to the blog that is the parent of this Post
        /// </summary>
        public int BlogId { get; set; }

        /// <summary>
        /// The Title of the Post
        /// </summary>

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at most {1} characters.", MinimumLength = 2)]
        public string Title { get; set; }

        /// <summary>
        /// Brief intro for the post
        /// </summary>
        [Required]
        [StringLength(150, ErrorMessage = "The {0} must be at least {2} and at most {1} characters.", MinimumLength = 2)]
        public string Abstract { get; set; }

        /// <summary>
        /// The main content of the Post
        /// </summary>

        [Required]
        public string Content { get; set; }

        /// <summary>
        /// When the post was created
        /// </summary>

        [DataType(DataType.Date)]
        [Display(Name = "Created Date")]
        public DateTime Created { get; set; }
        /// <summary>
        /// contains an optional data of when the post was updated
        /// </summary>

        [DataType(DataType.Date)]
        [Display(Name = "Updated Date")]
        public DateTime? Updated { get; set; }

        /// <summary>
        /// an internal field used to route to the details action
        /// </summary>

        public string Slug { get; set; }

        /// <summary>
        /// ids the the state of the post
        /// </summary>

        [Required]
        [Display(Name = "Publish State")]
        public PublishState PublishState { get; set; }
        
        /// <summary>
        /// Byte array that is the image
        /// </summary>


        public byte[] ImageData { get; set; }

        /// <summary>
        /// the is the file type 
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Image is not stored
        /// </summary>

        [NotMapped]
        [Display(Name = "Choose Post Image")]
        public IFormFile ImageFile { get; set; }


        //Navigational properties
        public virtual Blog Blog { get; set; }
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();

    }
}
