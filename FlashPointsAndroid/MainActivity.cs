using System;
using System.Linq;
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

namespace FlashPointsAndroid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        // This variable represents our Azure datbase access.
        public ApplicationDbContext db;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            
            // Here we initialize our database connection.
            if (db == null)
            {
                db = new ApplicationDbContext();
            }

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            Button b1 = FindViewById<Button>(Resource.Id.addTenPoints);
            b1.Click += ButtonOnClick;

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);

            // Check for an email string passed to this Activity.
            string email = Intent.GetStringExtra("email") ?? null;
            if (email == null)
            {
                // If email is null, the user is not logged in. Redirect them to the LoginActivity.
                var login = new Intent(this, typeof(LoginActivity));
                this.StartActivity(login);
                Finish();
            }
            else
            {
                // Otherwise, they're logged in, and we should display their email in the left menu.
                var headerView = navigationView.GetHeaderView(0);
                var text = headerView.FindViewById<TextView>(Resource.Id.email);
                text.Text = email;

                // Query the database for this user.
                var user = db.User.Where(u => u.Email == email);
                // If this user is not in the database, add them.
                if (user == null)
                {
                    var newUser = new User();
                    newUser.Email = email;
                    db.User.Add(newUser);
                    db.SaveChanges();
                }
            }
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
            var ev = db.Event.FirstOrDefault();
            int test = ev.PointValue;

            View view = (View)sender;
            Snackbar.Make(view, "point value is " + test.ToString(), Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }

        private void ButtonOnClick(object sender, EventArgs eventArgs)
        {
            View view = this.FindViewById(Android.Resource.Id.Content);
            Snackbar.Make(view, "hello", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            int id = item.ItemId;

            if (id == Resource.Id.nav_camera)
            {
                // QR code scanning code (or reference to it) goes here
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
    }
}

