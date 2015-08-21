﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace POESKillTree.Utils
{
    public static class Helpers
    {
        public static T FindAnchestor<T>(this DependencyObject current)
            where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

        public static T FindParent<T>(this FrameworkElement current)
            where T : FrameworkElement
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = current.Parent as FrameworkElement;
            }
            while (current != null);
            return null;
        }
    }
}
