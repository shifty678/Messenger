using System;
using System.Data;
using System.Data.SqlClient;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.DrawerLayout.Widget;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Navigation;
using Google.Android.Material.Snackbar;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;
using AlertDialog = Android.App.AlertDialog;
using Android.Content;
using ListView = Android.Widget.ListView;

namespace Messenger
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.AppCompat.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        // string for azure database
        string SqlConnect = @"Data Source = messenger1.database.windows.net; Initial Catalog = Messenger; User ID = shifty678; Password=Stargate1;Connect Timeout = 30; Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            // On application start login method will be called

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);            
            Login(null, null);
            
        }

        public void Login(object sender, EventArgs e)
        {
            // on call content view will be set to the login layout and buttons enabled along with the calls to find view ids

            SetContentView(Resource.Layout.Login);

            Button Createaccount1 = FindViewById<Button>(Resource.Id.LoginCreate);
            Createaccount1.Click += Createaccount;

            var userloginview = FindViewById<TextView>(Resource.Id.UserLoginView);
            var userlogintext = FindViewById<EditText>(Resource.Id.UsernameLogin);

            var passloginview = FindViewById<TextView>(Resource.Id.PassLoginView);
            var passlogintext = FindViewById<EditText>(Resource.Id.PasswordLogin);

            var incorrect = FindViewById<TextView>(Resource.Id.Incorrect);

            // text changed code used to get the correct string of the input data for correct corrolation with the database

            userlogintext.TextChanged += (object sender, Android.Text.TextChangedEventArgs n) =>
            {
                userloginview.Text = n.Text.ToString();
            };

            passlogintext.TextChanged += (object sender, Android.Text.TextChangedEventArgs l) =>
            {
                passloginview.Text = l.Text.ToString();
            };

            //login button call + database connection 

            Button LoginButton = FindViewById<Button>(Resource.Id.Login_button);
            LoginButton.Click += (sender, e) =>
            {
                SqlConnection con = new SqlConnection(SqlConnect);

                // command for checking if username exists

                SqlCommand check_User_Namelogin = new SqlCommand("SELECT COUNT (*) FROM [Users] WHERE ([Username] = @user)", con);
                check_User_Namelogin.Parameters.AddWithValue("@user", userloginview.Text);
                con.Open();
                int UserExistlogin = (int)check_User_Namelogin.ExecuteScalar();


                if (UserExistlogin > 0)
                {
                    // if user exists checks wether password is correct in corellation with the username

                    SqlCommand checkpass = new SqlCommand("SELECT COUNT (*) FROM [Users] WHERE [Username] LIKE (@user) AND [Password] LIKE (@pass)", con);
                    checkpass.Parameters.AddWithValue("@user", userloginview.Text);
                    checkpass.Parameters.AddWithValue("@pass", passloginview.Text);
                    int passcorrect = (int)checkpass.ExecuteScalar();

                    // password correct, calls to home method, if not error message is visible

                    if (passcorrect > 0)
                    {

                        Home(null, null);
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
            // call to set content layout and findviewbyid calls

            SetContentView(Resource.Layout.Acc_Create);

            var nameview = FindViewById<TextView>(Resource.Id.nameView);
            var nametext = FindViewById<EditText>(Resource.Id.Nametext);

            var userview = FindViewById<TextView>(Resource.Id.usernameView);
            var usertext = FindViewById<EditText>(Resource.Id.usernameText);

            var passview = FindViewById<TextView>(Resource.Id.passwordView);
            var passtext = FindViewById<EditText>(Resource.Id.passwordText);
            var passbad = FindViewById<TextView>(Resource.Id.PassTextBad);

            var required = FindViewById<TextView>(Resource.Id.Requiredfield);

            // textchanged to ensure correct strings for database check

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

            // user create button click

            Button Account = FindViewById<Button>(Resource.Id.CreateAccount_Create_Button);
            Account.Click += (sender, e) =>
            {

                // new connection to database and check if user exists

                SqlConnection con = new SqlConnection(SqlConnect);
                SqlCommand check_User_Name = new SqlCommand("SELECT COUNT(*) FROM [Users] WHERE ([Username] = @user)", con);
                check_User_Name.Parameters.AddWithValue("@user", userview.Text);
                con.Open();
                int UserExist = (int)check_User_Name.ExecuteScalar();



                try
                {
                    // if user exists, error message

                    if (UserExist > 0)
                    {
                        passbad.Visibility = ViewStates.Visible;
                        con.Close();
                    }
                    else
                    {
                        // no existsing user found, adds user and password to database and calls to login method

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
                // catches errors

                catch(SqlException)
                {
                    required.Visibility = ViewStates.Visible;
                    con.Close();
                }
            };
        }

        public void Home(object sender, EventArgs e)
        {
            // sets content layout  and findviewbyids

            SetContentView(Resource.Layout.activity_main);

            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            // enables hamburger menu

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            // enables a default preview of users added

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);
            string[] items;
            ListView mainList;
            items = new string[] 
            {
            "NEW USERS WILL APPEAR HERE"
            };

            
            // Set our view from the "main" layout resource  

            mainList = FindViewById<ListView>(Resource.Id.listView1);
            mainList.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, items);


            // currently not in use
            //mainList.ItemClick += mainlist_ItemClick; 

        }
        // working code to enable user interaction with other users
        //void mainlist_ItemClick (object sender, AdapterView.ItemClickEventArgs e)
        //{
        //    SetContentView(Resource.Layout.Settings);

        //}

        public void Settings(object sender, EventArgs e)
        {
            // sets conetent layout and findviewbyid

            SetContentView(Resource.Layout.Settings);

            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.settings_toolbar);
            SetSupportActionBar(toolbar);
            toolbar.Title = "Settings";
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);


            var fontsmall = FindViewById<CheckBox>(Resource.Id.Smalltext);
            var fontmed = FindViewById<CheckBox>(Resource.Id.Medtext);
            var fontlar = FindViewById<CheckBox>(Resource.Id.Lartext);


            var fontblue = FindViewById<CheckBox>(Resource.Id.Fontblue);
            var fontgreen = FindViewById<CheckBox>(Resource.Id.Fontgreen);
            var fontorange = FindViewById<CheckBox>(Resource.Id.Fontorange);


            var appblue = FindViewById<CheckBox>(Resource.Id.Appblue);
            var appgreen = FindViewById<CheckBox>(Resource.Id.Appgreen);
            var apporange = FindViewById<CheckBox>(Resource.Id.Apporange);

            // defualt px size ... 29px


            //font Sizes

            fontsmall.Click += (o, e) =>
            {
                if (fontsmall.Checked == true)
                    fontsmall.SetTextSize(Android.Util.ComplexUnitType.Px,20);
                Toast.MakeText(this, "You selected Small!", ToastLength.Short).Show();

                if (fontsmall.Checked == false)
                    fontsmall.SetTextSize(Android.Util.ComplexUnitType.Px,29);
                Toast.MakeText(this, "You selected Default!", ToastLength.Short).Show();
            };


            fontmed.Click += (o, e) =>
            {
                if (fontmed.Checked == true)
                    fontmed.SetTextSize(Android.Util.ComplexUnitType.Px, 45);
                Toast.MakeText(this, "You selected Medium!", ToastLength.Short).Show();

                if (fontmed.Checked == false)
                    fontmed.SetTextSize(Android.Util.ComplexUnitType.Px, 29);
                Toast.MakeText(this, "You selected Default!", ToastLength.Short).Show();
            };
            
            
            fontlar.Click += (o, e) =>
            {
                if (fontlar.Checked == true)
                    fontlar.SetTextSize(Android.Util.ComplexUnitType.Px, 75);
                Toast.MakeText(this, "You selected Large!", ToastLength.Short).Show();

                if (fontlar.Checked == false)
                    fontlar.SetTextSize(Android.Util.ComplexUnitType.Px, 29);
                Toast.MakeText(this, "You selected Default!", ToastLength.Short).Show();
            };



            // Font Colours
            fontblue.Click += (o, e) =>
            {
                if (fontblue.Checked == true)
                    fontblue.SetTextColor(Android.Graphics.Color.Blue);
                Toast.MakeText(this, "You selected Blue!", ToastLength.Short).Show();
                
                if (fontblue.Checked == false)
                    fontblue.SetTextColor(Android.Graphics.Color.FloralWhite);
                    Toast.MakeText(this, "You selected Default!", ToastLength.Short).Show();
            };

            fontgreen.Click += (o, e) =>
            {
                if (fontgreen.Checked == true)
                    fontgreen.SetTextColor(Android.Graphics.Color.Green);
                Toast.MakeText(this, "You selected Green!", ToastLength.Short).Show();

                if (fontgreen.Checked == false)
                    fontgreen.SetTextColor(Android.Graphics.Color.FloralWhite);
                Toast.MakeText(this, "You selected Default!", ToastLength.Short).Show();
            };

            fontorange.Click += (o, e) =>
            {
                if (fontorange.Checked == true)
                    fontorange.SetTextColor(Android.Graphics.Color.Orange);
                Toast.MakeText(this, "You selected Orange!", ToastLength.Short).Show();

                if (fontorange.Checked == false)
                    fontorange.SetTextColor(Android.Graphics.Color.FloralWhite);
                Toast.MakeText(this, "You selected Default!", ToastLength.Short).Show();
            };

            // app background colours
            appblue.Click += (o, e) =>
            {
                if (appblue.Checked == true)
                    appblue.SetBackgroundColor(Android.Graphics.Color.Blue);
                Toast.MakeText(this, "You selected Orange!", ToastLength.Short).Show();

                if (appblue.Checked == false)
                    appblue.SetBackgroundColor(default);
                Toast.MakeText(this, "You selected Default!", ToastLength.Short).Show();
            };

            appgreen.Click += (o, e) =>
            {
                if (appgreen.Checked == true)
                    appgreen.SetBackgroundColor(Android.Graphics.Color.Green);
                Toast.MakeText(this, "You selected Orange!", ToastLength.Short).Show();

                if (appgreen.Checked == false)
                    appgreen.SetBackgroundColor(default);
                Toast.MakeText(this, "You selected Default!", ToastLength.Short).Show();
            };

            apporange.Click += (o, e) =>
            {
                if (apporange.Checked == true)
                    apporange.SetBackgroundColor(Android.Graphics.Color.Orange);
                Toast.MakeText(this, "You selected Orange!", ToastLength.Short).Show();

                if (apporange.Checked == false)
                    apporange.SetBackgroundColor(default);
                Toast.MakeText(this, "You selected Default!", ToastLength.Short).Show();
            };
        }
   
        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            // upon add button click on the home layout add new user dialog appears

            AlertDialog.Builder dialog = new AlertDialog.Builder(this);
            AlertDialog alert = dialog.Create();
            alert.SetTitle("Add User");
            
            LayoutInflater inflater = (LayoutInflater)this.GetSystemService(Context.LayoutInflaterService);
            View view = inflater.Inflate(Resource.Layout.layout1, null);
            alert.SetView(view);

            EditText editText_name = view.FindViewById<EditText>(Resource.Id.Usernameadd);

            alert.SetButton("OK", (c, ev) =>
            {
                // upon ok click opens database connection and checks user name exists
                SqlConnection con = new SqlConnection(SqlConnect);
                SqlCommand check_User_Name = new SqlCommand("SELECT COUNT(*) FROM [Users] WHERE ([Username] = @user)", con);
                check_User_Name.Parameters.AddWithValue("@user", editText_name.Text);
                con.Open();
                int UserExist = (int)check_User_Name.ExecuteScalar();


                if (UserExist > 0 )
                {
                    //if exists user added text appears
                    // no current connection between users is enabled
                    string name = editText_name.Text;
                    View view1 = (View)sender;
                    Snackbar.Make(view1, "User Added", Snackbar.LengthShort)
                        .SetAction("Action", (View.IOnClickListener)null).Show();
                }

                else
                {
                    // no user found text
                    View view2 = (View)sender;
                    Snackbar.Make(view2, "User Not Found", Snackbar.LengthShort)
                        .SetAction("ACtion", (View.IOnClickListener)null).Show();
                }

            });
            alert.SetButton2("CANCEL", (c, ev) => { });
            alert.Show();
        }

        public override void OnBackPressed()
        {
            // Android back press - unable to get back button to work with app - added text to stop app closing on press
            Toast.MakeText(this, "OUCH!", ToastLength.Short).Show();
        }

        // ignore
        //public override bool OnCreateOptionsMenu(IMenu menu)
        //{
        //    MenuInflater.Inflate(Resource.Menu.menu_main, menu);
        //    return true;
        //}

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            // return to home call in settings layout

            switch (item.ItemId)
            {
                case global::Android.Resource.Id.Home:
                    Home(null, null);
                    return true;

                default:
                    return  base.OnOptionsItemSelected(item);  
            }
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            // hamburger menu options - makes calls to methods upon click

            int id = item.ItemId;

            if (id == Resource.Id.nav_profile)
            {
                Toast.MakeText(this, "Not Implemented", ToastLength.Short).Show();
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
            //unknown

            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

    }
}

