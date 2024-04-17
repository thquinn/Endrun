using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicScript : MonoBehaviour
{
    public static MusicScript instance;

    public AudioSource[] bgms;
    public AudioSource currentBGM;

    bool stopped;

    private void Start() {
        if (instance != null) {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update() {
        if (stopped) {
            return;
        }
        if (currentBGM == null || !currentBGM.isPlaying) {
            AudioSource oldBGM = currentBGM;
            while (oldBGM == currentBGM) {
                currentBGM = bgms[Random.Range(0, bgms.Length)];
            }
            currentBGM.Play();
        }
    }

    public void Stop() {
        currentBGM.Stop();
        stopped = true;
    }
}