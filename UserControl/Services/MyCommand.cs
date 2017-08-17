using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace XmlCompare.UseControl.Services
{
    class MyCommand<T>:ICommand
    {
        readonly Action<T> _executeMethod;

        Func<T, bool> _canExecuteMethod;

        public MyCommand(Action<T> executeMethod)
            : this(executeMethod, (o) => true)
        {
        }

        public MyCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
        {
            if (executeMethod == null || canExecuteMethod == null)
                throw new ArgumentNullException(nameof(executeMethod), "Delegate Command Delegates Cannot Be Null");

            TypeInfo genericTypeInfo = typeof(T).GetTypeInfo();

            // DelegateCommand allows object or Nullable<>.  

            // note: Nullable<> is a struct so we cannot use a class constraint.

            if (genericTypeInfo.IsValueType)

            {

                if ((!genericTypeInfo.IsGenericType) || (!typeof(Nullable<>).GetTypeInfo().IsAssignableFrom(genericTypeInfo.GetGenericTypeDefinition().GetTypeInfo())))

                {

                    throw new InvalidCastException("Delegate Command Invalid Generic Payload Type");

                }

            }
            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _canExecuteMethod((T)parameter);
        }

        public void Execute(object parameter)
        {
            _executeMethod((T)parameter);
        }
    }
}
