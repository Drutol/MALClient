using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace MALClient.ViewModels
{
    public interface IHamburgerControlView
    {
        double GetScrollBurgerActualHeight();
    }

    public class Parameter : INotifyPropertyChanged //wrapper
    {
        private Brush _value;
        public Brush Value //real value
        {
            get { return _value; }
            set { _value = value; RaisePropertyChanged("Value"); }
        }

        public Parameter(Brush value)
        {
            Value = value;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public class HamburgerControlViewModel : ViewModelBase
    {
        private bool? _prevState;
        private int _stackPanelHeightSum = 325; //base value
        private bool _subtractedHeightForButton = true;


        public IHamburgerControlView View { get; set; }
       

        private Dictionary<string,Parameter> _txtForegroundBrushes = new Dictionary<string, Parameter>
        {
            ["AnimeList"] = new Parameter(new SolidColorBrush(Colors.Black)),
            ["AnimeSearch"] = new Parameter(new SolidColorBrush(Colors.Black)),
            ["LogIn"] = new Parameter(new SolidColorBrush(Colors.Black)),
            ["Settings"] = new Parameter(new SolidColorBrush(Colors.Black)),
            ["Profile"] = new Parameter(new SolidColorBrush(Colors.Black)),
            ["Seasonal"] = new Parameter(new SolidColorBrush(Colors.Black)),
            ["About"] = new Parameter(new SolidColorBrush(Colors.Black)),
            ["Recommendations"] = new Parameter(new SolidColorBrush(Colors.Black)),
        };
        public Dictionary<string, Parameter> TxtForegroundBrushes
        {
            get { return _txtForegroundBrushes; }
            set
            {
                _txtForegroundBrushes = value;
                RaisePropertyChanged(() => TxtForegroundBrushes);
            }
        }

        public RelayCommand PaneOpenedCommand { get; private set; }

        private double _gridSeparatorHeight;
        public double GridSeparatorHeight
        {
            get { return _gridSeparatorHeight; }
            set
            {
                _gridSeparatorHeight = value;
                RaisePropertyChanged(() => GridSeparatorHeight);
            }
        }

        private double _gridBtmMarginHeight;
        public double GridBtmMarginHeight
        {
            get { return _gridBtmMarginHeight; }
            set
            {
                _gridBtmMarginHeight = value;
                RaisePropertyChanged(() => GridBtmMarginHeight);
            }
        }

        public HamburgerControlViewModel()
        {
               PaneOpenedCommand = new RelayCommand(this.PaneOpened);
        }

        public void ChangeBottomStackPanelMargin(bool up)
        {
            if (up == _prevState)
                return;

            _prevState = up;

            _stackPanelHeightSum += up ? 50 : -50;
        }

        public void PaneOpened()
        {
            var val = Convert.ToInt32(View.GetScrollBurgerActualHeight());
            GridSeparatorHeight = val - _stackPanelHeightSum < 0 ? 0 : val - _stackPanelHeightSum;
            GridBtmMarginHeight = GridSeparatorHeight < 1 ? 50 : 0;
        }
    }
}
