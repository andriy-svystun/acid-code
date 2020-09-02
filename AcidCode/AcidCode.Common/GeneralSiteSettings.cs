using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcidCode.Common
{
    public class GeneralSiteSettings
    {
        [Key]
        [Editable(false)]
        public string Settings { get; set; }

        public TimeSpan CompilationTime { get; set; }

        public TimeSpan RunningTime { get; set; }
    }
}
