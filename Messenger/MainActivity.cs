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
        string SqlConnect = @"Data Source = messenger1.database.windows.net; Initial Catalog = Messenger; User ID = shifty678; Password=Stargate1;Connect Timeout = 30; Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

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

            var userloginview = FindViewById<TextView>(Resource.Id.UserLoginView);
            var userlogintext = FindViewById<EditText>(Resource.Id.UsernameLogin);

            var passloginview = FindViewById<TextView>(Resource.Id.PassLoginView);
            var passlogintext = FindViewById<EditText>(Resource.Id.PasswordLogin);

            var incorrect = FindViewById <TextView>(Resource.Id.Incorrect);

            userlogintext.TextChanged += (object sender, Android.Text.TextChangedEventArgs n) =>
            {
                userloginview.Text = n.Text.ToString();
            };

            passlogintext.TextChanged += (object sender, Android.Text.TextChangedEventArgs n) =>
            {
                passloginview.Text = n.Text.ToString();
            };

            Button LoginButton = FindViewById<Button>(Resource.Id.Login_button);
            LoginButton.Click += (sender, e) =>
            {

                SqlConnection con = new SqlConnection(SqlConnect);

                SqlCommand check_User_Namelogin = new SqlCommand("SELECT COUNT (*) FROM [Users] WHERE ([Username] = @user)", con);
                check_User_Namelogin.Parameters.AddWithValue("@user", userloginview.Text);
                con.Open();
                int UserExistlogin = (int)check_User_Namelogin.ExecuteScalar();


                if (UserExistlogin > 0)
                {

                    SqlCommand checkpass = new SqlCommand("SELECT COUNT (*) FROM [Users] WHERE [Username] LIKE (@user) AND [Password] LIKE (@pass)", con);
                    checkpass.Parameters.AddWithValue("@user", userloginview.Text);
                    checkpass.Parameters.AddWithValue("@pass", passloginview.Text);
                    int passcorrect = (int)checkpass.ExecuteScalar();


                    if (passcorrect > 0)
                    {
                        Home(null,null);
                    }
                    else
                    {
                        incorrect.Visibility = ViewStates.Visible;
                    }
                }

                else
                {
                    incorrect.Visibility = ViewStates.Visible;
                }
                con.Close();
            };


        }

        public  void Createaccount(object sender, EventArgs even)
        {

            SetContentView(Resource.Layout.Acc_Create);

            var nameview = FindViewById<TextView>(Resource.Id.nameView);
            var nametext = FindViewById<EditText>(Resource.Id.Nametext);

            var userview = FindViewById<TextView>(Resource.Id.usernameView);
            var usertext = FindViewById<EditText>(Resource.Id.usernameText);

            var passview = FindViewById<TextView>(Resource.Id.passwordView);
            var passtext = FindViewById<EditText>(Resource.Id.passwordText);
            var passbad = FindViewById<TextView>(Resource.Id.PassTextBad);

            var required = FindViewById<TextView>(Resource.Id.Requiredfield);

            nametext.TextChanged += (object sender, Android.Text.TextChangedEventArgs n) =>
            {
                nameview.Text = n.Text.ToString();
            };

            usertext.TextChanged += (object sender, Android.Text.TextChangedEventArgs u) =>
            {
                userview.Text = u.Text.ToString();
            };

            passtext.TextChanged += (object sender, Android.Text.TextChangedEventArgs p) =>
            {
                passview.Text = p.Text.ToString();
            };

            Button Account = FindViewById<Button>(Resource.Id.CreateAccount_Create_Button);
            Account.Click += (sender, e) =>
            {
                SqlConnection con = new SqlConnection(SqlConnect);
                SqlCommand check_User_Name = new SqlCommand("SELECT COUNT(*) FROM [Users] WHERE ([Username] = @user)", con);
                check_User_Name.Parameters.AddWithValue("@user", userview.Text);
                con.Open();
                int UserExist = (int)check_User_Name.ExecuteScalar();

                try
                {
                    if (UserExist > 0)
                    {
                        passbad.Visibility = ViewStates.Visible;
                        con.Close();
                    }
                    else
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = $@" INSERT INTO Users 
                    ([Name_],[Username],[Password])
                    Values
                    ('{nameview.Text}','{userview.Text}','{passview.Text}')";
                        cmd.Connection = con;
                        cmd.ExecuteNonQuery();
                        con.Close();
                        Account.Click += Login;
                    }
                }
                catch(SqlException)
                {
                    required.Visibility = ViewStates.Visible;
                    con.Close();
                }
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

