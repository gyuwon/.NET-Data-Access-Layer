using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Commerce.Models
{
    public class Item
    {
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }

    public class Comment
    {
        public long Id { get; set; }
        public long ItemId { get; set; }
        [Required]
        public string AuthorId { get; set; }
        [Required]
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }

        [JsonIgnore]
        public Item Item { get; set; }
        [JsonIgnore]
        public ApplicationUser Author { get; set; }
    }

    public class CommentBindingModel
    {
        [Required]
        public string Content { get; set; }
    }
}