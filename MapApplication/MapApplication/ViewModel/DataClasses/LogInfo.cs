using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapApplication.ViewModel
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
                OnPropertyChanged("Element");
            }
        }
    }
}
