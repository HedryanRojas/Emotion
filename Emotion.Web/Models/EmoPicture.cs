using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Emotion.Models
{
    public class EmoPicture
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Required]
        [MaxLength(100)]
        public string Path { get; set; }

        public virtual ObservableCollection<EmoFace> Faces{ get; set; }
    }
}