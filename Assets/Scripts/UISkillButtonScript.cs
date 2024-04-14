using Assets.Code.Model.Skills;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class UISkillButtonScript : MonoBehaviour
{
    public SpriteAtlas atlasSkillIcons;

    public Image back, icon;
    public TextMeshProUGUI tmpHotkey;

    Skill skill;
    KeyCode hotkey;
    bool hovered;
    float backInitialAlpha, vBackAlpha;

    public void Init(Skill skill, KeyCode hotkey) {
        this.skill = skill;
        this.hotkey = hotkey;
        icon.sprite = atlasSkillIcons.GetSprite(skill.GetIconID());
        tmpHotkey.text = hotkey.ToString();
    }
    void Start() {
        if (skill.type == SkillType.Passive) {
            back.enabled = false;
        } else {
            backInitialAlpha = back.color.a;
            Color c = back.color;
            c.a = 0;
            back.color = c;
        }
    }

    void Update() {
        Color c = back.color;
        bool highlight = hovered || GameStateManagerScript.instance.gameState.skillDecision?.skill == skill;
        c.a = Mathf.SmoothDamp(c.a, highlight ? backInitialAlpha : 0, ref vBackAlpha, .05f);
        back.color = c;
        if (Input.GetKeyDown(hotkey)) {
            Activate();
        }
    }

    void Activate() {
        if (skill.type == SkillType.Active) {
            skill.Activate();
        }
    }

    public void OnClick() {
        Activate();
    }
    public void OnPointerEnter() {
        hovered = true;
    }
    public void OnPointerExit() {
        hovered = false;
    }
}
