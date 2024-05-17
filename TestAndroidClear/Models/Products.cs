using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TestAndroidClear.Models
{
    public class Products : INotifyPropertyChanged
    {
        private string _ingName;
        private bool _isSelected;

        public int ID { get; set; }

        public string IngName
        {
            get => _ingName;
            set
            {
                if (_ingName != value)
                {
                    _ingName = value;
                    OnPropertyChanged(nameof(IngName));
                }
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
