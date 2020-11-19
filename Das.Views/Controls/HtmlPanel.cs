﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;

namespace Das.Views.Controls
{
    public class HtmlPanel : BaseSurrogatedVisual
    {

        private String? _markup;

        public String? Markup
        {
            get => _markup;
            set => SetValue(ref _markup, value, OnMarkupChanged);
        }

        private void OnMarkupChanged(String? obj)
        {
            Debug.WriteLine("changing markup in html surrogater");
        }

        private Uri? _uri;

        public Uri? Uri
        {
            get => _uri;
            set => SetValue(ref _uri, value);
        }

        public override void Arrange(IRenderSize availableSpace,
                                     IRenderContext renderContext)
        {
            throw new NotSupportedException("A surrogate control is required for this control");
        }

        public override ValueSize Measure(IRenderSize availableSpace,
                                          IMeasureContext measureContext)
        {
            throw new NotSupportedException("A surrogate control is required for this control");
        }
    }
}