using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : Interactable
{
    public int loadScene;

    public override void Interact() {
        if(loadScene > 0)
            SceneManager.LoadSceneAsync(loadScene-1);
    }
}
