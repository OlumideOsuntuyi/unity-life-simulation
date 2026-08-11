using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting.FullSerializer;

public static class FileHandler
{
    // Function to save an object to a file at the specified path
    public static int SaveObject(object obj, string filePath, bool debug = true)
    {
        try
        {
            // Ensure that the directory structure exists
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                if(debug)
                    Debug.Log($"Previous file at {filePath} overwritten.");
            }

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fileStream, obj);
            }
            if (debug)
                Debug.Log($"Object saved to {filePath}");
            return 0; // Successfully saved
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error saving object: {e.Message}");
            return 1; // Failed to save
        }
    }

    // Function to check if a file exists at the specified path
    public static bool FileExists(string filePath)
    {
        return File.Exists(filePath);
    }
    // Function to load an object from a file at the specified path
    public static T LoadObject<T>(string filePath, bool debug = true)
    {
        try
        {
            if (File.Exists(filePath))
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                {
                    if (fileStream.Length == 0)
                    {
                        Debug.LogWarning($"File at {filePath} is empty. Creating a new object.");
                        T newObject = Activator.CreateInstance<T>();
                        SaveObject(newObject, filePath, debug); // Save the new object to the path
                        return newObject;
                    }

                    IFormatter formatter = new BinaryFormatter();
                    object obj = formatter.Deserialize(fileStream);
                    if (obj is T result)
                    {
                        if(debug)
                            Debug.Log($"Object loaded from {filePath}");
                        return result;
                    }
                    else
                    {
                        Debug.LogError($"Failed to cast loaded object to the specified type.");
                    }
                }
            }
            else
            {
                Debug.LogWarning($"File not found at {filePath}. Creating a new object.");
                T newObject = Activator.CreateInstance<T>();
                SaveObject(newObject, filePath, debug); // Save the new object to the path
                return newObject;
            }
        }
        catch (System.Exception e)
        {
            DeleteObject(filePath);
            Debug.LogError($"Error loading object: {e.Message}");
        }

        return default(T); // Return null for load failure
    }
    public static bool DeleteObject(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log($"File at {filePath} deleted.");
                return true; // Deletion successful
            }
            else
            {
                Debug.LogWarning($"File not found at {filePath}. No file deleted.");
                return false; // File not found, no deletion performed
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error deleting file or object: {e.Message}");
            return false; // Deletion failed
        }
    }
    public static List<T> LoadObjectsFromDirectory<T>(string directoryPath, string type, bool debug = false)
    {
        List<T> objects = new List<T>();

        try
        {
            // Check if the directory exists
            if (!Directory.Exists(directoryPath))
            {
                Debug.Log("Directory does not exist.");
                return objects;
            }

            // Get all XML files in the directory
            string[] files = Directory.GetFiles(directoryPath, $"*.{type}");

            // Deserialize objects from each file
            foreach (string file in files)
            {
                T obj = LoadObject<T>(file, debug);
                objects.Add(obj);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error loading objects from directory: {ex.Message}");
        }

        return objects;
    }
    public static long GetFileSize(string filePath)
    {
        try
        {
            FileInfo fileInfo = new FileInfo(filePath);
            return fileInfo.Length;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error getting file size: {e.Message}");
            return -1; // Return -1 to indicate an error
        }
    }
}
