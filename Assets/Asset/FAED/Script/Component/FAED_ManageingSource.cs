using FD.Program.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FAED_ManageingSource : MonoBehaviour
{

    private AudioSource source;
    private FAED_SoundManager manager;
    public bool isStack;
    public string clipName;

    public void Setting(FAED_SoundManager manager, AudioSource source, string name)
    {

        isStack = true;
        this.manager = manager;
        this.source = source;
        clipName = name;

    }

    private void LateUpdate()
    {
        
        if(isStack == false && source.isPlaying == false)
        {

            manager.ch.Push(this);
            manager.playingList.Remove(this);
            isStack = true;

        }

    }

}
