using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

public class MeshController : MonoBehaviour
{
    [SerializeField] GameObject MeshSkeliton,MeshGenerator;
    [SerializeField] Transform Player;
    [SerializeField] Shader shader;
    [SerializeField] [Range(1,6)]int chunks;
    [SerializeField] bool useFlatShading=false;
    Vector2 PrevPlayerPos,CurrPlayerPos;
    Dictionary<Vector2,GameObject> ObjectCoords=new Dictionary<Vector2,GameObject>();
    Dictionary<Vector2,float[,]> CoordsNoise=new Dictionary<Vector2,float[,]>();
    Vector2[] PrevMeshCoords;

    void Start() {
        PrevMeshCoords=Gen.NearMeshCoords(Player.position,chunks);
        SimulateMeshes();
    }

    void Update() {
        CurrPlayerPos=Gen.relativeCurrCoord(Player.position);
        if(PrevPlayerPos!=CurrPlayerPos) {
            SimulateMeshes();
            PrevPlayerPos=CurrPlayerPos;
        }
    }

    void SimulateMeshes() {
        Vector2[] NearMesh=Gen.NearMeshCoords(Player.position,chunks);
        for(int i=0;i<NearMesh.Length;i++) {
            if(!ObjectCoords.ContainsKey(NearMesh[i])) {
                CreateMeshObject(NearMesh[i]);
            }
            else {
                ActivateMeshObject(NearMesh[i]);
            }
        }
        DisableMeshes(NearMesh);
        PrevMeshCoords=NearMesh;
    }

    void DisableMeshes(Vector2[] NearMesh) {
        HashSet<Vector2> PrevMeshSet=new HashSet<Vector2>();
        for(int i=0;i<PrevMeshCoords.Length;i++) {
            PrevMeshSet.Add(PrevMeshCoords[i]);
        }
        for(int i=0;i<NearMesh.Length;i++) {
            if(PrevMeshSet.Contains(NearMesh[i])) {
                PrevMeshSet.Remove(NearMesh[i]);
            }
        }
        foreach(Vector2 item in PrevMeshSet) {
            if(ObjectCoords.ContainsKey(item)) {
                ObjectCoords[item].SetActive(false);
            }
        }
    }

    void CreateMeshObject(Vector2 newMeshPos) {
        Vector3 instancePosition=new Vector3(newMeshPos.x*(Gen.mapSize-1),0,newMeshPos.y*(Gen.mapSize-1));
        GameObject instance=Instantiate(MeshSkeliton,instancePosition,Quaternion.identity);
        instance.transform.parent=transform;
        ObjectCoords.Add(newMeshPos,instance);
        GenerateMesh(newMeshPos,true);
    }

    void ActivateMeshObject(Vector2 prevMeshPos) {
        GenerateMesh(prevMeshPos,false);
        ObjectCoords[prevMeshPos].SetActive(true);
    }

    void GenerateMesh(Vector2 MeshPos,bool needTex) {
        GameObject MeshObject=ObjectCoords[MeshPos];
        Vector2 relativemeshPos=MeshPos-CurrPlayerPos;
        int disload=(int)Mathf.Max(Mathf.Abs(relativemeshPos.x),Mathf.Abs(relativemeshPos.y));
        MeshGen mg=MeshGenerator.GetComponent<MeshGen>();
        float[,] noise;
        if(CoordsNoise.ContainsKey(MeshPos)) {
            noise=CoordsNoise[MeshPos];
        }
        else {
            noise=Gen.noiseGen(MeshPos*1.36f,Gen.mapSize,Gen.mapSize,mg.scale);
            CoordsNoise.Add(MeshPos,noise);
        }
        //Func<int,float,float,float[,],AnimationCurve,bool,Mesh> ggm=Gen.GenerateMesh;
        //Task<Mesh> genNewThread=Task.Factory.StartNew<Mesh>(()=>ggm(disload,mg.scale,mg.multiplier,noise,mg.ac,useFlatShading));
        Mesh mesh=Gen.GenerateMesh(disload,mg.scale,mg.multiplier,noise,mg.ac,useFlatShading);
        if(needTex) {
            Texture2D instanceTex=mg.ColTex(noise);
            Material mat=new Material(shader);
            mat.mainTexture=instanceTex;
            MeshObject.GetComponent<MeshRenderer>().material=mat;
        }
        //genNewThread.Wait();
        //Mesh mesh=genNewThread.Result;
        MeshObject.GetComponent<MeshFilter>().mesh=mesh; 
        if(disload<=1) {
            MeshObject.GetComponent<MeshCollider>().sharedMesh=mesh;
        }
    }
}
