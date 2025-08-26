using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportHub.Objects.DTOs
{
    public class ClaimsList
    {
        public int UserID { get; set; }
        public int CompanyId { get; set; }
        public int BranchId { get; set; }
        public int Role_Id { get; set; }
        public string UserName { get; set; }
    }
}
