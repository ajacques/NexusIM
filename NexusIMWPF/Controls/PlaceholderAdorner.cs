using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Diagnostics;
 
namespace NexusIM.Controls
{
	/// <summary>
	///   Represents an adorner that adds placeholder text to a <see cref="T:System.Windows.Controls.TextBox"/>,
	///   <see cref="T:System.Windows.Controls.RichTextBox"/> or <see cref="T:System.Windows.Controls.PasswordBox"/>.
	/// </summary>
	public class Placeholder : Adorner
	{
		/// <summary>
		///   Event handler for <see cref="E:System.Windows.Controls.Primitives.TextBoxBase.TextChanged" />.
		/// </summary>
		private readonly TextChangedEventHandler _textChangedHandler;
 
		/// <summary>
		///   Event handler for <see cref="E:System.Windows.Controls.PasswordBox.PasswordChanged" />.
		/// </summary>
		private readonly RoutedEventHandler _passwordChangedHandler;
 
		/// <summary>
		///   <see langword="true" /> when the placeholder text is visible, <see langword="false" /> otherwise.
		///   Used to avoid calling <see cref="M:System.Windows.UIElement.InvalidateVisual"/> unnecessarily.
		/// </summary>
		private bool _isPlaceholderVisible;
 
		/// <summary>
		///    Identifies the Huan.WhiteDwarf.UI.Placeholder.Text attached property.
		/// </summary>
		public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached("Text", typeof(string), typeof(Placeholder), new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback(OnTextChanged)));
		 
		#region Constructors
		/// <summary>
		///   Initializes a new instance of the <see cref="T:Huan.WhiteDwarf.UI.Placeholder"/> class.
		/// </summary>
		/// <param name="adornedElement">
		///   The element to bind the adorner to.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		///   Raised when adornedElement is null.
		/// </exception>
		protected Placeholder(Control adornedElement)
			: base(adornedElement)
		{
			this.IsHitTestVisible = false;
			_textChangedHandler = new TextChangedEventHandler(AdornedElement_ContentChanged);
			_passwordChangedHandler = new RoutedEventHandler(AdornedElement_ContentChanged);
 
			adornedElement.GotFocus += new RoutedEventHandler(AdornedElement_GotFocus);
			adornedElement.LostFocus += new RoutedEventHandler(AdornedElement_LostFocus);
		}
 
		/// <summary>
		///   Initializes a new instance of the <see cref="T:Huan.WhiteDwarf.UI.Placeholder"/> class.
		/// </summary>
		/// <param name="adornedElement">
		///   The element to bind the adorner to.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		///   Raised when adornedElement is null.
		/// </exception>
		public Placeholder(PasswordBox adornedElement)
			: this((Control)adornedElement)
		{
			if (!adornedElement.IsFocused)
				adornedElement.PasswordChanged += _passwordChangedHandler;
		}
 
		/// <summary>
		///   Initializes a new instance of the <see cref="T:Huan.WhiteDwarf.UI.Placeholder"/> class.
		/// </summary>
		/// <param name="adornedElement">
		///   The element to bind the adorner to.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		///   Raised when adornedElement is null.
		/// </exception>
		public Placeholder(TextBox adornedElement)
			: this((Control)adornedElement)
		{
			if (!adornedElement.IsFocused)
				adornedElement.TextChanged += _textChangedHandler;
		}
 
		/// <summary>
		///   Initializes a new instance of the <see cref="T:Huan.WhiteDwarf.UI.Placeholder"/> class.
		/// </summary>
		/// <param name="adornedElement">
		///   The element to bind the adorner to.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		///   Raised when adornedElement is null.
		/// </exception>
		public Placeholder(RichTextBox adornedElement)
			: this((Control)adornedElement)
		{
			if (!adornedElement.IsFocused)
				adornedElement.TextChanged += _textChangedHandler;
		}
		#endregion
 
		#region Property Changed Callbacks
		/// <summary>
		///   Invoked whenever Huan.WhiteDwarf.UI.Placeholder.Text attached property is changed.
		/// </summary>
		/// <param name="sender">
		///   The object where the event handler is attached.
		/// </param>
		/// <param name="e">
		///   Provides data about the event.
		/// </param>
		private static void OnTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			Control adornedElement = sender as Control;
			if (adornedElement.IsLoaded)
				AddAdorner(adornedElement);
			else
				adornedElement.Loaded += new RoutedEventHandler(AdornedElement_Loaded);
		}
		#endregion
 
		#region Event Handlers
		/// <summary>
		///   Event handler for AdornedElement.Loaded.
		/// </summary>
		/// <param name="sender">
		///   The AdornedElement where the event handler is attached.
		/// </param>
		/// <param name="e">
		///   Provides data about the event.
		/// </param>
		private static void AdornedElement_Loaded(object sender, RoutedEventArgs e)
		{
			Control adornedElement = (Control)sender;
			adornedElement.Loaded -= AdornedElement_Loaded;
			AddAdorner(adornedElement);
		}
 
		/// <summary>
		///   Event handler for AdornedElement.GotFocus.
		/// </summary>
		/// <param name="sender">
		///   The AdornedElement where the event handler is attached.
		/// </param>
		/// <param name="e">
		///   Provides data about the event.
		/// </param>
		private void AdornedElement_GotFocus(object sender, RoutedEventArgs e)
		{
			TextBoxBase textBoxBase = AdornedElement as TextBoxBase;
			if (textBoxBase != null)
				textBoxBase.TextChanged -= AdornedElement_ContentChanged;
			else
			{
				PasswordBox passwordBox = AdornedElement as PasswordBox;
				if (passwordBox != null)
					passwordBox.PasswordChanged -= AdornedElement_ContentChanged;
			}
 
			if (_isPlaceholderVisible)
				this.InvalidateVisual();
		}
 
		/// <summary>
		///   Event handler for AdornedElement.LostFocus.
		/// </summary>
		/// <param name="sender">
		///   The AdornedElement where the event handler is attached.
		/// </param>
		/// <param name="e">
		///   Provides data about the event.
		/// </param>
		public void AdornedElement_LostFocus(object sender, RoutedEventArgs e)
		{
			TextBoxBase textBoxBase = AdornedElement as TextBoxBase;
			if (textBoxBase != null)
				textBoxBase.TextChanged += _textChangedHandler;
			else
			{
				PasswordBox passwordBox = AdornedElement as PasswordBox;
				if (passwordBox != null)
					passwordBox.PasswordChanged += _passwordChangedHandler;
			}
 
			if (!_isPlaceholderVisible && IsElementEmpty())
				this.InvalidateVisual();
		}
 
		/// <summary>
		///   Event handler for AdornedElement.ContentChanged.
		/// </summary>
		/// <param name="sender">
		///   The AdornedElement where the event handler is attached.
		/// </param>
		/// <param name="e">
		///   Provides data about the event.
		/// </param>
		private void AdornedElement_ContentChanged(object sender, RoutedEventArgs e)
		{
			if (_isPlaceholderVisible ^ IsElementEmpty())
				this.InvalidateVisual();
		}
		#endregion
 
		#region Attached Property Getters and Setters
		/// <summary>
		///   Gets the value of the Huan.WhiteDwarf.UI.Placeholder.Text attached property for a specified element.
		/// </summary>
		/// <param name="adornedElement">
		///   The element from which the property value is read.
		/// </param>
		/// <returns>
		///   The placeholder text property value for the element.
		/// </returns>
		/// <exception cref="T:ArgumentNullException">
		///   Raised when adornedElement is null.
		/// </exception>
		public static string GetText(Control adornedElement)
		{
			if (adornedElement == null)
				throw new ArgumentNullException("adornedElement");
 
			return (string)adornedElement.GetValue(TextProperty);
		}
 
		/// <summary>
		///   Sets the value of the Huan.WhiteDwarf.UI.Placeholder.Text attached property to a specified element.
		/// </summary>
		/// <param name="adornedElement">
		///   The element to which the attached property is written.
		/// </param>
		/// <param name="placeholderText">
		///   The needed placeholder text value.
		/// </param>
		/// <exception cref="T:ArgumentNullException">
		///   Raised when adornedElement is null.
		/// </exception>
		/// <exception cref="T:InvalidOperationException">
		///   Raised when adornedElement is not a <see cref="T:System.Windows.Controls.TextBox"/>,
		///   <see cref="T:System.Windows.Controls.RichTextBox"/> or <see cref="T:System.Windows.Controls.PasswordBox"/>.
		/// </exception>
		public static void SetText(Control adornedElement, string placeholderText)
		{
			if (adornedElement == null)
				throw new ArgumentNullException("adornedElement");
 
			if (!(adornedElement is TextBox || adornedElement is RichTextBox || adornedElement is PasswordBox))
				throw new InvalidOperationException();
 
			adornedElement.SetValue(TextProperty, placeholderText);
		}
		#endregion
 
		/// <summary>
		///   Adds a <see cref="T:Huan.WhiteDwarf.UI.Placeholder"/> to the adorner layer.
		/// </summary>
		/// <param name="adornedElement">
		///   The adorned element.
		/// </param>
		private static void AddAdorner(Control adornedElement)
		{
			AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
			if (adornerLayer == null)
				return;
 
			Adorner[] adorners = adornerLayer.GetAdorners(adornedElement);
			if (adorners != null)
				foreach (Adorner adorner in adorners)
					if (adorner is Placeholder)
						return;
 
			TextBox textBox = adornedElement as TextBox;
			if (textBox != null)
			{
				adornerLayer.Add(new Placeholder(textBox));
				return;
			}
 
			RichTextBox richTextBox = adornedElement as RichTextBox;
			if (richTextBox != null)
			{
				adornerLayer.Add(new Placeholder(richTextBox));
				return;
			}
 
			PasswordBox passwordBox = adornedElement as PasswordBox;
			if (passwordBox != null)
			{
				adornerLayer.Add(new Placeholder(passwordBox));
				return;
			}
		}
 
		/// <summary>
		///   Checks if the content of the adorned element is empty.
		/// </summary>
		/// <returns>
		///   Returns <see langword="true" /> if the content is empty, <see langword="false" /> otherwise.
		/// </returns>
		private bool IsElementEmpty()
		{
			UIElement adornedElement = AdornedElement;
			TextBox textBox = adornedElement as TextBox;
			if (textBox != null)
				return string.IsNullOrEmpty(textBox.Text);
 
			PasswordBox passwordBox = adornedElement as PasswordBox;
			if (passwordBox != null)
				return string.IsNullOrEmpty(passwordBox.Password);
 
			RichTextBox richTextBox = adornedElement as RichTextBox;
			if (richTextBox != null)
			{
				BlockCollection blocks = richTextBox.Document.Blocks;
				if (blocks.Count == 0)
					return true;
				if (blocks.Count == 1)
				{
					Paragraph paragraph = blocks.FirstBlock as Paragraph;
					if (paragraph == null)
						return false;
 
					if (paragraph.Inlines.Count == 0)
						return true;
 
					if (paragraph.Inlines.Count == 1)
					{
						Run run = paragraph.Inlines.FirstInline as Run;
						return (run != null && string.IsNullOrEmpty(run.Text));
					}
				}
 
				return false;
			}
 
			return false;
		}
		
		private bool IsElementVisible()
		{
			
			return true;
		}
 
		/// <summary>
		///    Computes the text alignment of the adorned element.
		/// </summary>
		/// <returns>
		///   Returns the computed text alignment.
		/// </returns>
		private TextAlignment ComputedTextAlignment()
		{
			Control adornedElement = AdornedElement as Control;
			TextBox textBox = adornedElement as TextBox;
			if (textBox != null)
			{
				if (DependencyPropertyHelper.GetValueSource(textBox, TextBox.HorizontalContentAlignmentProperty)
					.BaseValueSource != BaseValueSource.Local ||
					DependencyPropertyHelper.GetValueSource(textBox, TextBox.TextAlignmentProperty)
					.BaseValueSource == BaseValueSource.Local)
 
					// TextAlignment dominates
					return textBox.TextAlignment;
			}
 
			RichTextBox richTextBox = adornedElement as RichTextBox;
			if (richTextBox != null)
			{
				BlockCollection blocks = richTextBox.Document.Blocks;
				TextAlignment textAlignment = richTextBox.Document.TextAlignment;
				if (blocks.Count == 0)
					return textAlignment;
 
				if (blocks.Count == 1)
				{
					Paragraph paragraph = blocks.FirstBlock as Paragraph;
					if (paragraph == null)
						return textAlignment;
 
					return paragraph.TextAlignment;
				}
 
				return textAlignment;
			}
 
			switch (adornedElement.HorizontalContentAlignment)
			{
				case HorizontalAlignment.Left:
					return TextAlignment.Left;
				case HorizontalAlignment.Right:
					return TextAlignment.Right;
				case HorizontalAlignment.Center:
					return TextAlignment.Center;
				case HorizontalAlignment.Stretch:
					return TextAlignment.Justify;
			}
 
			return TextAlignment.Left;
		}
 
		/// <summary>
		///   Draws the content of a <see cref="T:System.Windows.Media.DrawingContext" /> object during the render pass of a <see cref="T:NexusIM.Controls.Placeholder"/> element.
		/// </summary>
		/// <param name="drawingContext">
		///   The <see cref="T:System.Windows.Media.DrawingContext" /> object to draw. This context is provided to the layout system.
		/// </param>
		protected override void OnRender(DrawingContext drawingContext)
		{
			Control adornedElement = this.AdornedElement as Control;
			string placeholderText;

			if (adornedElement == null || adornedElement.IsFocused || adornedElement.Visibility != Visibility.Visible || !IsElementVisible() || !IsElementEmpty() || string.IsNullOrEmpty(placeholderText = (string)adornedElement.GetValue(TextProperty)))
				_isPlaceholderVisible = false;
			else {
				_isPlaceholderVisible = true;
				Size size = adornedElement.RenderSize;
				TextAlignment computedTextAlignment = ComputedTextAlignment();
				// foreground brush does not need to be dynamic. OnRender called when SystemColors changes.
				Brush foreground = SystemColors.GrayTextBrush.Clone();
				foreground.Opacity = adornedElement.Foreground.Opacity;
				Typeface typeface = new Typeface(adornedElement.FontFamily, FontStyles.Italic, adornedElement.FontWeight, adornedElement.FontStretch);
				FormattedText formattedText = new FormattedText(placeholderText, CultureInfo.CurrentCulture, adornedElement.FlowDirection, typeface, adornedElement.FontSize,foreground);
				formattedText.TextAlignment = computedTextAlignment;
				if (size.Height != 0)
					formattedText.MaxTextHeight = size.Height - adornedElement.BorderThickness.Top - adornedElement.BorderThickness.Bottom - adornedElement.Padding.Top - adornedElement.Padding.Bottom;

				if (size.Width != 0)
					formattedText.MaxTextWidth = size.Width - adornedElement.BorderThickness.Left - adornedElement.BorderThickness.Right - adornedElement.Padding.Left - adornedElement.Padding.Right - 4.0;

				double left;
				double top = 0.0;
				if (adornedElement.FlowDirection == FlowDirection.RightToLeft)
					left = adornedElement.BorderThickness.Right + adornedElement.Padding.Right + 2.0;
				else
					left = adornedElement.BorderThickness.Left + adornedElement.Padding.Left + 2.0;

				switch (adornedElement.VerticalContentAlignment)
				{
					case VerticalAlignment.Top:
					case VerticalAlignment.Stretch:
						top = adornedElement.BorderThickness.Top + adornedElement.Padding.Top;
						break;
					case VerticalAlignment.Bottom:
						top = size.Height - adornedElement.BorderThickness.Bottom - adornedElement.Padding.Bottom - formattedText.Height;
						break;
					case VerticalAlignment.Center:
						top = (size.Height + adornedElement.BorderThickness.Top - adornedElement.BorderThickness.Bottom + adornedElement.Padding.Top - adornedElement.Padding.Bottom - formattedText.Height) / 2.0;
						break;
				}

				if (adornedElement.FlowDirection == FlowDirection.RightToLeft)
				{
					// Somehow everything got drawn reflected. Add a transform to correct.
					drawingContext.PushTransform(new ScaleTransform(-1.0, 1.0, RenderSize.Width / 2.0, 0.0));
					drawingContext.DrawText(formattedText, new Point(left, top));
					drawingContext.Pop();
				} else
					drawingContext.DrawText(formattedText, new Point(left, top));
			}
		}
	}
}