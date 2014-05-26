﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fcmd.base_plugins.ve
{
	class PlainText : pluginner.IVEPlugin
	{
		#region Metadata
		public string Name { get { return new Localizator().GetString("VEptxtVer"); } }
		public string Version { get { return System.Windows.Forms.Application.ProductVersion; } }
		public string Author { get { return "Alexander Tauenis"; } }
		public event pluginner.TypedEvent<object[]> APICallHost;
		#endregion

		Xwt.Menu mnuFormat = new Xwt.Menu();
		Xwt.Table Layout = new Xwt.Table() { DefaultRowSpacing = 0 };
		Xwt.RichTextView RTV = new Xwt.RichTextView();
		Xwt.ScrollView ScrollBox;
		Xwt.Label lblFileName = new Xwt.Label("file name");
		Xwt.MenuButton mbMode = new Xwt.MenuButton("Text") { Sensitive = false, Type = Xwt.ButtonType.Normal, Style = Xwt.ButtonStyle.Flat };
		Xwt.MenuButton mbCodepage = new Xwt.MenuButton("codepage") { Type = Xwt.ButtonType.DropDown, Style = Xwt.ButtonStyle.Flat };

		pluginner.Stylist s = new pluginner.Stylist(fcmd.Properties.Settings.Default.UserTheme);
		int Codepage = Encoding.Default.CodePage;
		byte[] fileContent;
		string Txt = "";

		public PlainText() //constructor
		{
			ScrollBox = new Xwt.ScrollView(RTV);
			Layout.Add(ScrollBox, 0, 1, 1, 3, true, true);

			Layout.Add(lblFileName, 0, 0);
			Layout.Add(mbMode, 1, 0);
			Layout.Add(mbCodepage, 2, 0);

			foreach (EncodingInfo cp in Encoding.GetEncodings())
			{
				Xwt.MenuItem mi = new Xwt.MenuItem();
				mi.Tag = cp.CodePage;
				mi.Label = "CP" + cp.CodePage + " - " + cp.DisplayName;
				mi.Clicked += new EventHandler(Codepage_Clicked);
				mnuFormat.Items.Add(mi);
			}
			mbCodepage.Menu = mnuFormat;
		}

		void Codepage_Clicked(object sender, EventArgs e)
		{
			Xwt.MenuItem MI = (Xwt.MenuItem)sender;
			ChangeCodepage(Convert.ToInt32(MI.Tag));
		}

		void ChangeCodepage(int CP)
		{
			Codepage = Convert.ToInt32(CP);
			Txt = Encoding.GetEncoding(Codepage).GetString(fileContent ?? new byte[] { 0, 0 });
			RTV.LoadText(Txt, new Xwt.Formats.PlainTextFormat());
			mbCodepage.Label = Encoding.GetEncoding(Codepage).EncodingName;
		}

		public int[] APICompatibility {
			get{
				int[] fapiver = {0,1,0, 0,1,0};
				return fapiver;
			}
		}

		public object APICallPlugin(string Command, params object[] Arguments)
		{
			switch (Command)
			{
				case "codepage":
					ChangeCodepage(Convert.ToInt32(Arguments[1]));
					break;
				case "findreplace": break;
				case "cut": break;
				case "copy": break;
				case "paste": break;
				case "select": break;
				case "print": break;
				case "pagesetup": break;
				default: Xwt.MessageDialog.ShowWarning("Unknown command:", Command); break;
			}
			return null;
		}

		public void OpenFile(string url, pluginner.IFSPlugin fsplugin)
		{
			lblFileName.Text = url;
			fileContent = fsplugin.GetFileContent(url);
			ChangeCodepage(Codepage);
		}

		public void SaveFile(bool SaveAs = false) { }

		public Xwt.Widget Body {
			get { return Layout; }
		}

		public bool ReadOnly { get { return false; } set { } } //todo

		public bool CanEdit { get { return false; } }//todo (needs to edit xwt rtv or te)

		public Xwt.Menu FormatMenu { get { return mnuFormat;} }

		public bool ShowToolbar {
			set
			{
				lblFileName.Visible = value;
				mbMode.Visible = value;
				mbCodepage.Visible = value;
			}
		}

		public pluginner.Stylist Stylist
		{
			set
			{
				s = value;

				s.Stylize(RTV, "VEWorkingArea");
				s.Stylize(Layout, "VEToolbar");
				/*foreach (Xwt.Widget w in Layout.Children)
				{
					s.Stylize(w, "VEToolbar");
				}*/
				s.Stylize(lblFileName,"VEToolbarLabel");
				s.Stylize(mbMode,"VEToolbarButton");
				s.Stylize(mbCodepage,"VEToolbarButton");
			}
		}
	}
}
