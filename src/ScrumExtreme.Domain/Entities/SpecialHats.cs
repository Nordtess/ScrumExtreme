using System;
using System.Collections.Generic;
using System.Text;

namespace ScrumExtreme.Domain.Entities
{
    public class SpecialHats : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public List<string> Sizes { get; set; } = new();

        public double Price { get; set; }

        public string MaterialList { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;




    }
}
