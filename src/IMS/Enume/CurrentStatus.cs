using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Enume
{
    public enum CurrentStatus
    {
        待审核,
        待维修,
        自修方案待审,
        外修申请待审,
        待诊断,
        缓修申请待审,
        自修方案通过,
        自修方案失败,
        外修申请通过,
        外修申请失败,
        缓修申请通过,
        缓修申请失败,
        方案撤销中,
        撤销成功,
        撤销失败,
        已驳回,
        已总结
    };
}
