using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Das.GdiRenderKit;
using Das.Views;
using Das.Views.DevKit;
using Das.Views.Styles;
using ViewCompiler;

namespace GdiTest
{
    public class DesignViewWindow : ViewWindow
    {
        private readonly ViewDeserializer _serializer;
        private readonly FileInfo _fileDesigning;
        private readonly FileSystemWatcher _fileWatcher;
        private DevRenderKit _renderKit;

        private readonly DevInputHandler _inputHandler;
        private readonly InputContext _inputContext;
        private System.Windows.Forms.TrackBar trackBar1;
        private readonly ViewBuilderProvider _viewBuilderProvider;

        public DesignViewWindow(ViewDeserializer serializer, FileInfo fileDesigning)
        {
            _viewBuilderProvider = new ViewBuilderProvider(serializer);

            RenderMargin = new Thickness(0,0, 300,0);

            _serializer = serializer;
            _fileDesigning = fileDesigning ?? throw new ArgumentException();
            var dir = _fileDesigning.DirectoryName ?? throw new ArgumentException();

            _fileWatcher = new FileSystemWatcher(dir) {Filter = fileDesigning.Name};
            _fileWatcher.Changed += OnWatchedFileChanged;
            _fileWatcher.EnableRaisingEvents = true;

            _inputHandler = new DevInputHandler();
            _inputContext = new InputContext(_inputHandler, this);

            _inputHandler.SelectionChanged += OnSelectedElementsChanged;

            Load += OnFormLoaded;

            InitializeComponent();
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

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            _fileWatcher.EnableRaisingEvents = false;
            base.Dispose(disposing);
        }

        private async Task LoadFile()
        {
            var bldr = await _viewBuilderProvider.GetViewBuilder(_fileDesigning);

            bldr.Serializer = _serializer;
            var type = _serializer.GetTypeFromClearName(bldr.DesignObject);
            var vm = _serializer.BuildDefault(type, false);
            var styleContext = new BaseStyleContext(new DefaultStyle());

            foreach (var style in _serializer.GetStyles())
                styleContext.RegisterStyle(style);

            //has to be platform specific 
            var kit = new DevRenderKit(this, bldr, styleContext, _inputHandler, _inputContext)
                { DataContext = vm };

            _renderKit?.Dispose();

            _renderKit = kit;

            Invalidate();
        }

        private void OnSelectedElementsChanged(object sender, EventArgs e)
        {
            var selected = _inputHandler.SelectedVisuals.FirstOrDefault();
            _renderKit.InterrogateElement(selected);
        }

        private void InitializeComponent()
        {
            trackBar1 = new System.Windows.Forms.TrackBar();
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
            _renderKit.ZoomLevel = delta;
        }
    }
}
