using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Data;
using System.IO;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Image = System.Windows.Controls.Image;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Data;

namespace MediaManager
{
    public partial class Window2 : Window
    {
        DataSet ds;
        private string strName, imageName, pathToFileToDeleteFromList;

        string constr = @"Server=tcp:alireza-abbott.database.windows.net,1433;Initial Catalog=MediaManager;
Persist Security Info=False;User ID=dbadmin;Password=Alirezal2017;MultipleActiveResultSets=False;
Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public static Window2 Selector = null;
        public Window2()
        {
            InitializeComponent();
            Selector = this;
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FileDialog fldlg = new OpenFileDialog();
                fldlg.InitialDirectory = Environment.SpecialFolder.MyPictures.ToString();
                fldlg.Filter = "Image File (*.jpg;*.bmp;*.gif)|*.jpg;*.bmp;*.gif";
                fldlg.ShowDialog();
                {
                    strName = fldlg.SafeFileName;
                    imageName = fldlg.FileName;
                    ImageSourceConverter isc = new ImageSourceConverter();
                    image1.SetValue(Image.SourceProperty, isc.ConvertFromString(imageName));
                }
                fldlg = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            insertImageData();
        }

        private void btnShow_Click(object sender, RoutedEventArgs e)
        {


            DataTable dataTable = ds.Tables[0];

            foreach (DataRow row in dataTable.Rows)
            {
                if (row[0].ToString() == lvPictureResult.SelectedItem.ToString())
                {
                    //Store binary data read from the database in a byte array
                    byte[] blob = (byte[])row[1];
                    MemoryStream stream = new MemoryStream();
                    stream.Write(blob, 0, blob.Length);
                    stream.Position = 0;

                    System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();

                    MemoryStream ms = new MemoryStream();
                    img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                    ms.Seek(0, SeekOrigin.Begin);
                    bi.StreamSource = ms;
                    bi.EndInit();
                    image2.Source = bi;

                }
            }
        }

        private void insertImageData()
        {
            try
            {
                if (imageName != "")
                {
                    //Initialize a file stream to read the image file
                    FileStream fs = new FileStream(imageName, FileMode.Open, FileAccess.Read);

                    //Initialize a byte array with size of stream
                    byte[] imgByteArr = new byte[fs.Length];

                    //Read data from the file stream and put into the byte array
                    fs.Read(imgByteArr, 0, Convert.ToInt32(fs.Length));

                    //Close a file stream
                    fs.Close();

                    using (SqlConnection conn = new SqlConnection(constr))
                    {
                        conn.Open();
                        string sql = "insert into tbl_Image(id,img) values('" + strName + "',@img)";
                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            //Pass byte array into database
                            cmd.Parameters.Add(new SqlParameter("img", imgByteArr));
                            int result = cmd.ExecuteNonQuery();
                            if (result == 1)
                            {
                                TimeSpan scaleDuration = new TimeSpan(0, 0, 0, 0, 1000);
                                DoubleAnimation ProgressAnimation = new DoubleAnimation(0, 100, scaleDuration, FillBehavior.Stop);
                                ProgressBar2.BeginAnimation(ProgressBar.ValueProperty, ProgressAnimation);

                                MessageBox.Show("Image added successfully.");
                                BindImageList();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BindImageList()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(constr))
                {
                    conn.Open();

                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM tbl_Image", conn))
                    {
                        ds = new DataSet("myDataSet");
                        adapter.Fill(ds);
                        DataTable dt = ds.Tables[0];

                        lvPictureResult.Items.Clear();

                        foreach (DataRow dr in dt.Rows)
                            lvPictureResult.Items.Add(dr["id"].ToString());

                        lvPictureResult.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSave_Image_Click(object sender, RoutedEventArgs e)
        {

            TimeSpan scaleDuration = new TimeSpan(0, 0, 0, 0, 1000);
            DoubleAnimation ProgressAnimation = new DoubleAnimation(0, 100, scaleDuration, FillBehavior.Stop);
            ProgressBar1.BeginAnimation(ProgressBar.ValueProperty, ProgressAnimation);


            DataTable dataTable = ds.Tables[0];

            foreach (DataRow row in dataTable.Rows)
            {
                if (row[0].ToString() == lvPictureResult.SelectedItem.ToString())
                {
                    //Store binary data read from the database in a byte array
                    byte[] blob = (byte[])row[1];
                    MemoryStream stream = new MemoryStream();
                    stream.Write(blob, 0, blob.Length);
                    stream.Position = 0;

                    System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();

                    MemoryStream ms = new MemoryStream();
                    img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                    ms.Seek(0, SeekOrigin.Begin);
                    bi.StreamSource = ms;
                    bi.EndInit();
                    image2.Source = bi;
                    //save file image to directory
                    try
                    {
                        if (img != null)
                        {
                            img.Save("..\\..\\PhotosSaved\\" + lvPictureResult.SelectedItem.ToString());
                            MessageBox.Show("Image saved successfully.");
                            Window1.Editor.ClearPhotoList();
                            Window1.Editor.Photos.Update();


                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("There was a problem saving the file." +
                                        "Check the file permissions.");
                    }


                }

            }

        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // ... Get TabControl reference.
            var item = sender as TabControl;
            // ... Set Title to selected tab header.
            var selected = item.SelectedItem as TabItem;
            this.Title = selected.Header.ToString();
        }


        private void BindImageListView()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(constr))
                {
                    conn.Open();

                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM tbl_Image", conn))
                    {
                        ds = new DataSet("myDataSet");
                        adapter.Fill(ds);
                        DataTable dt = ds.Tables[0];

                        lvPictureResult.Items.Clear();

                        foreach (DataRow dr in dt.Rows)
                            lvPictureResult.Items.Add(dr["id"].ToString());

                        lvPictureResult.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnBindList1_Click(object sender, RoutedEventArgs e)
        {
            BindImageListView();
        }
        private void btnBindList2_Click(object sender, RoutedEventArgs e)
        {
            BindImageListView1();
        }

        private void BindImageListView1()
        {

            string query = tbPictureSearch.Text;
            try
            {
                using (SqlConnection conn = new SqlConnection(constr))
                {
                    conn.Open();

                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM tbl_Image WHERE id LIKE '%" + query + "%'", conn))
                    {
                        ds = new DataSet("myDataSet");
                        adapter.Fill(ds);
                        DataTable dt = ds.Tables[0];

                        lvPictureResult.Items.Clear();

                        foreach (DataRow dr in dt.Rows)
                            lvPictureResult.Items.Add(dr["id"].ToString());

                        lvPictureResult.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        public void btnLoadWin1_Click(object sender, System.Windows.RoutedEventArgs e)
        {


        }

        private void Editor_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Window1.Editor.Show();
            Window2.Selector.Hide();
        }

        private void Remove_Image_From_List_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //String path = ((sender as ListBox).SelectedItem.ToString());
            //MessageBox.Show(path);
        }
        private void PhotoListSelection(object sender, RoutedEventArgs e)
        {
            String path = ((sender as ListBox).SelectedItem.ToString());
            BitmapSource img = BitmapFrame.Create(new Uri(path));
            this.popup.IsOpen = true; //opens our custom msgbox

            image3.Source = img;

            pathToFileToDeleteFromList = path;
            // MessageBox.Show(path);
        }

        private void Show_Click(object sender, RoutedEventArgs e)
        {
            MyPopup.IsOpen = true;
        }

        private void Hide_Click(object sender, RoutedEventArgs e)
        {
            MyPopup.IsOpen = false;
            popup.IsOpen = false;

        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // MessageBox.Show(pathToFileToDeleteFromList);
            TbUrl.Text = pathToFileToDeleteFromList;
            //File.Delete(pathToFileToDeleteFromList);

        }

        private void Remove_File_From_List_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (null != PhotoListBox.SelectedItem)
            {
                var item = PhotoListBox.SelectedItem as ImageFile;
                Window1.Editor.Photos.Remove(item);
                PhotoListBox.SelectedIndex = Window1.Editor.Photos.Count - 1;



                //image3.Source = Null;

           
            }
        }

       

    }

}
