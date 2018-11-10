using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        string email;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            email = Intent.GetStringExtra("email") ?? null;

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
            
            if (email == null)
            {
                var login = new Intent(this, typeof(LoginActivity));
                this.StartActivity(login);
                Finish();
            } else
            {
                AddUserIfNotExists(email);

                NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
                navigationView.SetNavigationItemSelectedListener(this);

                var headerView = navigationView.GetHeaderView(0);
                var text = headerView.FindViewById<TextView>(Resource.Id.email);
                text.Text = email;

                var user = db.User.Where(u => u.Email == email).First();

                TextView points = FindViewById<TextView>(Resource.Id.pointValue);
                points.Text = user.Points.ToString();

            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 0 && resultCode == Result.Ok)
            {
                string qr = data.GetStringExtra("qr");
                var ev = db.Event.Where(e => e.Title == qr).First();

                var user = db.User.Where(u => u.Email == email).First();
                user.Points += ev.PointValue;
                db.SaveChanges();

                TextView points = FindViewById<TextView>(Resource.Id.pointValue);
                points.Text = user.Points.ToString();
                
                var text = "Thanks for attending: " + ev.Title + ". You earned " + ev.PointValue + " points!"; ;
                ToastLength duration = ToastLength.Long;

                Toast toast = Toast.MakeText(this, text, duration);
                toast.Show();
            }
        }

        public override void OnBackPressed()
        {
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            if(drawer.IsDrawerOpen(GravityCompat.Start))
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

            View view = (View) sender;
            Snackbar.Make(view, "point value is " + test.ToString(), Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            int id = item.ItemId;

            if (id == Resource.Id.nav_camera)
            {
                var qr = new Intent(this, typeof(QRCodeScannerActivity));
                StartActivityForResult(qr, 0);
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

        public void AddUserIfNotExists(string email)
        {
            var query = db.User.Where(e => e.Email == email);

            if (query.Count() == 0)
            {

                User newUser = new User();
                newUser.Email = email;
                newUser.PrizesRedeemed = new List<PrizeRedeemed>();
                newUser.EventsAttended = new List<EventAttended>();
                db.User.Add(newUser);
                db.SaveChanges();
            }
        }
    }
}

