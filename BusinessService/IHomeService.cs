using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessEntity;

namespace BusinessService
{
   public interface IHomeService
    {
                       
        HomeEntity GetLocaton(HomeEntity homeentity);

        CheckListEntity GetChecklistData(CheckListEntity ChkEntity);

        CheckListEntity SavechecklistData(CheckListEntity chkEntity);
    }
}
