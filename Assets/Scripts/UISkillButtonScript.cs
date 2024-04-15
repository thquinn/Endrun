using Assets.Code;
using Assets.Code.Model;
using Assets.Code.Model.Skills;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class UISkillButtonScript : TooltipBehavior
{
    public SpriteAtlas atlasSkillIcons, atlasUnitIcons;

    public CanvasGroup canvasGroup;
    public Image back, icon, summonIcon;
    public TextMeshProUGUI tmpHotkey;

    Skill skill;
    KeyCode hotkey;
    bool hovered;
    float backInitialAlpha, vBackAlpha;

    public void Init(Skill skill, KeyCode hotkey) {
        this.skill = skill;
        this.hotkey = hotkey;
        icon.sprite = atlasSkillIcons.GetSprite(skill.GetIconID());
        tmpHotkey.text = Util.KeyCodeToString(hotkey);
        // Summon.
        FakeSkillSummon summonSkill = skill as FakeSkillSummon;
        if (summonSkill != null) {
            icon.transform.localScale = new Vector3(1.33f, 1.33f, 1);
            Color c = icon.color;
            c.a = .25f;
            icon.color = c;
            summonIcon.enabled = true;
            summonIcon.sprite = atlasUnitIcons.GetSprite(summonSkill.template.iconID);
        }
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
        bool dim = !skill.CanActivate();
        canvasGroup.alpha = dim ? .5f : 1;
        Color c = back.color;
        bool highlight = (hovered && !dim) || GameStateManagerScript.instance.gameState.skillDecision?.skill == skill;
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

    public override ITooltippableObject GetTooltippableObject() {
        return skill;
    }
}
