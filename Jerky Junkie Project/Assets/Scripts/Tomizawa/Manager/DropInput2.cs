﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropInput2 : SingletonMonoBehaviour<DropInput2> {
    int nowX, nowY;//現在選択中のマス位置
    [SerializeField]
    bool InputFlg, ChangeFlg;
    bool comboFlg, stopper;

    [SerializeField]
    List<Character> saveList = new List<Character>();

    public Color ChangeColor { get {
			if (saveList.Count >= 12) return Color.yellow;
			if (saveList.Count >= 8) return Color.cyan;
			if (saveList.Count >= 4) return Color.black;
			return Color.grey;
		} }
	[SerializeField]
	Sprite[] image;

    private GameObject obj;
    public void ObjectPointerEnter(Character character) {
        obj = character.gameObject;
    }

    private GameObject GetMousePosObj() {
        if (Input.GetMouseButton(0))
            return obj;
        else return null;
    }
    
    private void Update()
    {
        if (Time.timeScale == 0 || PauseManager.Instance.isPause) return;
        /*
        foreach (Character obj in saveList) {
            if (obj.move != null)
                SaveListClear();
        }*/

        if (saveList.Count > 0 && saveList[0] == null) {
            SaveListClear();
        }
        if (Input.GetMouseButton(0))
        {
            RayhitDrop();
        }
        if (Input.GetMouseButtonUp(0))
        {
            InputFlg = ChangeFlg = comboFlg = stopper = false;
            nowX = nowY = 0;

			ComboDestroyCheck ();

            SaveListClear();
        }

		if (!Input.GetMouseButton(0))
		{
			obj = null;
		}
    }

    /// <summary>
    /// 指定されたオブジェクトがマスに配置されているか確認する処理
    /// </summary>
    /// <param name="target"></param>
    /// <returns>マス番号　存在しなければnull</returns>
    public int[] GetCharacterNumber(GameObject target)
    {
        for (int i = 0; i < StageManager.Instance.Stage.GetLength(0); i++)
        {
            for (int j = 0; j < StageManager.Instance.Stage.GetLength(1); j++)
            {
                // ヒットしたオブジェクトと一致していれば実行
                if (StageManager.Instance.Stage[i, j].holdCharacter && StageManager.Instance.Stage[i, j].holdCharacter.gameObject == target)
                {
                    return new int[] { i, j };
                }
            }
            if (ChangeFlg)
                break;
        }
        Debug.Log("none object");
        return null;
    }

    public void RayhitDrop() {
        if (GetMousePosObj() == null) return;
        
        GameObject obj = GetMousePosObj();
        if (obj.tag != ("Drop")) return;

        if (!InputFlg)
        {
            int[] num = GetCharacterNumber(obj);
            if (num != null)
            {
                // ヒットオブジェクトを格納
                //saveList.Clear();
                AddSaveList(StageManager.Instance.Stage[num[0], num[1]].holdCharacter); //saveList.Add(StageManager.Instance.Stage[num[0], num[1]].holdCharacter.gameObject);
                nowX = num[0];
                nowY = num[1];

                InputFlg = true;

				if (CharacterManager2.Instance.getObjFlg (num [0], num [1])) {
					comboFlg = true;
				}
            }
        }
        else
        {
			if (saveList.Count != 0 && !ChangeFlg) {

				// 他のマスに移動していれば(最初の入力で受け取ったオブジェクトと違うオブジェクトを選択していたら)実行 //ドロップの入れ替え処理
				/*if (saveList [0] == obj)
					return;*/

				if (saveList.Count == 0 || ChangeFlg)
					return;
				
				int[] num = GetCharacterNumber (obj);

				// 他のマスに移動していれば実行
				Character findSaveObj = saveList.Find (x => x.gameObject == obj);
				if (findSaveObj != null) {
					CheckSaveObj (findSaveObj);
					return;
				}

				if (num == null)
					return;

				// ヒットしたオブジェクトと一致していれば実行
				if (StageManager.Instance.Stage [num [0], num [1]].holdCharacter.gameObject != obj)
					return;

				GameObject hitObj = GetMousePosObj ();
				///////
				// コンボ中に削除対象から外れてしまったときの処理
				if (stopper) {
					//最後に選択されたキャラクターを再び選択したら、処理を再開する
					if (hitObj == saveList [saveList.Count - 1]) {
						stopper = false;
					}
				} else {

					// 削除対象オブジェクトを選択していれば実行
					if (CharacterManager2.Instance.getObjFlg (num [0], num [1]) && comboFlg) {
                        AddSaveList(StageManager.Instance.Stage[num[0], num[1]].holdCharacter);//saveList.Add (StageManager.Instance.Stage [num [0], num [1]].holdCharacter.gameObject);
							//Debug.Log (obj);
						}
                     // コンボ中に削除対象から外れてしまったら実行
                    else if (!CharacterManager2.Instance.getObjFlg (num [0], num [1]) && comboFlg) {
						//if (saveList.Count > 1) {
							ComboDestroyCheck ();

                            SaveListClear();//saveList.Clear ();

							stopper = true;
						//} else {
						//	CharacterManager2.Instance.DirectionObjMove (nowX, nowY, num [0], num [1]);

						//}
					 }
                      // 削除対象以外を選択時実行
                      else{

						//CharacterManager2.Instance.DirectionObjMove (nowX, nowY, num [0], num [1]);

						//Debug_CharacterDataLog ();
						//CharaManager.CombinationSearch (i, j);
						//CharaManager.CombinationSearch (nowX, nowY);

						//CharaManager.search (SaveObj [0].m_SpriteNum, ref SaveObj, i, j);
						//CharaManager.search (CharaManager.CharacterInstance [numX, numY].m_SpriteNum, ref SaveObj, numX, numY);

						ChangeFlg = true;
					}
				}
			}
        }
    }

	void ComboDestroyCheck()
	{
        CharacterManager2.Instance.Combination(saveList, DropType.usi);
	}

    void CheckSaveObj(Character target)
    {
        // 保存されているオブジェクトの数が2つあれば実行
        if (saveList.Count >= 2) {
            // 最後のほうから探索
            int num = saveList.FindIndex(x => x == target);
            for (int i = num + 1; i < saveList.Count; i++) {
                RemoveColorChange(saveList[i]);
            }
			for (int i = num + 1; i < saveList.Count; i++) {
				RemoveSaveList(saveList[i]);
			}
            //saveList.RemoveRange(num + 1, saveList.Count - (num + 1));

        }
    }

	private void UpdateColor()
	{
		foreach (Character obj in saveList) {
			ColorChange(obj);
		}
	}

    private void RemoveSaveList(Character target) {
		if (saveList.Remove(target)) {
			UpdateColor();
			RemoveColorChange(target);
		}
            
    }

    private void AddSaveList(Character target) {
        saveList.Add(target);
		UpdateColor();
	}

    private void SaveListClear() {
        foreach (Character obj in saveList) {
            RemoveColorChange(obj);
        }
        saveList.Clear();
    }

    private Character ColorChange(Character target) {
        if (!target) return null;

		target.GetComponent<Image>().color = ChangeColor;
		if(target.data.m_DropType == DropType.usi)
		{
			target.GetComponent<BeffeLineFace>().BeffeLineStart();
		}
	
		return target;
    }

    private Character RemoveColorChange(Character target) {
        if (!target) return null;

		target.GetComponent<Image>().color = Color.white;
		if (target.data.m_DropType == DropType.usi)
		{
			target.GetComponent<BeffeLineFace>().BeffeLineEnd();
		}
		return target;
    }

    /**/
}
