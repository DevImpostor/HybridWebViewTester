using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using Plugin.Toasts;
using UIKit;
using UserNotifications;
using Xamarin.Forms;
using XLabs.Forms.Controls;

namespace WebViewTest.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init();

			DependencyService.Register<ToastNotification>(); // Register your dependency
			ToastNotification.Init();

			LoadApplication(new App());

			var hybridRenderer = new HybridWebViewRenderer();

			UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound, (granted, error) =>
			{
				// Do something if needed
			});

			var x = typeof(Xamarin.Forms.Themes.DarkThemeResources);
			x = typeof(Xamarin.Forms.Themes.iOS.UnderlineEffect);

			return base.FinishedLaunching(app, options);
		}
	}
}
