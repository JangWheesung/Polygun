using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using FD.Program.Pool;
using FD.Program.SO;
using System.Linq;

namespace FD.Program.Managers
{

    public class FAED_SaveManager
    {
    
        public void Save(string path, string fileName, object obj)
        {

            FileStream fs = new FileStream(string.Format("{0}/{1}.json", path, fileName), FileMode.Create);
            byte[] data = Encoding.UTF8.GetBytes(JsonUtility.ToJson(obj));
            fs.Write(data, 0, data.Length);

        }

        public T Load<T>(string path, string fileName) where T : new()
        {

            if (Directory.Exists(path) == false)
            {

                Directory.CreateDirectory(path);

            }

            if(File.Exists(string.Format("{0}/{1}.json", path, fileName)) == false)
            {

                Save(path, fileName, new T());

            }

            FileStream fs = new FileStream(string.Format("{0}/{1}.json", path, fileName), FileMode.Open);
            byte[] data = new byte[fs.Length];

            fs.Read(data, 0, data.Length);
            fs.Close();

            string value = Encoding.UTF8.GetString(data);

            return JsonUtility.FromJson<T>(value);

        }

    }

    public class FAED_PoolManager
    {

        private Dictionary<string, FAED_Pool> pools = new Dictionary<string, FAED_Pool>();
        private Transform parent;

        public FAED_PoolManager(Transform parent)
        {

            this.parent = parent;

        }

        public void CreatePool(GameObject obj, string key, int poolCount = 10)
        {

            if (pools.ContainsKey(key) == false)
            {

                FAED_Pool pool = new FAED_Pool(obj, poolCount, key, parent);
                pools[key] = pool;

            }
            else
            {

                Debug.LogWarning($"FAED PoolManager : {key} pools is already exists");

            }

        }

        public void Push(GameObject obj)
        {

            if (pools.ContainsKey(obj.name))
            {

                pools[obj.name].Push(obj);

            }
            else
            {

                CreatePool(obj, obj.name, 1);
                pools[obj.name].Push(obj);

            }

        }

        public GameObject Pop(string key, Vector3 pos, Quaternion rot)
        {

            if (pools.ContainsKey(key))
            {

                return pools[key].Pop(pos, rot);

            }
            else
            {

                Debug.LogWarning($"FAED PoolManager : {key} pool is does not exist");

                return null;

            }

        }

    }
    
    public class FAED_SoundManager
    {

        private FAED_SoundList soundList;
        private Transform parent;
        public Stack<FAED_ManageingSource> ch = new Stack<FAED_ManageingSource>();
        public List<FAED_ManageingSource> playingList = new List<FAED_ManageingSource>();

        public FAED_SoundManager(FAED_SoundList list, Transform parent)
        {

            soundList = list;
            this.parent = parent;

            for(int i = 0; i < soundList.clipList.Count; i++)
            {

                GameObject go = new GameObject();
                go.gameObject.name = "@FAED_Ch";
                go.AddComponent<AudioSource>().playOnAwake = false;
                go.GetComponent<AudioSource>().loop = false;
                go.AddComponent<FAED_ManageingSource>().Setting(this, go.GetComponent<AudioSource>(), soundList.clipList[i].clipName);
                ch.Push(go.GetComponent<FAED_ManageingSource>());
                go.transform.SetParent(parent);

            }

            var awakeList = soundList.clipList.FindAll(x => x.playOnAwake == true).ToList();
            
            foreach(var item in awakeList)
            {

                FAED_ManageingSource go = ch.Pop();
                AudioSource source = go.GetComponent<AudioSource>();
                if (item.loop == true) source.loop = true;
                else source.loop = false;   

                go.clipName = item.clipName;
                source.clip = item.clip;
                source.volume = item.volume;
                source.pitch = item.pitch;

                source.Play();

                playingList.Add(go);

                go.isStack = false;

            }

        }
        
        public void PlaySound(string name)
        {

            if (soundList.clipList.Find(x => x.clipName == name) == null) return;

            var source = soundList.clipList.Where(x => x.clipName == name).First();

            FAED_ManageingSource go;

            if (ch.Count == 0)
            {

                go = CreateCh(name);

            }
            else
            {

                go = ch.Pop();

            }

            AudioSource audio = go.GetComponent<AudioSource>();

            audio.clip = source.clip;
            audio.volume = source.volume;
            audio.pitch = source.pitch;

            if (source.loop == true) audio.loop = true;
            else audio.loop = false;

            go.isStack = false;
            audio.Play();

            playingList.Add(go);

        }

        private FAED_ManageingSource CreateCh(string name)
        {

            GameObject go = new GameObject();
            go.gameObject.name = "@FAED_Ch";
            go.AddComponent<AudioSource>().playOnAwake = false;
            go.GetComponent<AudioSource>().loop = false;
            go.AddComponent<FAED_ManageingSource>().Setting(this, go.GetComponent<AudioSource>(), name);
            ch.Push(go.GetComponent<FAED_ManageingSource>());
            go.transform.SetParent(parent);

            return go.GetComponent<FAED_ManageingSource>();

        }

        public void Stop(string name)
        {

            if (playingList.Find(x => x.clipName == name) == null) return;

            var go = playingList.Where(x => x.clipName == name).First();

            go.GetComponent<AudioSource>().Stop();

            go.isStack = true;

            ch.Push(go);

        }

    }

}


