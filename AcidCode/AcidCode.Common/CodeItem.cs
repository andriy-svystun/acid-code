using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcidCode.Common
{
    public class CodeItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string CodeText { get; set; }

        public string EntryPoint { get; set; }

        AcidCodeUser AcidUser { get; set; }
    }
}
