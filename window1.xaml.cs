#define VISUALCHILD

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Input;
using System.Collections;
using System.IO;

namespace MediaManager
{
    
    public partial class Window1 : Window
    {
        public static Window1 Editor = null;
        public Window1()
        {
            InitializeComponent();
            UndoStack = new Stack();
            Editor = this;
        }


        private void WindowLoaded(object sender, EventArgs e)
        {
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(CurrentPhoto);
            CropSelector = new RubberbandAdorner(CurrentPhoto);
            CropSelector.Window = this;
            layer.Add(CropSelector);
#if VISUALCHILD
            CropSelector.Rubberband.Visibility = Visibility.Hidden;
#endif
#if NoVISUALCHILD
            CropSelector.ShowRect = false;
#endif
        }

        public PhotoList Photos;
        public MediaList ShoppingCart;
        private Stack UndoStack;
        private RubberbandAdorner CropSelector;

        public void ClearPhotoList()
        {
           this.Photos.Clear(); 
        }
        private void PhotoListSelection(object sender, RoutedEventArgs e)
        {
            String path = ((sender as ListBox).SelectedItem.ToString());
            BitmapSource img = BitmapFrame.Create(new Uri(path));
            CurrentPhoto.Source = img;
            ClearUndoStack();
            if (CropSelector != null)
            {
#if VISUALCHILD
                if (Visibility.Visible == CropSelector.Rubberband.Visibility)
                    CropSelector.Rubberband.Visibility = Visibility.Hidden;
#endif
#if NoVISUALCHILD
                if (CropSelector.ShowRect)
                    CropSelector.ShowRect=false;
#endif
            }
            CropButton.IsEnabled = false;
        }

        private void AddToShoppingCart(object sender, RoutedEventArgs e)
        {
            if (PackageAddTypeComboBox.SelectedItem != null)
            {
                PackageBase item;
                switch (PackageAddTypeComboBox.SelectedIndex)
                {
                    case 0:
                        item = new Package1(CurrentPhoto.Source as BitmapSource); break;
                    case 1:
                        item = new Package2(CurrentPhoto.Source as BitmapSource); break;
                    case 2:
                        item = new Package3(CurrentPhoto.Source as BitmapSource); break;
                    default:
                        return;
                }
                ShoppingCart.Add(item);
                ShoppingCartListBox.ScrollIntoView(item);
                ShoppingCartListBox.SelectedItem = item;
                if (false == UploadButton.IsEnabled)
                    UploadButton.IsEnabled = true;
                if (false == RemoveButton.IsEnabled)
                    RemoveButton.IsEnabled = true;
            }
        }

        private void RemoveShoppingCartItem(object sender, RoutedEventArgs e)
        {
            if (null != ShoppingCartListBox.SelectedItem)
            {
                PackageBase item = ShoppingCartListBox.SelectedItem as PackageBase;
                ShoppingCart.Remove(item);
                ShoppingCartListBox.SelectedIndex = ShoppingCart.Count - 1;
            }
            if (0 == ShoppingCart.Count)
            {
                RemoveButton.IsEnabled = false;
                UploadButton.IsEnabled = false;
            }
        }

        public void RemoveListItem(object sender, RoutedEventArgs e)
        {
            if (null != PhotoListBox.SelectedItem)
            {
                var item = PhotoListBox.SelectedItem as ImageFile;
                Photos.Remove(item);
                PhotoListBox.SelectedIndex = Photos.Count - 1;
            }
          
        }
        private void Upload(object sender, RoutedEventArgs e)
        {
            if (ShoppingCart.Count > 0)
            {
                TimeSpan scaleDuration = new TimeSpan(0, 0, 0, 0, ShoppingCart.Count * 200);
                DoubleAnimation ProgressAnimation = new DoubleAnimation(0, 100, scaleDuration, FillBehavior.Stop);
                UploadProgressBar.BeginAnimation(ProgressBar.ValueProperty, ProgressAnimation);
               // MessageBox.Show(ShoppingCart.Count.ToString());

                foreach (var imageCart in ShoppingCart)
                {
                    BitmapSource imageForCust = imageCart.Photo;
                    CurrentPhoto.Source = imageForCust;
                    MessageBox.Show(imageForCust.ToString());

                    using (var fileStream = new FileStream("..\\..\\photosCust", FileMode.Create))
                    {
                        BitmapEncoder encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(imageForCust));
                        encoder.Save(fileStream);
                    }
                  
                }
                ShoppingCart.Clear();
                UploadButton.IsEnabled = false;
                if (true == RemoveButton.IsEnabled)
                    RemoveButton.IsEnabled = false;
            }
        }

        private void Rotate(object sender, RoutedEventArgs e)
        {
            if (CurrentPhoto.Source != null)
            {
                BitmapSource img = (BitmapSource)(CurrentPhoto.Source);
                UndoStack.Push(img);
                CachedBitmap cache = new CachedBitmap(img, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                CurrentPhoto.Source = new TransformedBitmap(cache, new RotateTransform(90.0));
                if (false == UndoButton.IsEnabled)
                    UndoButton.IsEnabled = true;
                if (CropSelector != null)
                {
#if VISUALCHILD
                    if (Visibility.Visible == CropSelector.Rubberband.Visibility)
                        CropSelector.Rubberband.Visibility = Visibility.Hidden;
#endif
#if NoVISUALCHILD
                if (CropSelector.ShowRect)
                    CropSelector.ShowRect=false;
#endif
                }
                CropButton.IsEnabled = false;
            }
        }


        private void BlackAndWhite(object sender, RoutedEventArgs e)
        {
            if (CurrentPhoto.Source != null)
            {
                BitmapSource img = (BitmapSource)(CurrentPhoto.Source);
                UndoStack.Push(img);
                CurrentPhoto.Source = new FormatConvertedBitmap(img, PixelFormats.Gray8, BitmapPalettes.Gray256, 1.0);
                if (false == UndoButton.IsEnabled)
                    UndoButton.IsEnabled = true;
                if (CropSelector != null)
                {
#if VISUALCHILD
                    if (Visibility.Visible == CropSelector.Rubberband.Visibility)
                        CropSelector.Rubberband.Visibility = Visibility.Hidden;
#endif
#if NoVISUALCHILD
                    if (CropSelector.ShowRect)
                        CropSelector.ShowRect = false;
#endif
                }
                CropButton.IsEnabled = false;
            }
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point anchor = e.GetPosition(CurrentPhoto);
            CropSelector.CaptureMouse();
            CropSelector.StartSelection(anchor);
            CropButton.IsEnabled = true;
        }

        private void Crop(object sender, RoutedEventArgs e)
        {
            if (CurrentPhoto.Source != null)
            {
                BitmapSource img = (BitmapSource)(CurrentPhoto.Source);
                UndoStack.Push(img);
                Int32Rect rect = new Int32Rect();
                rect.X = (int)(CropSelector.SelectRect.X * img.PixelWidth / CurrentPhoto.ActualWidth);
                rect.Y = (int)(CropSelector.SelectRect.Y * img.PixelHeight / CurrentPhoto.ActualHeight);
                rect.Width = (int)(CropSelector.SelectRect.Width * img.PixelWidth / CurrentPhoto.ActualWidth);
                rect.Height = (int)(CropSelector.SelectRect.Height * img.PixelHeight / CurrentPhoto.ActualHeight);
                CurrentPhoto.Source = new CroppedBitmap(img, rect);
#if VISUALCHILD
                if (Visibility.Visible == CropSelector.Rubberband.Visibility)
                    CropSelector.Rubberband.Visibility = Visibility.Hidden;
#endif
#if NoVISUALCHILD
                if (CropSelector.ShowRect)
                    CropSelector.ShowRect = false;
#endif
                CropButton.IsEnabled = false;
                if (false == UndoButton.IsEnabled)
                    UndoButton.IsEnabled = true;
            }
        }

        private void Undo(object sender, RoutedEventArgs e)
        {
            if (UndoStack.Count > 0)
                CurrentPhoto.Source = (BitmapSource)UndoStack.Pop();
            if (0 == UndoStack.Count)
                UndoButton.IsEnabled = false;
#if VISUALCHILD
            if (Visibility.Visible == CropSelector.Rubberband.Visibility)
                CropSelector.Rubberband.Visibility = Visibility.Hidden;
#endif
#if NoVISUALCHILD
            if (CropSelector.ShowRect)
                CropSelector.ShowRect = false;
#endif

        }

        private void ClearUndoStack()
        {
            UndoStack.Clear();
            UndoButton.IsEnabled = false;
        }

        private void BtSelector_Click(object sender, RoutedEventArgs e)
        {
            Window1.Editor.Hide();
            Window2.Selector.Show();
        }

        
    }
}