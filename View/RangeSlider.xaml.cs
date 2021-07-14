using System;
using System.Collections.Generic;
using System.Linq;
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

namespace RealtorObjects.View
{
    /// <summary>
    /// Логика взаимодействия для RangeSlider.xaml
    /// </summary>
    public partial class RangeSlider : UserControl
    {

        public static readonly DependencyProperty LowerValueProperty = DependencyProperty.Register("LowerValue", typeof(double), typeof(RangeSlider), new UIPropertyMetadata(0d));
        public static readonly DependencyProperty UpperValueProperty = DependencyProperty.Register("UpperValue", typeof(double), typeof(RangeSlider), new UIPropertyMetadata(0d));
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(RangeSlider), new UIPropertyMetadata(0d));
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(RangeSlider), new UIPropertyMetadata(1d));
        public static readonly DependencyProperty IsSnapToTickEnabledProperty = DependencyProperty.Register("IsSnapToTickEnabled", typeof(bool), typeof(RangeSlider), new PropertyMetadata());
        public static readonly DependencyProperty TickFrequencyProperty = DependencyProperty.Register("TickFrequency", typeof(int), typeof(RangeSlider), new PropertyMetadata());
        public bool IsSnapToTickEnabled {
            get {
                return (bool)GetValue(IsSnapToTickEnabledProperty);
            }
            set {
                SetValue(IsSnapToTickEnabledProperty, value);
            }
        }
        public int TickFrequency {
            get {
                return (int)GetValue(TickFrequencyProperty);
            }
            set {
                SetValue(TickFrequencyProperty, value);
            }
        }
        public RangeSlider() {
            InitializeComponent();
            this.Loaded += RangeSlider_Loaded;
        }
        void RangeSlider_Loaded(object sender, RoutedEventArgs e) {
            LowerSlider.ValueChanged += LowerSlider_ValueChanged;
            UpperSlider.ValueChanged += UpperSlider_ValueChanged;
        }

        private void LowerSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            UpperSlider.Value = Math.Max(UpperSlider.Value, LowerSlider.Value);
        }

        private void UpperSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            LowerSlider.Value = Math.Min(UpperSlider.Value, LowerSlider.Value);
        }

        public double Minimum {
            get {
                return (double)GetValue(MinimumProperty);
            }
            set {
                SetValue(MinimumProperty, value);
            }
        }


        public double LowerValue {
            get {
                return (double)GetValue(LowerValueProperty);
            }
            set {
                SetValue(LowerValueProperty, value);
            }
        }


        public double UpperValue {
            get {
                return (double)GetValue(UpperValueProperty);
            }
            set {
                SetValue(UpperValueProperty, value);
            }
        }


        public double Maximum {
            get {
                return (double)GetValue(MaximumProperty);
            }
            set {
                SetValue(MaximumProperty, value);
            }
        }

    }
}
