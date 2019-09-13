using System;

namespace Das.Views.Rendering
{
    public class BasePerspective : IMutableViewPerspective
    {
        private readonly double _minZoomLevel;
        private readonly double _maxZoomLevel;

        public BasePerspective(Double minZoomLevel = Constants.DefaultMinZoom,
            Double maxZoomLevel = Constants.DefaultMaxZoom)
        {
            _minZoomLevel = minZoomLevel <= 0
                ? throw new ArgumentException(nameof(minZoomLevel))
                : minZoomLevel;

            _maxZoomLevel = maxZoomLevel <= 0
                ? throw new ArgumentException(nameof(maxZoomLevel))
                : maxZoomLevel;

            ZoomLevel = 1;
        }

        public double ZoomLevel { get; protected set; }

        public bool TrySetZoomLevel(double zoom)
        {
            if (ZoomLevel.AreEqualEnough(zoom))
                return true;

            if (zoom < _minZoomLevel || zoom > _maxZoomLevel)
                return false;

            ZoomLevel = zoom;
            IsChanged = true;
            return true;
        }

        public void AcceptChanges()
        {
            IsChanged = false;
        }

        public bool IsChanged { get; private set; }
    }
}