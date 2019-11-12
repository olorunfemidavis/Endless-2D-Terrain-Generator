using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Endless2DTerrain
{
    public class TerrainDisplayer : MonoBehaviour
    {
        // Pluggable Terrain Settings
        public Settings TerrainSettings;

        //Length of preview in design view
        [Range(100, 1000)] public float PreviewLength;
        [Range(0.1f, 3)] public float GenerateTerrainRefreshTime = 0.5f;

        //Our list of terrain generation and prefab generation rules
        [HideInInspector] public List<TerrainRule> Rules;
        [HideInInspector] public List<PrefabRule> PrefabRules;

        //References to the terrain and prefab managers
        [HideInInspector] public TerrainManager TerrainManager { get; set; }
        [HideInInspector] public PrefabManager PrefabManager { get; set; }

        private float currentX;
        private bool coroutineRunning = false;

        void Awake()
        {
            if (!TerrainSettings)
                Debug.LogError("TerrainSettings required");
            else
                Setup();
        }

        public void Setup()
        {
            if (Rules != null)
            {
                Cleanup(TerrainSettings.TerrainManagerName);
                TerrainSettings.terrainDisplayer = this;
                TerrainManager = new TerrainManager(TerrainSettings, transform);
                PrefabManager = new PrefabManager(TerrainSettings, transform);
            }
        }

        //Remove the terrain and prefab managers and reset what we need to for the rules.  This is called when we switch between edit and play modes
        public void Cleanup(string TerrainManagerName)
        {
            if (TerrainManager != null)
            {
                TerrainManager.RemoveTerrainObject(TerrainManagerName);
            }
            if (PrefabManager != null)
            {
                PrefabManager.RemovePrefabObject(TerrainManagerName);
            }

            for (int i = 0; i < PrefabRules.Count; i++)
            {
                PrefabRules[i].CurrentLocation = Vector3.zero;
                PrefabRules[i].LastPrefabLocation = Vector3.zero;
            }
        }

        // Use this for initialization
        void Start()
        {
            //Generate the initial terrain to avoid slowdown once we start
            Shader.WarmupAllShaders();
        }

        // Update is called once per frame
        void Update()
        {
            if (!coroutineRunning)
            {
                StartCoroutine(GenerateTerrainCoroutine(TerrainSettings.LeadAmount));
            }
        }

        private IEnumerator GenerateTerrainCoroutine(float leadAmount)
        {
            coroutineRunning = true;

            GenerateTerrain(leadAmount);

            //No need to run this every frame...just run it every so often
            yield return new WaitForSeconds(GenerateTerrainRefreshTime);

            coroutineRunning = false;
        }

        public void GenerateTerrain(float leadAmount)
        {
            //Track the right and left sides of the screen so we know how much terrain to generate
            Vector3 rightSide = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, -Camera.main.transform.position.z));
            Vector3 leftSide = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, -Camera.main.transform.position.z));
            float endX = rightSide.x + leadAmount;

            while (TerrainManager.VertexGen.CurrentTerrainRule != null && TerrainManager.GetFarthestX() < endX)
            {
                TerrainManager.Generate(endX);
                //Update our prefabs with the current terrain info       
                PrefabManager.PlacePrefabs(TerrainManager);

                TerrainManager.Cleanup(leftSide.x - leadAmount);
                PrefabManager.Cleanup(leftSide.x - leadAmount);
            }

            //Only need this when we are no longer generating any new terrain but still need to do the final object cleanup
            if (TerrainManager.VertexGen.CurrentTerrainRule == null && TerrainManager.Pool != null && TerrainManager.Pool.TerrainPieces.Count > 0)
            {
                TerrainManager.Cleanup(leftSide.x);
                PrefabManager.Cleanup(leftSide.x);
            }
        }
    }
}
