using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmadeusBlog.Models
{
    public class PostComment
    {
        public int Id { get; set; }
        public int CategoryPostid { get; set; }
        public string BlogUserId { get; set; }

        public string CommentBody { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        public DateTime? Moderated { get; set; }
        public string ModeratedReason { get; set; }
        public string ModeratedBody { get; set; }

        public virtual CategoryPost CategoryPost { get; set; }
        public virtual BlogUser BlogUser { get; set; }
    }
}
