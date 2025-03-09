using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

//using StarTrekValuables;
using UnityEngine;

namespace StarTrekValuables.Scripts
{
    public class DaxMug : MonoBehaviour
    {
        private bool localSeen = false;
        private bool localSeenEffect = false;
        private float localSeenEffectTimer = 2f;
        public AudioClip seenSound;
        private PhysGrabObject physGrabObject;

        void Start()
        {
            Debug.Log("Dax is awake.");
            this.physGrabObject = base.GetComponent<PhysGrabObject>();
        }

        void Update()
        {
            if (this.localSeenEffect)
            {
                this.localSeenEffectTimer -= Time.deltaTime;
                CameraZoom.Instance.OverrideZoomSet(75f, 0.1f, 0.25f, 0.25f, base.gameObject, 150);
                PostProcessing.Instance.VignetteOverride(Color.black, 0.4f, 1f, 1f, 0.5f, 0.1f, base.gameObject);
                PostProcessing.Instance.SaturationOverride(-50f, 1f, 0.5f, 0.1f, base.gameObject);
                PostProcessing.Instance.ContrastOverride(5f, 1f, 0.5f, 0.1f, base.gameObject);
                GameDirector.instance.CameraImpact.Shake(10f * Time.deltaTime, 0.1f);
                GameDirector.instance.CameraShake.Shake(10f * Time.deltaTime, 1f);
                if (this.localSeenEffectTimer <= 0f)
                {
                    this.localSeenEffect = false;
                }
            }
        }

        public void DiscoverScare()
        {
            Plugin.Logger.LogInfo("Doing DaxMug scare");
            this.localSeenEffect = true;
            CameraGlitch.Instance.PlayLong();
            GameDirector.instance.CameraImpact.Shake(2f, 0.5f);
            GameDirector.instance.CameraShake.Shake(2f, 1f);
            this.localSeen = true;

            Plugin.Logger.LogInfo($"Playing audio scare \"{this.seenSound}\"");
            AudioScare.instance.PlayCustom(this.seenSound, 0.3f, 60f);
        }
    }
}
