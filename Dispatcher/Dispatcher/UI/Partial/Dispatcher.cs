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
using GMap.NET;
using Newtonsoft.Json;

namespace Dispatcher.UI.Forms
{
    public partial class MainForm
    {
        private Model.Dispatcher LoggedDispatcher { get; set; }
        private Token Token { get; set; }
        private readonly List<Profile> _driversForDeleting = new List<Profile>(); 

        private readonly JobsQuery _deleteJobsQuery = new JobsQuery(200);

        private void ShowDispatcherStatus()
        {
            if (LoggedDispatcher != null)
            {
                statusPanel.BackColor = Color.FromArgb(2, 251, 2);
            }
            else
            {
                statusPanel.BackColor = Color.Red;
            }
        }

        private void logoutDispatcherButton_Click(object sender, EventArgs e)
        {
            _jobsQuery.Stop();

            LoggedDispatcher = null;
            Token = null;
            ShowDispatcherStatus();
        }
        
        private void loginDispatcherButton_Click(object sender, EventArgs e)
        {
            Utils.ShowWaitingForm("Ожидайте окончания обработки операции...");

            Task<HttpResponseMessage> task = Document.HttpManager.LoginDispatcher(dispatcherLoginTextBox.Text, dispatcherPromoTextBox.Text, licenseTextBox.Text);
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
                                LogionDispatcherResponse data = typedRestore.ParsedData<LogionDispatcherResponse>();
                                if (data != null)
                                {
                                    LoggedDispatcher = data.Dispatcher;
                                    Document.Me = LoggedDispatcher;
                                    Token = data.Token;
                                }

                                //Invoke((MethodInvoker) (() => SetMapRegion(Document.CurrentRegion)));
                                Invoke((MethodInvoker) (ShowDispatcherStatus));

                                InitJobs();
                                _jobsQuery.Run();

                                refreshOrdersUpDown.ValueChanged += (o, args) =>
                                {
                                    Job j = _jobsQuery.FindByName("_orders_Timer_Tick");
                                    if (j != null)
                                    {
                                        j.IntervalInMillis = (long)(1000 * refreshOrdersUpDown.Value);
                                    }
                                };

                                refreshCarsUpDown.ValueChanged += (o, args) =>
                                {
                                    Job j = _jobsQuery.FindByName("_drivers_Timer_Tick");
                                    if (j != null)
                                    {
                                        j.IntervalInMillis = (long)(1000 * refreshCarsUpDown.Value);
                                    }
                                };
                            }
                            else
                            {
                                Document.Me = null;
                                LoggedDispatcher = null;
                                Token = null;

                                MessageBox.Show(typedRestore.ErrorMessage);
                            }
                        }
                        else
                        {
                            LoggedDispatcher = null;
                            Token = null;

                            MessageBox.Show("Введены неверные данные. Попробуйте еще раз");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Произошла ошибка на сервере. Попробуй еще раз или обратитесь за помощью к разработчикам.");
                    }
                });
        }

        private void driversRefreshButton_Click(object sender, EventArgs e)
        {
            Utils.ShowWaitingForm("Получаю список машин...");

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
                            MessageBox.Show(String.Format("Сервер вернул ошибку: {0}", response.StatusCode));
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
                                    Invoke((MethodInvoker)(() => DispatcherCarsBinding(Document.Profiles)));
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
        }

        private void DispatcherCarsBinding(List<Profile> profiles)
        {
            driversGridView.Rows.Clear();
            foreach (Profile profile in profiles)
            {
                string status = "Не Определен";
                if (profile.Status != null)
                {
                    if (profile.Status == (int)ServiceType.Offline)
                        status = "Вне сети";
                    else if (profile.Status == (int)ServiceType.Free)
                        status = "Свободен";
                    else if (profile.Status == (int)ServiceType.Busy)
                        status = "Занят";
                }

                if (profile.ServiceType == ServiceType.Customer)
                    status = "Клиент";

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

                // проверить, когда истекают лицензии водителей (на неделю вперед) и создать добавить задачи в очередь с напоминанием
                if (profile.ExpiresWith != null)
                {
                    TimeSpan ts = (DateTime) profile.ExpiresWith - DateTime.Now;
                    if (ts < new TimeSpan(7, 0, 0, 0))
                    {
                        Job j = _jobsQuery.SingleShot();
                        j.Name = String.Format("Expires License Job, Driver {0}", profile.Phone);
                        j.IntervalInMillis = (long) ts.TotalMilliseconds;
                        j.FuncEvent += job =>
                        {
                            Invoke((MethodInvoker)(() => ShowExpiresMessageBox(profile)));
                            return true;
                        };
                        j.Ready = true;
                    }
                }

                object[] displayRow = { profile.Id,
                                        profile.DisplayName,
                                        profile.Phone,
                                        profile.Car != null ? profile.Car.Number : "-",
                                        status,
                                        profile.Active ? "Активирован" : "Отключен",
                                        profile.ExpiresWith != null ? ((DateTime)profile.ExpiresWith).ToShortDateString() : "-"};
                driversGridView.Rows.Add(displayRow);
            }
        }

        private void ShowExpiresMessageBox(Profile profile)
        {
            MessageBox.Show(String.Format("У водителя {0} заканчивается срок действия лицензии", profile.Phone));
        }

        private void DriversGridViewOnMouseClick(object sender, MouseEventArgs e)
        {
            var hti = driversGridView.HitTest(e.X, e.Y);
            if (hti.RowIndex == -1)
                return;
            
            if (e.Button == MouseButtons.Right)
            {
                driversGridView.ClearSelection();
                driversGridView.Rows[hti.RowIndex].Selected = true;

                // get selected profile
                int profileId = (int)driversGridView.Rows[hti.RowIndex].Cells["Taxist1Id"].Value;
                Profile profile = Document.Profiles.FirstOrDefault(p => p.Id == profileId);

                if (profile == null)
                    return;

                ContextMenu m = new ContextMenu();
                MenuItem mi = new MenuItem("Показать детали");
                mi.Click += (o, args) => { ShowTaxistDetails(profile, true); };
                m.MenuItems.Add(mi);
                m.Show(driversGridView, new Point(e.X, e.Y));
            }
        }

        private void DriversGridViewOnMouseDoubleClick(object sender, MouseEventArgs e)
        {
            var hti = driversGridView.HitTest(e.X, e.Y);
            if (hti.RowIndex == -1)
                return;

            driversGridView.ClearSelection();
            driversGridView.Rows[hti.RowIndex].Selected = true;

            // get selected profile
            int profileId = (int)driversGridView.Rows[hti.RowIndex].Cells["Taxist1Id"].Value;
            Profile profile = Document.Profiles.FirstOrDefault(p => p.Id == profileId);

            if (profile == null)
                return;

            ShowTaxistDetails(profile, true);
        }

        private void driverDeleteButton_Click(object sender, EventArgs e)
        {
            // предварительно уточнить
            DialogResult dialogResult = MessageBox.Show("Вы уверены?", "", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.No)
            {
                return;
            }

            Utils.ShowWaitingForm("Ожидайте окончания операции");

            _driversForDeleting.Clear();
            foreach (DataGridViewRow row in driversGridView.SelectedRows)
            {
                if (row.Cells["Taxist1Id"].Value == null)
                    continue;

                int profileId = (int) row.Cells["Taxist1Id"].Value;
                Profile profile = Document.Profiles.FirstOrDefault(p => p.Id == profileId);

                if (profile == null)
                    continue;

                _driversForDeleting.Add(profile);
            }

            // выделить отдельную задачу, в которой последовательно удалять водителей из базы
            Job j = _deleteJobsQuery.AddJob();
            j.Name = "Delete Profile Job";
            j.IntervalInMillis = 200;
            j.FuncEvent += job =>
            {
                DeleteProfile(j);
                return true;
            };
            j.Ready = true;
            _deleteJobsQuery.Run();
        }

        private void DeleteProfile(Job j)
        {
            if (_inProgress)
                return;

            if (_driversForDeleting.Count == 0)
            {
                Invoke((MethodInvoker)(Utils.HideWaitingForm));
                _deleteJobsQuery.DeleteJob(j);
                return;
            }

            _inProgress = true;

            Profile profile = _driversForDeleting[0];
            Task<HttpResponseMessage> task = Document.HttpManager.DeleteDriverWithDispatcher(Token, profile);
            task.ContinueWith(
                t =>
                {
                    if (t.Status == TaskStatus.RanToCompletion)
                    {
                        _driversForDeleting.Remove(profile);
                    }

                    _inProgress = false;
                });
        }

        private void additionalSettingsButton_Click(object sender, EventArgs e)
        {
            Utils.ShowWaitingForm("Ожидайте окончания запроса...");

            Task<HttpResponseMessage> task = Document.HttpManager.GetDispatcherSettings(Token);
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
                                Model.DispatcherSettings settings = typedRestore.ParsedData<Model.DispatcherSettings>();
                                Invoke((MethodInvoker) (() => ShowDispatcherSettings(settings)));
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

        private void ShowDispatcherSettings(Model.DispatcherSettings settings)
        {
            DispatcherSettings settingsForm = new DispatcherSettings(Token, settings);
            settingsForm.ShowDialog(this);
        }

        private void dispatcherCitiesButton_Click(object sender, EventArgs e)
        {
            Utils.ShowWaitingForm("Ожидайте окончания запроса...");

            Task<HttpResponseMessage> task = Document.HttpManager.GetCities(Token);
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
                                List<City> cities = typedRestore.ParsedData<List<City>>();
                                Invoke((MethodInvoker)(() =>
                                {
                                    dispatcherCitiesListBox.DataSource = cities;
                                    dispatcherCitiesListBox.DisplayMember = "Name";
                                    dispatcherCitiesListBox.ValueMember = "Id";
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

        private void dispatcherCitiesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            City city = dispatcherCitiesListBox.SelectedItem as City;
            if (city == null)
                return;

            double lat;
            double lng;

            CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ".";

            if (!double.TryParse(city.Center.Latitude, NumberStyles.Any, ci, out lat) || !double.TryParse(city.Center.Longtitude, NumberStyles.Any, ci, out lng))
                return;

            PointLatLng latLng = new PointLatLng(lat, lng);
            mainMap.Position = latLng;
        }

        private void newDriverButton_Click(object sender, EventArgs e)
        {
            Profile driver = new Profile {Id = 0, License = LoggedDispatcher.License};
            driver.Car = new Car();
            TaxistDetails td = new TaxistDetails(driver, Token, true);
            td.ShowDialog(this);
        }

        private void searchDriverTextBox_TextChanged(object sender, EventArgs e)
        {
            string searchtext = searchDriverTextBox.Text.ToLower();
            if (string.IsNullOrEmpty(searchtext))
            {
                DispatcherCarsBinding(Document.Profiles);
                return;
            }

            List<Profile> searchProfiles = new List<Profile>();
            foreach (Profile profile in Document.Profiles)
            {
                if ((!string.IsNullOrEmpty(profile.Phone) && profile.Phone.ToLower().Contains(searchtext)) ||
                    (!string.IsNullOrEmpty(profile.Name) && profile.Name.ToLower().Contains(searchtext)) ||
                    (!string.IsNullOrEmpty(profile.LastName) && profile.LastName.ToLower().Contains(searchtext)) ||
                    (!string.IsNullOrEmpty(profile.SurName) && profile.SurName.ToLower().Contains(searchtext)) ||
                    (profile.Car != null && profile.Car.Number.ToLower().Contains(searchtext)) ||
                    (profile.Car != null && profile.Car.Model.ToLower().Contains(searchtext)) ||
                    (profile.Car != null && profile.Car.Color.ToLower().Contains(searchtext)))
                {
                    searchProfiles.Add(profile);
                }
            }

            DispatcherCarsBinding(searchProfiles);
        }

        private void bonusButton_Click(object sender, EventArgs e)
        {
            CampaignsForm campaignForm = new CampaignsForm(Token);
            campaignForm.ShowDialog();
        }
    }
}
