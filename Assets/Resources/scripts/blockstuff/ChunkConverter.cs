using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;

public class ChunkConverter : MonoBehaviour
{
    public string filename = "default";
    public bool saveActive = false;
    public bool loadActive = false;
    public bool inEditor = true;
    public GameObject cursor;
    private int frame = 0;
    private bool done1 = false;
    private bool done2 = false;
    private bool done3 = false;
    private Vector3 min = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
    private Vector3 max = new Vector3(-Mathf.Infinity, -Mathf.Infinity, -Mathf.Infinity);
    private List<GameObject> messyPile = new List<GameObject>();
    public List<List<List<Block>>> finalPile = new List<List<List<Block>>>();

    LevelData finalData;
    string _data;
    string _FileLocation;
    string _FileName;

    void Start ()
    {
        if (inEditor)
        {
            Trace("Creating map...");
        }
        _FileLocation = Application.dataPath;
        _FileName = filename + ".xml";
        finalData = new LevelData();
    }

    private void Update()
    {
        if (inEditor)
        {
            if (Input.GetKeyDown("s"))
            {
                saveActive = true;
                ResetEverything();
            }

            if (Input.GetKeyDown("l"))
            {
                loadActive = true;
                ResetEverything();
            }

            if (saveActive)
            {
                if (frame < transform.childCount)
                {
                    Transform obj = transform.GetChild(frame);
                    if (!done1)
                    {
                        if (obj.gameObject.GetComponent<FakeBlock>() != null)
                        {
                            Vector3 pos = obj.position;
                            min.x = Mathf.Min(min.x, pos.x);
                            min.y = Mathf.Min(min.y, pos.y);
                            min.z = Mathf.Min(min.z, pos.z);
                            max.x = Mathf.Max(max.x, pos.x);
                            max.y = Mathf.Max(max.y, pos.y);
                            max.z = Mathf.Max(max.z, pos.z);
                            messyPile.Add(obj.gameObject);
                        }
                        frame++;
                    }
                }
                else
                {
                    if (!done1)
                    {
                        Trace("Done with phase 1");
                        done1 = true;
                        frame = 0;
                        int xCount = (int)Mathf.Floor((max.x - min.x) / MakeLevel.blockSize.x) + 1;
                        int yCount = (int)Mathf.Floor((max.y - min.y) / MakeLevel.blockSize.y) + 1;
                        int zCount = (int)Mathf.Floor((max.z - min.z) / MakeLevel.blockSize.z) + 1;
                        for (var x = 0; x < xCount; x++)
                        {
                            finalPile.Add(new List<List<Block>>());
                            for (var y = 0; y < yCount; y++)
                            {
                                finalPile[x].Add(new List<Block>());
                                for (var z = 0; z < zCount; z++)
                                    finalPile[x][y].Add(Block.AIR);
                            }
                        }
                    }
                }

                if (!done2 && done1)
                {
                    if (frame < messyPile.Count)
                    {
                        GameObject obj = messyPile[frame];
                        int x = (int)Mathf.Floor((obj.transform.position.x - min.x) / MakeLevel.blockSize.x);
                        int y = (int)Mathf.Floor((obj.transform.position.y - min.y) / MakeLevel.blockSize.y);
                        int z = (int)Mathf.Floor((obj.transform.position.z - min.z) / MakeLevel.blockSize.z);
                        Block blk = obj.GetComponent<FakeBlock>().block;
                        finalPile[x][y][z] = blk;
                        frame++;
                    }
                    else
                    {
                        finalData._iLevel.map = finalPile;
                        _data = SerializeObject(finalData);
                        CreateXML();
                        Trace("COMPLETELY SAVED");
                        done2 = true;
                        saveActive = false;
                    }
                }
            }
            if (loadActive)
            {
                if (!done1)
                {
                    LoadChunk();
                    done1 = true;
                    loadActive = false;
                }
            }
        }
	}

    public void SetFile(string name)
    {
        _FileName = name + ".xml";
    }

    public List<List<List<Block>>> CopyMap()
    {
        List<List<List<Block>>> ret = new List<List<List<Block>>>();
        for (var x = 0; x < finalPile.Count; x++)
        {
            ret.Add(new List<List<Block>>());
            for (var y = 0; y < finalPile[0].Count; y++)
            {
                ret[x].Add(new List<Block>());
                for (var z = 0; z < finalPile[0][0].Count; z++)
                    ret[x][y].Add(finalPile[x][y][z]);
            }
        }
        return ret;
    }

    public void LoadChunk()
    {
        if (inEditor)
            for (int i = transform.childCount - 1; i > 0; i--)
            {
                GameObject obj = transform.GetChild(i).gameObject;
                if (obj.GetComponent<FakeBlock>() != null)
                    Destroy(obj);
            }

        LoadXML();
        if (_data.ToString() != "")
        {
            finalData = (LevelData)DeserializeObject(_data);
            finalPile = finalData._iLevel.map;
            if (cursor != null)
            {
                CursorControl cc = cursor.GetComponent<CursorControl>();
                for (int x = 0; x < finalPile.Count; x++)
                    for (int y = 0; y < finalPile[0].Count; y++)
                        for (int z = 0; z < finalPile[0][0].Count; z++)
                        {

                            cursor.transform.localPosition = new Vector3(x * MakeLevel.blockSize.x, y * MakeLevel.blockSize.y, z * MakeLevel.blockSize.z);
                            cc.Place(finalPile[x][y][z]);
                        }
                cursor.transform.position = Vector3.zero;
            }
        }
    }

    private void ResetEverything()
    {
        frame = 0;
        finalPile.Clear();
        messyPile.Clear();
        done1 = false;
        done2 = false;
        done3 = false;
        min = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
        max = new Vector3(-Mathf.Infinity, -Mathf.Infinity, -Mathf.Infinity);
    }

    private void Trace(string str)
    {
        Debug.Log("<<<" + frame + ">>> : " + str);
    }

    /*Everything below here is from the Unity wiki*/

    /* The following metods came from the referenced URL */
    string UTF8ByteArrayToString(byte[] characters)
    {
        UTF8Encoding encoding = new UTF8Encoding();
        string constructedString = encoding.GetString(characters);
        return (constructedString);
    }

    byte[] StringToUTF8ByteArray(string pXmlString)
    {
        UTF8Encoding encoding = new UTF8Encoding();
        byte[] byteArray = encoding.GetBytes(pXmlString);
        return byteArray;
    }

    // Here we serialize our UserData object of myData 
    string SerializeObject(object pObject)
    {
        string XmlizedString = null;
        MemoryStream memoryStream = new MemoryStream();
        XmlSerializer xs = new XmlSerializer(typeof(LevelData));
        XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
        xs.Serialize(xmlTextWriter, pObject);
        memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
        XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());
        return XmlizedString;
    }

    // Here we deserialize it back into its original form 
    object DeserializeObject(string pXmlizedString)
    {
        XmlSerializer xs = new XmlSerializer(typeof(LevelData));
        MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString));
        XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
        return xs.Deserialize(memoryStream);
    }

    // Finally our save and load methods for the file itself 
    void CreateXML()
    {
        StreamWriter writer;
        FileInfo t = new FileInfo(_FileLocation + "\\" + _FileName);
        if (!t.Exists)
        {
            writer = t.CreateText();
        }
        else
        {
            t.Delete();
            writer = t.CreateText();
        }
        writer.Write(_data);
        writer.Close();
    }

    void LoadXML()
    {
        StreamReader r = File.OpenText(_FileLocation + "\\" + _FileName);
        string _info = r.ReadToEnd();
        r.Close();
        _data = _info;
    }
}

/*From the Unity Wiki*/
public class LevelData
{
    // We have to define a default instance of the structure 
    public Data _iLevel;
    // Default constructor doesn't really do anything at the moment 
    public LevelData(){ }

    // Anything we want to store in the XML file, we define it here 
    public struct Data
    {
        public List<List<List<Block>>> map;
    }
}
