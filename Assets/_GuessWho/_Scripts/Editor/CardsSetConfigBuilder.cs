#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GuessWho.Editor
{
    public class CardsSetConfigBuilder
    {
        [MenuItem("Assets/GuessWho/Generate Cards Config", false, 1)]
        private static void BuildCardsConfigFromFolder()
        {
            string folderPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                Debug.LogWarning("Please select a valid folder in the Project window.");
                return;
            }

            string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { folderPath });
            if (guids.Length == 0)
            {
                Debug.LogWarning($"No sprites found in {folderPath}.");
                return;
            }

            string folderName = Path.GetFileName(folderPath);
            string targetDirectory = "Assets/_GuessWho/Resources/Configs/Cards";

            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
                AssetDatabase.Refresh();
            }

            string configPath = $"{targetDirectory}/{folderName}.asset";
            CardsSetConfig config = AssetDatabase.LoadAssetAtPath<CardsSetConfig>(configPath);

            if (config == null)
            {
                config = ScriptableObject.CreateInstance<CardsSetConfig>();
                AssetDatabase.CreateAsset(config, configPath);
            }

            List<CharacterInfo> characterList = new List<CharacterInfo>();

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);

                if (sprite == null) continue;

                string fileName = Path.GetFileNameWithoutExtension(path);
                string index = fileName.ToLowerInvariant().Replace(" ", "-");
                string charName = char.ToUpper(fileName[0]) + fileName.Substring(1);

                characterList.Add(new CharacterInfo
                {
                    Index = index,
                    Name = charName,
                    Image = sprite
                });
            }

            SerializedObject serializedConfig = new SerializedObject(config);

            SerializedProperty charsProp = serializedConfig.FindProperty("_characters");

            if (charsProp != null && charsProp.isArray)
            {
                charsProp.ClearArray();

                for (int i = 0; i < characterList.Count; i++)
                {
                    charsProp.InsertArrayElementAtIndex(i);
                    SerializedProperty element = charsProp.GetArrayElementAtIndex(i);

                    element.FindPropertyRelative("Index").stringValue = characterList[i].Index;
                    element.FindPropertyRelative("Name").stringValue = characterList[i].Name;
                    element.FindPropertyRelative("Image").objectReferenceValue = characterList[i].Image;
                }

                serializedConfig.ApplyModifiedProperties();
                EditorUtility.SetDirty(config);
                AssetDatabase.SaveAssets();

                Debug.Log($"Config '{folderName}' successfully generated/updated at {configPath}. Total cards: {characterList.Count}");
            }
            else
            {
                Debug.LogError("Could not find the '_characters' array in the configuration file.");
            }
        }
    }
}
#endif