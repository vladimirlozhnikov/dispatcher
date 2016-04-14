using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dispatcher.Business;
using Dispatcher.Model;

namespace Dispatcher.UI.Forms
{
    public partial class CarGalleryForm : Form
    {
        private Profile Driver { get; set; }
        public CarGalleryForm(Profile driver)
        {
            InitializeComponent();

            if (driver.Car.ServiceImage == null)
                return;
            Driver = driver;

            // show images
            UpdateImages();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private Task<Image> GetImageFromUrl(string url, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                try
                {
                    WebRequest req = WebRequest.Create(url);
                    WebResponse response = req.GetResponse();
                    Stream stream = response.GetResponseStream();

                    if (stream != null) return Image.FromStream(stream);
                }
                catch (Exception)
                {
                    return null;
                }
                return null;
            }, cancellationToken);
        }

        private void PictureBox1OnMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Image image;
                string base64string = OpenImageDialog(out image);
                if (string.IsNullOrEmpty(base64string))
                    return;

                int count = Driver.Car.ServiceImage.Count;
                if (count < 1)
                {
                    ServiceImage si = new ServiceImage { Stream = base64string };
                    Driver.Car.ServiceImage.Add(si);
                }

                Driver.Car.ServiceImage[0].Url = null;
                Driver.Car.ServiceImage[0].Stream = base64string;
                UpdateImages();
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (Driver.Car.ServiceImage.Count > 0)
                {
                    ContextMenu m = new ContextMenu();
                    MenuItem mi1 = new MenuItem("Удалить фото");
                    mi1.Click += (o, args) =>
                    {
                        Driver.Car.ServiceImage.Remove(Driver.Car.ServiceImage[0]);
                        UpdateImages();
                    };
                    m.MenuItems.Add(mi1);
                    m.Show(pictureBox1, new Point(e.X, e.Y));
                }
            }
        }

        private void PictureBox2OnMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Image image;
                string base64string = OpenImageDialog(out image);
                if (string.IsNullOrEmpty(base64string))
                    return;

                int count = Driver.Car.ServiceImage.Count;
                if (count < 2)
                {
                    ServiceImage si = new ServiceImage { Stream = base64string };
                    Driver.Car.ServiceImage.Add(si);
                    count++;
                }

                Driver.Car.ServiceImage[count >= 2 ? 1 : count - 1].Url = null;
                Driver.Car.ServiceImage[count >= 2 ? 1 : count - 1].Stream = base64string;
                UpdateImages();
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (Driver.Car.ServiceImage.Count > 1)
                {
                    ContextMenu m = new ContextMenu();
                    MenuItem mi1 = new MenuItem("Удалить фото");
                    mi1.Click += (o, args) =>
                    {
                        Driver.Car.ServiceImage.Remove(Driver.Car.ServiceImage[1]);
                        UpdateImages();
                    };
                    m.MenuItems.Add(mi1);
                    m.Show(pictureBox2, new Point(e.X, e.Y));
                }
            }
        }

        private void PictureBox3OnMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Image image;
                string base64string = OpenImageDialog(out image);
                if (string.IsNullOrEmpty(base64string))
                    return;

                int count = Driver.Car.ServiceImage.Count;
                if (count < 3)
                {
                    ServiceImage si = new ServiceImage { Stream = base64string };
                    Driver.Car.ServiceImage.Add(si);
                    count++;
                }

                Driver.Car.ServiceImage[count >= 3 ? 2 : count - 1].Url = null;
                Driver.Car.ServiceImage[count >= 3 ? 2 : count - 1].Stream = base64string;
                UpdateImages();
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (Driver.Car.ServiceImage.Count > 2)
                {
                    ContextMenu m = new ContextMenu();
                    MenuItem mi1 = new MenuItem("Удалить фото");
                    mi1.Click += (o, args) =>
                    {
                        Driver.Car.ServiceImage.Remove(Driver.Car.ServiceImage[2]);
                        UpdateImages();
                    };
                    m.MenuItems.Add(mi1);
                    m.Show(pictureBox3, new Point(e.X, e.Y));
                }
            }
        }

        private void PictureBox4OnMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Image image;
                string base64string = OpenImageDialog(out image);
                if (string.IsNullOrEmpty(base64string))
                    return;

                int count = Driver.Car.ServiceImage.Count;
                if (count < 4)
                {
                    ServiceImage si = new ServiceImage { Stream = base64string };
                    Driver.Car.ServiceImage.Add(si);
                    count++;
                }

                Driver.Car.ServiceImage[count >= 4 ? 3 : count - 1].Url = null;
                Driver.Car.ServiceImage[count >= 4 ? 3 : count - 1].Stream = base64string;
                UpdateImages();
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (Driver.Car.ServiceImage.Count > 3)
                {
                    ContextMenu m = new ContextMenu();
                    MenuItem mi1 = new MenuItem("Удалить фото");
                    mi1.Click += (o, args) =>
                    {
                        Driver.Car.ServiceImage.Remove(Driver.Car.ServiceImage[3]);
                        UpdateImages();
                    };
                    m.MenuItems.Add(mi1);
                    m.Show(pictureBox4, new Point(e.X, e.Y));
                }
            }
        }

        private void PictureBox5OnMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Image image;
                string base64string = OpenImageDialog(out image);
                if (string.IsNullOrEmpty(base64string))
                    return;

                int count = Driver.Car.ServiceImage.Count;
                if (count < 5)
                {
                    ServiceImage si = new ServiceImage { Stream = base64string };
                    Driver.Car.ServiceImage.Add(si);
                    count++;
                }

                Driver.Car.ServiceImage[count >= 5 ? 4 : count - 1].Url = null;
                Driver.Car.ServiceImage[count >= 5 ? 4 : count - 1].Stream = base64string;
                UpdateImages();
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (Driver.Car.ServiceImage.Count > 4)
                {
                    ContextMenu m = new ContextMenu();
                    MenuItem mi1 = new MenuItem("Удалить фото");
                    mi1.Click += (o, args) =>
                    {
                        Driver.Car.ServiceImage.Remove(Driver.Car.ServiceImage[4]);
                        UpdateImages();
                    };
                    m.MenuItems.Add(mi1);
                    m.Show(pictureBox5, new Point(e.X, e.Y));
                }
            }
        }

        private void UpdateImages()
        {
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.Image = null;
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.Image = null;
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.Image = null;
            pictureBox4.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox4.Image = null;
            pictureBox5.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox5.Image = null;

            foreach (ServiceImage img in Driver.Car.ServiceImage)
            {
                Image image = null;
                if (img.Stream != null)
                {
                    byte[] data = Convert.FromBase64String(img.Stream);

                    using (MemoryStream ms = new MemoryStream(data))
                    {
                        image = Image.FromStream(ms);

                        switch (Driver.Car.ServiceImage.IndexOf(img))
                        {
                            case 0:
                                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                                    pictureBox1.Image = image;
                                break;
                            case 1:
                                    pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
                                    pictureBox2.Image = image;
                                break;
                            case 2:
                                    pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
                                    pictureBox3.Image = image;
                                break;
                            case 3:
                                    pictureBox4.SizeMode = PictureBoxSizeMode.StretchImage;
                                    pictureBox4.Image = image;
                                break;
                            case 4:
                                    pictureBox5.SizeMode = PictureBoxSizeMode.StretchImage;
                                    pictureBox5.Image = image;
                                break;
                        }
                    }
                }
                else
                {
                    CancellationTokenSource source = new CancellationTokenSource();
                    CancellationToken token = source.Token;

                    Task<Image> task = GetImageFromUrl(img.Url, token);
                    task.ContinueWith(t =>
                    {
                        image = t.Result;
                        if (image != null)
                        {
                            switch (Driver.Car.ServiceImage.IndexOf(img))
                            {
                                case 0:
                                    Invoke((MethodInvoker) (() =>
                                    {
                                        pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                                        pictureBox1.Image = image;
                                    }));
                                    break;
                                case 1:
                                    Invoke((MethodInvoker) (() =>
                                    {
                                        pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
                                        pictureBox2.Image = image;
                                    }));
                                    break;
                                case 2:
                                    Invoke((MethodInvoker) (() =>
                                    {
                                        pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
                                        pictureBox3.Image = image;
                                    }));
                                    break;
                                case 3:
                                    Invoke((MethodInvoker) (() =>
                                    {
                                        pictureBox4.SizeMode = PictureBoxSizeMode.StretchImage;
                                        pictureBox4.Image = image;
                                    }));
                                    break;
                                case 4:
                                    Invoke((MethodInvoker) (() =>
                                    {
                                        pictureBox5.SizeMode = PictureBoxSizeMode.StretchImage;
                                        pictureBox5.Image = image;
                                    }));
                                    break;
                            }
                        }
                    });
                }
            }
        }

        private string OpenImageDialog(out Image image)
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
                            image = Image.FromStream(stream);
                            string base64String = Utils.ImageToBase64(image);

                            return base64String;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка. Не могу прочитать файл с диска. " + ex.Message);
                }
            }

            image = null;
            return String.Empty;
        }
    }
}
