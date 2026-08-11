
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System;

public static class DeepCopyHelper
{
    public static T DeepCopy<T>(T obj)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj), "Object to be copied cannot be null.");

        // Perform the deep copy using serialization and deserialization.
        using MemoryStream memoryStream = new MemoryStream();
        IFormatter formatter = new BinaryFormatter();
        formatter.Serialize(memoryStream, obj);
        memoryStream.Seek(0, SeekOrigin.Begin);
        return (T)formatter.Deserialize(memoryStream);
    }
}