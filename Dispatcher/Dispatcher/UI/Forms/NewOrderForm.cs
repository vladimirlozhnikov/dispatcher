using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dispatcher.Business;
using Dispatcher.Model;
using Newtonsoft.Json;

namespace Dispatcher.UI.Forms
{
    public partial class NewOrderForm : Form
    {
        private readonly PopupMapForm _popupMapForm = new PopupMapForm();
        private enum ActiveAddress
        {
            None,
            From,
            To
        };

        private ActiveAddress _activeAddress;

        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public Address From { get; set; }
        public Address To { get; set; }
        public Profile SelectedDriver { get; set; }
        private bool IsShow { get; set; }

        private void MovePopupForm()
        {
            if (_activeAddress == ActiveAddress.None)
                return;

            Point point = new Point
                              {
                                  X = Location.X + Width + 10,
                                  Y =
                                      _activeAddress == ActiveAddress.From
                                          ? fromTextBox.Location.Y + 20
                                          : toTextBox.Location.Y + 20
                              };

            _popupMapForm.Location = point;
        }

        public NewOrderForm()
        {
            InitializeComponent();

            _popupMapForm.AddressSelected += _popupMapForm_AddressSelected;
            _activeAddress = ActiveAddress.None;
            IsShow = true;

            driverComboBox.MeasureItem += DriverComboBoxOnMeasureItem;
            driverComboBox.DrawMode = DrawMode.OwnerDrawVariable;
            driverComboBox.DrawItem += DriverComboBoxOnDrawItem;
            driverComboBox.TextUpdate += DriverComboBoxOnTextUpdate;

            if (Document.Profiles != null)
            {
                List<Profile> mainProfiles = Document.Profiles.Where(profile => profile.Active).ToList();

                driverComboBox.DataSource = mainProfiles;
                driverComboBox.DisplayMember = "DisplayName";
                driverComboBox.SelectedIndex = -1;
            }
        }

        private void DriverComboBoxOnMeasureItem(object sender, MeasureItemEventArgs e)
        {
            Profile profile = driverComboBox.Items[e.Index] as Profile;
            if (profile == null)
                return;

            string status = "Вне сети";
            if (profile.Status == null)
                status = "Вне сети";
            else if (profile.Status == (int)ServiceType.Free)
                status = "Свободен";
            else if (profile.Status == (int)ServiceType.Busy)
                status = "Занят";

            string line1 = String.Format("Телефон: {0}", profile.Phone);
            string line2 = String.Format("Имя: {0}", string.IsNullOrEmpty(profile.Name) ? "Не указано" : profile.Name);
            string line3 = String.Format("Статус: {0}", status);
            string line4 = string.Format("Номер машины: {0}", (profile.Car != null) ? profile.Car.Number : "Машине не зарегистрирована в системе");
            string line5 = string.Format("Модель машины: {0}", (profile.Car != null) ? profile.Car.Model : "Машине не зарегистрирована в системе");

            // вычислить высоту каждой строки
            SizeF size1 = Utils.GetListItemSize(line1, driverComboBox.Width, driverComboBox.Font, e.Graphics);
            SizeF size2 = Utils.GetListItemSize(line2, driverComboBox.Width, driverComboBox.Font, e.Graphics);
            SizeF size3 = Utils.GetListItemSize(line3, driverComboBox.Width, driverComboBox.Font, e.Graphics);
            SizeF size4 = Utils.GetListItemSize(line4, driverComboBox.Width, driverComboBox.Font, e.Graphics);
            SizeF size5 = Utils.GetListItemSize(line5, driverComboBox.Width, driverComboBox.Font, e.Graphics);
            SizeF sf = new SizeF(driverComboBox.Width, size1.Height + size2.Height + size3.Height + size4.Height + size5.Height);

            e.ItemHeight = (int)sf.Height;
            e.ItemWidth = driverComboBox.Width;
        }

        private void DriverComboBoxOnDrawItem(object sender, DrawItemEventArgs e)
        {
            Profile profile = driverComboBox.Items[e.Index] as Profile;
            if (profile == null)
                return;

            string status = "Вне сети";
            if (profile.Status == null)
                status = "Вне сети";
            else if (profile.Status == (int)ServiceType.Free)
                status = "Свободен";
            else if (profile.Status == (int)ServiceType.Busy)
                status = "Занят";

            string line1 = String.Format("Телефон: {0}", profile.Phone);
            string line2 = String.Format("Имя: {0}", string.IsNullOrEmpty(profile.Name) ? "Не указано" : profile.Name);
            string line3 = String.Format("Статус: {0}", status);
            string line4 = string.Format("Номер машины: {0}", (profile.Car != null) ? profile.Car.Number : "Машине не зарегистрирована в системе");
            string line5 = string.Format("Модель машины: {0}", (profile.Car != null) ? profile.Car.Model : "Машине не зарегистрирована в системе");

            // вычислить высоту каждой строки
            SizeF size1 = Utils.GetListItemSize(line1, driverComboBox.Width, driverComboBox.Font, e.Graphics);
            SizeF size2 = Utils.GetListItemSize(line2, driverComboBox.Width, driverComboBox.Font, e.Graphics);
            SizeF size3 = Utils.GetListItemSize(line3, driverComboBox.Width, driverComboBox.Font, e.Graphics);
            SizeF size4 = Utils.GetListItemSize(line4, driverComboBox.Width, driverComboBox.Font, e.Graphics);
            SizeF size5 = Utils.GetListItemSize(line5, driverComboBox.Width, driverComboBox.Font, e.Graphics);

            if ((e.State & DrawItemState.Focus) == 0)
            {
                e.Graphics.FillRectangle(new SolidBrush(SystemColors.Window), e.Bounds);

                Utils.DrawListboxItem(line1, 0, e, driverComboBox.Font, SystemColors.WindowText);
                Utils.DrawListboxItem(line2, 0 + size1.Height, e, driverComboBox.Font, SystemColors.WindowText);
                Utils.DrawListboxItem(line3, 0 + size1.Height + size2.Height, e, driverComboBox.Font, SystemColors.WindowText);
                Utils.DrawListboxItem(line4, 0 + size1.Height + size2.Height + size3.Height, e, driverComboBox.Font, SystemColors.WindowText);
                Utils.DrawListboxItem(line5, 0 + size1.Height + size2.Height + size3.Height + size4.Height, e, driverComboBox.Font, SystemColors.WindowText);

                e.Graphics.DrawRectangle(new Pen(SystemColors.Highlight), e.Bounds);
            }
            else
            {
                e.Graphics.FillRectangle(new SolidBrush(SystemColors.Highlight), e.Bounds);

                Utils.DrawListboxItem(line1, 0, e, driverComboBox.Font, SystemColors.WindowText);
                Utils.DrawListboxItem(line2, 0 + size1.Height, e, driverComboBox.Font, SystemColors.WindowText);
                Utils.DrawListboxItem(line3, 0 + size1.Height + size2.Height, e, driverComboBox.Font, SystemColors.WindowText);
                Utils.DrawListboxItem(line4, 0 + size1.Height + size2.Height + size3.Height, e, driverComboBox.Font, SystemColors.WindowText);
                Utils.DrawListboxItem(line5, 0 + size1.Height + size2.Height + size3.Height + size4.Height, e, driverComboBox.Font, SystemColors.WindowText);
            }
        }

        private bool IsValid(ref string message)
        {
            if (string.IsNullOrEmpty(phoneTextBox.Text))
            {
                message = "Номер телефона не указан";
                return false;
            }

            if (string.IsNullOrEmpty(nameTextBox.Text))
            {
                message = "Имя клиента не указано";
                return false;
            }

            if (From == null)
            {
                message = "Адрес \"Откуда\" не указан";
                return false;
            }

            return true;
        }

        void _popupMapForm_AddressSelected(object sender, EventArgs e)
        {
            results r = sender as results;
            if (r != null)
            {
                Position position = new Position
                {
                    Latitude = r.geometry.location.lat,
                    Longtitude = r.geometry.location.lng
                };

                Address address = new Address
                {
                    Description = r.DisplayStreetName,
                    Position = position
                };

                if (_activeAddress == ActiveAddress.From)
                {
                    From = address;
                    fromTextBox.Text = r.DisplayStreetName;
                }
                else if (_activeAddress == ActiveAddress.To)
                {
                    To = address;
                    toTextBox.Text = r.DisplayStreetName;
                }
            }
        }

        private void CreateButtonClick(object sender, EventArgs e)
        {
            string message = String.Empty;
            if (!IsValid(ref message))
            {
                MessageBox.Show(message);
                return;
            }

            CustomerName = nameTextBox.Text;
            Phone = phoneTextBox.Text;

            DialogResult = DialogResult.Yes;

            _popupMapForm.Close();
            Close();
        }

        private void CancelButtonClick(object sender, EventArgs e)
        {
            IsShow = false;
            _popupMapForm.Close();
            Close();
        }

        private void FromTextBoxMouseClick(object sender, MouseEventArgs e)
        {
            _popupMapForm.Show();
            _activeAddress = ActiveAddress.From;
            MovePopupForm();

            fromTextBox.Focus();
        }

        private void ToTextBoxMouseClick(object sender, MouseEventArgs e)
        {
            _popupMapForm.Show();
            _activeAddress = ActiveAddress.To;
            MovePopupForm();

            toTextBox.Focus();
        }

        private void PhoneTextBoxMouseClick(object sender, MouseEventArgs e)
        {
            _popupMapForm.Hide();
        }

        private void NameTextBoxMouseClick(object sender, MouseEventArgs e)
        {
            _popupMapForm.Hide();
        }

        private void DescriptionTextBoxMouseClick(object sender, MouseEventArgs e)
        {
            _popupMapForm.Hide();
        }

        private void NewOrderForm_LocationChanged(object sender, EventArgs e)
        {
            MovePopupForm();
        }

        private void fromTextBox_TextChanged(object sender, EventArgs e)
        {
            string prefix = fromTextBox.Text;
            Task<HttpResponseMessage> task = Document.HttpManager.FindAddresses(prefix);

            task.ContinueWith(
                t =>
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
                        GoogleGeoCodeResponse searchResponse =
                            JsonConvert.DeserializeObject<GoogleGeoCodeResponse>(result);

                        if (searchResponse.status.ToLower() == "ok")
                        {
                            if (IsShow)
                            {
                                Invoke((MethodInvoker) (() => ShowSearchResults(searchResponse.results)));
                            }
                        }
                    }
                });
        }

        private void ShowSearchResults(results[] results)
        {
            _popupMapForm.ShowResults(results);
        }

        private void NewOrderForm_Load(object sender, EventArgs e)
        {
            fromTextBox.Text = String.Format("{0}, ", Document.CurrentRegion.Name);
            toTextBox.Text = String.Format("{0}, ", Document.CurrentRegion.Name);
        }

        private void clearFromButton_Click(object sender, EventArgs e)
        {
            From = null;

            fromTextBox.Text = String.Format("{0}, ", Document.CurrentRegion.Name);
            fromTextBox.SelectAll();
            fromTextBox.Focus();
        }

        private void clearToButton_Click(object sender, EventArgs e)
        {
            To = null;

            toTextBox.Text = String.Format("{0}, ", Document.CurrentRegion.Name);
            toTextBox.SelectAll();
            toTextBox.Focus();
        }

        private void driverComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedDriver = driverComboBox.SelectedItem as Profile;
        }

        private void DriverComboBoxOnTextUpdate(object sender, EventArgs eventArgs)
        {
            if (Document.Profiles == null)
                return;
            
            string prefix = driverComboBox.Text;
            List<Profile> filteredCars = new List<Profile>();

            foreach (Profile profile in Document.Profiles)
            {
                if (profile.Phone.Contains(prefix) ||
                    (!string.IsNullOrEmpty(profile.LastName) && profile.LastName.Contains(prefix)) ||
                    (!string.IsNullOrEmpty(profile.Name) && profile.Name.Contains(prefix)) ||
                    (!string.IsNullOrEmpty(profile.SurName) && profile.SurName.Contains(prefix)) ||
                    (profile.Car != null && !string.IsNullOrEmpty(profile.Car.Model) && profile.Car.Model.Contains(prefix)) ||
                    (profile.Car != null && !string.IsNullOrEmpty(profile.Car.Number) && profile.Car.Number.Contains(prefix)))
                {
                    filteredCars.Add(profile);
                }
            }

            driverComboBox.DataSource = filteredCars;
            driverComboBox.DroppedDown = true;
            driverComboBox.Text = prefix;
            driverComboBox.SelectionStart = prefix.Length;
        }
    }
}
