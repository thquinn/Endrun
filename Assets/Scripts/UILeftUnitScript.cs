using Assets.Code.Model;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILeftUnitScript : MonoBehaviour
{
    public GameObject prefabHPPip;

    public RectTransform rtHPPips;
    public TextMeshProUGUI tmpName, tmpFocusCost;

    public Unit unit;
    GameStateManagerScript gameStateManager;

    List<Image> hpPips;

    public void Init(Unit unit) {
        this.unit = unit;
        gameStateManager = GameStateManagerScript.instance;
        tmpName.text = unit.name;
        tmpFocusCost.text = unit.focusCost == 0 ? "" : unit.focusCost.ToString();
    }
    void Start() {
        hpPips = new List<Image>();
        Update();
    }

    void Update() {
        SyncHP();
    }
    void SyncHP() {
        int pipDelta = unit.hp.y - hpPips.Count;
        for (; pipDelta > 0; pipDelta--) {
            hpPips.Add(Instantiate(prefabHPPip, rtHPPips).GetComponent<Image>());
        }
        for (int i = 0; i > pipDelta; i--) {
            Destroy(hpPips[hpPips.Count - 1]);
            hpPips.RemoveAt(hpPips.Count - 1);
        }
        for (int i = 0; i < hpPips.Count; i++) {
            hpPips[i].enabled = unit.hp.x > i;
        }
    }
}
