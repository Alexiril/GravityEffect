
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class lightController : MonoBehaviour
{
    // Private variables
    [SerializeField, Range(0, 24)]
    private float timeOfDay;
    [SerializeField]
    private float orbitSpeed = 1f;
    [SerializeField]
    private Volume skyVolume;
    [SerializeField]
    private AnimationCurve starsCurve;
    [SerializeField]
    private Transform sunTransform, moonTransform;
    [SerializeField]
    private List<GameObject> nightLightingSources;

    private List<Animation> nightLightSourcesAnimation = new List<Animation>();
    private HDAdditionalLightData sunAdditionalLightData, moonAdditionalLightData;
    private Light sun, moon;
    private PhysicallyBasedSky sky;
    private bool isNight;

    // Private functions
    private void Awake()
    {
        sun = sunTransform.GetComponent<Light>();
        moon = sunTransform.GetComponent<Light>();
        sunAdditionalLightData = sunTransform.GetComponent<HDAdditionalLightData>();
        moonAdditionalLightData = moonTransform.GetComponent<HDAdditionalLightData>();
        Animation animation;
        foreach (GameObject source in nightLightingSources)
        {
            if (source.TryGetComponent<Animation>(out animation))
            {
                nightLightSourcesAnimation.Add(animation);
            }
        }
    }
    private void Start()
    {
        timeOfDay = DateTime.Now.Hour;
        if (timeOfDay > 18 || timeOfDay < 6) StartNight();
        else StartDay();
        skyVolume.profile.TryGet(out sky);
    }
    private void Update()
    {
        timeOfDay += Time.deltaTime * orbitSpeed;
        if (timeOfDay > 24) timeOfDay = 0;
        UpdateTime();
    }
    private void OnValidate()
    {
        Awake();
        skyVolume.profile.TryGet(out sky);
        UpdateTime();
    }
    private void UpdateTime()
    {
        float alpha = timeOfDay / 24.0f;
        float sunRotation = Mathf.Lerp(-90, 270, alpha);
        float moonRotation = sunRotation - 180f;
        sunTransform.rotation = Quaternion.Euler(sunRotation, 0f, 0f);
        moonTransform.rotation = Quaternion.Euler(moonRotation, 0f, 0f);
        sky.spaceEmissionMultiplier.value = starsCurve.Evaluate(alpha);
        sky.spaceRotation.value = new Vector3(sunRotation, 40f, 155f);
        CheckNightDayTransition();
    }
    private void CheckNightDayTransition()
    {
        if (isNight)
        {
            if (moonTransform.rotation.eulerAngles.x > 180f)
            {
                StartDay();
            }
        }
        else
        {
            if (sunTransform.rotation.eulerAngles.x > 180f)
            {
                StartNight();
            }
        }
    }
    private void StartDay()
    {
        isNight = false;
        moonAdditionalLightData.EnableShadows(false);
        sunAdditionalLightData.EnableShadows(true);
        if (nightLightSourcesAnimation.Count > 0)
        {
            foreach (Animation animation in nightLightSourcesAnimation)
            {
                animation.Stop();
            }
        }
        if (nightLightingSources.Count > 0)
        {
            foreach (GameObject source in nightLightingSources)
            {
                source.SetActive(false);
            }
        }
    }
    private void StartNight()
    {
        isNight = true;
        sunAdditionalLightData.EnableShadows(false);
        moonAdditionalLightData.EnableShadows(true);
        if (nightLightingSources.Count > 0)
        {
            foreach (GameObject source in nightLightingSources)
            {
                source.SetActive(true);
            }
        }
        if (nightLightSourcesAnimation.Count > 0)
        {
            foreach (Animation animation in nightLightSourcesAnimation)
            {
                animation.Play();
            }
        }
    }
}
