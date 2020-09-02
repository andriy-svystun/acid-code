using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AcidCode.Web.Models
{
    public class HomeViewModel
    {
        [Required(ErrorMessage = "Put your code here")]
        [Display(Name = "Put sample code")]
        [DataType(DataType.MultilineText)]
        public string CodeText { get; set; }

        public bool IsSucceeded { get; set; }

        [Editable(allowEdit:false)]
        public string RunningTime { get; set; }

        [Editable(allowEdit: false)]
        public string CodeOutput { get; set; }

        [Editable(allowEdit: false)]
        public string ErrorsText { get; set; }
    }
}