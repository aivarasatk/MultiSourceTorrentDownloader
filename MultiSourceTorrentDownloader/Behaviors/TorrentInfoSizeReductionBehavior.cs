﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace MultiSourceTorrentDownloader.Behaviors
{
    public class TorrentInfoSizeReductionBehavior : Behavior<UserControl>
    {
        private bool _sizeChangeInProgress = true;
        private const int MaxDialogHeight = 890;
        protected override void OnAttached()
        {
            AssociatedObject.SizeChanged += SizeChanged;
        }

        private void SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _sizeChangeInProgress = !_sizeChangeInProgress;
            if (_sizeChangeInProgress) return;

            AssociatedObject.Width = AssociatedObject.ActualWidth * 0.85;

            var newHeight = AssociatedObject.ActualHeight * 0.9;
            AssociatedObject.Height = newHeight > MaxDialogHeight ? MaxDialogHeight : newHeight;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.SizeChanged -= SizeChanged;
        }
    }
}
