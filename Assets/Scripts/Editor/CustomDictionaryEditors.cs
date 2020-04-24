using UnityEditor;

namespace Editor
{
    public class CustomDictionaryEditors
    {
        [CustomPropertyDrawer(typeof(StringAudioDictionary))]
        //[CustomPropertyDrawer(typeof(StringStringDictionary))] // Also add here
        public class AnySerializableDictionaryStoragePropertyDrawer: SerializableDictionaryPropertyDrawer {}
    }
}