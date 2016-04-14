using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dispatcher.Business;
using Dispatcher.Model;
using Dispatcher.Properties;
using Newtonsoft.Json;

namespace Dispatcher.UI.Forms
{
    public partial class TaxistDetails : Form
    {
        public Profile Driver { get; set; }
        public Token Token { get; set; }

        public TaxistDetails(Profile driver, Token dispatcherToken, bool edit = false)
        {
            Driver = driver;
            Token = dispatcherToken;
            InitializeComponent();

            saveDriverButton.Enabled = edit;
            refreshDriverButton.Enabled = edit;

            ShowDriver();

            if (Driver.ServiceImage == null)
                Driver.ServiceImage = new List<ServiceImage>();
            if (Driver.Car != null && Driver.Car.ServiceImage == null)
                Driver.Car.ServiceImage = new List<ServiceImage>();

            // очистить стрим с картинкой, чтобы не посылать трафик на сервер
            foreach (ServiceImage si in Driver.ServiceImage)
            {
                si.Stream = null;
            }

            if (Driver.Car != null)
            {
                // очистить стрим с картинкой, чтобы не посылать трафик на сервер
                foreach (ServiceImage si in Driver.Car.ServiceImage)
                {
                    si.Stream = null;
                }
            }
        }

        private void ShowDriver()
        {
            if (Driver == null)
                return;

            if (!string.IsNullOrEmpty(Driver.Name))
                driverNameTextBox.Text = Driver.Name;
            if (!string.IsNullOrEmpty(Driver.Phone))
                driverPhoneTextBox.Text = Driver.Phone;
            if (!string.IsNullOrEmpty(Driver.LastName))
                driverLastNameTextBox.Text = Driver.LastName;
            if (!string.IsNullOrEmpty(Driver.SurName))
                driverSurNameTextBox.Text = Driver.SurName;
            verifyDriverCheckBox.Checked = Driver.Active;
            serviceNameTextBox.Text = Driver.ServiceName;

            if (Driver.Car != null)
            {
                carModelTextBox.Text = Driver.Car.Model;
                carNumberTextBox.Text = Driver.Car.Number;
                carColorTextBox.Text = Driver.Car.Color;

                switch (Driver.Car.ServiceType)
                {
                    case ServiceType.Passenger:
                        taxiTypeComboBox.SelectedIndex = 0;
                        break;
                    case ServiceType.Miniven:
                        taxiTypeComboBox.SelectedIndex = 1;
                        break;
                    case ServiceType.BusinessClass:
                        taxiTypeComboBox.SelectedIndex = 2;
                        break;
                }

                switch (Driver.Car.ServiceType2)
                {
                    case ServiceType.Passenger:
                        taxiType2ComboBox.SelectedIndex = 0;
                        break;
                    case ServiceType.Miniven:
                        taxiType2ComboBox.SelectedIndex = 1;
                        break;
                    case ServiceType.BusinessClass:
                        taxiType2ComboBox.SelectedIndex = 2;
                        break;
                }

                switch (Driver.Car.ServiceType3)
                {
                    case ServiceType.Passenger:
                        taxiType3ComboBox.SelectedIndex = 0;
                        break;
                    case ServiceType.Miniven:
                        taxiType3ComboBox.SelectedIndex = 1;
                        break;
                    case ServiceType.BusinessClass:
                        taxiType3ComboBox.SelectedIndex = 2;
                        break;
                }
            }

            pinCodeTextBox.Text = Driver.PromoCode;
            if (Driver.ExpiresWith != null && Driver.ExpiresWith > DateTime.MinValue && Driver.ExpiresWith < DateTime.MaxValue)
            {
                expiresDateTimePicker.Value = ((DateTime) Driver.ExpiresWith);
            }

            driverProfilePictureBox.Image = null;
            if (Driver.ServiceImage != null && Driver.ServiceImage.Count > 0)
            {
                if (string.IsNullOrEmpty(Driver.ServiceImage[0].Stream))
                {
                    CancellationTokenSource source = new CancellationTokenSource();
                    CancellationToken token = source.Token;

                    driverProfilePictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
                    driverProfilePictureBox.Image = Resources.ajax_loader;

                    Task<Image> task = Utils.GetImageFromUrl(Driver.ServiceImage[0].Url, token);
                    task.ContinueWith(t =>
                    {
                        Image image = t.Result;
                        if (image != null)
                        {
                            Invoke((MethodInvoker)(() =>
                            {
                                driverProfilePictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                                driverProfilePictureBox.Image = image;
                            }));
                        }
                    });
                }
                else
                {
                    driverProfilePictureBox.Image = Utils.Base64ToImage(Driver.ServiceImage[0].Stream);
                }
            }
        }

        private void saveDriverButton_Click(object sender, EventArgs e)
        {
            if (Driver == null)
            {
                MessageBox.Show("Водитель не выбран.");
                return;
            }

            Utils.ShowWaitingForm("Ожидайте окончания операции");

            Driver.Phone = driverPhoneTextBox.Text;
            Driver.Name = driverNameTextBox.Text;
            Driver.LastName = driverLastNameTextBox.Text;
            Driver.SurName = driverSurNameTextBox.Text;
            Driver.Active = verifyDriverCheckBox.Checked;
            Driver.ExpiresWith = expiresDateTimePicker.Value;
            Driver.ServiceName = serviceNameTextBox.Text;

            if (Driver.Car != null)
            {
                Driver.Car.Model = carModelTextBox.Text;
                Driver.Car.Number = carNumberTextBox.Text;
                Driver.Car.Color = carColorTextBox.Text;

                switch (taxiTypeComboBox.SelectedIndex)
                {
                    case 0:
                        Driver.Car.ServiceType = ServiceType.Passenger;
                        break;
                    case 1:
                        Driver.Car.ServiceType = ServiceType.Miniven;
                        break;
                    case 2:
                        Driver.Car.ServiceType = ServiceType.BusinessClass;
                        break;
                }

                switch (taxiType2ComboBox.SelectedIndex)
                {
                    case 0:
                        Driver.Car.ServiceType2 = ServiceType.Passenger;
                        break;
                    case 1:
                        Driver.Car.ServiceType2 = ServiceType.Miniven;
                        break;
                    case 2:
                        Driver.Car.ServiceType2 = ServiceType.BusinessClass;
                        break;
                    default:
                        Driver.Car.ServiceType2 = null;
                        break;
                }

                switch (taxiType3ComboBox.SelectedIndex)
                {
                    case 0:
                        Driver.Car.ServiceType3 = ServiceType.Passenger;
                        break;
                    case 1:
                        Driver.Car.ServiceType3 = ServiceType.Miniven;
                        break;
                    case 2:
                        Driver.Car.ServiceType3 = ServiceType.BusinessClass;
                        break;
                    default:
                        Driver.Car.ServiceType3 = null;
                        break;
                }
            }

            if (Driver.ServiceType == ServiceType.Taxists)
            {
                Driver.DriverActive = Driver.Active;
            }

            Task<HttpResponseMessage> task = Document.HttpManager.SaveDriverWithDispatcher(Token, Driver);
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

        private void refreshDriverButton_Click(object sender, EventArgs e)
        {
            ShowDriver();
        }

        private void carGalleryButton_Click_1(object sender, EventArgs e)
        {
            CarGalleryForm galleryForm = new CarGalleryForm(Driver);
            galleryForm.ShowDialog();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void driverProfilePictureBox_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "png files (*.png)|*.png|jpeg files (*.jpg)|*.jpg",
                FilterIndex = 2
            };


            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Stream stream;
                    if ((stream = openFileDialog.OpenFile()) != null)
                    {
                        using (stream)
                        {
                            Driver.ServiceImage = new List<ServiceImage>();

                            Image image = Image.FromStream(stream);
                            string base64String = Utils.ImageToBase64(image);

                            ServiceImage si = new ServiceImage();
                            si.Stream = base64String;
                            Driver.ServiceImage.Add(si);

                            Invoke((MethodInvoker)(() =>
                            {
                                driverProfilePictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                                driverProfilePictureBox.Image = image;
                            }));
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка. Не могу прочитать файл с диска. " + ex.Message);
                }
            }
        }

        private void newPinCodeButton_Click(object sender, EventArgs e)
        {
            Task<HttpResponseMessage> task = Document.HttpManager.NewPromoCode(Token, Driver);
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
                                string promoCode = typedRestore.ParsedData<string>();
                                Driver.PromoCode = promoCode;

                                Invoke((MethodInvoker)(ShowDriver));

                                if (sendSmsCheckBox.Checked)
                                {
                                    Sms sms = new Sms
                                    {
                                        Profile = new Profile {Phone = Driver.Phone},
                                        Message = promoCode
                                    };

                                    if (!Utils.SendSms(sms))
                                    {
                                        MessageBox.Show("Произошла ошибка при отправке SMS", "Ошибка");
                                    }
                                }
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
}
