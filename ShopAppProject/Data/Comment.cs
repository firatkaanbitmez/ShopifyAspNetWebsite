//Data/Comment.cs

using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
namespace ShopAppProject.Data
{
    public class Comment
    {
        public int CommentId { get; set; }
        public string? UserId { get; set; }
        public int ProductId { get; set; }
        public string? UserName { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsBlocked { get; set; }

        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public int Rating { get; set; }
        public virtual ICollection<CommentReaction> Reactions { get; set; }

        public Comment()
        {
            Reactions = new HashSet<CommentReaction>();
        }
    }

}
