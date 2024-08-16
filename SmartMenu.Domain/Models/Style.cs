using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SmartMenu.Domain.Models
{
    public class Style
    {
        [JsonPropertyName("textColor")]
        public string? TextColor { get; set; } = "#ffffff";

        [JsonPropertyName("bFontId")]
        public int? BFontId { get; set; } = null;

        [JsonPropertyName("fontSize")]
        public float FontSize { get; set; }

        [JsonPropertyName("fontStyle")]
        public FontStyle FontStyle { get; set; } // gioi han bold italic underline

        [JsonPropertyName("alignment")]
        public StringAlignment Alignment { get; set; } // chi left center right th

        [JsonPropertyName("transparency")]
        public int Transparency { get; set; }

        [JsonPropertyName("uppercase")]
        public bool Uppercase { get; set; }

        [JsonPropertyName("currency")]
        public int Currency { get; set; }

        [JsonPropertyName("rotation")]
        public float Rotation { get; set; } = 0f;
    }
}
