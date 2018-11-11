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
using Microsoft.EntityFrameworkCore;

namespace FlashPointsAndroid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
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

            var qrButton = FindViewById<Button>(Resource.Id.scanQR);
            qrButton.Click += OnQRButtonClicked;

            var prizeButton = FindViewById<Button>(Resource.Id.prizeCatalog);
            prizeButton.Click += OnPrizeButtonClicked;

            var calendarButton = FindViewById<Button>(Resource.Id.eventCalendar);
            calendarButton.Click += OnCalendarButtonClicked;

            var requestEventButton = FindViewById<Button>(Resource.Id.requestEvent);
            requestEventButton.Click += OnRequestEventButtonClicked;

            email = Intent.GetStringExtra("email") ?? null;

            // Here we initialize our database connection.
            if (db == null)
            {
                db = new ApplicationDbContext();
            }
            
            
            if (email == null)
            {
                var login = new Intent(this, typeof(LoginActivity));
                this.StartActivity(login);
                Finish();
            } else
            {
                AddUserIfNotExists(email);
                
                var text = FindViewById<TextView>(Resource.Id.email);
                text.Text = email;

                var user = db.User.Where(u => u.Email == email).First();

                TextView points = FindViewById<TextView>(Resource.Id.pointValue);
                points.Text = user.Points.ToString();

            }
        }

        private void OnQRButtonClicked(object sender, EventArgs e)
        {
            var qr = new Intent(this, typeof(QRCodeScannerActivity));
            StartActivityForResult(qr, 0);
        }

        private void OnPrizeButtonClicked(object sender, EventArgs e)
        {
            var uri = Android.Net.Uri.Parse("https://flashpoints-web-app.azurewebsites.net/Prizes");
            var intent = new Intent(Intent.ActionView, uri);
            StartActivity(intent);
        }

        private void OnCalendarButtonClicked(object sender, EventArgs e)
        {
            var uri = Android.Net.Uri.Parse("https://flashpoints-web-app.azurewebsites.net/Events");
            var intent = new Intent(Intent.ActionView, uri);
            StartActivity(intent);
        }

        private void OnRequestEventButtonClicked(object sender, EventArgs e)
        {
            var uri = Android.Net.Uri.Parse("https://flashpoints-web-app.azurewebsites.net/Events/Create");
            var intent = new Intent(Intent.ActionView, uri);
            StartActivity(intent);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 0 && resultCode == Result.Ok)
            {
                string qr = data.GetStringExtra("qr");

                var ev = db.Event.Where(e => e.Title == qr).First();

                var user = db.User.Where(u => u.Email == email)
                    .Include(u => u.EventsAttended)
                    .First();

                var attended = user.EventsAttended.Where(e => e.EventID == ev.ID);

                string txt;

                if (attended.Count() == 0)
                {
                    user.Points += ev.PointValue;
                    user.EventsAttended.Add(new EventAttended
                    {
                        User = user,
                        Event = ev
                    });
                    db.SaveChanges();

                    TextView points = FindViewById<TextView>(Resource.Id.pointValue);
                    points.Text = user.Points.ToString();

                    txt = "Thanks for attending: " + ev.Title + ". You earned " + ev.PointValue + " points!";

                } else
                {
                    txt = "You already attended this event! You can't redeem an event twice.";
                }

                ToastLength duration = ToastLength.Long;
                Toast toast = Toast.MakeText(this, txt, duration);
                toast.Show();
            }
        }

        public override void OnBackPressed()
        {
                base.OnBackPressed();
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

