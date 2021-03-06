﻿using roastedrooster.chickenrun.player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace roastedrooster.chickenrun.laws
{
    public class LawManager : MonoBehaviour
    {
        #region Singleton Pattern
        private static LawManager sInstance;
        public static LawManager Instance
        {
            get
            {
                return sInstance;
            }
        }
        #endregion

        #region Inspector Fields
        public List<Law> availableLaws;
        public float timeBeforeFirstLaw = 5f;
        public AudioClip noRuleClip;
        public AudioClip noJumpClip;
        public AudioClip punishedClip;
        public AudioClip noTouchingClip;
        #endregion

        public Law AppliedLaw { get; private set; }
        public float RemainingTimeForLaw
        {
            get
            {
                return Mathf.Max(_nextLawChange - Time.realtimeSinceStartup, 0);
            }
        }

        private float _nextLawChange = 0f;

        public void PlayerEvent(TriggeringEventID eventID, Player player, float optionalArg = 0f)
        {
            var law = AppliedLaw;
            if (law.eventID == eventID && law.optionalArg == optionalArg)
            {
                AudioSource.PlayClipAtPoint(punishedClip, Camera.main.transform.position);
                player.Punished(law);
            }
        }

        public void Start()
        {
            if (sInstance == null)
                sInstance = this;
        }

        public void Update()
        {
            var now = Time.realtimeSinceStartup;
            if ((AppliedLaw == null && now > timeBeforeFirstLaw) || now > _nextLawChange)
            {
                AppliedLaw = availableLaws[UnityEngine.Random.Range(0, availableLaws.Count)];
                if(AppliedLaw.eventID == TriggeringEventID.PlayerJump)
                {
                    AudioSource.PlayClipAtPoint(noJumpClip, Camera.main.transform.position);
                }
                else if(AppliedLaw.eventID == TriggeringEventID.None)
                {
                    AudioSource.PlayClipAtPoint(noRuleClip, Camera.main.transform.position);
                }
                else if(AppliedLaw.eventID == TriggeringEventID.PlayerTouching)
                {
                    AudioSource.PlayClipAtPoint(noTouchingClip, Camera.main.transform.position);
                }

                _nextLawChange = now + AppliedLaw.duration;
            }
        }
    }
}
