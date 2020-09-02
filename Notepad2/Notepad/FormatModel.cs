using Notepad2.Utilities;
using System;
using System.Windows;
using System.Windows.Media;

namespace Notepad2.Notepad
{
    public class FormatModel : BaseViewModel
    {
        private double _size;
        private FontFamily _family;
        private FontStyle _style;
        private FontWeight _weight;
        private TextDecorationCollection _decoration;
        private string _strDecoration;
        private TextWrapping _wrap;
        private bool _isWrapped;
        private bool _isReadOnly;

        /// <summary>
        /// The Font Size
        /// </summary>
        public double Size
        {
            get => _size;
            set
            {
                if (value > 0 && value <= GlobalPreferences.MAX_FONT_SIZE)
                    RaisePropertyChanged(ref _size, value);
            }
        }

        /// <summary>
        /// The Font
        /// </summary>
        public FontFamily Family
        {
            get => _family;
            set => RaisePropertyChanged(ref _family, value);
        }

        public FontStyle Style
        {
            get => _style;
            set => RaisePropertyChanged(ref _style, value);
        }

        public FontWeight Weight
        {
            get => _weight;
            set => RaisePropertyChanged(ref _weight, value);
        }

        public TextDecorationCollection Decoration
        {
            get => _decoration;
            set => RaisePropertyChanged(ref _decoration, value);
        }

        public string DecorationReadable
        {
            get => _strDecoration;
            set => RaisePropertyChanged(ref _strDecoration, value, () =>
            {
                switch (value.ToString())
                {
                    case "None": Decoration = null; break;
                    case "Underline": Decoration = TextDecorations.Underline; break;
                    case "Strikethrough": Decoration = TextDecorations.Strikethrough; break;
                    case "OverLine": Decoration = TextDecorations.OverLine; break;
                    case "Baseline": Decoration = TextDecorations.Baseline; break;
                }
            });
        }

        /// <summary>
        /// Binding only. Change <see cref="IsWrapped"/> to change wrapping
        /// </summary>
        public TextWrapping Wrap
        {
            get => _wrap;
            set => RaisePropertyChanged(ref _wrap, value);
        }

        public bool IsWrapped
        {
            get => _isWrapped;
            set
            {
                RaisePropertyChanged(ref _isWrapped, value);
                Wrap = value ? TextWrapping.Wrap : TextWrapping.NoWrap;
            }
        }

        public bool IsReadOnly
        {
            get => _isReadOnly;
            set => RaisePropertyChanged(ref _isReadOnly, value);
        }

        // Only used for TextEditorLinesViewModel, which isn't used due to lag
        // and inconsistency (aka doesn't work well when scrolling :[ )
        //public Action<double> FontSizeChanged { get; set; }
        //public Action<FontFamily> FontFamilyChanged { get; set; }

        public FormatModel()
        {
            DecorationReadable = "None";
            Style = FontStyles.Normal;
            Weight = FontWeights.Normal;
        }
    }
}
