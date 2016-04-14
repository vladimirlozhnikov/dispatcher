using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Dispatcher.Business;
using Dispatcher.Model;
using Dispatcher.Properties;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

namespace Dispatcher.UI.Forms
{
    public partial class PopupMapForm : Form
    {
        private readonly GMapOverlay _markersOverlay;

        public event EventHandler AddressSelected;

        public PopupMapForm()
        {
            InitializeComponent();

            // initialize map
            mapControl.SetPositionByKeywords("BY");
            mapControl.MapProvider = OpenStreetMapProvider.Instance;
            mapControl.Manager.Mode = AccessMode.ServerAndCache;

            _markersOverlay = new GMapOverlay("markers");
            mapControl.Overlays.Add(_markersOverlay);

            streetsListBox.DoubleClick += streetsListBox_DoubleClick;
        }

        void streetsListBox_DoubleClick(object sender, EventArgs e)
        {
            if (AddressSelected != null)
            {
                AddressSelected(streetsListBox.SelectedItem, null);
            }
        }

        private void PopupMapForm_Load(object sender, EventArgs e)
        {
            double lat;
            double lng;

            CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ".";

            if (!double.TryParse(Document.CurrentRegion.Position.Latitude, NumberStyles.Any, ci, out lat) || !double.TryParse(Document.CurrentRegion.Position.Longtitude, NumberStyles.Any, ci, out lng))
                return;

            PointLatLng latLng = new PointLatLng(lat, lng);
            mapControl.Position = latLng;
            mapControl.Zoom = Document.CurrentMapZoom;
        }

        public void ShowResults(results[] results)
        {
            streetsListBox.DataSource = results;
            streetsListBox.DisplayMember = "DisplayStreetName";
            streetsListBox.ValueMember = "Id";

            _markersOverlay.Clear();
            foreach (results r in results)
            {
                if (r.geometry != null && r.geometry.location != null)
                {
                    double lat;
                    double lng;

                    CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                    ci.NumberFormat.CurrencyDecimalSeparator = ".";

                    if (double.TryParse(r.geometry.location.lat, NumberStyles.Any, ci, out lat)
                        && double.TryParse(r.geometry.location.lng, NumberStyles.Any, ci, out lng))
                    {
                        GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(lat, lng),
                            new Bitmap(Resources.home_map_pin_gray));

                        _markersOverlay.Markers.Add(marker);
                    }
                }
            }
        }

        private void streetsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            results selectResults = streetsListBox.SelectedItem as results;
            if (selectResults != null)
            {
                if (selectResults.geometry != null && selectResults.geometry.location != null)
                {
                    double lat;
                    double lng;

                    CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                    ci.NumberFormat.CurrencyDecimalSeparator = ".";

                    if (double.TryParse(selectResults.geometry.location.lat, NumberStyles.Any, ci, out lat)
                        && double.TryParse(selectResults.geometry.location.lng, NumberStyles.Any, ci, out lng))
                    {
                        mapControl.Position = new PointLatLng(lat, lng);
                    }
                }
            }
        }
    }
}
