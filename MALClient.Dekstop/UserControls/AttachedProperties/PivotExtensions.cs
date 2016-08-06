using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MALClient.UserControls.AttachedProperties
{
    public class PivotExtensions : DependencyObject
    {
        public static readonly DependencyProperty HidePivotItemIndexProperty =
           DependencyProperty.RegisterAttached(
               "HidePivotItemIndex",
               typeof(int),
               typeof(Pivot),
               new PropertyMetadata(-1,PropertyChangedCallback)
               );
        //string - pivot item
        private static readonly Dictionary<object,object> _itemsCahe = new Dictionary<object, object>();

        private static void PropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            try
            {
                var pivot = sender as Pivot;
                var val = (int)args.NewValue;
                if (val > 0 && !_itemsCahe.ContainsKey(pivot.Tag))
                {
                    _itemsCahe.Add(pivot.Tag, pivot.Items[val]);
                    pivot.Items.RemoveAt(val);
                }
                else
                {
                    pivot.Items.Insert(1, _itemsCahe[pivot.Tag]); //TODO: If i'm ever gonna need this -> replace this "1" with variable
                    _itemsCahe.Remove(pivot.Tag);
                }
            }
            catch (Exception)
            {
               //
            }        
        }

        public static void SetHidePivotItemIndex(UIElement element, int value)
        {
            element.SetValue(HidePivotItemIndexProperty, value);
        }

        public static int GetHidePivotItemIndex(UIElement element)
        {
            return (int)element.GetValue(HidePivotItemIndexProperty);
        }
    }
}
