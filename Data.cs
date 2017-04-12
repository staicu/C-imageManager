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
           
        }

        public override string ToString()
        {
            return Path;
        }

        private String _path;

        public String Path
        {
            get { return _path; }
        }

    }

    public class MediaList : ObservableCollection<ImageFile>
    {


        public string Path
        {
            set
            {
                _directory = new DirectoryInfo(value);
                Update();
            }
            get { return _directory.FullName; }
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
        
        public override string ToString()
        {
            return _description;
        }
        private string _description;
 
    }


}