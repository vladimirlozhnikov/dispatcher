using System;
using System.Collections.Generic;
using System.Globalization;
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
    public partial class NewCampaignForm : Form
    {
        private DiscountCampaign Campaign { get; set; }
        private List<Profile> Sources { get; set; }
        private List<Profile> Destination { get; set; }
        private Token Token { get; set; }

        public NewCampaignForm(Token token, DiscountCampaign campaign = null)
        {
            Token = token;
            Campaign = campaign;
            InitializeComponent();

            // fill sources
            List<Profile> sourses = new List<Profile>(Document.Profiles.Where(p1 => p1.ServiceType == ServiceType.Customer));
            if (Campaign != null)
            {
                foreach (Bonus bonus in Campaign.Bonuses)
                {
                    sourses.RemoveAll(s1 => (s1.Id == bonus.Id));
                }
            }

            Sources = sourses;
            BindSources(sourses);

            // fill destination
            List<Profile> destination = new List<Profile>();
            if (Campaign != null)
            {
                destination.AddRange(Campaign.Bonuses.Select(bonus => bonus.Customer));
            }

            Destination = destination;
            BindDestination(destination);

            if (Campaign != null)
            {
                campaignNameTextBox.Text = Campaign.Name;
                campaignDescriptionTextBox.Text = Campaign.Description;
                campaignBeginTimePicker.Value = Campaign.CreatedDate;
                campaignFinishTimePicker.Value = Campaign.ExpiresTo;
                fbCheckBox.Checked = Campaign.fb;
                vkCheckBox.Checked = Campaign.vk;
                okCheckBox.Checked = Campaign.ok;
                useCheckBox.Checked = Campaign.use;
                activeCheckBox.Checked = Campaign.Active;
                discountNumericTextBox.Text = Math.Round(Campaign.Discount, 1).ToString(CultureInfo.CurrentCulture.NumberFormat);
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BindSources(List<Profile> sourses)
        {
            sourceDataGridView.Rows.Clear();
            foreach (Profile profile in sourses)
            {

                object[] displayRow = { profile.Id,
                                        profile.Name,
                                        profile.Phone
                                      };
                sourceDataGridView.Rows.Add(displayRow);
            }
        }

        private void BindDestination(List<Profile> sourses)
        {
            desctinationDataGridView.Rows.Clear();
            foreach (Profile profile in sourses)
            {

                object[] displayRow = { profile.Id,
                                        profile.Name,
                                        profile.Phone
                                      };
                desctinationDataGridView.Rows.Add(displayRow);
            }
        }

        private void sourceSearchTextBox_TextChanged(object sender, EventArgs e)
        {
            if (Sources == null)
                return;

            string searchText = sourceSearchTextBox.Text;
            List<Profile> filtered = new List<Profile>();
            foreach (Profile profile in Sources)
            {
                if (!string.IsNullOrEmpty(profile.Name) && profile.Name.ToLower().Contains(searchText.ToLower()))
                {
                    filtered.Add(profile);
                }
                else if (!string.IsNullOrEmpty(profile.Phone) && profile.Phone.ToLower().Contains(searchText.ToLower()))
                {
                    filtered.Add(profile);
                }
            }

            BindSources(filtered);
        }

        private void destinationSearchTextBox_TextChanged(object sender, EventArgs e)
        {
            if (Destination == null)
                return;

            string searchText = destinationSearchTextBox.Text;
            List<Profile> filtered = new List<Profile>();
            foreach (Profile profile in Destination)
            {
                if (!string.IsNullOrEmpty(profile.Name) && profile.Name.ToLower().Contains(searchText.ToLower()))
                {
                    filtered.Add(profile);
                }
                else if (!string.IsNullOrEmpty(profile.Phone) && profile.Phone.ToLower().Contains(searchText.ToLower()))
                {
                    filtered.Add(profile);
                }
            }

            BindDestination(filtered);
        }

        private void toRightOneButton_Click(object sender, EventArgs e)
        {
            List<Profile> deleting = new List<Profile>();
            foreach (DataGridViewRow row in sourceDataGridView.SelectedRows)
            {
                if (row.Cells["SourseId"].Value == null)
                    continue;

                int profileId = (int)row.Cells["SourseId"].Value;
                Profile profile = Document.Profiles.FirstOrDefault(p => p.Id == profileId);

                if (profile == null)
                    continue;

                deleting.Add(profile);
                Sources.Remove(profile);
                sourceDataGridView.Rows.RemoveAt(row.Index);
            }

            Destination.AddRange(deleting);
            BindDestination(Destination);
            sourceDataGridView.ClearSelection();
        }

        private void toRightAllButton_Click(object sender, EventArgs e)
        {
            List<Profile> deleting = new List<Profile>();
            foreach (DataGridViewRow row in sourceDataGridView.Rows)
            {
                if (row.Cells["SourseId"].Value == null)
                    continue;

                int profileId = (int)row.Cells["SourseId"].Value;
                Profile profile = Document.Profiles.FirstOrDefault(p => p.Id == profileId);

                if (profile == null)
                    continue;

                deleting.Add(profile);
            }

            Destination.AddRange(deleting);

            Sources.Clear();
            sourceDataGridView.Rows.Clear();
            BindDestination(Destination);
            sourceDataGridView.ClearSelection();
        }

        private void toLeftOneButton_Click(object sender, EventArgs e)
        {
            List<Profile> deleting = new List<Profile>();
            foreach (DataGridViewRow row in desctinationDataGridView.SelectedRows)
            {
                if (row.Cells["DestinationId"].Value == null)
                    continue;

                int profileId = (int)row.Cells["DestinationId"].Value;
                Profile profile = Document.Profiles.FirstOrDefault(p => p.Id == profileId);

                if (profile == null)
                    continue;

                deleting.Add(profile);
                Destination.Remove(profile);
                desctinationDataGridView.Rows.RemoveAt(row.Index);
            }

            Sources.AddRange(deleting);
            BindSources(Sources);
            desctinationDataGridView.ClearSelection();
        }

        private void toLeftAllButton_Click(object sender, EventArgs e)
        {
            List<Profile> deleting = new List<Profile>();
            foreach (DataGridViewRow row in desctinationDataGridView.Rows)
            {
                if (row.Cells["DestinationId"].Value == null)
                    continue;

                int profileId = (int)row.Cells["DestinationId"].Value;
                Profile profile = Document.Profiles.FirstOrDefault(p => p.Id == profileId);

                if (profile == null)
                    continue;

                deleting.Add(profile);
            }

            Sources.AddRange(deleting);

            Destination.Clear();
            desctinationDataGridView.Rows.Clear();
            BindSources(Sources);
            desctinationDataGridView.ClearSelection();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(campaignNameTextBox.Name))
            {
                MessageBox.Show("Введите имя кампании");
                return;
            }

            if (string.IsNullOrEmpty(campaignDescriptionTextBox.Text))
            {
                MessageBox.Show("Введите описание кампании");
                return;
            }

            if (desctinationDataGridView.Rows.Count == 0)
            {
                MessageBox.Show("Не выбраны участники кампании");
                return;
            }

            if (campaignFinishTimePicker.Value <= campaignBeginTimePicker.Value)
            {
                MessageBox.Show("Дата окончания кампании должна превышать дату начала");
                return;
            }

            if (!fbCheckBox.Checked && !vkCheckBox.Checked && !okCheckBox.Checked && !useCheckBox.Checked)
            {
                MessageBox.Show("Выберите социальную сеть, через которую проводится кампания");
                return;
            }

            if (!discountNumericTextBox.IsDecimalValid)
            {
                MessageBox.Show("Введен не правильный скидочный процент");
                return;
            }

            if (Campaign == null)
            {
                Campaign = new DiscountCampaign
                {
                    Bonuses = new List<Bonus>()
                };
            }
            Campaign.Name = campaignNameTextBox.Text;
            Campaign.Description = campaignDescriptionTextBox.Text;
            Campaign.CreatedDate = campaignBeginTimePicker.Value;
            Campaign.ExpiresTo = campaignFinishTimePicker.Value;
            Campaign.fb = fbCheckBox.Checked;
            Campaign.vk = vkCheckBox.Checked;
            Campaign.ok = okCheckBox.Checked;
            Campaign.use = useCheckBox.Checked;
            Campaign.Active = activeCheckBox.Checked;
            Campaign.Discount = (double) Math.Round(discountNumericTextBox.DecimalValue, 1);

            Campaign.Bonuses.Clear();
            foreach (Profile profile in Destination)
            {
                Bonus bonus = new Bonus {Customer = profile};
                Campaign.Bonuses.Add(bonus);
            }

            Utils.ShowWaitingForm("Ожидайте...");
            Task<HttpResponseMessage> task = Document.HttpManager.CreateOrUpdateCampaign(Token, Campaign);
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

        private void useCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            fbCheckBox.Enabled = !useCheckBox.Checked;
            vkCheckBox.Enabled = !useCheckBox.Checked;
            okCheckBox.Enabled = !useCheckBox.Checked;
        }
    }
}
