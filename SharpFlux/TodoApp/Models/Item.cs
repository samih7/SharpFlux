using System;

namespace TodoApp.Models
{
    public class Item : IEquatable<Item>
    {
        public string Id { get; set; }

        public string Text { get; set; }

        public string Description { get; set; }

        public bool IsComplete { get; set; }

        public bool Equals(Item other)
        {
            return Id == other.Id;
        }
    }
}
