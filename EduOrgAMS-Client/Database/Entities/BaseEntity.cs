using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EduOrgAMS.Client.Database.Entities
{
    public abstract class BaseEntity : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this,
                new PropertyChangedEventArgs(propertyName));
        }

        public virtual void OnAllPropertiesChanged()
        {
            PropertyChanged?.Invoke(this,
                new PropertyChangedEventArgs(string.Empty));
        }
    }
}
