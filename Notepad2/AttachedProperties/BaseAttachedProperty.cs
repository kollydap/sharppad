using System;
using System.Windows;

namespace Notepad2.AttachedProperties
{
    /// <summary>
    /// A base class for simplifying the creation of attached properties 
    /// </summary>
    /// <typeparam name="Parent">The parent class (aka the class name of the </typeparam>
    /// <typeparam name="Property">The Attacged Property type (bool, string, etc)</typeparam>
    public class BaseAttachedProperty<Parent, Property> where Parent : BaseAttachedProperty<Parent, Property>, new()
    {
        // "Singleton" parent
        public static Parent Instance { get; private set; } = new Parent();

        /// <summary>
        /// Attached property for this class
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.RegisterAttached(
                "Value",
                typeof(Property),
                typeof(BaseAttachedProperty<Parent, Property>),
                new PropertyMetadata(
                    new PropertyChangedCallback(OnValuePropertyChanged)));

        public event Action<DependencyObject, DependencyPropertyChangedEventArgs> ValueChanged = (s, e) => { };

        /// <summary>
        /// Callback event for when the property changes
        /// </summary>
        /// <param name="d">UI Element that has changed</param>
        /// <param name="e">Event args</param>
        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Instance.OnValueChanged(d, e);
            Instance.ValueChanged(d, e);
        }

        /// <summary>
        /// Gets the attached property's value
        /// </summary>
        /// <param name="d">The element to get the property from</param>
        /// <returns></returns>
        public static Property GetValue(DependencyObject d)
        {
            return (Property)d.GetValue(ValueProperty);
        }

        /// <summary>
        /// Sets the attached property's value
        /// </summary>
        /// <param name="d">The element to get the property from</param>
        /// <param name="value">The new value to set the property to</param>
        public static void SetValue(DependencyObject d, Property value)
        {
            d.SetValue(ValueProperty, value);
        }

        /// <summary>
        /// Method that is called when any attached property is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) { }
    }
}
