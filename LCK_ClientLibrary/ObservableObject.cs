using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace LCK_ClientLibrary 
{
    [Serializable]
    public class ObservableObject : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        /// Need to implement this interface in order to get data binding
        /// to work properly.
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }

    // More Info: http://softwarebydefault.com/2013/02/10/deep-copy-generics/
    /// <summary>
    /// Extension object to add capability
    /// </summary>
    public static class ExtObject  
    { 
        /// <summary>
        /// Deep Copy Object Extension (Objects to be copied need to be '[Serializable]')
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToCopy"></param>
        /// <returns></returns>
        public static T DeepCopy<T>(this T objectToCopy) 
        { 
             MemoryStream memoryStream = new MemoryStream(); 
             BinaryFormatter binaryFormatter = new BinaryFormatter(); 
             binaryFormatter.Serialize(memoryStream, objectToCopy); 
  
             memoryStream.Position = 0; 
             T returnValue = (T)binaryFormatter.Deserialize(memoryStream); 
  
             memoryStream.Close(); 
             memoryStream.Dispose(); 
  
             return returnValue; 
        } 
    }


}
