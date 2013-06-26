using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace PhoneApp3
{
    [TemplatePart(Name = SCROLLER_PARTNAME, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = LEFTVIEW_PARTNAME, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = MAINVIEW_PARTNAME, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = RIGHTVIEW_PARTNAME, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = TRANSLATE_PARTNAME, Type = typeof(TranslateTransform))]
    public class SlideView : Control
    {
        private const string SCROLLER_PARTNAME = "Scroller";
        private const string LEFTVIEW_PARTNAME = "LeftView";
        private const string MAINVIEW_PARTNAME = "MainView";
        private const string RIGHTVIEW_PARTNAME = "RightView";
        private const string TRANSLATE_PARTNAME = "Translate";

        private FrameworkElement _scroller;
        private FrameworkElement _leftView;
        private FrameworkElement _mainView;
        private FrameworkElement _rightView;
        private TranslateTransform _transform;
        private DateTime _startManipulation;
        public SlideView()
        {
            DefaultStyleKey = typeof(SlideView);
            this.SizeChanged += SlideView_SizeChanged;
            this.Background = new SolidColorBrush(Colors.Transparent);
            IsHitTestVisible = true;

        }
        protected override void OnManipulationStarted(System.Windows.Input.ManipulationStartedEventArgs e)
        {
            _startManipulation = DateTime.Now;
            base.OnManipulationStarted(e);
        }

        protected override void OnManipulationDelta(System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            var totalManipulation = _rightView.Width * 2;
            var translate = _transform.X + e.DeltaManipulation.Translation.X;
            _transform.X = Math.Min(0, Math.Max(translate, -totalManipulation));
        }
        private Storyboard _slideStoryboard;
        protected override void OnManipulationCompleted(System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            var duration = DateTime.Now.Subtract(_startManipulation);
            var manip = e.TotalManipulation.Translation.X;
            
            double? to = null;
            //left slide
            if (manip > 0)
            {
                //duration used to know if it(s a quick gesture or not (flick)
                if ((_transform.X >= -770 && _transform.X < -410) 
                    || (duration.Milliseconds <= 100 && _transform.X > -820 && _transform.X < -410))
                {
                    to = -410;
                }
                else if (_transform.X < -770)
                {
                    to = -820;
                }
                else if ((_transform.X >= -290) || (duration.Milliseconds <= 100 && _transform.X > -410))
                {
                    to = 0;
                }
                else
                {
                    to = -410;
                }

            }
            //right slide
            else if (manip < 0)
            {
                if ((_transform.X <= -50 && _transform.X > 530)
                    || (duration.Milliseconds <= 100 && _transform.X < 0 && _transform.X > -410))
                {
                    to = -410;
                }
                else if (_transform.X > -50)
                {
                    to = 0;
                }
                else if ((_transform.X <= -530)
                    || (duration.Milliseconds <= 100 && _transform.X < -410))
                {
                    to = -820;
                }
                else
                {
                    to = -410;
                }
            }

            if (to.HasValue)
            {
                if (_slideStoryboard != null)
                    _slideStoryboard.Stop();

                _slideStoryboard = new Storyboard();
                var da = new DoubleAnimation()
                {
                    To = to,
                    Duration = TimeSpan.FromMilliseconds(200),
                    FillBehavior = FillBehavior.HoldEnd,
                    EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
                };
                _slideStoryboard.Children.Add(da);

                Storyboard.SetTarget(_slideStoryboard, _transform);
                Storyboard.SetTargetProperty(_slideStoryboard, new PropertyPath("X"));

                _slideStoryboard.Begin();
            }
        }

        void SlideView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Clip = new RectangleGeometry() { Rect = new Rect(new Point(), e.NewSize) };
        }

        public override void OnApplyTemplate()
        {
            _scroller = GetTemplateChild(SCROLLER_PARTNAME) as FrameworkElement;
            _leftView = GetTemplateChild(LEFTVIEW_PARTNAME) as FrameworkElement;
            _mainView = GetTemplateChild(MAINVIEW_PARTNAME) as FrameworkElement;
            _rightView = GetTemplateChild(RIGHTVIEW_PARTNAME) as FrameworkElement;
            _transform = GetTemplateChild(TRANSLATE_PARTNAME) as TranslateTransform;

            base.OnApplyTemplate();
        }



        public object LeftContent
        {
            get { return (object)GetValue(LeftContentProperty); }
            set { SetValue(LeftContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftContentProperty =
            DependencyProperty.Register("LeftContent", typeof(object), typeof(SlideView), new PropertyMetadata(null));




        public object MainContent
        {
            get { return (object)GetValue(MainContentProperty); }
            set { SetValue(MainContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MainContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MainContentProperty =
            DependencyProperty.Register("MainContent", typeof(object), typeof(SlideView), new PropertyMetadata(null));


        public object RightContent
        {
            get { return (object)GetValue(RightContentProperty); }
            set { SetValue(RightContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RightContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightContentProperty =
            DependencyProperty.Register("RightContent", typeof(object), typeof(SlideView), new PropertyMetadata(null));



    }
}
