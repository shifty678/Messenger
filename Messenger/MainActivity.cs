using System;
using System.Data;
using System.Data.SqlClient;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.View;
using AndroidX.DrawerLayout.Widget;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Navigation;
using Google.Android.Material.Snackbar;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace Messenger
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.AppCompat.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Login(null, null);

        }

        public void Login(object sender, EventArgs e)
        {

            SetContentView(Resource.Layout.Login);

            Button Createaccount1 = FindViewById<Button>(Resource.Id.LoginCreate);
            Createaccount1.Click += Createaccount;

            Button LoginButton = FindViewById<Button>(Resource.Id.Login_button);
            LoginButton.Click += Home;
        }

        public  void Createaccount(object sender, EventArgs even)
        {

            SetContentView(Resource.Layout.Acc_Create);

            Button Createaccount = FindViewById<Button>(Resource.Id.CreateAccount_Create_Button);
            Createaccount.Click += Login;

                
            var nameview = FindViewById<TextView>(Resource.Id.nameView);
            var nametext = FindViewById<EditText>(Resource.Id.Nametext);

            var userview = FindViewById<TextView>(Resource.Id.usernameView);
            var usertext = FindViewById<EditText>(Resource.Id.usernameText);

            var passview = FindViewById<TextView>(Resource.Id.passwordView);
            var passtext = FindViewById<EditText>(Resource.Id.passwordText);

            nametext.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) =>
            {
                nameview.Text = e.Text.ToString();
            };

            usertext.TextChanged += (object sender, Android.Text.TextChangedEventArgs t) =>
            {
                userview.Text = t.Text.ToString();
            };

            passtext.TextChanged += (object sender, Android.Text.TextChangedEventArgs f) =>
            {
                passview.Text = f.Text.ToString();
            };



            Button Account = FindViewById<Button>(Resource.Id.CreateAccount_Create_Button);
            Account.Click += (sender, e) =>
            {


                SqlConnection con =
                new SqlConnection(@"Data Source=messenger1.database.windows.net;Initial Catalog=Messenger;User ID=shifty678;Password=Stargate1;Connect Timeout=30;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = $@" INSERT INTO Users 
                ([Name],[Username],[Password])
                Values
                ('{nameview.Text}','{userview.Text}','{passview.Text}')";

                cmd.Connection = con;

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();


            };
        }


        public void Home(object sender, EventArgs e)
        {
            SetContentView(Resource.Layout.activity_main);

            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);
        }

        public void Settings(object sender, EventArgs e)
        {

            SetContentView(Resource.Layout.Settings);

            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.settings_toolbar);
            SetSupportActionBar(toolbar);

            toolbar.Title = "Settings";


        }
        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }
    


        public override void OnBackPressed()
        {

            //Toast.MakeText(this, "you touched me!", ToastLength.Short).Show();
            //base.OnBackPressed();
            //var intent = new Intent(this, typeof(MainActivity));
            //StartActivity(intent);
            //base.OnBackPressed();
            //DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            //if(drawer.IsDrawerOpen(GravityCompat.Start))
            //{          
            //var intent = new Intent(this, typeof(MainActivity))
            //.SetFlags(ActivityFlags.ReorderToFront);
            //StartActivity(intent);
            //    drawer.CloseDrawer(GravityCompat.Start);
            //}
            //else
            //{
            //    base.OnBackPressed();
            //}
        }

        // public override bool OnCreateOptionsMenu(IMenu menu)
        //{
        //  MenuInflater.Inflate(Resource.Menu.menu_main, menu);
        //return true;
        //}

        //public override bool OnOptionsItemSelected(IMenuItem item)
        //{
        //    int id = item.ItemId;
        //    if (id == Resource.Id.action_settings)
        //    {
        //        // SetContentView(Resource.Layout.Login);

        //    }

        //    return base.OnOptionsItemSelected(item);
        //}


        public bool OnNavigationItemSelected(IMenuItem item)
        {
            int id = item.ItemId;

            if (id == Resource.Id.nav_profile)
            {

            }
            else if (id == Resource.Id.nav_settings)
            {
                Settings(null, null);
            }
            else if (id == Resource.Id.nav_logout)
            {
                Login(null, null);
            }

            //DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            //drawer.CloseDrawer(GravityCompat.Start);
            return true;
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

    }
}

