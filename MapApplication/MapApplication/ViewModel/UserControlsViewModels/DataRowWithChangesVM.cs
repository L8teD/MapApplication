using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MapApplication.Model.Types;

namespace MapApplication.ViewModel
{
    public class DataRowWithChangesVM : BaseViewModel
    {
        public DataRowVM dataRowVM { get; set; }
        public DataRowWithChangesVM(PlotName plotName, PlotCharacter character)
        {
            dataRowVM = new DataRowVM(plotName, character);
        }
        public void UpdateValueMessage(string obj)
        {
            dataRowVM.UpdateValueMessage(obj);
        }
    }
}
