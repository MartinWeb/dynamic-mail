using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dynamic_mail.Models
{
    public class Mail
    {
        public String to { get; set; }
        public String subject { get; set; }
        public String content { get; set; }

        public DynamicValues dynamicValues { get; set; }
    }
}