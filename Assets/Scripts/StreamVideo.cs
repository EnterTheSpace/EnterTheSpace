using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
public class StreamVideo : MonoBehaviour {
    public RawImage rawImage;
    public VideoPlayer videoPlayer;
    // Use this for initialization
    void Start() {
        Keep();
    }

    public void Pause() {
        StopCoroutine(PlayVideo());
    }

    public void Keep() {
        StartCoroutine(PlayVideo());
    }

    IEnumerator PlayVideo() {
        videoPlayer.Prepare();
        WaitForSeconds waitForSeconds = new WaitForSeconds(1);
        while (!videoPlayer.isPrepared) {
            yield return waitForSeconds;
            break;
        }
        rawImage.texture = videoPlayer.texture;
        videoPlayer.Play();
    }
}
