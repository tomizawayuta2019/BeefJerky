﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージのマネージャーClass
/// </summary>
public class StageManager : SingletonMonoBehaviour<StageManager> {
    public StageChip[,] Stage;//ステージ配列
    [SerializeField]
    private int x, y;//Stageの大きさ
    [SerializeField]//ステージ枠の大きさ ステージの中心位置
    private Vector2 StageSize,DefaultPosition;


    //[SerializeField]
    //private Sprite[] data;//使用するマス画像
    //[SerializeField]
    //private Vector2 spriteSize;//画像の基本サイズ
    [SerializeField]
    private StageChip StageChipPrefab;
    [SerializeField]
    private Vector2 chipSze,offset;
    [SerializeField]
    private List<Character> characterPrefabs;

    // Use this for initialization
    void Start () {
        InitStage();
        CharacterInit();
	}

    /// <summary>
    /// ステージの初期化・生成
    /// </summary>
    private void InitStage()
    {
        Vector2 InitStageSize = StageSize;
        StageSize.x += offset.x;//右端に余白
        Stage = new StageChip[x, y];
        ////ステージの左上の場所を取得
        //Vector2 leftUpSide = new Vector2(-StageSize.x / 2, StageSize.y / 2) + DefaultPosition;//ステージ左上のポジションを取得
        ////マスの大きさを取得
        //Vector2 scale = new Vector2((StageSize.x / ((chipSze.x) * x + offset.x)), StageSize.y / (chipSze.x * y + offset.y));
        //float res = StageSize.x / (chipSze.x * x + offset.x * (x - 1));
        //Debug.Log(res);
        //scale.x = res;

        Vector2 leftUpSide = new Vector2(-StageSize.x / 2, StageSize.y / 2) + DefaultPosition;
        Vector2 scale = new Vector2((InitStageSize.x / ((chipSze.x) * x + offset.x)), (InitStageSize.y / ((chipSze.y) * y + offset.y)));

        

        Vector2 pos = leftUpSide;
        //マスを生成
        for (int i = 0; i < y; i++) {
            pos.x = leftUpSide.x + ((chipSze.x * scale.x) * 0.5f);
            for (int j = 0; j < x; j++) {
                Stage[j, i] = Instantiate(StageChipPrefab, transform);//StageChip.InitStageChip(data[j % data.Length]);
                Stage[j, i].transform.localPosition = pos;
                //次に配置するマスの位置を計算
                pos.x += chipSze.x * scale.x + offset.x;
                Stage[j, i].transform.localScale = scale;
            }
            pos.y -= ((chipSze.x * scale.x)) + offset.y;
        }
    }

    //Characterをマス一杯に生成する初期配置用関数
    private void CharacterInit() {
        foreach (StageChip chips in Stage) {
            Character character =  Instantiate(characterPrefabs[0]);
            character.transform.SetParent(chips.transform);
            character.transform.localPosition = Vector2.zero;
        }
    }

    /// <summary>
    /// 一番下の行のCharacterを全て削除する
    /// </summary>
    public void DeleteDownLineCharacter() {
        int line = Stage.GetLength(0);
        int length = Stage.GetLength(1);
        for (int i = 0; i < length; i++) {
            if (Stage[line, i].stayCharacter) {
                DeleteCharacter(Stage[line, i].stayCharacter);
            }
        }
    }

    private void DeleteCharacter(Character target) {
        Destroy(target);
    }

    public void GravityUpdate() {

    }
}
