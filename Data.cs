using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Media.Imaging;


namespace MediaManager
{
    public class ImageFile
    {
        public ImageFile(string path)
        {
            Path = path;
            Uri = new Uri(Path);
            Image = BitmapFrame.Create(Uri);
        }

        public string Path { get; private set; }
        public Uri Uri { get; private set; }
        public BitmapFrame Image { get; private set; }

        public override string ToString()
        {
            return Path;
        }
    }

    public class PhotoList : ObservableCollection<ImageFile>
    {
        private DirectoryInfo _directory;

        public PhotoList()
        {
        }

        public PhotoList(string path) : this(new DirectoryInfo(path))
        {
        }

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

        public void Update()
        {
            foreach (var f in _directory.GetFiles("*.jpg"))
                Add(new ImageFile(f.FullName));
        }
    }
    
    
    //Database Connection Base

    /*public class PhotoListContext : DbContext
    {
        public PhotoListContext(): base()
        {

        }

        public DbSet<ImageFile> ImageFIles { get; set; }
        

    }*/

    public class PackageType
    {
        public PackageType(string description, double cost)
        {
            Description = description;
            Cost = cost;
        }

        public string Description { get; private set; }
        public double Cost { get; private set; }

        public override string ToString()
        {
            return Description;
        }
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
        private PackageType _packageType;

        private BitmapSource _photo;

        private int _value;

        public PackageBase(BitmapSource photo, PackageType packagetype, int value)
        {
            Photo = photo;
            PackageType = packagetype;
            Value = value;
        }

        public PackageBase(BitmapSource photo, string description, double cost)
        {
            Photo = photo;
            PackageType = new PackageType(description, cost);
            Value = 0;
        }

        public BitmapSource GetPhotoFromPackage()
        {
            return _photo;
        }
        public BitmapSource Photo
        {
            set
            {
                _photo = value;
                OnPropertyChanged("Photo");
            }
            get { return _photo; }
        }

        public PackageType PackageType
        {
            set
            {
                _packageType = value;
                OnPropertyChanged("PackageType");
            }
            get { return _packageType; }
        }

        public int Value
        {
            set
            {
                _value = value;
                OnPropertyChanged("Value");
            }
            get { return _value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
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
        public Package1(BitmapSource photo) : base(photo, " Package1", 100)
        {
        }
    }

    public class Package2 : PackageBase
    {
        public Package2(BitmapSource photo) : base(photo, "Package2", 150)
        {
        }
    }

    public class Package3 : PackageBase
    {
        public Package3(BitmapSource photo) : base(photo, "Package3", 200)
        {
        }
    }

    public class MediaList : ObservableCollection<PackageBase>
    {
    }
}