using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Das.Gdi;
using Das.Gdi.Controls;
using Das.Views;
using Das.Views.Core.Geometry;
using Das.Views.Panels;
using Das.Views.Styles;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace ViewCompiler
{
    /// <summary>
    ///     Is tied to a single file
    /// </summary>
    public class DesignViewWindow : ViewWindow
    {
        public DesignViewWindow(ViewDeserializer serializer, 
                                FileInfo fileDesigning,
                                IVisualBootStrapper templateResolver)
            : this(serializer, fileDesigning, new BaseStyleContext(new DefaultStyle(),
                    new DefaultColorPalette()),
                templateResolver)
            //: base(new GdiHostedElement(new BaseStyleContext(new DefaultStyle())))
        {
            
        }

        private DesignViewWindow(ViewDeserializer serializer,
                                 FileInfo fileDesigning,
                                 IStyleContext styleContext,
                                 IVisualBootStrapper templateResolver)
            : base(new GdiHostedElement(new View<Object>(templateResolver), styleContext))
        {
            trackBar1 = new TrackBar();
            _viewBuilderProvider = new ViewBuilderProvider(serializer);

            RenderMargin = new Thickness(0, 0, 300, 0);

            _serializer = serializer;
            _fileDesigning = fileDesigning ?? throw new ArgumentException();
            var dir = _fileDesigning.DirectoryName ?? throw new ArgumentException();

            _fileWatcher = new FileSystemWatcher(dir) {Filter = fileDesigning.Name};
            _fileWatcher.Changed += OnWatchedFileChanged;
            _fileWatcher.EnableRaisingEvents = true;

            Load += OnFormLoaded;

            InitializeComponent();

            trackBar1.BringToFront();
        }

        public override Boolean IsChanged => _isChanged || base.IsChanged;

        public override IStyleContext StyleContext
            => _styleContext ?? base.StyleContext;

        protected override void Dispose(Boolean disposing)
        {
            _fileWatcher.EnableRaisingEvents = false;
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            ((ISupportInitialize) trackBar1).BeginInit();
            SuspendLayout();
            // 
            // trackBar1
            // 
            trackBar1.Location = new Point(12, 393);
            trackBar1.Maximum = 50;
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new Size(104, 45);
            trackBar1.TabIndex = 0;
            trackBar1.Value = 25;
            trackBar1.ValueChanged += TrackBar1_ValueChanged;
            // 
            // DesignViewWindow
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            ClientSize = new Size(800, 450);
            Controls.Add(trackBar1);
            Name = "DesignViewWindow";
            ((ISupportInitialize) trackBar1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private async Task LoadFile()
        {
            var bldr = await _viewBuilderProvider.GetViewBuilder(_fileDesigning);

            bldr.Serializer = _serializer;
            var type = _serializer.TypeInferrer.GetTypeFromClearName(bldr.DesignObject);
            if (type == null)
                return;
            var vm = _serializer.ObjectInstantiator.BuildDefault(type, false);

            _styleContext = bldr.StyleContext;

            await bldr.SetDataContextAsync(vm);

            View = bldr;

            _isChanged = true;
        }

        private async void OnFormLoaded(Object sender, EventArgs e)
        {
            await LoadFile();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            _isChanged = false;
        }

        private async void OnWatchedFileChanged(Object sender, FileSystemEventArgs e)
        {
            await Task.Delay(500);
            await LoadFile();
        }

        private void TrackBar1_ValueChanged(Object sender, EventArgs e)
        {
            var delta = (Double) trackBar1.Value - 25;
            delta = 1 + delta / 25;
            
            ZoomLevel = Math.Max(delta, 0.1);
            _isChanged = true;
        }

        private readonly FileInfo _fileDesigning;
        private readonly FileSystemWatcher _fileWatcher;
        private readonly ViewDeserializer _serializer;
        private readonly ViewBuilderProvider _viewBuilderProvider;
        private Boolean _isChanged;

        private IStyleContext? _styleContext;

        private TrackBar trackBar1;
    }
}