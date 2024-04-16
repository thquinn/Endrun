using Assets.Code.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TooltipBehavior : MonoBehaviour
{
    public abstract ITooltippableObject GetTooltippableObject();
}

public class Tooltip {
    public string header, content, upperRight;

    public override string ToString() {
        return $"<b>{header}</b>\n\n{content}";
    }
}