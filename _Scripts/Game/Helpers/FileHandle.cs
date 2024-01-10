using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

public static class FileHandle
{
    // Encrypt, Decrypt
    private const int lineLenght = 20;
    private static readonly bool isEncrypt = true;
    private static string Encrypt(string _jsonText)
    {
        var _bytes = Encoding.UTF8.GetBytes(_jsonText);
        var _encodedText = Convert.ToBase64String(_bytes);
        var sb = new StringBuilder();
        for (var i = 0; i < _encodedText.Length; i+= lineLenght)
        {
            var _lenght = Mathf.Min(lineLenght, _encodedText.Length - i);
            sb.AppendLine(_encodedText.Substring(i, _lenght));
        }
        return sb.ToString();
    }
    private static string Decrypt(string _encryptText)
    {
        var _bytes = Convert.FromBase64String(_encryptText);
        var _decryptText = Encoding.UTF8.GetString(_bytes);
        return _decryptText; 
    }
    
    
    public static void Save<T>(T _data, string _fileName)
    {        
        if (string.IsNullOrEmpty(_fileName)) return;
        var _path = Path.Combine(Application.persistentDataPath, _fileName);
        
        SaveToFile(_path, _data);
    }
    public static void Save<T>(T _data, string _folder, string _fileName)
    {
        if (string.IsNullOrEmpty(_fileName)) return;
        var _path = Path.Combine(Application.persistentDataPath, _folder, _fileName);
        Directory.CreateDirectory(Path.GetDirectoryName(_path) ?? string.Empty);
        
        SaveToFile(_path, _data);
    }
    private static void SaveToFile<T> (string _path, T _data)
    {
        var jsonText = JsonUtility.ToJson(_data, true);
        
        File.WriteAllText(_path, isEncrypt ? Encrypt(jsonText) : jsonText);
    }
    private static void SaveToFile_JSON<T> (string _path, T _data)
    {
        var jsonText = JsonConvert.SerializeObject(_data, Formatting.Indented);
        File.WriteAllText(_path, isEncrypt ? Encrypt(jsonText) : jsonText);
    }
    
    public static bool Load<T>(string _fileName, out T _data)
    {
        var _path = Path.Combine(Application.persistentDataPath, _fileName);
        return LoadFromFile(_path,out _data);
    }
    public static bool Load<T>(string _folder, string _fileName, out T _data)
    {
        var _path = Path.Combine(Application.persistentDataPath, _folder, _fileName);
        return LoadFromFile(_path, out _data);
    }
    private static bool LoadFromFile<T>(string _path, out T _data)
    {
        if (!File.Exists(_path))
        {
            _data = default;
            return false;
        }
        var _jsonText = File.ReadAllText(_path);
        _data = JsonUtility.FromJson<T>(isEncrypt ? Decrypt(_jsonText) : _jsonText);
        return true;
    }
    private static bool LoadFromFile_JSON<T>(string _path, out T _data)
    {
        if (!File.Exists(_path))
        {
            _data = default;
            return false;
        }
        var _jsonText = File.ReadAllText(_path);
        _data = JsonConvert.DeserializeObject<T>(isEncrypt ? Decrypt(_jsonText) : _jsonText);
        return true;
    }
    
    public static void Delete(string _fileName)
    {
        var _path = Path.Combine(Application.persistentDataPath, _fileName);
        if (File.Exists(_path))
            File.Delete(_path);
    }
    public static void Delete(string _folder, string _fileName)
    {
        var _path = Path.Combine(Application.persistentDataPath, _folder, _fileName);
        if (File.Exists(_path)) 
            File.Delete(_path);
    }
    
}
