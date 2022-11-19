using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Gen;

public class MeshGen : MonoBehaviour
{
    [SerializeField] public float scale,multiplier=1;
    [SerializeField] Material mat;
    [Range(0,6)] int iLod;
    [SerializeField] public AnimationCurve ac;
    [SerializeField] Vector2 offset=new Vector2(0,0);
    [SerializeField] bool useFlatShading=false;
    
    public Landpoint[] land;
    [System.Serializable]
    public struct Landpoint{
        public Color color;
        public string name;
        public float point;
    }

    /*void OnValidate() {
        scale=scale>0?  scale:1;
        float[,] noise=Gen.noiseGen(offset,Gen.mapSize,Gen.mapSize,scale);
        Mesh mesh=Gen.GenerateMesh(iLod,scale,multiplier,noise,ac,useFlatShading);
        setmesh(mesh);
        Texture2D texture=ColTex(noise);
        settex(texture);
    }*/

    public Texture2D ColTex(float[,] noise) {
        Color[] colMap=new Color[Gen.mapSize*Gen.mapSize];
        for(int i=0;i<mapSize;i++) {
            for(int j=0;j<mapSize;j++ ) {
                for(int k=0;k<land.Length;k++) {
                    if(noise[i,j]<land[k].point*2-1) {
                        colMap[i+j*mapSize]=land[k].color;
                        break;
                    }
                }
            }
            
        }    
        Texture2D texture=new Texture2D(Gen.mapSize,Gen.mapSize);
        texture.SetPixels(colMap);
        texture.wrapMode=TextureWrapMode.Clamp;
        texture.filterMode=FilterMode.Point;
        texture.Apply();
        return texture;
    }

    void settex(Texture2D texture) {
        mat.SetTexture("_MainTex",texture);
    }

    void setmesh(Mesh mesh) {
        MeshFilter mr=GetComponent<MeshFilter>();
        mr.sharedMesh=mesh;
    }
}
