using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateAudio : MonoBehaviour
{
    [System.Serializable]
    public class KeyBoard
    {
        public KeyCode key;
        [Range(1, 20000)]
        public float frequency;
    }


    public float frequency1;


    public float sampleRate = 44100;
    public float waveLengthInSeconds = 2.0f;

    AudioSource audioSource;

    [SerializeField]
    ComputeShader genAudioWave;
    
    ComputeBuffer audioBuffer;

    List<float[]> audioData;

    [SerializeField]
    public List<KeyBoard> allKeys;

    int audioIndex = 0;

    int offset = 0;

    private int needGen = 1;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0; //force 2D sound
        audioSource.Stop(); //avoids audiosource from starting to play automatically
        audioSource.loop = true;

    }


    private void OnEnable()
    {
        audioBuffer = new ComputeBuffer(2048*2, sizeof(float));
        genAudioWave.SetBuffer(0, "audioWave", audioBuffer);
        genAudioWave.SetFloat("samples", sampleRate);
        audioData = new List<float[]>();// new float[2048*2];
        AddAudioData();
    }

    private void AddAudioData()
    {
        audioData.Add(new float[1024]);
    }

    private void OnDisable()
    {
        audioBuffer.Release();
        audioBuffer = null;
    }

    void CheckKey (KeyCode key, float freq)
    {
        if (Input.GetKeyDown(key))
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
                frequency1 = freq;
            }
        }
        if (Input.GetKeyUp(key))
        {
            audioSource.Stop();
            audioIndex = 0;
            offset = 0;
        }
    }

    void Update()
    {
        foreach (var key in allKeys)
        {
            CheckKey(key.key, key.frequency);
        }
    }


    private void FixedUpdate()
    {

        while (needGen>0)
        {
            while (audioIndex >= audioData.Count)
            {
                AddAudioData();
            }
            offset += 1024;
            genAudioWave.SetFloat("frequency", frequency1);
            genAudioWave.SetFloat("timeOffset", offset);
            genAudioWave.Dispatch(0, 1024, 1, 1);
            //Debug.Log(audioData.Count + ", " + audioIndex);
            audioBuffer.GetData(audioData[audioIndex]);
            audioIndex++;            
            needGen --;
        }
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        //Debug.Log(data.Length); //2048 where does that come from?
        //
        //Debug.Log(channels);
        //Debug.Log("dataLength " + "s=" + audioData[0].ToString("F2") + "|" + audioData[2048].ToString("F2") + "|"+ audioData[audioData.Length-1].ToString("F2") + "  == " + needGen);
        for (int i=0;i<data.Length;i++)
        {
            data[i] = audioData[0][i/2];
        }
        audioIndex--;
        if (audioIndex>0)
        {
            var temp = audioData[0];
            audioData.RemoveAt(0);
            audioData.Add(temp);
        } else
        {
            audioIndex = 0;
        }
        needGen ++;

    }

    /*   //Creates a sinewave
       public float CreateSine(int timeIndex, float frequency, float sampleRate)
       {
           return Mathf.Sin(2 * Mathf.PI * timeIndex * frequency / sampleRate);
       }*/
}



