//Data/CommentReaction.cs

using System.ComponentModel.DataAnnotations;

namespace ShopAppProject.Data
{
    public class CommentReaction
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public int CommentId { get; set; }
        public bool IsLike { get; set; }

        public virtual ApplicationUser? User { get; set; }
        public virtual Comment? Comment { get; set; }
    }


}
