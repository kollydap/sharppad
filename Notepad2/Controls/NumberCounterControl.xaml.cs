using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Notepad2.Controls
{
    /// <summary>
    /// Not even used xddddddd
    /// </summary>
    public partial class NumberCounterControl : UserControl
    {
        public static DependencyProperty LabelTextProperty =
            DependencyProperty.Register(
                nameof(LabelText),
                typeof(string),
                typeof(NumberCounterControl),
                new PropertyMetadata("Label Here", OnLabelTextChanged));

        public static DependencyProperty StartProperty =
            DependencyProperty.Register(
                nameof(Start),
                typeof(int),
                typeof(NumberCounterControl),
                new PropertyMetadata(OnNumberRangesChanged));

        public static DependencyProperty EndProperty =
            DependencyProperty.Register(
                nameof(End),
                typeof(int),
                typeof(NumberCounterControl),
                new PropertyMetadata(OnNumberRangesChanged));

       public static DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                nameof(ItemsSource),
                typeof(Collection<int>),
                typeof(NumberCounterControl));

        public static DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(
                nameof(SelectedIndex),
                typeof(int),
                typeof(NumberCounterControl),
                new PropertyMetadata(0, OnSelectedIndexChanged));

        public static DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(
                nameof(SelectedItem),
                typeof(int),
                typeof(NumberCounterControl),
                new PropertyMetadata(0, OnSelectedItemChanged));

        public static void OnLabelTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NumberCounterControl control)
            {
                string newLabelText = (string)e.NewValue;
                control.labelText.Content = newLabelText;
            }
        }

        public static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NumberCounterControl control)
            {
                int newIndex = (int)e.NewValue;
                if (control.ItemsSource != null && control.HasItems)
                    control.SelectedItem = control.ItemsSource[newIndex];
            }
        }
        public static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NumberCounterControl control)
            {
                int newContent = (int)e.NewValue;
                control.selectedContent.Text = newContent.ToString();
            }
        }
        public static void OnNumberRangesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NumberCounterControl control)
            {
                NumberRange range = control.Range;
                if (range == null) 
                    range = new NumberRange(0,0);

                if (e.Property == EndProperty)
                    if (int.TryParse(e.NewValue.ToString(), out int newVal1))
                        range.End = newVal1;
                    else if (e.Property == StartProperty)
                        if (int.TryParse(e.NewValue.ToString(), out int newVal2))
                            range.Start = newVal2;

                if (range.End > 0)
                {
                    if (control.ItemsSource != null)
                    {
                        control.ItemsSource.Clear();
                        foreach (int item in range.CalculateArray())
                        {
                            control.ItemsSource.Add(item);
                        }
                    }
                }
            }
        }

        public NumberRange Range { get; set; }

        public string LabelText
        {
            get => (string)GetValue(LabelTextProperty);
            set => SetValue(LabelTextProperty, value);
        }
        public Collection<int> ItemsSource
        {
            get => (Collection<int>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }
        public int Start
        {
            get => (int)GetValue(StartProperty);
            set => SetValue(StartProperty, value);
        }
        public int End
        {
            get => (int)GetValue(EndProperty);
            set => SetValue(EndProperty, value);
        }
        public int SelectedIndex
        {
            get => (int)GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }
        public int SelectedItem
        {
            get => (int)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public NumberCounterControl()
        {
            InitializeComponent();
            Range = new NumberRange(0,0);
            ItemsSource = new Collection<int>();
        }

        public void SetSelectedItem(int value)
        {
            SelectedItem = value;
        }

        public int GetPreviousItem()
        {
            if (ItemsSource != null && HasItems)
                if (HasItems && SelectedPosition <= ItemsCount)
                    return ItemsSource[SelectedIndex - 1];
            return -1;
        }

        public int GetNextItem()
        {
            if (ItemsSource != null && HasItems)
                if (!IsLastItemSelected)
                    return ItemsSource[SelectedIndex + 1];
            return -1;
        }

        public void ResetSelectedItem()
        {
            SelectedIndex = 0;
        }

        public bool HasItems
        {
            get => ItemsCount > 0;
        }

        public int SelectedPosition
        {
            get => SelectedIndex + 1;
        }

        public bool IsLastItemSelected
        {
            get => SelectedPosition == ItemsCount;
        }

        public int ItemsCount
        {
            get => ItemsSource != null ? ItemsSource.Count : 0;
        }

        public void MoveItemRight()
        {
            if (HasItems && SelectedIndex < (ItemsCount - 1))
                SelectedIndex++;
        }

        public void MoveItemLeft()
        {
            if (HasItems)
            {
                if (SelectedIndex > 0 && SelectedPosition <= ItemsCount)
                    SelectedIndex--;
            }
        }

        private void moveItemRightClick(object sender, RoutedEventArgs e)
        {
            MoveItemRight();
        }

        private void moveItemLeftClick(object sender, RoutedEventArgs e)
        {
            MoveItemLeft();
        }
    }
}
