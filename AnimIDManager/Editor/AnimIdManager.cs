using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// アニメーションに使うIDを簡単に参照できるツール
/// </summary>
public class AnimIdManager : EditorWindow
{
    private const string UNARMED = "Unarmed", SWORD = "Sword", SWORD_RANDOM = "RandomSword", TWOHAND = "2Hand";
    public AnimIdSet animIdSet;
    public AnimIdInfo[] meleeMovementIds, meleeAttackIds, shooterMovementIds, shooterAttackIds;
    private const int MELEE = 0;
    public string[] categories = new string[] { "melee", "shooter" };
    private int currentCategory;
    SerializedObject so;

    [MenuItem("Tools/AnimIdManager")]
    private static void Create() {
        CreateWindow<AnimIdManager>("AnimIdManager");
    }

    private void OnEnable() {
        ScriptableObject target = this;
        so = new SerializedObject(target);
        animIdSet = LoadFile(Application.dataPath + Path.DirectorySeparatorChar + "AnimIDManager" + Path.DirectorySeparatorChar + "AnimIdInfo.json");
        if (animIdSet == null) {
            animIdSet = new AnimIdSet();
            animIdSet.meleeMovementIds = InitIdList("melee", "movement").ToArray();
            animIdSet.meleeAttackIds = InitIdList("melee", "attack").ToArray();
            shooterAttackIds = animIdSet.shooterMovementIds = InitIdList("shooter", "movement").ToArray();
            animIdSet.shooterAttackIds = InitIdList("shooter", "attack").ToArray();
            animIdSet.defenceIds = InitIdList("defence", "").ToArray();
        }

        meleeMovementIds = animIdSet.meleeMovementIds;
        meleeAttackIds = animIdSet.meleeMovementIds;
        shooterMovementIds = animIdSet.shooterMovementIds;
        shooterAttackIds = animIdSet.shooterAttackIds;
        so.Update();
    }

    private void OnDisable() {
        meleeMovementIds = meleeAttackIds = shooterMovementIds = shooterAttackIds = null;
    }

    private void OnGUI() {
        if (animIdSet == null)
            return;
        
        currentCategory = EditorGUILayout.Popup("Melee or Shooter", currentCategory, categories);
        using (new EditorGUILayout.HorizontalScope()) {
            DrawLabels(so, currentCategory == MELEE ? "meleeMovementIds" : "shooterMovementIds", "移動ID");
            DrawLabels(so, currentCategory == MELEE ? "meleeAttackIds" : "shooterAttackIds", "攻撃ID");
        }

        if (GUILayout.Button("Save")) {
            animIdSet.meleeMovementIds = meleeMovementIds;
            animIdSet.meleeMovementIds = meleeAttackIds;
            animIdSet.shooterMovementIds = shooterMovementIds;
            animIdSet.shooterAttackIds = shooterAttackIds;
            SaveFile(animIdSet,
                Application.dataPath + Path.DirectorySeparatorChar + "AnimIDManager" + Path.DirectorySeparatorChar + "AnimIdInfo.json");
        }
    }

    /// <summary>
    /// アニメーションIDリストを表示する
    /// </summary>
    /// <param name="so">リスト表示するためのScriptableObject</param>
    /// <param name="listName">表示したい変数名</param>
    /// <param name="header">ヘッダー</param>
    private void DrawLabels(SerializedObject so, string listName, string header) {
        using (new EditorGUILayout.VerticalScope()) {
            EditorGUILayout.LabelField(header);
            
            EditorGUILayout.PropertyField(so.FindProperty(listName), true);
            so.ApplyModifiedProperties();
        }   
    }
    
    /// <summary>
    /// ファイルにID情報を保存する
    /// </summary>
    /// <param name="animset">アニメーションID設定</param>
    /// <param name="fileLocation">ファイル保存場所</param>
    private void SaveFile(AnimIdSet animset, string fileLocation) {
        string convertedIdSet = JsonUtility.ToJson(animset);
        using (StreamWriter writer = new StreamWriter(fileLocation)) {
            writer.WriteLine(convertedIdSet);
        }
    }

    /// <summary>
    /// ファイルからID情報を読み込む
    /// </summary>
    /// <param name="fileLocation">ファイルの場所</param>
    /// <returns>ファイルから読み取ったID情報</returns>
    private AnimIdSet LoadFile(string fileLocation) {
        if (File.Exists(fileLocation) == false)
            return null;
        string loadIdList;
        using (StreamReader reader = new StreamReader(fileLocation)) {
            loadIdList = reader.ReadToEnd();
        }

        return JsonUtility.FromJson<AnimIdSet>(loadIdList);    
    }

    /// <summary>
    /// 初期表示用リストを作成する
    /// </summary>
    /// <param name="category">Melee / Shooter</param>
    /// <param name="idtype">移動か攻撃IDか</param>
    /// <returns>初期表示用AnimationIdのリスト</returns>
    private List<AnimIdInfo> InitIdList(string category, string idtype) {
        List<AnimIdInfo> ids = new List<AnimIdInfo>();
        /* 近接*/
        if (category.Equals("melee")) {
            if (idtype.Equals("movement")) {
                
                ids.Add(new AnimIdInfo(UNARMED, 1));
                ids.Add(new AnimIdInfo(SWORD, 2));
                ids.Add(new AnimIdInfo(TWOHAND, 3));
                return ids;
            } else {
                ids.Add(new AnimIdInfo(UNARMED, 0));
                ids.Add(new AnimIdInfo(SWORD, 1));
                ids.Add(new AnimIdInfo(SWORD_RANDOM, 2));
                ids.Add(new AnimIdInfo(TWOHAND, 3));
                return ids;
            }
        } else if (category.Equals("shooter")) {
            /* TPS */
            if (idtype.Equals("movement")) {
                ids.Add(new AnimIdInfo(UNARMED, 0));
                ids.Add(new AnimIdInfo("Shooter", 1));
                ids.Add(new AnimIdInfo(SWORD, 2));
                ids.Add(new AnimIdInfo(TWOHAND, 3));
                ids.Add(new AnimIdInfo("Bow", 4));
                return ids;
            } else {
                ids.Add(new AnimIdInfo(UNARMED, 0));
                ids.Add(new AnimIdInfo(SWORD, 1));
                ids.Add(new AnimIdInfo(SWORD_RANDOM, 2));
                ids.Add(new AnimIdInfo(TWOHAND, 4));
                ids.Add(new AnimIdInfo("DualSword", 5));
                return ids;
            }
        } else {
            /* 防御 */
            ids.Add(new AnimIdInfo(UNARMED, 0));
            ids.Add(new AnimIdInfo(SWORD, 1));
            ids.Add(new AnimIdInfo("Shield", 3));
            return ids;
        }

    }
}

[System.Serializable]
public class AnimIdSet {
    public AnimIdInfo[] meleeMovementIds;
    public AnimIdInfo[] shooterMovementIds;
    public AnimIdInfo[] meleeAttackIds;
    public AnimIdInfo[] shooterAttackIds;
    public AnimIdInfo[] defenceIds;
}

[System.Serializable]
public class AnimIdInfo {
    public string name;
    public int id;

    public AnimIdInfo(string name, int id) {
        this.name = name;
        this.id = id;
    }
}
