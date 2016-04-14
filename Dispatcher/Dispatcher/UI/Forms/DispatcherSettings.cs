using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dispatcher.Business;
using Dispatcher.Model;
using Newtonsoft.Json;

namespace Dispatcher.UI.Forms
{
    public partial class DispatcherSettings : Form
    {
        private Token Token { get; set; }
        private Model.DispatcherSettings Settings { get; set; }

        public DispatcherSettings(Token dispatcherToken, Model.DispatcherSettings settings)
        {
            Settings = settings;
            Token = dispatcherToken;

            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            hoursTextBox.Text = Settings.OrdersPeriodInHours.ToString();
            metersTextBox.Text = Settings.RadiusInMeters.ToString();

            enableQueueCheckBox.Checked = Settings.EnableQueue;
            secondsTextBox.Enabled = enableQueueCheckBox.Checked;
            ignoreTextBox.Enabled = enableQueueCheckBox.Checked;

            if (Settings.TaxistResponseInSeconds != null)
                secondsTextBox.Text = Settings.TaxistResponseInSeconds.ToString();
            if (Settings.AllowIgnoreOrders != null)
                ignoreTextBox.Text = Settings.AllowIgnoreOrders.ToString();
        }

        private void enableQueueCheckBox_CheckedChanged(object sender, System.EventArgs e)
        {
            secondsTextBox.Enabled = enableQueueCheckBox.Checked;
            ignoreTextBox.Enabled = enableQueueCheckBox.Checked;
        }

        private void closeButton_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void saveButton_Click(object sender, System.EventArgs e)
        {
            // сделать валидацию
            if (!hoursTextBox.IsValid)
            {
                MessageBox.Show("Не заполнено поле для периода");
                return;
            }

            if (!metersTextBox.IsValid)
            {
                MessageBox.Show("Не заполнено поле для радиуса");
                return;
            }

            if (enableQueueCheckBox.Checked)
            {
                if (!secondsTextBox.IsValid)
                {
                    MessageBox.Show("Не заполнено поле времени отклика");
                    return;
                }

                if (!ignoreTextBox.IsValid)
                {
                    MessageBox.Show("Не заполнено поле количества проигнорированных заказов");
                    return;
                }
            }

            // сохранить настройки в базе данных.
            Settings.OrdersPeriodInHours = hoursTextBox.IntValue;
            Settings.RadiusInMeters = metersTextBox.IntValue;
            Settings.EnableQueue = enableQueueCheckBox.Checked;
            Settings.TaxistResponseInSeconds = enableQueueCheckBox.Checked ? secondsTextBox.IntValue : (int?) null;
            Settings.AllowIgnoreOrders = enableQueueCheckBox.Checked ? ignoreTextBox.IntValue : (int?) null;

            Task<HttpResponseMessage> task = Document.HttpManager.SaveDispatcherSettings(Token, Settings);
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
                                MessageBox.Show("Сохранение прошло успешно");
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
