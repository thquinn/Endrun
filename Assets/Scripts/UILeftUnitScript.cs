using Assets.Code;
using Assets.Code.Model;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class UILeftUnitScript : TooltipBehavior
{
    public GameObject prefabHPPip, prefabManaPip;
    public SpriteAtlas atlasUnitIcons;
    public Sprite spriteUnitFrameSummoner;

    public RectTransform rtHPPips, rtManaPips;
    public GameObject summonerInfo;
    public Image imageFrame, imageIcon;
    public TextMeshProUGUI tmpName, tmpFocusCost;
    public GridLayoutGroup gridHP;
    // Summoner info.
    public TextMeshProUGUI tmpFocus;

    RectTransform rt;
    public Unit unit;
    GameStateManagerScript gameStateManager;

    List<Image> hpPips, manaPips;

    public void Init(Unit unit) {
        rt = transform as RectTransform;
        this.unit = unit;
        gameStateManager = GameStateManagerScript.instance;
        imageIcon.sprite = atlasUnitIcons.GetSprite(unit.iconID);
        Color c = Util.GetUnitUIColor(unit);
        c.a = imageIcon.color.a;
        imageIcon.color = c;
        if (unit.isSummoner) {
            rt.sizeDelta = new Vector2(100, 220);
            summonerInfo.SetActive(true);
            imageFrame.sprite = spriteUnitFrameSummoner;
        }
        tmpName.text = unit.name;
        tmpFocusCost.text = unit.focusCost == 0 ? "" : unit.focusCost.ToString();
    }
    void Start() {
        hpPips = new List<Image>();
        manaPips = new List<Image>();
        Update();
    }

    void Update() {
        SyncHP();
        SyncSummonerInfo();
    }
    void SyncHP() {
        int pipDelta = unit.hp.y - hpPips.Count;
        for (; pipDelta > 0; pipDelta--) {
            hpPips.Add(Instantiate(prefabHPPip, rtHPPips).GetComponent<Image>());
        }
        for (int i = 0; i > pipDelta; i--) {
            Destroy(hpPips[hpPips.Count - 1].gameObject);
            hpPips.RemoveAt(hpPips.Count - 1);
        }
        for (int i = 0; i < hpPips.Count; i++) {
            hpPips[i].enabled = unit.hp.x > i;
        }
        gridHP.cellSize = hpPips.Count <= 10 ? new Vector2(24, 48) : new Vector2(12, 24);
        gridHP.spacing = hpPips.Count <= 10 ? new Vector2(0, 4) : new Vector2(0, 1.4f);
    }
    void SyncSummonerInfo() {
        if (!unit.isSummoner) return;
        int focusUsed = gameStateManager.gameState.GetTotalAllyFocus();
        int focusMax = gameStateManager.gameState.maxFocus;
        tmpFocus.text = $"{focusMax - focusUsed}/{focusMax}";
        SyncMana();
    }
    void SyncMana() {
        int pipDelta = gameStateManager.gameState.mana.y - manaPips.Count;
        for (; pipDelta > 0; pipDelta--) {
            manaPips.Add(Instantiate(prefabManaPip, rtManaPips).GetComponent<Image>());
        }
        for (int i = 0; i > pipDelta; i--) {
            Destroy(manaPips[manaPips.Count - 1]);
            manaPips.RemoveAt(manaPips.Count - 1);
        }
        for (int i = 0; i < manaPips.Count; i++) {
            manaPips[i].enabled = gameStateManager.gameState.mana.x <= i;
        }
    }

    public override ITooltippableObject GetTooltippableObject() {
        return unit;
    }
}
