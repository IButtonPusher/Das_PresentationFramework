using System;

namespace Das.Views.Rendering
{
    public class BasePerspective : IMutableViewPerspective
    {
        private readonly Double _minZoomLevel;
        private readonly Double _maxZoomLevel;

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

        public Double ZoomLevel { get; protected set; }

        public Boolean TrySetZoomLevel(Double zoom)
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

        public Boolean IsChanged { get; private set; }
    }
}