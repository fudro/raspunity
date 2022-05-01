using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
public class VideoController : MonoBehaviour
{
    public GameObject tv;
    public Renderer rend;
    public Material on;
    public Material off;
    private bool isPlaying;
    private VideoPlayer videoPlayer;
    private AudioSource audioSource;
    // Use this for initialization
    void Start()
    {

        videoPlayer = gameObject.AddComponent<VideoPlayer>();
        //Add AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        //Disable Play on Awake for both Video and Audio
        videoPlayer.playOnAwake = false;
        audioSource.playOnAwake = false;
        audioSource.Pause();
        // Video clip from Url
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4";

        //Set Audio Output to AudioSource
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        //Assign the Audio from Video to AudioSource to be played
        videoPlayer.EnableAudioTrack(0, true);
        videoPlayer.SetTargetAudioSource(0, audioSource);
        //Set video To Play then prepare Audio to prevent Buffering        
        videoPlayer.Prepare();

        //Play Video
        videoPlayer.Play();
        //Play Sound
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        isPlaying = videoPlayer.isPlaying;
        if (isPlaying)
        {
            rend.material = on;
        }
        else
        {
            rend.material = off;
        }

    }
}
