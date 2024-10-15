using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novel.Models.ViewModels
{
    public class DashboardView
    {
        public int TotalOrders { get; set; }
        public int TotalInProcess { get; set; }
        public int TotalApproved { get; set; }
        public int TotalPending { get; set; }
        public int TotalCompleted { get; set; }
    }
}
