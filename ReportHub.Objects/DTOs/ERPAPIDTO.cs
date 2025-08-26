using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportHub.Objects.DTOs
{
    public class ERPAPIDTO
    {
        public class UserGroupResponse
        {
            public string Permissions { get; set; }
            public string Groups { get; set; }
        }
    }
}
