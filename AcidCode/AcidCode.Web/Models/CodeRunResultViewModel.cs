using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace AcidCode.Web.Models
{
    public class CodeRunResultViewModel
    {
        [DataType(DataType.MultilineText)]
        [Editable(allowEdit: false)]
        public string CodeText { get; set; }

        public bool IsSucceeded { get; set; }

        [Editable(allowEdit: false)]
        public string RunningTime { get; set; }

        [Editable(allowEdit: false)]
        public string CodeOutput { get; set; }

        [Editable(allowEdit: false)]
        public string ErrorsText { get; set; }
    }
}