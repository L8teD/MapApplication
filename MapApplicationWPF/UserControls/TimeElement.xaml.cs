using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MapApplicationWPF.UserControls
{
    /// <summary>
    /// Логика взаимодействия для TimeElement.xaml
    /// </summary>
    public partial class TimeElement : UserControl, INotifyPropertyChanged
    {
        public string Text { get; set; }
        private Visibility visibility;
        public Visibility sp_Visibility
        {
            get
            {
                return visibility;
            }
            set
            {
                visibility = value;

                OnPropertyChanged("sp_Visibility");
            }
        }
        public string TimeOperationName
        {
            get { return (string)GetValue(TimeOperationProperty); }
            set { SetValue(TimeOperationProperty, value); }
        }
        public static readonly DependencyProperty TimeOperationProperty =
            DependencyProperty.Register("TimeOperationName", typeof(string), typeof(TimeElement), new PropertyMetadata(default(string)));

        public TimeElement()
        {
            InitializeComponent();
            DataContext = this;
        }
        public void SendTimeMessage(object messageRaw)
        {
            TimeOperationName = (string)messageRaw;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
