using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugApp
{
    public class LogInfo : BaseViewModel
    {
        string element;
        public string Element
        {
            get { return element; }
            set
            {
                element = value;
                OnPropertyChanged("ID");
            }
        }
    }
}
