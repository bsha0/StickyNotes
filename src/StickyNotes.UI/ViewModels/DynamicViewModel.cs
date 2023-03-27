using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace StickyNotes.UI.ViewModels
{
    public sealed class DynamicViewModel<T>
       : DynamicObject, INotifyPropertyChanged where T : class
    {
        public T Model { get; init; }
        private readonly Dictionary<string, Dictionary<string, PropertyInfo>> _properties = new();

        public DynamicViewModel(T model)
        {
            Model = model;
            AddProperties(typeof(T));
        }

        private void AddProperties(Type type)
        {
            var typeName = type.FullName;
            if (_properties.ContainsKey(typeName)) return;

            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            _properties.Add(typeName, props.ToDictionary(prop => prop.Name));
        }

        public override bool TryGetMember(GetMemberBinder binder, out object? result)
        {
            var typeName = typeof(T).FullName;
            if (_properties.TryGetValue(typeName, out var props))
            {
                if (props.TryGetValue(binder.Name, out var pi))
                {
                    result = pi.GetValue(Model);
                    return true;
                }
            }

            result = null;
            return false;
        }

        public override bool TrySetMember(SetMemberBinder binder, object? value)
        {
            var typeName = typeof(T).FullName;
            if (_properties.TryGetValue(typeName, out var props))
            {
                if (props.TryGetValue(binder.Name, out var pi))
                {
                    pi.SetValue(Model, value);
                    NotifyPropertyChanged(binder.Name);
                    return true;
                }
            }

            return false;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
