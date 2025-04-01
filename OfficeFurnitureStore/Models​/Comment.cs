using System;
using System.Collections.Generic;

namespace OfficeFurnitureStore.Models​;

public partial class Comment
{
    public int CommentId { get; set; }

    public int? CustomerId { get; set; }

    public int? ProductId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Product? Product { get; set; }
}
