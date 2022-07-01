using BusinessEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessService
{
   public interface IBreakdownService
    {
        BreakdownEntity GetBreakdownData(BreakdownEntity BreakdownEntity);

        BreakdownEntity SetBreakReport(BreakdownEntity breakdownEntity);
    }
}
