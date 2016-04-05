using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class AbstractModel {
    
    // Deep clone -- modified from http://www.codeproject.com/KB/tips/SerializedObjectCloner.aspx
    public T Clone<T>() {
        if (!typeof(T).IsSerializable)
        {
            Debug.Log("The type must be serializable.");
        }

        // Don't serialize a null object, simply return the default for that object
        if (Object.ReferenceEquals(this, null)) {
            return default(T);
        }

        IFormatter formatter = new BinaryFormatter();
        Stream stream = new MemoryStream();
        using (stream) {
            formatter.Serialize(stream, this);
            stream.Seek(0, SeekOrigin.Begin);
            return (T)formatter.Deserialize(stream);
        }
    }
}