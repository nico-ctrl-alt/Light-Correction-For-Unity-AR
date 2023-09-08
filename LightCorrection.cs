using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightCorrection : MonoBehaviour
{
    public RenderTexture tex;
    public Light mainlight;
    private int _width;
    private int _height;
    public Color avgcolor;
    public Vector3 vel = Vector3.zero;


    void Start()
    {
        _width = Camera.main.pixelWidth;
        _height = Camera.main.pixelHeight;
        Debug.Log(_width + ", " + _height);
        StartCoroutine(Repeat());
    }


    private void Update()
    {
        if (RenderSettings.ambientLight != avgcolor)
        {
            Vector3 color = new Vector3(RenderSettings.ambientLight.r, RenderSettings.ambientLight.g, RenderSettings.ambientLight.b);
            Vector3 targetcolor = new Vector3(avgcolor.r, avgcolor.g, avgcolor.b);
            color = Vector3.SmoothDamp(color, targetcolor, ref vel, .5f);
            RenderSettings.ambientLight = new Vector4(color.x, color.y, color.z, 1);
            mainlight.color = new Vector4(color.x, color.y, color.z, 1) * 1.5f;
            reflect.backgroundColor = new Vector4(color.x, color.y, color.z, 1);
        }
    }


    public IEnumerator Repeat()
    {
        while(true)
        {
            yield return new WaitForEndOfFrame();
            avgcolor = AverageColor(tex);
            yield return new WaitForSeconds(.3f);
        }
    }


    Vector4 AverageColor(RenderTexture rendertex)
    {
        Camera.main.targetTexture = rendertex;
        Camera.main.targetTexture = null;
        Texture2D tex = new Texture2D(_width, _height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, _width, _height), 0, 0);
        tex.Apply();
        Vector4 sum = Vector4.zero;
        for (int x = 0; x < tex.width / 50; x++)
        {
            for (int y = 0; y < tex.height / 50; y++)
            {
                sum += (Vector4)tex.GetPixel(Random.Range(0,_width),Random.Range(0,_height));
            }
        }
        return sum / (tex.width / 50 * tex.height / 50);
    }
}
