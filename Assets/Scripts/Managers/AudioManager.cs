using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField]
    private AudioSource[] sfx;

    [SerializeField]
    private AudioSource[] music;

    [SerializeField]
    private float sfxMaxPitch = 1.0f;

    [SerializeField]
    private float sfxMinPitch = 0.85f;

    [SerializeField]
    private float sfxMaxDistance = 5.0f;

    private bool playMusic = true;
    private bool canPlaySFX;
    private int musicIndex = 2;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
        else
        {
            instance = this;
        }

        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            musicIndex = 0;
        }
        else if (SceneManager.GetActiveScene().name == "EndScene")
        {
            musicIndex = 4;
        }
        else
        {
            Invoke("AllowSFX", 0.3f); // constant delay before sfx can be played, just to avoid some bugs.
        }
    }

    private void Update()
    {
        if (!playMusic)
        {
            StopAllMusic();
        }
        else if (!music[musicIndex].isPlaying)
        {
            PlayMusic(musicIndex);
        }
    }

    public void PlaySFX(int _sfxIndex, float _delay, Transform _source)
    {
        if (canPlaySFX && 0 <= _sfxIndex && _sfxIndex < sfx.Length)
        {
            /*
            if (sfx[_sfxIndex].isPlaying)
            {
                return;
            }
            */

            // If we pass the maximum distance from the source of the sound, and care about it (_source isn't null) - we will not play the sound.
            if (_source != null &&
                Vector2.Distance(PlayerManager.instance.player.transform.position, _source.position) > sfxMaxDistance)
            {
                return;
            }

            sfx[_sfxIndex].pitch = Random.Range(sfxMinPitch, sfxMaxPitch);


            if (_delay <= 0)
            {
                sfx[_sfxIndex].Play();
            }
            else
            {
                sfx[_sfxIndex].PlayDelayed(_delay);
            }
        }
    }

    public void StopSFX(int _sfxIndex)
    {
        if (0 <= _sfxIndex && _sfxIndex < sfx.Length)
        {
            sfx[_sfxIndex].Stop();
        }
    }

    public bool IsPlayingSFX(int _sfxIndex)
    {
        if (0 <= _sfxIndex && _sfxIndex < sfx.Length)
        {
            return sfx[_sfxIndex].isPlaying;
        }

        return false;
    }

    public void StopSFXWithTime(int _index)
    {
        if (0 <= _index && _index <= sfx.Length - 1)
        {
            StartCoroutine(DecreaseVolume(sfx[_index]));
        }
    }

    private IEnumerator DecreaseVolume(AudioSource _audio)
    {
        var defaultVolume = _audio.volume;

        while (_audio.volume > 0.05f)
        {
            _audio.volume *= 0.5f;

            yield return new WaitForSeconds(0.25f);

            if (_audio.volume <= 0.05f)
            {
                _audio.Stop();
                _audio.volume = defaultVolume;
                break;
            }
        }
    }

    public void PlayMusic(int _musicIndex)
    {
        playMusic = true;
        
        if (0 > _musicIndex || _musicIndex >= sfx.Length)
        {
            return;
        }

        musicIndex = _musicIndex;
        StopAllMusic();
        music[_musicIndex].Play();
    }

    public void DisableMusic()
    {
        playMusic = false;
    }

    public void StopAllMusic()
    {
        for (var i = 0; i < music.Length; i++)
        {
            music[i].Stop();
        }
    }

    private void AllowSFX()
    {
        canPlaySFX = true;
    }
}