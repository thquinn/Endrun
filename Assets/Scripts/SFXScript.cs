using Assets.Code;
using UnityEngine;

public class SFXScript : MonoBehaviour
{
    public static SFXScript instance;
    public AudioSource[] moves;
    public AudioSource click, clickDisabled, levelChange, undo;

    void Start() {
        instance = this;
    }

    public static void SFXClick() {
        instance?.click.PlayOneShot(instance.click.clip);
    }
    public static void SFXClickDisabled() {
        instance?.clickDisabled.PlayOneShot(instance.clickDisabled.clip);
    }
    public static void SFXLevelChange() {
        instance?.levelChange.PlayOneShot(instance.levelChange.clip);
    }
    public static void SFXUndo() {
        instance?.undo.PlayOneShot(instance.undo.clip);
    }
}