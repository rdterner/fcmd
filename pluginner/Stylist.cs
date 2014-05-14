﻿/* The File Commander - plugin API
 * File Commander Stylist.
 * CSS declaration parser and Xwt theme enabler
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2014, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mucss;

namespace pluginner
{
	/// <summary>Simplifies CSS-based theming of some Xwt widgets</summary>
	public class Stylist
	{
		/// <summary>The μCSS parser that is the kernel of the Stylist</summary>
		public mucss.Stylesheet CSS;

		bool semaphore = false;

		/// <summary>Initialize Stylist</summary>
		/// <param name="CSS_File">The path to the CSS file (or null if only internal styles are need to be used)</param>
		public Stylist(string CSS_File = null)
		{
			DefaultStyle = Utilities.GetEmbeddedResource("Resources.Default.css");

			if (DefaultStyle == null) Environment.FailFast("File Commander has been crashed: the default theme's stylesheet is unable to load. Possibly there is a failure of the pluginner.dll body or RAM banks. Try to reinstall FC.", new InvalidProgramException("Default style isn't loading"));
			if(CSS_File != null && CSS_File != "")
				{ CSS = new Stylesheet(System.IO.File.ReadAllText(CSS_File) + DefaultStyle); }
			else
				CSS = new Stylesheet(DefaultStyle);
		}

		/// <summary>Enable theming of the widget</summary>
		/// <param name="Widget">The widget that needs to be themized</param>
		/// <param name="Selector">The selector</param>
		public void Stylize(Xwt.Widget Widget, string Selector = "Widget"){
			if (!semaphore) {
				semaphore = true;
				Stylize(Widget, "Widget"); //apply default style for all widgets
				try {
					Stylize(Widget, Widget.GetType().ToString().Substring(Widget.GetType().ToString().IndexOf('.') + 1)); //apply default style for the widget type
				}
				catch { Console.WriteLine("NOTICE: No style is set for widgets of type " + Widget.GetType().ToString().Substring(Widget.GetType().ToString().IndexOf('.') + 1)); }
				semaphore = false;
			}

			ApplyStyle(Widget, Selector);

			Widget.MouseEntered+=(o,ea)=>
			{ ApplyStyle(Widget, Selector + ":hover"); };

			Widget.MouseExited+=(o,ea)=>
			{
				ApplyStyle(Widget, Selector);
				if (Widget.HasFocus)
					ApplyStyle(Widget, Selector + ":focus");
			};

			Widget.ButtonPressed+=(o,ea)=>
			{ ApplyStyle(Widget, Selector + ":active"); };

			Widget.GotFocus+=(o,ea)=>
			{ ApplyStyle(Widget, Selector + ":focus"); };

			Widget.LostFocus += (o, ea) =>
			{ ApplyStyle(Widget, Selector); };

			Widget.ButtonReleased+=(o,ea)=>
			{
				ApplyStyle(Widget, Selector);
				if (Widget.HasFocus)
					ApplyStyle(Widget, Selector + ":focus");
			};
		}


		/// <summary>Apply the specified selector (style) to the specified widget</summary>
		/// <param name="Widget">The widget that should "got" the style</param>
		/// <param name="Style">The specified selector with the desired style</param>
		public void ApplyStyle(Xwt.Widget Widget, string Pattern)
		{
			if (Widget.GetType() == typeof(Xwt.Label)) { ApplyStyle((Xwt.Label)Widget, Pattern); return; }
			if (Widget.GetType() == typeof(Xwt.Box)) { ApplyStyle((Xwt.Box)Widget, Pattern); return; }

			Selector Selector = CSS[Pattern];
			Widget.BackgroundColor =
			Utilities.GetXwtColor(
				Selector.Declarations["background-color"].Value
			);

			Widget.Font = Xwt.Drawing.Font.FromName(
				Selector.Declarations["font-family"].Value
			);

			Widget.Visible = Selector.Declarations["display"].Value == "none" ? false : true;

		}

		/// <summary>Apply the specified selector (style) to the specified widget</summary>
		/// <param name="Widget">The widget that should "got" the style</param>
		/// <param name="Style">The specified selector with the desired style</param>
		public void ApplyStyle(Xwt.Label Widget, string Pattern)
		{
			Selector Selector = CSS[Pattern];
			Widget.BackgroundColor =
			Utilities.GetXwtColor(
				Selector.Declarations["background-color"].Value
			);

			Widget.TextColor =
			Utilities.GetXwtColor(
				Selector.Declarations["color"].Value
			);

			Widget.Font = Xwt.Drawing.Font.FromName(
				Selector.Declarations["font-family"].Value
			);

			Widget.Visible = Selector.Declarations["display"].Value == "none" ? false : true;
		}

		/// <summary>Apply the specified selector (style) to the specified widget</summary>
		/// <param name="Widget">The widget that should "got" the style</param>
		/// <param name="Style">The specified selector with the desired style</param>
		public void ApplyStyle(Xwt.Box Widget, string Pattern)
		{
			Selector Selector = CSS[Pattern];
			Widget.BackgroundColor =
			Utilities.GetXwtColor(
				Selector.Declarations["background-color"].Value
			);

			foreach (Xwt.Widget Child in Widget.Children)
			{
				ApplyStyle(Child,Pattern);
			}

			

			Widget.Visible = Selector.Declarations["display"].Value == "none" ? false : true;
		}

		/// <summary>Apply the specified selector (style) to the specified widget</summary>
		/// <param name="Widget">The widget that should "got" the style</param>
		/// <param name="Style">The specified selector with the desired style</param>
		public void ApplyStyle(Xwt.Button Widget, string Pattern)
		{
			Selector Selector = CSS[Pattern];
			Widget.BackgroundColor =
			Utilities.GetXwtColor(
				Selector.Declarations["background-color"].Value
			);


			if (GetBorder(Selector.Declarations["border-style"].Value))
				Widget.Style = Xwt.ButtonStyle.Normal;
			else
				Widget.Style = Xwt.ButtonStyle.Borderless;


			Widget.Visible = Selector.Declarations["display"].Value == "none" ? false : true;
		}


		private bool GetBorder(string borderStyle)
		{
			if (borderStyle != "none")  return false; else return true;
			//it's need to understand all css borderstyles
		}

		private string DefaultStyle;
	}
}
