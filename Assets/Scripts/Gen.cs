using UnityEngine;

public static class Gen {
    public struct meshData{
        public static Vector3[] vertices;
        public static int[] triangles;
        public static Vector2[] uvs;
    }
    public const int mapSize=129;
    public static float[,] noiseGen(Vector2 offset,int mapWidth,int mapHeight,float mapScale) {
        float[,] HeightMap=new float[mapWidth,mapHeight];
        for(int j=0;j<mapHeight;j++) {
            for(int i=0;i<mapWidth;i++) {
                float sampleX=i/mapScale+offset.x;
                float sampleY=j/mapScale+offset.y;
                HeightMap[i,j]=Mathf.PerlinNoise(sampleX,sampleY);
                HeightMap[i,j]=HeightMap[i,j]*2-1;
            }
        }
        return HeightMap;
    }

    public static Mesh GenerateMesh(int lod,float mapScale,float HeightMultiplier,float[,] noise,AnimationCurve ac,bool useFlatShading) {
        int VertIncrement=(int)Mathf.Pow(2,lod);
        int VertCount=(mapSize-1)/VertIncrement;
        Vector3[] Vertices=new Vector3[(VertCount+1)*(VertCount+1)];
        int[] Triangles=new int[VertCount*VertCount*6];
        Vector2[] uvs=new Vector2[Vertices.Length];
        int trindex=0;
        int verdex=0;
        for(int i=0;i<mapSize;i+=VertIncrement) {
            for(int j=0;j<mapSize;j+=VertIncrement) {
                Vertices[verdex]=new Vector3(i,ac.Evaluate(noise[i,j])*HeightMultiplier,j);
                uvs[verdex]=new Vector2(i/(float)mapSize,j/(float)mapSize);
                verdex+=1;
                int iproj=i/VertIncrement;
                int jproj=j/VertIncrement;
                if(iproj<VertCount && jproj<VertCount){
                    Triangles[trindex]=(iproj+jproj*(VertCount+1));
                    Triangles[trindex+1]=(iproj+jproj*(VertCount+1))+VertCount+1+1;
                    Triangles[trindex+2]=(iproj+jproj*(VertCount+1))+VertCount+1;
                    Triangles[trindex+3]=(iproj+jproj*(VertCount+1));
                    Triangles[trindex+4]=(iproj+jproj*(VertCount+1))+1;
                    Triangles[trindex+5]=(iproj+jproj*(VertCount+1))+VertCount+1+1;
                    trindex+=6;
                }
            }
        }
        if(useFlatShading) {
            GenerateFlatMesh(Vertices,Triangles,uvs);
        }
        else {
            meshData.vertices=Vertices;
            meshData.triangles=Triangles;
            meshData.uvs=uvs;
        }
        return setMesh();
    }

    public static Vector2 relativeCurrCoord(Vector3 position) {
        return new Vector2((int)position.x/(mapSize-1),(int)position.z/(mapSize-1));
    }

    public static Vector2[] NearMeshCoords(Vector3 position,int chunks) {
        Vector2 CurrCoord=relativeCurrCoord(position);
        int NMcoord=(3+(2*(chunks-1)))*(3+(2*(chunks-1)));
        Vector2[] NearMesh=new Vector2[NMcoord];
        int NMcoordcount=0;
        for(int i=chunks;i>-chunks-1;i--) {
            for(int j=chunks;j>-chunks-1;j--) {
                NearMesh[NMcoordcount]=new Vector2(i+CurrCoord.x,j+CurrCoord.y);
                NMcoordcount+=1;
            }
        }
        return NearMesh;
    }

    static Mesh setMesh() {
        Mesh mesh=new Mesh();
        mesh.vertices=meshData.vertices;
        mesh.triangles=meshData.triangles;
        mesh.uv=meshData.uvs;
        mesh.RecalculateNormals();
        return mesh;
    }

    static void GenerateFlatMesh(Vector3[] vertices,int[] triangles,Vector2[] uvs) {
        Vector3[] FSvertices=new Vector3[triangles.Length];
        Vector2[] FSuvs=new Vector2[triangles.Length];
        for(int i=0;i<triangles.Length;i++) {
            FSvertices[i]=vertices[triangles[i]];
            FSuvs[i]=uvs[triangles[i]];
            triangles[i]=i;
        }
        meshData.vertices=FSvertices;
        meshData.uvs=FSuvs;
        meshData.triangles=triangles;
    }
}