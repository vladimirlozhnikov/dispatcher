using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
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

namespace Dispatcher.UI.Forms
{
    public partial class MainForm
    {
        private DateTime LicenseFrom { get; set; }
        private DateTime LicenseTo { get; set; }
        private Token AdminToken { get; set; }
        private Model.Dispatcher CurrentDispatcher { get; set; }
        private City SelectedCity { get; set; }

        private enum CityAction
        {
            [Description("")]
            NoCityAction,
            [Description("Установите центр города")]
            SetCityCenter,
            [Description("Укажите зону действия города")]
            SetCityRegion,
            [Description("Укажите границы города")]
            SetCityBorder,
        }

        private CityAction _cityAction = CityAction.NoCityAction;

        static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            foreach (byte t in data)
            {
                sBuilder.Append(t.ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        // Verify a hash against a string.
        static bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
        {
            // Hash the input.
            string hashOfInput = GetMd5Hash(md5Hash, input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            return false;
        }

        private void ShowOnlineStatus()
        {
            if (AdminToken != null)
            {
                panel2.BackColor = Color.FromArgb(8, 240, 8);
            }
            else
            {
                panel2.BackColor = Color.Red;
            }

        }

        private void adminLoginButton_Click(object sender, EventArgs e)
        {
            Utils.ShowWaitingForm("Ожидайте окончания операции");

            // вычислить хэш пароля
            using (MD5 md5Hash = MD5.Create())
            {
                string salt = "salt";
                string hash = GetMd5Hash(md5Hash, adminPasswordTextBox.Text + salt);
                //string hash = "b305cadbb3bce54f3aa59c64fec00dea";

                Task<HttpResponseMessage> task = Document.HttpManager.LoginAdmin(adminLoginTextBox.Text, hash);
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
                                    AdminToken = typedRestore.ParsedData<Token>();
                                }
                                else
                                {
                                    MessageBox.Show(typedRestore.ErrorMessage);
                                }
                            }

                            Invoke((MethodInvoker) (ShowOnlineStatus));
                        }
                        else
                        {
                            MessageBox.Show("Произошла ошибка на сервере. Попробуй еще раз или обратитесь за помощью к разработчикам.");
                        }
                    });
            }
        }

        private void adminLogoutButton_Click(object sender, EventArgs e)
        {
            AdminToken = null;
            ShowOnlineStatus();
        }

        private void getDispatchersButton_Click(object sender, EventArgs e)
        {
            Utils.ShowWaitingForm("Ожидайте окончания операции");

            Task<HttpResponseMessage> task = Document.HttpManager.GetDispatchers(AdminToken);
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
                                List<Model.Dispatcher> dispatchers = typedRestore.ParsedData<List<Model.Dispatcher>>();
                                Invoke((MethodInvoker) (() => DispatchersBinding(dispatchers)));
                            }
                            else
                            {
                                MessageBox.Show(typedRestore.ErrorMessage);
                            }
                        }

                        Invoke((MethodInvoker) (ShowOnlineStatus));
                    }
                    else
                    {
                        MessageBox.Show("Произошла ошибка на сервере. Попробуй еще раз или обратитесь за помощью к разработчикам.");
                    }
                });
        }

        private void DispatchersBinding(List<Model.Dispatcher> dispatchers)
        {
            dispatchersListBox.DataSource = dispatchers;
            dispatchersListBox.DisplayMember = "Name";
            dispatchersListBox.ValueMember = "Id";
        }

        private void dispatchersListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dispatchersListBox == sender)
            {
                CurrentDispatcher = dispatchersListBox.SelectedItem as Model.Dispatcher;
                ShowDispatcher(CurrentDispatcher);
            }
        }

        private void ShowDispatcher(Model.Dispatcher dispatcher)
        {
            if (dispatcher == null)
                return;

            CurrentDispatcher = dispatcher;

            dispatcherNameTextBox.Text = dispatcher.Name;
            dispatcherLicenseTextBox.Text = dispatcher.License;

            if (dispatcher.ExpiresDate != null)
            {
                toDateTimePicker.Value = (DateTime) dispatcher.ExpiresDate;
            }
            activeDispatcherCheckBox.Checked = dispatcher.Active;
        }

        private void citiesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            City city = citiesListBox.SelectedItem as City;
            SelectedCity = city;

            ShowCity(city);
        }

        private void ShowCity(City city)
        {
            if (city == null)
                return;

            cityNameTextBox.Text = city.Name;
            cityDescriptionTextBox.Text = city.Description;
            orderTariffTextBox.Text = city.Tariff.City.ToString(CultureInfo.InvariantCulture);
            waitingTariffTextBox.Text = city.Tariff.Wait.ToString(CultureInfo.InvariantCulture);
            outCityTariffTextBox.Text = city.Tariff.Outcity.ToString(CultureInfo.InvariantCulture);
            reservationTariffTextBox.Text = city.Tariff.Reservation.ToString(CultureInfo.InvariantCulture);
            cargoOrderTariffTextBox.Text = city.CargoTariff.City.ToString(CultureInfo.InvariantCulture);
            cargoWaitingTariffTextBox.Text = city.CargoTariff.Wait.ToString(CultureInfo.InvariantCulture);
            cargoOutCityTariffTextBox.Text = city.CargoTariff.Outcity.ToString(CultureInfo.InvariantCulture);
            cargoReservationTariffTextBox.Text = city.CargoTariff.Reservation.ToString(CultureInfo.InvariantCulture);
            vipOrderTariffTextBox.Text = city.VipTariff.City.ToString(CultureInfo.InvariantCulture);
            vipWaitingTariffTextBox.Text = city.VipTariff.Wait.ToString(CultureInfo.InvariantCulture);
            vipOutCityTariffTextBox.Text = city.VipTariff.Outcity.ToString(CultureInfo.InvariantCulture);
            vipReservationTariffTextBox.Text = city.VipTariff.Reservation.ToString(CultureInfo.InvariantCulture);
            activeCityCheckBox.Checked = city.Active;

            _cityOverlay.Markers.Clear();

            float centerLatitude;
            float centerLongtitude;

            CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ".";

            if (float.TryParse(SelectedCity.Center.Latitude, NumberStyles.Any, ci, out centerLatitude) &&
                float.TryParse(SelectedCity.Center.Longtitude, NumberStyles.Any, ci, out centerLongtitude))
            {
                PointLatLng latLng = new PointLatLng(centerLatitude, centerLongtitude);
                cityMap.Position = latLng;
            }
        }

        private void saveDispatcherButton_Click(object sender, EventArgs e)
        {
            if (CurrentDispatcher == null)
            {
                MessageBox.Show("Диспетчер не выбран.");
                return;
            }
            Utils.ShowWaitingForm("Ожидайте окончания операции");

            CurrentDispatcher.Name = dispatcherNameTextBox.Text;
            CurrentDispatcher.License = dispatcherLicenseTextBox.Text;

            CurrentDispatcher.ExpiresDate = toDateTimePicker.Value;
            CurrentDispatcher.Active = activeDispatcherCheckBox.Checked;

            Task<HttpResponseMessage> task = Document.HttpManager.SaveDispatcher(AdminToken, CurrentDispatcher);
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
                                Invoke((MethodInvoker) (() => getDispatchersButton_Click(sender, e)));
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

        private void restoreDispatcherButton_Click(object sender, EventArgs e)
        {
            ShowDispatcher(CurrentDispatcher);
        }

        private void newDispatcherButton_Click(object sender, EventArgs e)
        {
            CurrentDispatcher = new Model.Dispatcher
            {
                Name = "Введите имя",
                License = "Введите лицензию",
                ExpiresDate = DateTime.Now.AddYears(1),
                Active = false
            };

            ShowDispatcher(CurrentDispatcher);
            dispatchersListBox.SelectedIndex = -1;
        }

        private void deleteDispatcherButton_Click(object sender, EventArgs e)
        {
            // предварительно уточнить
            DialogResult dialogResult = MessageBox.Show("Вы уверены?", "", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.No)
            {
                return;
            }

            Utils.ShowWaitingForm("Ожидайте окончания операции");

            Task<HttpResponseMessage> task = Document.HttpManager.DeleteDispatcher(AdminToken, CurrentDispatcher);
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
                                Invoke((MethodInvoker) (() => getDispatchersButton_Click(sender, e)));
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

        private void cityRefreshButton_Click(object sender, EventArgs e)
        {
            if (CurrentDispatcher == null)
            {
                MessageBox.Show("Не выбран диспетчер");
                return;
            }

            Utils.ShowWaitingForm("Ожидайте окончания запроса...");

            Task<HttpResponseMessage> task = Document.HttpManager.GetCities(CurrentDispatcher.Token);
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
                                CurrentDispatcher.Cities = cities;
                                Invoke((MethodInvoker)(() =>
                                {
                                    CitiesBinding(cities);
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

        private void CitiesBinding(List<City> cities)
        {
            citiesListBox.DataSource = cities;
            citiesListBox.DisplayMember = "Name";
            citiesListBox.ValueMember = "Id";
        }

        private void cityAddButton_Click(object sender, EventArgs e)
        {
            SelectedCity = new City {Name = cityNameTextBox.Text, Tariff = new Tariff(), Center = new Position(), Region = new List<Position>(), Border = new List<Position>()};
            ShowCity(SelectedCity);
            citiesListBox.SelectedIndex = -1;
        }

        private void cityDeleteButton_Click(object sender, EventArgs e)
        {
        }

        private void citySaveButton_Click(object sender, EventArgs e)
        {
            if (SelectedCity == null)
            {
                MessageBox.Show("Не выбран город");
                return;
            }

            if (CurrentDispatcher == null)
            {
                MessageBox.Show("Не выбран диспетчер");
                return;
            }

            if (string.IsNullOrEmpty(cityNameTextBox.Text))
            {
                MessageBox.Show("Введите имя города");
                return;
            }

            if (string.IsNullOrEmpty(orderTariffTextBox.Text) ||
                string.IsNullOrEmpty(waitingTariffTextBox.Text) ||
                string.IsNullOrEmpty(outCityTariffTextBox.Text) ||
                string.IsNullOrEmpty(reservationTariffTextBox.Text))
            {
                MessageBox.Show("Не указаны все тарифы");
                return;
            }

            float tariffCity;
            float tariffWaiting;
            float tariffOutCity;
            float tariffReservation;

            float cargoTariffCity;
            float cargoTariffWaiting;
            float cargoTariffOutCity;
            float cargoTariffReservation;

            float vipTariffCity;
            float vipTariffWaiting;
            float vipTariffOutCity;
            float vipTariffReservation;

            CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ".";

            if (!float.TryParse(orderTariffTextBox.Text, NumberStyles.Any, ci, out tariffCity) ||
                !float.TryParse(waitingTariffTextBox.Text, NumberStyles.Any, ci, out tariffWaiting) ||
                !float.TryParse(outCityTariffTextBox.Text, NumberStyles.Any, ci, out tariffOutCity) ||
                !float.TryParse(reservationTariffTextBox.Text, NumberStyles.Any, ci, out tariffReservation) ||

                !float.TryParse(cargoOrderTariffTextBox.Text, NumberStyles.Any, ci, out cargoTariffCity) ||
                !float.TryParse(cargoWaitingTariffTextBox.Text, NumberStyles.Any, ci, out cargoTariffWaiting) ||
                !float.TryParse(cargoOutCityTariffTextBox.Text, NumberStyles.Any, ci, out cargoTariffOutCity) ||
                !float.TryParse(cargoReservationTariffTextBox.Text, NumberStyles.Any, ci, out cargoTariffReservation) ||

                !float.TryParse(vipOrderTariffTextBox.Text, NumberStyles.Any, ci, out vipTariffCity) ||
                !float.TryParse(vipWaitingTariffTextBox.Text, NumberStyles.Any, ci, out vipTariffWaiting) ||
                !float.TryParse(vipOutCityTariffTextBox.Text, NumberStyles.Any, ci, out vipTariffOutCity) ||
                !float.TryParse(vipReservationTariffTextBox.Text, NumberStyles.Any, ci, out vipTariffReservation))
            {
                MessageBox.Show("Тарифы не верные");
                return;
            }

            float centerLatitude;
            float centerLongtitude;

            if (!float.TryParse(SelectedCity.Center.Latitude, NumberStyles.Any, ci, out centerLatitude) ||
                !float.TryParse(SelectedCity.Center.Longtitude, NumberStyles.Any, ci, out centerLongtitude))
            {
                MessageBox.Show("Не указаны координаты центра города");
                return;
            }

            float latitude;
            float longtitude;

            List<float> arrayRegion = new List<float>();
            if (SelectedCity.Region != null)
            {
                foreach (Position pos in SelectedCity.Region)
                {
                    if (!float.TryParse(pos.Latitude, NumberStyles.Any, ci, out latitude) ||
                        !float.TryParse(pos.Longtitude, NumberStyles.Any, ci, out longtitude))
                    {
                        continue;
                    }

                    arrayRegion.Add(latitude);
                    arrayRegion.Add(longtitude);
                }
            }

            List<float> arrayBorder = new List<float>();
            if (SelectedCity.Border != null)
            {
                foreach (Position pos in SelectedCity.Border)
                {
                    if (!float.TryParse(pos.Latitude, NumberStyles.Any, ci, out latitude) ||
                        !float.TryParse(pos.Longtitude, NumberStyles.Any, ci, out longtitude))
                    {
                        continue;
                    }

                    arrayBorder.Add(latitude);
                    arrayBorder.Add(longtitude);
                }
            }

            Utils.ShowWaitingForm("Ожидайте окончания запроса...");

            Task<HttpResponseMessage> task = Document.HttpManager.CreateOrUpdateCity(AdminToken, CurrentDispatcher.Id, cityNameTextBox.Text,
                cityDescriptionTextBox.Text,
                tariffCity, tariffWaiting, tariffOutCity, tariffReservation,
                cargoTariffCity, cargoTariffWaiting, cargoTariffOutCity, cargoTariffReservation,
                vipTariffCity, vipTariffWaiting, vipTariffOutCity, vipTariffReservation,
                activeCityCheckBox.Checked, centerLatitude, centerLongtitude,
                arrayRegion.ToArray(), arrayBorder.ToArray());
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

        private void centerCityButton_Click(object sender, EventArgs e)
        {
            _cityAction = CityAction.SetCityCenter;
            cityActionLabel.Text = "Установите центр города";

            if (SelectedCity != null)
            {
                double lat;
                double lng;

                CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                ci.NumberFormat.CurrencyDecimalSeparator = ".";

                if (!double.TryParse(SelectedCity.Center.Latitude, NumberStyles.Any, ci, out lat) || !double.TryParse(SelectedCity.Center.Longtitude, NumberStyles.Any, ci, out lng))
                    return;

                _cityOverlay.Markers.Clear();
                Bitmap b = new Bitmap(Resources.finish);
                GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(lat, lng), b);
                _cityOverlay.Markers.Add(marker);
            }
        }

        private void regionCityButton_Click(object sender, EventArgs e)
        {
            _cityAction = CityAction.SetCityRegion;
            cityActionLabel.Text = "Укажите зону действия города";

            if (SelectedCity != null && SelectedCity.Region != null)
            {
                CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                ci.NumberFormat.CurrencyDecimalSeparator = ".";

                _cityOverlay.Markers.Clear();
                foreach (Position position in SelectedCity.Region)
                {
                    double lat;
                    double lng;
                    if (!double.TryParse(position.Latitude, NumberStyles.Any, ci, out lat) || !double.TryParse(position.Longtitude, NumberStyles.Any, ci, out lng))
                        return;

                    Bitmap b = new Bitmap(Resources.finish);
                    GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(lat, lng), b);
                    _cityOverlay.Markers.Add(marker);
                }
            }
        }

        private void borderCityButton_Click(object sender, EventArgs e)
        {
            _cityAction = CityAction.SetCityBorder;
            cityActionLabel.Text = "Укажите границы города";

            if (SelectedCity != null && SelectedCity.Border != null)
            {
                CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                ci.NumberFormat.CurrencyDecimalSeparator = ".";

                _cityOverlay.Markers.Clear();
                foreach (Position position in SelectedCity.Border)
                {
                    double lat;
                    double lng;
                    if (!double.TryParse(position.Latitude, NumberStyles.Any, ci, out lat) || !double.TryParse(position.Longtitude, NumberStyles.Any, ci, out lng))
                        return;

                    Bitmap b = new Bitmap(Resources.finish);
                    GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(lat, lng), b);
                    _cityOverlay.Markers.Add(marker);
                }
            }
        }

        private void CityMapOnMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (SelectedCity == null)
                {
                    MessageBox.Show("Выберите город или назначьте новый");
                    return;
                }

                double lat = cityMap.FromLocalToLatLng(e.X, e.Y).Lat;
                double lng = cityMap.FromLocalToLatLng(e.X, e.Y).Lng;

                if (_cityAction == CityAction.SetCityCenter)
                {
                    Bitmap b = new Bitmap(Resources.finish);
                    GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(lat, lng), b);
                    _cityOverlay.Markers.Clear();
                    _cityOverlay.Markers.Add(marker);

                    if (SelectedCity != null)
                    {
                        if (SelectedCity.Center == null)
                        {
                            SelectedCity.Center = new Position();
                        }

                        SelectedCity.Center.Latitude = lat.ToString(CultureInfo.InvariantCulture);
                        SelectedCity.Center.Longtitude = lng.ToString(CultureInfo.InvariantCulture);
                    }
                }
                else if (_cityAction == CityAction.SetCityBorder)
                {
                    Bitmap b = new Bitmap(Resources.finish);
                    GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(lat, lng), b);
                    _cityOverlay.Markers.Add(marker);

                    if (SelectedCity != null)
                    {
                        if (SelectedCity.Border == null)
                        {
                            SelectedCity.Border = new List<Position>();
                        }

                        Position position = new Position();
                        position.Latitude = lat.ToString(CultureInfo.InvariantCulture);
                        position.Longtitude = lng.ToString(CultureInfo.InvariantCulture);

                        SelectedCity.Border.Add(position);
                    }
                }
                else if (_cityAction == CityAction.SetCityRegion)
                {
                    Bitmap b = new Bitmap(Resources.finish);
                    GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(lat, lng), b);
                    _cityOverlay.Markers.Add(marker);

                    if (SelectedCity != null)
                    {
                        if (SelectedCity.Region == null)
                        {
                            SelectedCity.Region = new List<Position>();
                        }

                        Position position = new Position();
                        position.Latitude = lat.ToString(CultureInfo.InvariantCulture);
                        position.Longtitude = lng.ToString(CultureInfo.InvariantCulture);

                        SelectedCity.Region.Add(position);
                    }
                }
            }
        }

        private void cityResetButton_Click(object sender, EventArgs e)
        {
            if (SelectedCity != null)
            {
                _cityOverlay.Markers.Clear();
                if (_cityAction == CityAction.SetCityCenter)
                {
                    SelectedCity.Center = null;
                }
                else if (_cityAction == CityAction.SetCityBorder)
                {
                    SelectedCity.Border = null;
                }
                else if (_cityAction == CityAction.SetCityRegion)
                {
                    SelectedCity.Region = null;
                }
            }
        }
    }
}
