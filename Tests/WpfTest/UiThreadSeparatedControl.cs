using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using MultiUiThreadedExample.CustomVisuals;


namespace MultiUiThreadedExample
{
    /// <summary>
    /// Delegates arrange to fwe _uiContent
    /// </summary>
    ///   /// References:
    /// http://blogs.msdn.com/b/dwayneneed/archive/2007/04/26/multithreaded-ui-hostvisual.aspx
    /// http://eprystupa.wordpress.com/2008/07/28/running-wpf-application-with-multiple-ui-threads/
    /// http://gettinggui.com/creating-a-busy-indicator-in-a-separate-thread-in-wpf/
    [ContentProperty("Child")]
    public class UiThreadSeparatedControl : FrameworkElement
    {
        #region fields

        private readonly AutoResetEvent _resentEvent;
        private IntPtr _parentWindow;

        private HostVisual _hostVisual;
        private Func<FrameworkElement> _createContentFromStyle;

        protected FrameworkElement _uiContent;
        protected Dispatcher _threadSeparatedDispatcher;
        protected VisualTargetPresentationSource _visualTarget;

        #endregion

        #region properties

        #region IsContentShowingProperty

        public static readonly DependencyProperty IsContentShowingProperty = DependencyProperty.Register(
            "IsContentShowing",
            typeof(bool),
            typeof(UiThreadSeparatedControl),
            new FrameworkPropertyMetadata(true, OnIsContentShowingChanged));

        public bool IsContentShowing
        {
            get => (bool)GetValue(IsContentShowingProperty);
            set => SetValue(IsContentShowingProperty, value);
        }

        private static void OnIsContentShowingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uiBase = (UiThreadSeparatedControl)d;

            if ((bool)e.NewValue)
            {
                uiBase.CreateThreadSeparatedElement();
            }
            else
            {
                uiBase.DestroyThreadSeparatedElement();
            }
        }
        #endregion

        #region ThreadSeparatedStyle

        public static readonly DependencyProperty ThreadSeparatedStyleProperty = DependencyProperty.Register(
            "ThreadSeparatedStyle",
            typeof(Style),
            typeof(UiThreadSeparatedControl),
            new FrameworkPropertyMetadata(null, OnThreadSeparatedStyleChanged));

        public Style ThreadSeparatedStyle
        {
            get => (Style)GetValue(ThreadSeparatedStyleProperty);
            set => SetValue(ThreadSeparatedStyleProperty, value);
        }

        private static void OnThreadSeparatedStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (UiThreadSeparatedControl)d;
            var style = e.NewValue as Style;
            if (style != null)
            {
                var templateDict = new Dictionary<DependencyProperty, string>();
                var invokingType = style.TargetType;
                var setters = style.Setters.ToArray();

                foreach (var setterBase in setters)
                {
                    var setter = (Setter)setterBase;

                    var oldTemp = setter.Value as FrameworkTemplate;

                    // templates are instantiated on the thread its defined in, this may cause UI thread access issues
                    // we need to deconstruct the template as a string so it can be accessed on our other thread
                    if (oldTemp != null && !templateDict.ContainsKey(setter.Property))
                    {
                        var templateString = XamlWriter.Save(oldTemp);
                        templateDict.Add(setter.Property, templateString);
                    }
                }

                control._createContentFromStyle = () =>
                {
                    var newStyle = new Style
                    {
                        TargetType = invokingType,
                    };
                    foreach (var setterBase in setters)
                    {
                        var setter = (Setter)setterBase;

                        // now that we are on our new UI thread, we can reconstruct the template
                        string templateString;
                        if (templateDict.TryGetValue(setter.Property, out templateString))
                        {
                            //http://msdn.microsoft.com/en-us/library/ms754193(v=vs.110).aspx
                            // Solution: Place resources into the first element of your control
                            // Or: template the entire UserControl instead
                            var reader = new StringReader(templateString);
                            var xmlReader = XmlReader.Create(reader);
                            var template = XamlReader.Load(xmlReader);
                            setter = new Setter(setter.Property, template);
                        }

                        newStyle.Setters.Add(setter);
                    }

                    var content = (FrameworkElement)Activator.CreateInstance(newStyle.TargetType);
                    content.Style = newStyle;

                    return content;
                };
            }
            else
            {
                control._createContentFromStyle = null;
            }
        }


        #endregion

        #region Child Property

        private UIElement _child;
        public UIElement Child
        {
            get => _child;
            set
            {
                if (_child != null)
                {
                    RemoveVisualChild(_child);
                }

                _child = value;

                if (_child != null)
                {
                    AddVisualChild(_child);
                }
            }
        }

        #endregion

        #endregion

        #region constructor

        public UiThreadSeparatedControl()
        {
            _resentEvent = new AutoResetEvent(false);

            Initialized += UiThreadSeparatedBase_Initialized;
        }

        #endregion

        #region protected

        protected virtual FrameworkElement CreateUiContent()
        {
            if (_createContentFromStyle != null)
            {
                return _createContentFromStyle();
            }

            return null;
        }

        protected virtual void CreateThreadSeparatedElement()
        {

//            HwndSourceParameters hwsp = new HwndSourceParameters("AddIn");
//            hwsp.WindowStyle = 0; // no style, in particular no WM_CHILD
//            var hwndSource = new HwndSource(hwsp);
//            Native.ConvertToChildWindow(hwndSource.Handle);
//            hwndSource.CompositionTarget.BackgroundColor = Colors.White;
//            hwndSource.SizeToContent = SizeToContent.Manual; // The host will do the sizing, via its HwndHost.
//            ComponentDispatcher.ThreadPreprocessMessage += new ThreadMessageEventHandler(ThreadPreprocessMessage);
//            hwndSource.AddHook(PPPP);



            _hostVisual = new HostVisual();

            AddLogicalChild(_hostVisual);
            AddVisualChild(_hostVisual);

            // Spin up a worker thread, and pass it the HostVisual that it
            // should be part of.
            var thread = new Thread(CreateContentOnSeparateThread)
            {
                IsBackground = true
            };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Name = "Map UI Thread";
            thread.Start();

            // Wait for the worker thread to spin up and create the VisualTarget.
            _resentEvent.WaitOne();

            InvalidateMeasure();
        }

        private IntPtr PPPP(IntPtr hwnd, Int32 msg, IntPtr wparam, IntPtr lparam, ref Boolean handled)
        {
            return IntPtr.Zero;
        }

        private void ThreadPreprocessMessage(ref MSG msg, ref Boolean handled)
        {
            return;
        }

        

        protected virtual void DestroyThreadSeparatedElement()
        {
            if (_threadSeparatedDispatcher == null)
                return;

            _threadSeparatedDispatcher.InvokeShutdown();

            RemoveLogicalChild(_hostVisual);
            RemoveVisualChild(_hostVisual);

            _hostVisual = null;
            _uiContent = null;
            _threadSeparatedDispatcher = null;
        }

        #endregion

        #region private

        private void CreateContentOnSeparateThread()
        {
            if (_hostVisual == null)
                return;

            // Create the VisualTargetPresentationSource and then signal the
            // calling thread, so that it can continue without waiting for us.
            _visualTarget = new VisualTargetPresentationSource(_hostVisual, _parentWindow);
            
            //ActiveSource = _visualTarget;

            _uiContent = CreateUiContent();

            if (_uiContent == null)
            {
                throw new InvalidOperationException("Created UI Content cannot return null. Either override 'CreateUiContent()' or assign a style to 'ThreadSeparatedStyle'");
            }

            _threadSeparatedDispatcher = _uiContent.Dispatcher;
            ComponentDispatcher.ThreadPreprocessMessage += OnThreadMessage;
            
            

            _resentEvent.Set();

            _visualTarget.RootVisual = _uiContent;

            // Run a dispatcher for this worker thread.  This is the central
            // processing loop for WPF.
            Dispatcher.Run();

            _visualTarget.Dispose();
        }

//        private IntPtr MouseHookMethod(Int32 code, IntPtr wparam, IntPtr lparam)
//        {
//            Debug.WriteLine("000 " + wparam);
//            return MouseHook.CallNextHookEx(IntPtr.Zero, code, wparam, lparam);
//        }


        private void OnThreadMessage(ref MSG msg, ref Boolean handled)
        {
            //System.Diagnostics.Debug.WriteLine("no msg " + msg.message);
        }

        private void UiThreadSeparatedBase_Initialized(object sender, EventArgs e)
        {
            var halp = new WindowInteropHelper(Application.Current.MainWindow);
            _parentWindow = halp.EnsureHandle();

            if (IsContentShowing)
            {
                CreateThreadSeparatedElement();
            }
        }

        #endregion

        #region overrides

        protected override int VisualChildrenCount =>
            Child != null && _hostVisual != null ? 2
            : Child != null || _hostVisual != null ? 1
            : 0;

        protected override IEnumerator LogicalChildren
        {
            get
            {
                if (Child != null)
                {
                    yield return Child;
                }

                if (_hostVisual != null)
                {
                    yield return _hostVisual;
                }
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (Child != null)
            {
                switch (index)
                {
                    case 0:
                        return Child;

                    case 1:
                        return _hostVisual;
                }
            }
            else if (index == 0)
            {
                return Child != null ? (Visual)Child : _hostVisual;
            }

            throw new IndexOutOfRangeException("index");
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var childSize = new Size();
            var uiSize = new Size();

            if (Child != null)
            {
                Child.Measure(constraint);

                var element = Child as FrameworkElement;
                childSize.Width = element != null ? element.ActualWidth : Child.DesiredSize.Width;
                childSize.Height = element != null ? element.ActualHeight : Child.DesiredSize.Height;
            }

            if (_uiContent != null)
            {
                _uiContent.Dispatcher.Invoke(DispatcherPriority.Background, 
                    new Action(() => _uiContent.Measure(constraint)));
                uiSize.Width = _uiContent.ActualWidth;
                uiSize.Height = _uiContent.ActualHeight;
            }

            var size = new Size(Math.Max(childSize.Width, uiSize.Width), 
                Math.Max(childSize.Height, uiSize.Height)); 
            return constraint;
        }

        //public PresentationSource ActiveSource { get; private set; }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Child != null)
            {
                Child.Arrange(new Rect(finalSize));
            }

            if (_uiContent != null)
            {
                _uiContent.Dispatcher.BeginInvoke(DispatcherPriority.Background, 
                    new Action(() => _uiContent.Arrange(new Rect(finalSize))));
            }

            return finalSize;
        }

        #endregion
    }
}
