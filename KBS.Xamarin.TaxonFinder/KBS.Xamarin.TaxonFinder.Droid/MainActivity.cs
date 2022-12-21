
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Plugin.CurrentActivity;
using Plugin.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KBS.App.TaxonFinder.Droid
{
    [Activity(Label = "BODENTIER hoch 4", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    //public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    public class MainActivity : global::NavigationSam.Droid.FormsAppCompatActivitySam
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            CrossCurrentActivity.Current.Init(this, bundle);
            Xamarin.Essentials.Platform.Init(this, bundle);
            global::Xamarin.Forms.Forms.Init(this, bundle);
            Xamarin.FormsMaps.Init(this, bundle);

            //NavigationSam specific
            NavigationSam.Droid.Preserver.Preserve();
            Xamarin.Forms.Forms.Init(this, bundle);
            //LoadApplication(new App());

            LoadApplication(new App());

        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        public override void OnBackPressed()
        {
            if (App.Current.MainPage.Navigation.NavigationStack.Count > 1)
            {
                try
                {
                    IReadOnlyList<Xamarin.Forms.Page> pageList = App.Current.MainPage.Navigation.NavigationStack;
                    List<Xamarin.Forms.Page> pageList2 = new List<Xamarin.Forms.Page>(pageList);
                    Xamarin.Forms.Page lastPage = pageList2.Last();
                    var dbg = lastPage.GetType().Name;

                    if (dbg == "FilterSelectionV2" || dbg == "FilterSelection")
                    {
                        base.OnBackPressed();
                    }
                    else
                    {
                        App.Current.MainPage.Navigation.PopAsync();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    App.Current.MainPage.Navigation.PopAsync();
                }
            }
            else
            {
                this.FinishAffinity();
            }
        }
    }
}

