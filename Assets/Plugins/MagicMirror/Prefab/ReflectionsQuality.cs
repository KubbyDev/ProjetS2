using UnityEngine;

public class ReflectionsQuality
{
    //0: Off
    //1: Low
    //2: High
    public static int quality;

    public static void Update()
    {
        foreach (GameObject mirror in GameObject.FindGameObjectsWithTag("ReflectiveSurface"))
        {
            //Modification de la qualite de la texture reflechie
            mirror.GetComponent<MirrorScript>().TextureSize = GetTextureSize(quality);
            //Activation/Desactivation de la surface reflective
            mirror.transform.Find("MirrorReflection").gameObject.SetActive(quality != 0);
        }
    }

    private static int GetTextureSize(int quality)
    {
        switch (quality)
        {
            case 2: return 784;
            case 1: return 196;
            default: return 0;
        }
    }

    public static void Set(int newQuality)
    {
        quality = newQuality;
        Update();
    }

    public void OnLevelWasLoaded(int i)
    {
         Update();
    }
}