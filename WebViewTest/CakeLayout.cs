using System;
using Xamarin.Forms;

namespace WebViewTest
{
	public class CakeLayout : StackLayout
	{
		public CakeLayout()
		{
			TopStack = new StackLayout // TOP stack
			{
				Orientation = StackOrientation.Horizontal,
				VerticalOptions = LayoutOptions.Start
			};

			CenterStack = new StackLayout // CENTER stack
			{
				VerticalOptions = LayoutOptions.CenterAndExpand
			};

			BottomStack = new StackLayout // BOTTOM stack
			{
				Orientation = StackOrientation.Horizontal,
				VerticalOptions = LayoutOptions.End
			};

			Children.Add(TopStack);
			Children.Add(CenterStack);
			Children.Add(BottomStack);
		}

		public StackLayout BottomStack { get; private set; }
		public StackLayout CenterStack { get; private set; }
		public StackLayout TopStack { get; private set; }
	}
}
