using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Slider _musicScrollbar, _sfxScrollbar;

    public void MusicVolume()
    {
        AudioManager.Instance.MusicVolume(_musicScrollbar.value);
    }

    public void SFXVolume()
    {
        AudioManager.Instance.SFXVolume(_sfxScrollbar.value);
    }
 
}
