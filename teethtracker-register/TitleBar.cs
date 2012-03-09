using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Controls.Local {
    /// <summary>
    /// ========================================
    /// .NET Framework 3.0 Custom Control
    /// ========================================
    ///
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:TestAlex"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:TestAlex;assembly=TestAlex"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file. Note that Intellisense in the
    /// XML editor does not currently work on custom controls and its child elements.
    ///
    ///     <MyNamespace:TitleBar/>
    ///
    /// </summary>
    public class TitleBar : System.Windows.Controls.Control {
        ImageButton closeButton;
        ImageButton maxButton;
        ImageButton minButton;

        public TitleBar() {
            this.MouseLeftButtonDown += new MouseButtonEventHandler(OnTitleBarLeftButtonDown);
            this.MouseDoubleClick += new MouseButtonEventHandler(TitleBar_MouseDoubleClick);
            this.Loaded += new RoutedEventHandler(TitleBar_Loaded);
        }

        void TitleBar_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            MaxButton_Click(sender, e);
        }

        void TitleBar_Loaded(object sender, RoutedEventArgs e) {
            closeButton = (ImageButton)this.Template.FindName("CloseButton", this);
            minButton = (ImageButton)this.Template.FindName("MinButton", this);
            maxButton = (ImageButton)this.Template.FindName("MaxButton", this);

            closeButton.Click += new RoutedEventHandler(CloseButton_Click);
            minButton.Click += new RoutedEventHandler(MinButton_Click);
            maxButton.Click += new RoutedEventHandler(MaxButton_Click);
        }


        static TitleBar() {
            //This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            //This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TitleBar), new FrameworkPropertyMetadata(typeof(TitleBar)));
        }

        #region event handlers

        void OnTitleBarLeftButtonDown(object sender, MouseEventArgs e) {
            Window window = this.TemplatedParent as Window;
            if (window != null) {
                window.DragMove();
            }
        }

        void CloseButton_Click(object sender, RoutedEventArgs e) {
            Window window = this.TemplatedParent as Window;
            if (window != null) {
                window.Close();
            }
        }

        void MinButton_Click(object sender, RoutedEventArgs e) {
            Window window = this.TemplatedParent as Window;
            if (window != null) {
                window.WindowState = WindowState.Minimized;
            }
        }

        void MaxButton_Click(object sender, RoutedEventArgs e) {
            Window window = this.TemplatedParent as Window;
            if (window != null) {
                if (window.WindowState == WindowState.Maximized) {
                    maxButton.ImageNormal = "/images/max_n.png";
                    maxButton.ImageOver = "/images/max_hover.png";
                    maxButton.ImageDown = "/images/max_click.png";

                    window.WindowState = WindowState.Normal;
                } else {
                    maxButton.ImageNormal = "/images/normal.png";
                    maxButton.ImageOver = "/images/normal_hover.png";
                    maxButton.ImageDown = "/images/normal_click.png";

                    window.WindowState = WindowState.Maximized;
                }
            }
        }

        #endregion

        #region properties

        public string Title {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public ImageSource Icon {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }


        #endregion

        #region dependency properties

        public static readonly DependencyProperty TitleProperty =
           DependencyProperty.Register(
               "Title", typeof(string), typeof(TitleBar));

        public static readonly DependencyProperty IconProperty =
           DependencyProperty.Register(
               "Icon", typeof(ImageSource), typeof(TitleBar));

        #endregion
    }
}
