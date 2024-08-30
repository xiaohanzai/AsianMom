using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MusicGame
{
    public enum MusicKeyName
    {
        Do,
        Re,
        Mi,
        Fa,
        Sol,
        La,
        Si,
        DO,
    }

    public class MusicKey : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;

        [SerializeField] private MusicKeyName keyName;

        public UnityEvent<MusicKeyName> Evt_OnKeyHit = new UnityEvent<MusicKeyName>();

        public MusicKeyName GetKeyName()
        {
            return keyName;
        }

        private void OnTriggerExit(Collider other)
        {
            Debug.Log(other.gameObject.GetComponent<DrumstickCollider>() != null);
            if (other.gameObject.GetComponent<DrumstickCollider>() != null)
            {
                audioSource.Play();
                Evt_OnKeyHit.Invoke(keyName);
                Debug.Log("???");
            }
        }
        //private void OnCollisionExit(Collision collision)
        //{
        //    Debug.Log(collision.gameObject.name);
        //    Debug.Log(collision.gameObject.GetComponent<DrumstickCollider>() == null);
            
        //}
    }
}