using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FD.Program.Class;

namespace FD.Program.SO
{

    [CreateAssetMenu(menuName = "FAED/SoundList")]
    public class FAED_SoundList : ScriptableObject
    {
        
        public List<FAED_ClipList> clipList;

    }

}


