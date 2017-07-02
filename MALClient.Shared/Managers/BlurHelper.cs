using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;
using Microsoft.Graphics.Canvas.Effects;
using WinRTXamlToolkit.Controls.Extensions;

namespace MALClient.UWP.Shared.Managers
{
    public class BlurHelper
    {
        private readonly FrameworkElement _element;
        private readonly bool _hostBackdrop;
        Compositor _compositor;
        SpriteVisual _hostSprite;

        public BlurHelper(FrameworkElement element,bool hostBackdrop)
        {
            _element = element;
            _hostBackdrop = hostBackdrop;
            if(hostBackdrop)
                BlurifyWithHostBackdrop();
            else
                BlurifyWithBackdrop();
        }

        public BlurHelper(CommandBar element)
        {
            _hostBackdrop = false;
            _element = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            var background = (element.GetDescendants().First() as Panel);
            background.Background = new SolidColorBrush(Color.FromArgb(0x7f, 0, 0, 0));
            background.Children.Add(_element);
            BlurifyWithHostBackdrop();
        }

        private void BlurifyWithHostBackdrop()
        {
            try
            {
                _compositor = ElementCompositionPreview.GetElementVisual(_element).Compositor;
                _hostSprite = _compositor.CreateSpriteVisual();
                _hostSprite.Size = new Vector2((float)_element.ActualWidth, (float)_element.ActualHeight);

                ElementCompositionPreview.SetElementChildVisual(_element, _hostSprite);
                if(_hostBackdrop)
                    _hostSprite.Brush = _compositor.CreateHostBackdropBrush();
                else
                    _hostSprite.Brush = _compositor.CreateBackdropBrush();
            }
            catch (Exception)
            {
                //not CU
            }
        }

        private void BlurifyWithBackdrop()
        {
            try
            {
                _compositor = ElementCompositionPreview.GetElementVisual(_element).Compositor;

                GaussianBlurEffect blurEffect = new GaussianBlurEffect()
                {
                    Name = "Blur",
                    BlurAmount = 3.0f,
                    BorderMode = EffectBorderMode.Soft,
                    Optimization = EffectOptimization.Balanced
                };
                blurEffect.Source = new CompositionEffectSourceParameter("source");

                CompositionEffectFactory blurEffectFactory = _compositor.CreateEffectFactory(blurEffect);

                CompositionEffectBrush blurBrush = blurEffectFactory.CreateBrush();

                CompositionBackdropBrush backdropBrush = _compositor.CreateBackdropBrush();

                blurBrush.SetSourceParameter("source", backdropBrush);

                _hostSprite = _compositor.CreateSpriteVisual();
                _hostSprite.Size = new Vector2((float)_element.ActualWidth, (float)_element.ActualHeight);
                _hostSprite.Brush = blurBrush;

                _element.SizeChanged += (sender, args) =>
                {
                    _hostSprite.Size = new Vector2((float) _element.ActualWidth, (float) _element.ActualHeight);
                };

                ElementCompositionPreview.SetElementChildVisual(_element,_hostSprite);
            }
            catch (Exception)
            {
                //not CU
            }
        }

    }
}
