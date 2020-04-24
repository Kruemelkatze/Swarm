using System;
using General;
using UnityEditor;

[Serializable]
public class StringAudioDictionary : SerializableDictionary<string, Audio> {}
// public class StringStringDictionary: SerializeableDictionary<string, string> {} // Add here

// Also add to ../Editor/CustomDictionaryEditors.cs
