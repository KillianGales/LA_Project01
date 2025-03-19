using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;

public class FXPoolManager : MonoBehaviour
{
    public static FXPoolManager Instance;
    public List<BulletType> AllBullet;

        // Dictionary: bulletType -> fxType -> queue of FX objects
    private Dictionary<string, Dictionary<string, Queue<Component>>> fxPools = new Dictionary<string, Dictionary<string, Queue<Component>>>();

    void Start()
    {
        foreach (BulletType bulletType in AllBullet)
        {
            string bName = bulletType.name;

            if(bulletType.startParticleSystem != null)
                PreloadFX(bName, bulletType.startParticleSystem.name, bulletType.startParticleSystem, 10);
            
            if(bulletType.activeParticleSystem != null)
                PreloadFX(bName, bulletType.activeParticleSystem.name, bulletType.activeParticleSystem, 10);
            
            if(bulletType.endParticleSystem != null)
                PreloadFX(bName, bulletType.endParticleSystem.name, bulletType.endParticleSystem, 10);

            if(bulletType.startAudio != null)
                PreloadFX(bName, bulletType.startAudio.name, bulletType.startAudio, 5);
            
            if(bulletType.activeAudio != null)
                PreloadFX(bName, bulletType.activeAudio.name, bulletType.activeAudio, 5);

            if(bulletType.endAudio != null)
                PreloadFX(bName, bulletType.endAudio.name, bulletType.endAudio, 5);

            if(bulletType.activeTrail != null)
                PreloadFX(bName, bulletType.activeTrail.name, bulletType.activeTrail, 40);
        }
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    //  Preload FX (any type of Component)
    public void PreloadFX<T>(string bulletType, string fxType, T fxPrefab, int amount) where T : Component
    {
        if (!fxPools.ContainsKey(bulletType))
            fxPools[bulletType] = new Dictionary<string, Queue<Component>>();

        if (!fxPools[bulletType].ContainsKey(fxType))
            fxPools[bulletType][fxType] = new Queue<Component>();

        for (int i = 0; i < amount; i++)
        {
            T newFX = Instantiate(fxPrefab, transform);
            newFX.gameObject.SetActive(false);
            fxPools[bulletType][fxType].Enqueue(newFX);
        }
    }

    //  Get FX (returns as a Component, must be cast to the correct type)
    public T GetFX<T>(string bulletType, string fxType, Vector3 position) where T : Component
    {
        if (!fxPools.ContainsKey(bulletType) || !fxPools[bulletType].ContainsKey(fxType))
        {
            //Debug.LogWarning($"FX type '{fxType}' for bullet '{bulletType}' not found in pool!");
            return null;
        }

        var pool = fxPools[bulletType][fxType];

        if (pool.Count == 1 && pool.TryPeek( out Component prefab))
        {
            if (pool.Count < 20)  // Prevents infinite expansion -- MAX 20 on screen
            {
                T newFX = Instantiate(prefab, transform) as T;
                return newFX;
            }
                return null;
        }

        T fx = pool.Dequeue() as T;
        fx.transform.position = position;
        fx.gameObject.SetActive(true);
        if(fx != null)
        {
            return fx;
        }
        return null;
        
    }

    //  Return FX to pool
    public void ReturnFX<T>(string bulletType, string fxType, T fx) where T : Component
    {
        fx.gameObject.SetActive(false);
        
        if (!fxPools.ContainsKey(bulletType))
            fxPools[bulletType] = new Dictionary<string, Queue<Component>>();

        if (!fxPools[bulletType].ContainsKey(fxType))
            fxPools[bulletType][fxType] = new Queue<Component>();

        fxPools[bulletType][fxType].Enqueue(fx);
    }

/// <summary>
/// 
/// </summary>
/// <param name="particle"></param>
/// 

    public void PlayParticle(string bName, string particle, Vector3 position)
    {
        ParticleSystem fx = GetFX<ParticleSystem>(bName, particle, position);
        
        //Debug.Break();
        if (fx != null)
            StartCoroutine(ReturnFXAfterDelay(bName, particle, fx, fx.main.duration));

    }

    public void PlayAudio(string bName, string sfx)
    {
        AudioSource audio = GetFX<AudioSource>(bName, sfx, transform.position);

        if (audio != null)
            StartCoroutine(ReturnFXAfterDelay(bName, sfx, audio, audio.clip.length));
    }

    public void AttachTrail(string bName, string l_trail, Transform parent)
    {
        TrailRenderer trail = GetFX<TrailRenderer>(bName, l_trail, Vector3.zero);
        trail.transform.parent = parent;
        trail.gameObject.SetActive(true);

        if (trail != null)
            StartCoroutine(ReturnFXAfterDelay(bName, l_trail, trail, 3f));
    }

    IEnumerator ReturnFXAfterDelay<T>(string bulletType, string fxType, T fx, float delay) where T : Component
    {
        yield return new WaitForSeconds(delay);
        FXPoolManager.Instance.ReturnFX(bulletType, fxType, fx);
    }





}