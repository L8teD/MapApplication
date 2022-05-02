using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapApplication.ViewModel
{
    public class EquipmentData : BaseViewModel
    {
        private string name;
        private double _value;
        private string dimension;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }
        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged("Value");
            }
        }
        public string Dimension
        {
            get { return dimension; }
            set
            {
                dimension = value;
                OnPropertyChanged("Dimension");
            }
        }
    }
}
