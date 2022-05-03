using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapApplication.ViewModel
{
    public class WindInfoL : BaseViewModel
    {
        EquipmentData l_u;
        EquipmentData l_v;
        EquipmentData l_w;


        public EquipmentData L_u
        {
            get { return l_u; }
            set
            {
                l_u = value;
                OnPropertyChanged("L_u");
            }
        }
        public EquipmentData L_v
        {
            get { return l_v; }
            set
            {
                l_v = value;
                OnPropertyChanged("L_v");
            }
        }
        public EquipmentData L_w
        {
            get { return l_w; }
            set
            {
                l_w = value;
                OnPropertyChanged("L_w");
            }
        }
    }
    public class WindInfoSigma : BaseViewModel
    {
        EquipmentData sigma_u;
        EquipmentData sigma_v;
        EquipmentData sigma_w;
        public EquipmentData Sigma_u
        {
            get { return sigma_u; }
            set
            {
                sigma_u = value;
                OnPropertyChanged("Sigma_u");
            }
        }
        public EquipmentData Sigma_v
        {
            get { return sigma_v; }
            set
            {
                sigma_v = value;
                OnPropertyChanged("Sigma_v");
            }
        }
        public EquipmentData Sigma_w
        {
            get { return sigma_w; }
            set
            {
                sigma_w = value;
                OnPropertyChanged("Sigma_w");
            }
        }
    }
    public class WindInfoWindProjection : BaseViewModel
    {
        EquipmentData wind_n;
        EquipmentData wind_e;
        EquipmentData wind_h;
        public EquipmentData Wind_n
        {
            get { return wind_n; }
            set
            {
                wind_n = value;
                OnPropertyChanged("Wind_n");
            }
        }
        public EquipmentData Wind_e
        {
            get { return wind_e; }
            set
            {
                wind_e = value;
                OnPropertyChanged("Wind_e");
            }
        }
        public EquipmentData Wind_h
        {
            get { return wind_h; }
            set
            {
                wind_h = value;
                OnPropertyChanged("Wind_h");
            }
        }
    }
    public class WindInfoOther : BaseViewModel
    {
        EquipmentData wind_angle;
        public EquipmentData Wind_angle
        {
            get { return wind_angle; }
            set
            {
                wind_angle = value;
                OnPropertyChanged("Wind_angle");
            }
        }
    }
}
