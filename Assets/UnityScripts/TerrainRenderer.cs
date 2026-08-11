using System.Collections.Generic;

using UnityEngine;

namespace Simulation.Unity
{
    public class TerrainRenderer : MonoBehaviour
    {
        [SerializeField] private List<Ter> terrains;
        [SerializeField] private TerrainGenerationSettings settings;
        private TerrainGenerator generator;

        public bool generate;
        private bool generating;
        private int loaded;
        private void Awake()
        {
            generator = new(settings);
        }
        private void Update()
        {
            if(generate && !generating && Application.isPlaying)
            {
                generate = false;
                generating = true;
                foreach(var terrain in terrains)
                {
                    Generate(terrain);
                    terrain.terrain.transform.localPosition = terrain.position;
                }
            }
        }
        public void Generate(Ter terrainData)
        {
            ThreadsManager.QueueWorker(0, () =>
            {
                UnityEngine.TerrainData terrain = terrainData.data;
                Vector3 position = terrainData.position;
                generator.SetSettings(settings);
                terrainData.terrainData = generator.Generate(position, out Color[] colors);
                ThreadsManager.QueueMain(() =>
                {
                    HandleTexture(terrainData.material, colors);
                    terrain.SetHeights(0, 0, terrainData.terrainData.GetHeightMap());
                    loaded++;
                    if(loaded == terrains.Count)
                    {
                        loaded = 0;
                        generating = false;
                    }
                });
            });
        }
        public void HandleTexture(Material material, Color[] colors)
        {
            Texture2D resizedTexture = new(1024, 1024);
            resizedTexture.SetPixels(colors);
            resizedTexture.Apply();

            material.SetTexture("_BaseMap", resizedTexture);
        }
        Texture2D ResizeTexture(Texture2D originalTexture, int newWidth, int newHeight)
        {
            RenderTexture rt = new RenderTexture(newWidth, newHeight, 24);
            RenderTexture.active = rt;
            Graphics.Blit(originalTexture, rt);
            Texture2D newTexture = new Texture2D(newWidth, newHeight);
            newTexture.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
            newTexture.Apply();
            return newTexture;
        }
        [System.Serializable]
        public class Ter
        {
            public TerrainData data;
            public Material material;
            public UnityEngine.Terrain terrain;
            public Vector3 position;
            public Terrain terrainData;
        }
    }
}