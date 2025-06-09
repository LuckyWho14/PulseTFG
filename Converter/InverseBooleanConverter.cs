﻿// Origen: PulseTFG/Converters/InverseBooleanConverter.cs
using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace PulseTFG.Converter
{
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return !b;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return !b;
            return false;
        }
    }
}
