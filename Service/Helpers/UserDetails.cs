using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Helpers
{
    public class UserDetails
    {
        [KernelFunction]
        public int GetUserAge()
        {
            return 22;
        }
    }
}
