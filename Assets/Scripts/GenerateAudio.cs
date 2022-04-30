using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateAudio : MonoBehaviour
{
    [Range(1, 20000)]  //Creates a slider in the inspector
    public float frequency1;

    [Range(1, 20000)]  //Creates a slider in the inspector
    public float frequency2;

    public float sampleRate = 44100;
    public float waveLengthInSeconds = 2.0f;

    AudioSource audioSource;
    int timeIndex = 0;

    [SerializeField]
    ComputeShader genAudioWave;

    [SerializeField]
    float sinMult = 0.01f;

    ComputeBuffer audioBuffer;

    List<float[]> audioData;

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
        audioData.Add(new float[2048]);
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
                timeIndex = 0;  //resets timer before playing sound
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
        CheckKey(KeyCode.H, 600f);
        CheckKey(KeyCode.J, 800f);
        CheckKey(KeyCode.K, 1000f);
        CheckKey(KeyCode.L, 1200f);
    }


    private void FixedUpdate()
    {

        while (needGen>0)
        {
            while (audioIndex >= audioData.Count)
            {
                AddAudioData();
            }
            offset += 2048;
            genAudioWave.SetFloat("frequency", frequency1);
            genAudioWave.SetFloat("timeOffset", offset);
            genAudioWave.Dispatch(0, 2048, 1, 1);
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
            data[i] = audioData[0][i];
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



