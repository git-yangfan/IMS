using IMS.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Model.ViewModel
{
  public class EngineerViewModel
    {
      public EngineerViewModel() { }
      public EngineerViewModel(Users userModel)
      {
          this.EngineerName = userModel.Name;
          this.TeamName = userModel.BanZu;
          this.EngineerId = userModel.Id;
      }

      public  string TeamName { set; get; }
      public  string  EngineerName { get; set; }
      public int EngineerId { get; set; }
    }
}
