using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dispatcher.Business;
using Dispatcher.Model;
using Dispatcher.Properties;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using Newtonsoft.Json;
using Color = System.Drawing.Color;
using Region = Dispatcher.Model.Region;

namespace Dispatcher.UI.Forms
{
    public partial class MainForm : Form
    {
        private readonly GMapOverlay _markersOverlay;
        private readonly GMapOverlay _cityOverlay;
        private readonly JobsQuery _jobsQuery = new JobsQuery(1000);
        private Profile SelectedProfile { get; set; }

        public MainForm()
        {
            _cityAction = CityAction.NoCityAction;
            InitializeComponent();

            // initialize main map
            mainMap.SetPositionByKeywords("BY");
            mainMap.MapProvider = OpenStreetMapProvider.Instance;
            mainMap.DragButton = MouseButtons.Left;
            mainMap.Manager.Mode = AccessMode.ServerAndCache;
            mainMap.Zoom = Document.CurrentMapZoom;
            mainMap.OnMarkerClick += MainMapOnOnMarkerClick;

            _markersOverlay = new GMapOverlay("markers");
            mainMap.Overlays.Add(_markersOverlay);

            // initialize city map
            cityMap.SetPositionByKeywords("BY");
            cityMap.MapProvider = OpenStreetMapProvider.Instance;
            cityMap.DragButton = MouseButtons.Left;
            cityMap.Manager.Mode = AccessMode.ServerAndCache;
            cityMap.Zoom = Document.CurrentMapZoom;
            cityMap.MouseClick += CityMapOnMouseClick;

            _cityOverlay = new GMapOverlay("markers");
            cityMap.Overlays.Add(_cityOverlay);

            ordersGridView.MouseClick += OrdersGridViewOnMouseClick;
            ordersGridView.MouseDoubleClick += OrdersGridViewOnMouseDoubleClick;
            ordersGridView.ClearSelection();

            carsGridView.MouseClick += CarsGridViewOnMouseClick;
            carsGridView.MouseDoubleClick += CarsGridViewOnMouseDoubleClick;

            driversGridView.MouseClick += DriversGridViewOnMouseClick;
            driversGridView.MouseDoubleClick += DriversGridViewOnMouseDoubleClick;

            complainDataGridView.CellContentClick += ComplainDataGridViewOnCellContentClick;

            FormClosing += OnClosed;
        }

        private void OnClosed(object sender, EventArgs eventArgs)
        {
            GC.Collect();
            _testUpdate.Stop();
            GC.Collect();
            _jobsQuery.Stop();
            GC.Collect();
            _testDriversJobsQuery.Stop();
            GC.Collect();
            _testCustomersJobsQuery.Stop();
            GC.Collect();
        }

        private void SetMapRegion(Region region)
        {
            double lat;
            double lng;

            CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ".";

            if (!double.TryParse(region.Position.Latitude, NumberStyles.Any, ci, out lat) || !double.TryParse(region.Position.Longtitude, NumberStyles.Any, ci, out lng))
                return;

            // update position
            Task<HttpResponseMessage> task = Document.HttpManager.UpdatePosition(Token, region.Position);
            task.ContinueWith(
                t =>
                {
                    Invoke((MethodInvoker)(Utils.HideWaitingForm));

                    if (t.Status == TaskStatus.RanToCompletion)
                    {
                        HttpResponseMessage response = t.Result;
                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            MessageBox.Show(String.Format("Сервер вернул ошибку: {0}", response.StatusCode));
                            return;
                        }

                        string result = response.Content.ReadAsStringAsync().Result;
                        TypedResponse typedRestore = JsonConvert.DeserializeObject<TypedResponse>(result);

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            if (typedRestore.ErrorCode == ErrorCodeEnum.Ok)
                            {
                            }
                            else
                            {
                                MessageBox.Show(typedRestore.ErrorMessage);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Произошла ошибка на сервере. Попробуй еще раз или обратитесь за помощью к разработчикам.");
                    }
                });
            

            PointLatLng latLng = new PointLatLng(lat, lng);
            mainMap.Position = latLng;
        }

        private void OrdersBinding(List<Order> orders)
        {
            if (orders == null)
                return;

            int selected = -1;
            if (ordersGridView.SelectedRows.Count > 0)
            {
                selected = ordersGridView.SelectedRows[0].Index;
            }

            ordersGridView.ClearSelection();
            List<Order> resultOrders = new List<Order>();
            List<Order> sortedOrders = orders.OrderByDescending(o => o.Id).ToList();

            if (inProgressOblyCheckBox.Checked)
            {
                // показывать только активные заказы
                resultOrders = sortedOrders.Where(o =>
                    (o.Status == ServiceType.Submitted || o.Status == ServiceType.Agree || o.Status == ServiceType.Arrived || o.Status == ServiceType.InProgress || o.Status == ServiceType.Reserved))
                    .ToList();
            }
            else
            {
                resultOrders.AddRange(sortedOrders);
            }

            // если у заказов есть водители, то назначить их
            foreach (Order order in resultOrders)
            {
                if (order.Taxist != null)
                {
                    Profile taxist = Document.Profiles.FirstOrDefault(p1 => p1.Id == order.Taxist.Id);
                    order.Taxist = taxist;
                }
            }

            // заполнить таблицу с заказами
            ordersGridView.Rows.Clear();
            foreach (Order order in resultOrders)
            {
                string status = "Не Определен";
                switch (order.Status)
                {
                    case ServiceType.Submitted:
                        status = "Создан Клиентом";
                        break;
                    case ServiceType.Agree:
                        status = "Одобрен Клиентом (машина в пути)";
                        break;
                    case ServiceType.Arrived:
                        status = "Машина Прибыла";
                        break;
                    case ServiceType.Canceled:
                        status = "Заказ Отменен";
                        break;
                    case ServiceType.Completed:
                        status = "Заказ Завершен";
                        break;
                    case ServiceType.InProgress:
                        status = "Машина в пути";
                        break;
                    case ServiceType.Rejected:
                        status = "Заказ Отклонен";
                        break;
                    case ServiceType.Reserved:
                        status = "Заказ Зарезервирован";
                        break;
                }

                TimeSpan ts = new TimeSpan(0);
                if (order.StatusCreatedDate != null)
                    ts = (TimeSpan) (order.StatusCreatedDate - order.CreatedDate);

                string command = string.Empty;
                switch (order.Status)
                {
                    case ServiceType.Reserved:
                        command = "Подтвердить или отменить заказ";
                        // автоматически подтвердить заказ через 5 секунд, если заказ был создан диспетчером
                        /*
                        Job j = _jobsQuery.SingleShot();
                        j.Name = "Automatic Approve Order Job";
                        j.IntervalInMillis = 5000;
                        j.Ready = true;
                        var order1 = order;
                        j.FuncEvent += job =>
                        {
                            Invoke((MethodInvoker) (() => ApproveOrder(order1, false)));
                            return true;
                        };*/
                        break;
                    case ServiceType.Agree:
                        command = "Отменить заказ";
                        break;
                }

                order.TotalSeconds = ts.TotalSeconds;
                object[] displayRow = { order.Id,
                                        order.Customer.Phone,
                                        order.From.Description,
                                        order.To != null ? order.To.Description : "-",
                                        order.Taxist != null ? order.Taxist.Phone : "-",
                                        string.Format("{0}{1}{2}", status, Environment.NewLine, command),
                                        order.CreatedDate.ToString("F"),
                                        order.StatusCreatedDate != null ?
                                        string.Format("{0:00}:{1:00}:{2:00}", order.TotalSeconds / 3600, (order.TotalSeconds / 60) % 60, order.TotalSeconds % 60) : "-",
                                        string.Format(order.Cost != null ? String.Format("{0:C}", order.Cost) : "-" )};
                ordersGridView.Rows.Add(displayRow);
            }

            foreach (DataGridViewRow row in ordersGridView.Rows)
            {
                if (row.Cells["OrderId"].Value == null)
                {
                    continue;
                }

                int orderId = (int)row.Cells["OrderId"].Value;
                Order order = Document.Orders.FirstOrDefault(o => o.Id == orderId);
                if (order == null)
                {
                    continue;
                }

                switch (order.Status)
                {
                    case ServiceType.Canceled:
                        row.DefaultCellStyle.BackColor = Color.LightSalmon;
                        break;
                    case ServiceType.Completed:
                        row.DefaultCellStyle.BackColor = Color.LightBlue;
                        break;
                    case ServiceType.InProgress:
                        row.DefaultCellStyle.BackColor = Color.Aqua;
                        break;
                    case ServiceType.Rejected:
                        row.DefaultCellStyle.BackColor = Color.LightSalmon;
                        break;
                    case ServiceType.Reserved:
                        row.DefaultCellStyle.BackColor = Color.LightSkyBlue;
                        break;
                }
            }

            if (selected != -1 && selected < ordersGridView.Rows.Count)
            {
                ordersGridView.Rows[selected].Selected = true;
            }
        }

        private void OrdersGridViewOnMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hti = ordersGridView.HitTest(e.X, e.Y);
                if (hti.RowIndex == -1)
                    return;

                ordersGridView.ClearSelection();
                ordersGridView.Rows[hti.RowIndex].Selected = true;

                // get selected order
                int orderId = (int)ordersGridView.Rows[hti.RowIndex].Cells["OrderId"].Value;
                Order order = Document.Orders.FirstOrDefault(o => o.Id == orderId);

                if (order == null)
                    return;

                ContextMenu m = new ContextMenu();
                switch (order.Status)
                {
                    case ServiceType.Reserved:
                        MenuItem mi1 = new MenuItem("Подтвердить Заказ");
                        MenuItem mi2 = new MenuItem("Отменить Заказ");
                        mi1.Click += (o, args) => { ApproveOrder(order); };
                        mi2.Click += (o, args) => { CancelOrder(order); };
                        m.MenuItems.Add(mi1);
                        m.MenuItems.Add(mi2);
                        break;
                    case ServiceType.Agree:
                        MenuItem mi3 = new MenuItem("Отменить заказ");
                        mi3.Click += (o, args) => { CancelOrder(order); };
                        m.MenuItems.Add(mi3);
                        break;
                }

                MenuItem mi4 = new MenuItem("История заказа");
                mi4.Click += (o, args) => { ShowOrderHistory(order); };
                m.MenuItems.Add(mi4);

                m.Show(ordersGridView, new Point(e.X, e.Y));
            }
        }

        private void OrdersGridViewOnMouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (Document.Orders == null)
                return;

            var hti = ordersGridView.HitTest(e.X, e.Y);
            if (hti.RowIndex == -1)
                return;

            ordersGridView.ClearSelection();
            ordersGridView.Rows[hti.RowIndex].Selected = true;

            // get selected order
            int orderId = (int)ordersGridView.Rows[hti.RowIndex].Cells["OrderId"].Value;
            Order order = Document.Orders.FirstOrDefault(o => o.Id == orderId);

            if (order == null)
                return;

            ShowOrderHistory(order);
        }

        private void CarsGridViewOnMouseClick(object sender, MouseEventArgs e)
        {
            var hti = carsGridView.HitTest(e.X, e.Y);
            if (hti.RowIndex == -1)
                return;
            
            carsGridView.ClearSelection();
            carsGridView.Rows[hti.RowIndex].Selected = true;

            if (carsGridView.Rows[hti.RowIndex].Cells["TaxistId"].Value == null)
                return;

            // get selected profile
            int profileId = (int)carsGridView.Rows[hti.RowIndex].Cells["TaxistId"].Value;
            Profile profile = Document.Profiles.FirstOrDefault(p => p.Id == profileId);

            if (profile == null)
                return;

            SelectedProfile = profile;
            ShowProfileOnMap(profile);

            if (e.Button == MouseButtons.Right)
            {
                ContextMenu m = new ContextMenu();
                MenuItem mi = new MenuItem("Показать детали");
                mi.Click += (o, args) => { ShowTaxistDetails(profile); };
                m.MenuItems.Add(mi);

                MenuItem mi2 = new MenuItem("Показать последний заказ");
                Order order = Document.Orders.FirstOrDefault(o1 => (o1.Taxist != null && o1.Taxist.Id == profile.Id));
                mi2.Click += (o, args) => { ShowOrderHistory(order); };
                m.MenuItems.Add(mi2);

                m.Show(carsGridView, new Point(e.X, e.Y));
            }
        }

        private void CarsGridViewOnMouseDoubleClick(object sender, MouseEventArgs e)
        {
            var hti = carsGridView.HitTest(e.X, e.Y);
            if (hti.RowIndex == -1)
                return;

            carsGridView.ClearSelection();
            carsGridView.Rows[hti.RowIndex].Selected = true;

            // get selected profile
            int profileId = (int)carsGridView.Rows[hti.RowIndex].Cells["TaxistId"].Value;
            Profile profile = Document.Profiles.FirstOrDefault(p => p.Id == profileId);

            if (profile == null)
                return;

            ShowProfileOnMap(profile);
            ShowTaxistDetails(profile);
        }

        private void ShowTaxistDetails(Profile profile, bool edit = false)
        {
            TaxistDetails td = new TaxistDetails(profile, Token, edit);
            td.ShowDialog(this);
        }

        private void MainMapOnOnMarkerClick(GMapMarker item, MouseEventArgs mouseEventArgs)
        {
            Profile profile = item.Tag as Profile;
            if (profile == null)
                return;

            SelectedProfile = profile;
            ShowProfileOnMap(SelectedProfile);
            foreach (DataGridViewRow row in carsGridView.Rows)
            {
                if (profile.Id == (int) row.Cells["TaxistId"].Value)
                {
                    row.Selected = true;
                    break;
                }
            }
        }

        private void ShowOrderHistory(Order order)
        {
            if (order == null)
                return;

            Utils.ShowWaitingForm("Ожидайте окончания запроса...");

            Task<HttpResponseMessage> task = Document.HttpManager.GetOrderHistory(Token, order);
            task.ContinueWith(
                t =>
                {
                    Invoke((MethodInvoker)(Utils.HideWaitingForm));

                    if (t.Status == TaskStatus.RanToCompletion)
                    {
                        HttpResponseMessage response = t.Result;
                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            MessageBox.Show(String.Format("Сервер вернул ошибку: {0}", response.StatusCode));
                            return;
                        }

                        string result = response.Content.ReadAsStringAsync().Result;
                        TypedResponse typedRestore = JsonConvert.DeserializeObject<TypedResponse>(result);

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            if (typedRestore.ErrorCode == ErrorCodeEnum.Ok)
                            {
                                OrderHistoreResponse data = typedRestore.ParsedData<OrderHistoreResponse>();
                                Invoke((MethodInvoker) (() =>
                                {
                                    OdrerHistoryForm form = new OdrerHistoryForm(order, data.Collectors, data.Statuses);
                                    form.ShowDialog(this);
                                }));
                            }
                            else
                            {
                                MessageBox.Show(typedRestore.ErrorMessage);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Произошла ошибка на сервере. Попробуй еще раз или обратитесь за помощью к разработчикам.");
                    }
                });
        }

        private void ApproveOrder(Order order, bool ask = true)
        {
            DialogResult dialogResult = DialogResult.Yes;
            if (ask)
            {
                dialogResult = MessageBox.Show("Вы уверены, что хотите подтвердить заказ?", "", MessageBoxButtons.YesNoCancel);
            }

            if (dialogResult == DialogResult.Yes)
            {
                Utils.ShowWaitingForm("Ожидайте окончания запроса...");

                Token token = new Token { Ticket = order.Customer.Token };

                Task<HttpResponseMessage> task = Document.HttpManager.ApproveOrder(token);
                task.ContinueWith(
                    t =>
                    {
                        Invoke((MethodInvoker)(Utils.HideWaitingForm));

                        if (ask)
                        {
                            if (t.Status == TaskStatus.RanToCompletion)
                            {
                                HttpResponseMessage response = t.Result;
                                if (response.StatusCode != HttpStatusCode.OK)
                                {
                                    MessageBox.Show(String.Format("Сервер вернул ошибку: {0}", response.StatusCode));
                                    return;
                                }

                                string result = response.Content.ReadAsStringAsync().Result;
                                TypedResponse typedRestore = JsonConvert.DeserializeObject<TypedResponse>(result);

                                if (response.StatusCode == HttpStatusCode.OK)
                                {
                                    if (typedRestore.ErrorCode == ErrorCodeEnum.Ok)
                                    {
                                    }
                                    else
                                    {
                                        MessageBox.Show(typedRestore.ErrorMessage);
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Произошла ошибка на сервере. Попробуй еще раз или обратитесь за помощью к разработчикам.");
                            }
                        }
                        else
                        {
                            Invoke((MethodInvoker)(() => refreshOrdersButton_Click(null, null)));
                        }
                    });
            }
        }

        private void CancelOrder(Order order)
        {
            DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите отменить заказ?", "", MessageBoxButtons.YesNoCancel);
            if (dialogResult == DialogResult.Yes)
            {
                Utils.ShowWaitingForm("Ожидайте окончания запроса...");

                Token token = new Token { Ticket = order.Customer.Token };

                Task<HttpResponseMessage> task = Document.HttpManager.CancelOrder(token);
                task.ContinueWith(
                    t =>
                    {
                        Invoke((MethodInvoker)(Utils.HideWaitingForm));

                        if (t.Status == TaskStatus.RanToCompletion)
                        {
                            HttpResponseMessage response = t.Result;
                            if (response.StatusCode != HttpStatusCode.OK)
                            {
                                MessageBox.Show(String.Format("Сервер вернул ошибку: {0}", response.StatusCode));
                                return;
                            }

                            string result = response.Content.ReadAsStringAsync().Result;
                            TypedResponse typedRestore = JsonConvert.DeserializeObject<TypedResponse>(result);

                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                if (typedRestore.ErrorCode == ErrorCodeEnum.Ok)
                                {
                                }
                                else
                                {
                                    MessageBox.Show(typedRestore.ErrorMessage);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Произошла ошибка на сервере. Попробуй еще раз или обратитесь за помощью к разработчикам.");
                        }
                    });
            }
        }

        private void MainCarsBinding(List<Profile> profiles)
        {
            // показывать только водителей онлайн
            List<Profile> verifiedProfiles = profiles.Where(profile => (profile.Active && (profile.Status != null && profile.Status != (int)ServiceType.Offline && profile.ServiceType == ServiceType.Taxists))).ToList();

            carsGridView.Rows.Clear();
            foreach (Profile profile in verifiedProfiles)
            {
                string status = "Не Определен";
                if (profile.Status != null)
                {
                    if (profile.Status == (int) ServiceType.Offline)
                        status = "Вне сети";
                    else if (profile.Status == (int)ServiceType.Free)
                        status = "Свободен";
                    else if (profile.Status == (int)ServiceType.Busy)
                        status = "Занят";
                }

                // найти текущие заказы этого профиля
                if (Document.Orders != null)
                {
                    foreach (Order order in Document.Orders)
                    {
                        if (order.Taxist == null)
                            continue;

                        if (order.Taxist.Id == profile.Id &&
                            (order.Status == ServiceType.Agree || order.Status == ServiceType.Arrived ||
                             order.Status == ServiceType.InProgress))
                        {
                            status = "В пути";
                        }
                    }
                }

                object[] displayRow = { profile.Id,
                                        profile.DisplayName,
                                        profile.Phone,
                                        profile.Car != null ? profile.Car.Number : "-",
                                        status };
                carsGridView.Rows.Add(displayRow);
            }
        }

        private void ShowMarkers(List<Profile> profiles)
        {
            if (_markersOverlay.Markers == null)
                return;

            CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ".";

            List<Profile> verifiedProfiles = profiles.Where(profile => (profile.Active && profile.Status != null && profile.Status != (int)ServiceType.Offline)).ToList();

            _markersOverlay.Markers.Clear();
            foreach (Profile p in verifiedProfiles)
            {
                double lat;
                double lng;

                if (p.Position == null)
                    continue;

                if (!double.TryParse(p.Position.Latitude, NumberStyles.Any, ci, out lat) || !double.TryParse(p.Position.Longtitude, NumberStyles.Any, ci, out lng))
                    continue;

                Bitmap b = null;
                if (p.ServiceType == ServiceType.Customer)
                {
                    //b = new Bitmap(Resources.person_marker);
                }
                else
                {
                    b = new Bitmap(p.Status == (int) ServiceType.Free ? Resources.marker_free : Resources.marker_busy);
                    GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(lat, lng), b) {Tag = p};
                    _markersOverlay.Markers.Add(marker);
                }
            }

            // центрировать по выбранному профилю
            ShowProfileOnMap(SelectedProfile);
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
        }

        private void CreateOrderButtonClick(object sender, EventArgs e)
        {
            NewOrderForm newOrderForm = new NewOrderForm();
            if (newOrderForm.ShowDialog(this) == DialogResult.Yes)
            {
                string name = newOrderForm.CustomerName;
                string phone = newOrderForm.Phone;
                Address from = newOrderForm.From;
                Address to = newOrderForm.To;
                Profile taxist = newOrderForm.SelectedDriver;

                Utils.ShowWaitingForm("Ожидайте окончания операции");

                Task<HttpResponseMessage> task = Document.HttpManager.CreateDispatcherOrder(Token, name, phone, from, to, taxist);
                task.ContinueWith(
                    t =>
                    {
                        Invoke((MethodInvoker)(Utils.HideWaitingForm));

                        if (t.Status == TaskStatus.RanToCompletion)
                        {
                            HttpResponseMessage response = t.Result;
                            if (response.StatusCode != HttpStatusCode.OK)
                            {
                                MessageBox.Show(String.Format("Сервер вернул ошибку: {0}", response.StatusCode));
                                return;
                            }

                            string result = response.Content.ReadAsStringAsync().Result;
                            TypedResponse typedRestore = JsonConvert.DeserializeObject<TypedResponse>(result);

                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                if (typedRestore.ErrorCode == ErrorCodeEnum.Ok)
                                {
                                }
                                else
                                {
                                    MessageBox.Show(typedRestore.ErrorMessage);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Произошла ошибка на сервере. Попробуй еще раз или обратитесь за помощью к разработчикам.");
                        }
                    });
            }
            newOrderForm.Dispose();
        }

        private void refreshOrdersButton_Click(object sender, EventArgs e)
        {
            Utils.ShowWaitingForm("Ожидайте окончания запроса...");

            Task<HttpResponseMessage> task = Document.HttpManager.GetDispatcherOrders(Token);
            task.ContinueWith(
                t =>
                {
                    Invoke((MethodInvoker)(Utils.HideWaitingForm));

                    if (t.Status == TaskStatus.RanToCompletion)
                    {
                        HttpResponseMessage response = t.Result;
                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            MessageBox.Show(String.Format("Сервер вернул ошибку: {0}", response.StatusCode));
                            return;
                        }

                        string result = response.Content.ReadAsStringAsync().Result;
                        TypedResponse typedRestore = JsonConvert.DeserializeObject<TypedResponse>(result);

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            if (typedRestore.ErrorCode == ErrorCodeEnum.Ok)
                            {
                                List<Order> data = typedRestore.ParsedData<List<Order>>();
                                Document.Orders = data;
                                Invoke((MethodInvoker) (() => OrdersBinding(data)));
                            }
                            else
                            {
                                MessageBox.Show(typedRestore.ErrorMessage);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Произошла ошибка на сервере. Попробуй еще раз или обратитесь за помощью к разработчикам.");
                    }
                });
        }

        private void historyButton_Click(object sender, EventArgs e)
        {
            OrdersHistoryFromToForm historyForm = new OrdersHistoryFromToForm(Token);
            historyForm.ShowDialog();
            historyForm.Dispose();
        }

        private void ShowProfileOnMap(Profile profile)
        {
            if (profile == null)
                return;

            if (profile.Position == null)
                return;

            // show marker in center of map
            double lat;
            double lng;

            CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ".";

            if (!double.TryParse(profile.Position.Latitude, NumberStyles.Any, ci, out lat) || !double.TryParse(profile.Position.Longtitude, NumberStyles.Any, ci, out lng))
                return;

            // выделить профиль в таблице с машинами
            foreach (DataGridViewRow row in carsGridView.Rows)
            {
                if (row.Cells["TaxistId"].Value == null)
                    continue;

                int profileId = (int)row.Cells["TaxistId"].Value;
                if (profileId == profile.Id)
                {
                    row.Selected = true;
                    break;
                }
            }

            PointLatLng latLng = new PointLatLng(lat, lng);
            mainMap.Position = latLng;
        }

        private void InitJobs()
        {
#if (DEBUG_LOCAL_DB)
            Job j1 = _jobsQuery.AddJob();
            j1.Name = "ClearOrders Job";
            j1.IntervalInMillis = 1000*60; // задача вызывается раз в минуту
            j1.Ready = true;
            j1.FuncEvent += job =>
            {
                Task<HttpResponseMessage> task = Document.HttpManager.ClearOrders(Token);
                task.ContinueWith(
                    t =>
                    {
                    });
                return true;
            };

            Job j2 = _jobsQuery.AddJob();
            j2.Name = "ForceDriversOffline Job";
            j2.IntervalInMillis = 1000 * 60 * 15; // задача вызывается раз в 15 минут
            j2.Ready = true;
            j2.FuncEvent += job =>
            {
                Task<HttpResponseMessage> task = Document.HttpManager.ForceDriversOffline();
                task.ContinueWith(
                    t =>
                    {
                    });
                return true;
            };
#endif

            // обновлять время в заказах каждую секунду
            Job j3 = _jobsQuery.AddJob();
            j3.Name = "Update Orders Time";
            j3.IntervalInMillis = 1000;
            j3.Ready = true;
            j3.FuncEvent += job =>
            {
                Invoke((MethodInvoker)(() => UpdateOrdersTime(1)));
                return true;
            };

            // обновлять список заказов каждые 30 секунд
            Job j4 = _jobsQuery.AddJob();
            j4.Name = "_orders_Timer_Tick";
            j4.IntervalInMillis = (long) (1000*refreshOrdersUpDown.Value);
            j4.Ready = true;
            j4.FuncEvent += job => _orders_Timer_Tick();

            // обновлять список машин каждые 60 секунд
            Job j5 = _jobsQuery.AddJob();
            j5.Name = "_drivers_Timer_Tick";
            j5.IntervalInMillis = (long)(1000 * refreshCarsUpDown.Value);
            j5.Ready = true;
            j5.FuncEvent += job => _drivers_Timer_Tick();

            // обновлять список жалоб каждые 5 минут
            //Job j6 = _jobsQuery.AddJob();
            //j6.Name = "Refresh Complains";
            //j6.IntervalInMillis = 1000 * 50 * 5;
            //j6.Ready = true;
            //j6.FuncEvent += job =>
            //{
            //    Invoke((MethodInvoker)(() => refreshComplainsButton_Click(null, null)));
            //    return true;
            //};
        }

        private bool UpdateOrdersTime(int seconds)
        {
            if (Document.Orders == null)
                return true;

            if (_inProgress)
                return true;

            foreach (DataGridViewRow row in ordersGridView.Rows)
            {
                DataGridViewCell cell = row.Cells["TotalTime"];

                if (row.Cells["OrderId"].Value == null)
                    continue;

                // получаем заказ
                int orderId = (int)row.Cells["OrderId"].Value;
                Order order = Document.Orders.FirstOrDefault(o => o.Id == orderId);

                if (order == null)
                    continue;

                // если статус заказа не завершен или не отклонен, то прибавляем секунду к последнему времени
                if (order.Status == ServiceType.Agree ||
                    order.Status == ServiceType.Arrived ||
                    order.Status == ServiceType.InProgress ||
                    order.Status == ServiceType.Submitted ||
                    order.Status == ServiceType.Reserved)
                {
                    order.TotalSeconds += seconds;
                    cell.Value = string.Format("{0:00}:{1:00}:{2:00}", order.TotalSeconds / 3600, (order.TotalSeconds / 60) % 60, order.TotalSeconds % 60);
                }
            }

            return true;
        }

        private bool _orders_Timer_Tick()
        {
            Task<HttpResponseMessage> task = Document.HttpManager.GetDispatcherOrders(Token);
            task.ContinueWith(
                t =>
                {
                    if (t.Status == TaskStatus.RanToCompletion)
                    {
                        HttpResponseMessage response = t.Result;
                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            return;
                        }

                        string result = response.Content.ReadAsStringAsync().Result;
                        TypedResponse typedRestore = JsonConvert.DeserializeObject<TypedResponse>(result);

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            if (typedRestore.ErrorCode == ErrorCodeEnum.Ok)
                            {
                                List<Order> data = typedRestore.ParsedData<List<Order>>();
                                Document.Orders = data;
                                Invoke((MethodInvoker)(() => OrdersBinding(data)));
                            }
                        }
                    }
                });

            return true;
        }

        private bool _drivers_Timer_Tick()
        {
            Task<HttpResponseMessage> task = Document.HttpManager.GetDispatcherDrivers(Token);
            task.ContinueWith(
                t =>
                {
                    Invoke((MethodInvoker)(Utils.HideWaitingForm));

                    if (t.Status == TaskStatus.RanToCompletion)
                    {
                        HttpResponseMessage response = t.Result;
                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            return;
                        }

                        string result = response.Content.ReadAsStringAsync().Result;
                        TypedResponse typedRestore = JsonConvert.DeserializeObject<TypedResponse>(result);

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            if (typedRestore.ErrorCode == ErrorCodeEnum.Ok)
                            {
                                List<Profile> data = typedRestore.ParsedData<List<Profile>>();
                                if (data != null)
                                {
                                    Document.Profiles.Clear();
                                    Document.Profiles = null;
                                    Document.Profiles = data;

                                    Invoke((MethodInvoker)(() => MainCarsBinding(Document.Profiles)));
                                    Invoke((MethodInvoker)(() => ShowMarkers(Document.Profiles)));
                                }
                            }
                            else
                            {
                                MessageBox.Show(typedRestore.ErrorMessage);
                            }
                        }
                    }
                });

            return true;
        }

        private void inProgressOblyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            refreshOrdersButton_Click(sender, e);
        }

        private void refreshComplainsButton_Click(object sender, EventArgs e)
        {
            Utils.ShowWaitingForm("Получение списка жалоб");

            Task<HttpResponseMessage> task = Document.HttpManager.GetComplains(Token);
            task.ContinueWith(
                t =>
                {
                    Invoke((MethodInvoker)(Utils.HideWaitingForm));

                    if (t.Status == TaskStatus.RanToCompletion)
                    {
                        HttpResponseMessage response = t.Result;
                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            MessageBox.Show(String.Format("Сервер вернул ошибку: {0}", response.StatusCode));
                            return;
                        }

                        string result = response.Content.ReadAsStringAsync().Result;
                        TypedResponse typedRestore = JsonConvert.DeserializeObject<TypedResponse>(result);

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            if (typedRestore.ErrorCode == ErrorCodeEnum.Ok)
                            {
                                List<Complain> complains = typedRestore.ParsedData<List<Complain>>();
                                Document.Complains = complains;
                                Invoke((MethodInvoker)(ShowComplains));
                            }
                            else
                            {
                                MessageBox.Show(typedRestore.ErrorMessage);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Произошла ошибка на сервере. Попробуй еще раз или обратитесь за помощью к разработчикам.");
                    }
                });
        }

        private void ComplainDataGridViewOnCellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = complainDataGridView.Rows[e.RowIndex];
            if (row.Cells["ComplainId"].Value == null)
                return;

            int complainId = (int)row.Cells["ComplainId"].Value;
            Complain complain = Document.Complains.FirstOrDefault(c => c.Id == complainId);
            if (complain == null)
                return;

            string message = !complain.Approved
                ? "Вы уверены, что хотите подтвердить жалобу?"
                : "Вы уверены, что хотите отклонить жалобу?";

            DialogResult dialogResult = MessageBox.Show(message, "", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                // сохранить изменения на сервере
                Utils.ShowWaitingForm("Ожидайте окончания операции...");

                complain.Approved = !complain.Approved;
                Task<HttpResponseMessage> task = Document.HttpManager.ApproveComplain(Token, complain);
                task.ContinueWith(
                    t =>
                    {
                        Invoke((MethodInvoker)(Utils.HideWaitingForm));

                        if (t.Status == TaskStatus.RanToCompletion)
                        {
                            HttpResponseMessage response = t.Result;
                            if (response.StatusCode != HttpStatusCode.OK)
                            {
                                MessageBox.Show(String.Format("Сервер вернул ошибку: {0}", response.StatusCode));
                                return;
                            }

                            string result = response.Content.ReadAsStringAsync().Result;
                            TypedResponse typedRestore = JsonConvert.DeserializeObject<TypedResponse>(result);

                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                if (typedRestore.ErrorCode == ErrorCodeEnum.Ok)
                                {
                                    // обновить список
                                    Invoke((MethodInvoker)(() => refreshComplainsButton_Click(null, null)));
                                }
                                else
                                {
                                    MessageBox.Show(typedRestore.ErrorMessage);
                                    Invoke((MethodInvoker)(ShowComplains));
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Произошла ошибка на сервере. Попробуй еще раз или обратитесь за помощью к разработчикам.");
                            Invoke((MethodInvoker)(ShowComplains));
                        }
                    });
            }
            else
            {
                ShowComplains();
            }
        }

        private void ShowComplains()
        {
            complainDataGridView.Rows.Clear();
            foreach (Complain complain in Document.Complains)
            {
                object[] displayRow = { complain.Id,
                                        complain.Owner.DisplayName,
                                        complain.Respondent.DisplayName,
                                        complain.Message,
                                        complain.Rate != null ? complain.Rate.ToString() : "-",
                                        complain.Created.ToString(CultureInfo.InvariantCulture),
                                        complain.UnderConsideration ? "Нет" : "Да",
                                        complain.Approved
                                      };
                complainDataGridView.Rows.Add(displayRow);
            }
        }

    }
}
