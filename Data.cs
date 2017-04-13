using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Collections.Specialized;
using System.Windows.Controls;


namespace MediaManager
{
    public class ImageFile
    {
        public ImageFile(string path)
        {
            _path = path;
            _uri = new Uri(_path);
            _image = BitmapFrame.Create(_uri);
        }

        public override string ToString()
        {
            return Path;
        }

        private String _path;
        public String Path { get { return _path; } }
        private Uri _uri;
        public Uri Uri { get { return _uri; } }
        private BitmapFrame _image;
        public BitmapFrame Image { get { return _image; } }
    }

    public class PhotoList : ObservableCollection<ImageFile>
    {
        public PhotoList() { }

        public PhotoList(string path) : this(new DirectoryInfo(path)) { }

        public PhotoList(DirectoryInfo directory)
        {
            _directory = directory;
            Update();
        }

        public string Path
        {
            set
            {
                _directory = new DirectoryInfo(value);
                Update();
            }
            get { return _directory.FullName; }
        }

        public DirectoryInfo Directory
        {
            set
            {
                _directory = value;
                Update();
            }
            get { return _directory; }
        }
        private void Update()
        {
            foreach (FileInfo f in _directory.GetFiles("*.jpg"))
            {
                Add(new ImageFile(f.FullName));
            }
        }

        DirectoryInfo _directory;
    }

    public class PackageType
    {
        public PackageType(string description, double cost)
        {
            _description = description;
            _cost = cost;
        }

        public override string ToString()
        {
            return _description;
        }

        private string _description;
        public String Description {get {return _description; }}

        private double _cost;
        public double Cost { get { return _cost; } }
    }

    public class PackageTypeList : ObservableCollection<PackageType>
    {
        public PackageTypeList()
        {
            Add(new PackageType("Pack1", 100));
            Add(new PackageType("Pack2", 150));
            Add(new PackageType("Pack3", 200));
        }
    }

    public class PackageBase : INotifyPropertyChanged
    {
        public PackageBase(BitmapSource photo, PackageType printtype, int quantity)
        {
            Photo = photo;
            PackageType = printtype;
            Quantity = quantity;
        }

        public PackageBase(BitmapSource photo, string description, double cost)
        {
            Photo = photo;
            PackageType = new PackageType(description, cost);
            Quantity = 0;
        }

        private BitmapSource _photo;
        public BitmapSource Photo
        {
            set { _photo = value; OnPropertyChanged("Photo"); }
            get { return _photo; }
        }

        private PackageType _packageType;
        public PackageType PackageType
        {
            set { _packageType = value; OnPropertyChanged("PackageType"); }
            get { return _packageType; }
        }

        private int _quantity;
        public int Quantity
        {
            set { _quantity = value; OnPropertyChanged("Quantity"); }
            get { return _quantity; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }

        public override string ToString()
        {
            return PackageType.ToString();
        }
    }

    public class Package1 : PackageBase 
    {
        public Package1(BitmapSource photo) : base(photo, " Package1",100) { }
    }

    public class Package2 : PackageBase
    {
        public Package2(BitmapSource photo) : base(photo, "Package2", 150) { }
    }

    public class Package3 : PackageBase
    {
        public Package3(BitmapSource photo) : base(photo, "Package3", 200) { }
    }

    public class MediaList : ObservableCollection<PackageBase> { }


}
