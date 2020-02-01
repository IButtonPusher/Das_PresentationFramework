using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Das.Gdi;
using Das.Gdi.Controls;
using Das.Views.Core.Geometry;
using Das.Views.Styles;

namespace ViewCompiler
{
    /// <summary>
    /// Is tied to a single file
    /// </summary>
    public class DesignViewWindow : ViewWindow
    {
        private readonly ViewDeserializer _serializer;
        private readonly FileInfo _fileDesigning;
        private readonly FileSystemWatcher _fileWatcher;
        
        private TrackBar trackBar1;
        private readonly ViewBuilderProvider _viewBuilderProvider;
        private Boolean _isChanged;

        private IStyleContext _styleContext;
        public override IStyleContext StyleContext
            => _styleContext ?? base.StyleContext;

        public DesignViewWindow(ViewDeserializer serializer, FileInfo fileDesigning) 
            : base(new GdiHostedElement(new BaseStyleContext(new DefaultStyle())))
        {
            _viewBuilderProvider = new ViewBuilderProvider(serializer);

            RenderMargin = new Thickness(0,0, 300,0);

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

        private async void OnFormLoaded(object sender, EventArgs e)
        {
            await LoadFile();
        }

        private async void OnWatchedFileChanged(object sender, FileSystemEventArgs e)
        {
            await Task.Delay(500);
            await LoadFile();
        }

        public override bool IsChanged => _isChanged || base.IsChanged;

        protected override void Dispose(bool disposing)
        {
            _fileWatcher.EnableRaisingEvents = false;
            base.Dispose(disposing);
        }

        private async Task LoadFile()
        {
            var bldr = await _viewBuilderProvider.GetViewBuilder(_fileDesigning);

            bldr.Serializer = _serializer;
            var type = _serializer.TypeInferrer.GetTypeFromClearName(bldr.DesignObject);
            var vm = _serializer.ObjectInstantiator.BuildDefault(type, false);

            _styleContext = bldr.StyleContext;

            bldr.SetDataContext(vm);

            View = bldr;

            _isChanged = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            _isChanged = false;
        }

        private void InitializeComponent()
        {
            trackBar1 = new TrackBar();
            ((System.ComponentModel.ISupportInitialize)(trackBar1)).BeginInit();
            SuspendLayout();
            // 
            // trackBar1
            // 
            trackBar1.Location = new System.Drawing.Point(12, 393);
            trackBar1.Maximum = 50;
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new System.Drawing.Size(104, 45);
            trackBar1.TabIndex = 0;
            trackBar1.Value = 25;
            trackBar1.ValueChanged += TrackBar1_ValueChanged;
            // 
            // DesignViewWindow
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            ClientSize = new System.Drawing.Size(800, 450);
            Controls.Add(trackBar1);
            Name = "DesignViewWindow";
            ((System.ComponentModel.ISupportInitialize)(trackBar1)).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        private void TrackBar1_ValueChanged(object sender, EventArgs e)
        {
            var delta = (double)trackBar1.Value - 25;
            delta = 1 + (delta / 25);
            //_renderKit.ZoomLevel = delta;
            ZoomLevel = Math.Max(delta,0.1);
            _isChanged = true;
        }
    }
}
