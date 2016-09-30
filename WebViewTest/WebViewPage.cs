using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Toasts;
using Xamarin.Forms;
using XLabs;
using XLabs.Forms.Controls;
using XLabs.Serialization.JsonNET;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace WebViewTest
{
	public class WebViewPage : ContentPage
	{
		public static HybridWebView webView;
		public static Entry urlEntry;
		public static Button urlButton;
		public static Button scriptButton;
		public static Label jsFiles;
		public static List<Script> scripts;
		public static string currentUrl;

		public WebViewPage()
		{
			GetScripts(false);

			currentUrl = "https://demo.wheels.com/DriverView/Home/Account/LogOn";

			webView = new HybridWebView(new XLabs.Serialization.JsonNET.JsonSerializer())
			{
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Uri = new Uri(currentUrl)
			};

			urlEntry = new Entry()
			{
				Placeholder = "URL...",
				FontSize = 8,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = 20,
				Margin = new Thickness(5, 0, 0, 0)
			};

			urlButton = new Button()
			{
				Text = "Go",
				HeightRequest = 20
			};

			urlButton.Clicked += (sender, e) =>
			{
				webView.Uri = new Uri(urlEntry.Text);
			};

			scriptButton = new Button()
			{
				Text = "Re-Load JS",
				HeightRequest = 20,
				Margin = new Thickness(5, 0, 0, 0),
			};

			scriptButton.Clicked += (sender, e) =>
			{
				GetScripts(true);
			};


			jsFiles = new Label()
			{
				Text = "JS Injected: test.js, home.js",
				FontSize = 10,
				Margin = new Thickness(5,0,0,0),
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalTextAlignment = TextAlignment.Center,
				HeightRequest = 20
			};

			var hr1 = new BoxView()
			{
				HeightRequest = 1,
				Color = Color.Gray
			};

			var hr2 = new BoxView()
			{
				HeightRequest = 1,
				Color = Color.Gray
			};

			var hr3 = new BoxView()
			{
				HeightRequest = 1,
				Color = Color.Silver
			};

			webView.Navigating += (object sender, EventArgs<Uri> e) =>
			{
				currentUrl = e.Value.AbsoluteUri;
				urlEntry.Text = currentUrl;
				System.Diagnostics.Debug.WriteLine("(!!!!!!!!!!!!!!!!!!!! Navigating " + e.Value.AbsoluteUri);

				Device.BeginInvokeOnMainThread(() =>
				{
					//_picker.Items.Clear();
					//foreach (var script in _scripts)
					//{
					//	_picker.Items.Add(script.Name);
					//}
				});
			};

			webView.LoadFinished += (object sender, EventArgs e) =>
			{
				ApplyScripts();
			};

		Content = new StackLayout
			{
				Padding = new Thickness(0,20,0,0),
				Orientation = StackOrientation.Vertical,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Children = {
					hr1,
					new StackLayout {
						Orientation = StackOrientation.Horizontal,
						HorizontalOptions = LayoutOptions.FillAndExpand,
						VerticalOptions = LayoutOptions.Start,
						Children = {
							urlEntry,
							urlButton
						}
					},
					hr3,
					new StackLayout {
						Orientation = StackOrientation.Horizontal,
						HorizontalOptions = LayoutOptions.FillAndExpand,
						VerticalOptions = LayoutOptions.Start,
						Children = {
							scriptButton,
							jsFiles
						}
					},
					hr2,
					webView
				}
			};



		}

		public async void GetScripts(bool apply)
		{
			Device.BeginInvokeOnMainThread(() =>
			{
				jsFiles.Text = "Loading...";
			});

			var uri = "http://wheelsapi.azurewebsites.net/api/script";

			HttpClient httpClient = new HttpClient();
			var task = await httpClient.GetAsync(uri);

			var jsonString = await task.Content.ReadAsStringAsync();
			scripts =  JsonConvert.DeserializeObject<List<Script>>(jsonString);

			if (apply)
			{
				ApplyScripts();
			}

			//var notificator = DependencyService.Get<IToastNotificator>();

			//var options = new NotificationOptions()
			//{
			//	Title = "Scripts Loaded",
			//	Description = string.Format("{0} JavaScript Files Loaded", scripts.Count)
			//};

			//var result = await notificator.Notify(options);

		}

		public void ApplyScripts()
		{
			List<Script> scriptsToApply = new List<Script>();
			string appliedScript = "Scripts Injected: ";

			foreach (var script in scripts)
			{

				foreach (var pageUrl in script.PageRelativeUrls)
				{
					string s = "^" + Regex.Escape(pageUrl).Replace("%", ".+").Replace("_", ".?") + "$";
					if (Regex.IsMatch(currentUrl, s, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase))
					{
						scriptsToApply.Add(script);
						break;
					};
				}
			}

			foreach (var script in scriptsToApply)
			{
				appliedScript += script.Name + ", ";
				Device.BeginInvokeOnMainThread(() =>
				{
					webView.InjectJavaScript(script.Source);
				});
			}

			if (scriptsToApply.Count == 0)
			{
				appliedScript += "None";
			}

			appliedScript = appliedScript.TrimEnd(' ', ',');

			Device.BeginInvokeOnMainThread(() =>
			{
				jsFiles.Text = appliedScript;
			});

		}
	}

	public class Script
	{
		public string Name { get; set; }

		public string[] PageRelativeUrls { get; set; }

		public string Source { get; set; }

	}

	public static class EnumerableExtensions
	{
		public static IEnumerable<T> MatchesWildcard<T>(this IEnumerable<T> sequence, Func<T, string> expression, string pattern)
		{
			var regEx = WildcardToRegex(pattern);

			return sequence.Where(item => Regex.IsMatch(expression(item), regEx));
		}

		public static string WildcardToRegex(string pattern)
		{
			return "^" + Regex.Escape(pattern).
			Replace("\\*", ".*").
			Replace("\\?", ".") + "$";
		}
	}
}

