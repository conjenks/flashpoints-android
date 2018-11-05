using System;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using FlashPoints.Data;
using FlashPoints.Models;
using Microsoft.WindowsAzure.MobileServices;

namespace FlashPointsAndroid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        // This variable represents our Azure datbase access.
        public ApplicationDbContext db;
        // Client reference.
        MobileServiceClient client;
        // Define an authenticated user.
        private MobileServiceUser user;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            client = new MobileServiceClient("https://flashpoints-mobile.azurewebsites.net");

            // Here we initialize our database connection.
            if (db == null)
            {
                db = new ApplicationDbContext();
            }

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);

            // Check for an email string passed to this Activity.
            //string email = Intent.GetStringExtra("email") ?? null;
            //if (email == null)
            //{
            //    // If email is null, the user is not logged in. Redirect them to the LoginActivity.
            //    //var login = new Intent(this, typeof(LoginActivity));
            //    //StartActivity(login);
            //}
            //else
            //{
            //    // Otherwise, they're logged in, and we should display their email in the left menu.
            //    var headerView = navigationView.GetHeaderView(0);
            //    var text = headerView.FindViewById<TextView>(Resource.Id.email);
            //    text.Text = email;

            //    // Query the database for this user.
            //    var user = db.User.Where(u => u.Email == email);
            //    // If this user is not in the database, add them.
            //    if (user.First() == null)
            //    {
            //        var newUser = new User();
            //        newUser.Email = email;
            //        db.User.Add(newUser);
            //        db.SaveChanges();
            //    }

            //    var points = FindViewById<TextView>(Resource.Id.pointBalance);
            //    points.Text = user.First().Points.ToString();
            //}
        }

        public override void OnBackPressed()
        {
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            if (drawer.IsDrawerOpen(GravityCompat.Start))
            {
                drawer.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                base.OnBackPressed();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {

            // Testing a databse call. These two lines set int test equal to the PointValue of the first Event in the database.
            //var ev = db.Event.FirstOrDefault();
            //int test = ev.PointValue;

            //View view = (View)sender;
            //Snackbar.Make(view, "point value is " + test.ToString(), Snackbar.LengthLong)
            //    .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();

            var qr = new Intent(this, typeof(QRCodeScannerActivity));
            StartActivity(qr);
            Finish();
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            int id = item.ItemId;

            if (id == Resource.Id.nav_camera)
            {
                var qr = new Intent(this, typeof(QRCodeScannerActivity));
                StartActivity(qr);
                Finish();
            }
            else if (id == Resource.Id.nav_gallery)
            {
                // Prize gallery code (or reference to it) goes here
            }
            else if (id == Resource.Id.nav_manage)
            {
                // Tools (?) code (or reference to it) goes here
            }
            else if (id == Resource.Id.nav_share)
            {
                // "Share" code (or reference to it) goes here
            }

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawer.CloseDrawer(GravityCompat.Start);
            return true;
        }

        
        private async Task<bool> Authenticate()
        {
            var success = false;
            try
            {
                // Sign in with Azure AD using a server-managed flow.
                user = await client.LoginAsync(this, MobileServiceAuthenticationProvider.WindowsAzureActiveDirectory, "flashpointsmobile");

                var token = user.MobileServiceAuthenticationToken;

                CreateAndShowDialog(string.Format("you are now logged in - {0}",
                    user.UserId), "Logged in!");

                success = true;
            }
            catch (Exception ex)
            {
                CreateAndShowDialog(ex, "Authentication failed");
            }
            return success;
        }

        [Java.Interop.Export()]
        public async void LoginUser(View view)
        {
            // Load data only after authentication succeeds.
            if (await Authenticate())
            {
                //Hide the button after authentication succeeds.
                FindViewById<Button>(Resource.Id.buttonLoginUser).Visibility = ViewStates.Gone;
            }
        }

        void CreateAndShowDialog(Exception exception, string title)
        {
            CreateAndShowDialog(exception.Message, title);
        }

        void CreateAndShowDialog(string message, string title)
        {
            var builder = new Android.App.AlertDialog.Builder(this);

            builder.SetMessage(message);
            builder.SetTitle(title);
            builder.Create().Show();
        }
    }
}

