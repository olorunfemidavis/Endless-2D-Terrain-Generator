using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Endless2DTerrain
{
    [CreateAssetMenu(menuName = "Endless2DTerrain/Settings")]
    public class Settings : ScriptableObject
    {
        [Header("Terrain Identifier")]
        [Tooltip("A Terrain Manager Unique Name. Provide a different name if There is more than one instance")]
        public string TerrainManagerName = "Terrain";

        [Header("Terrain Dimensions")]
        public Vector3 OriginalStartPoint;
        [Tooltip("Lead Amount Left and Right of the Camera to Delete or Create Terrain Pieces")]
        public float LeadAmount = 100f;
        [Tooltip("The height of the main plane mesh")]
        [Range(1, 100)] public float MainPlaneHeight;
        [Tooltip("The depth of the top plane mesh")]
        public float TopPlaneHeight;
        [Tooltip("The height of the detail plane mesh")]
        public float DetailPlaneHeight;
        [Tooltip("Width for corner mesh where two planes with different angles meet")]
        public float CornerMeshWidth;
        [Tooltip("Detail Plane Offset")]
        public Vector3 DetailPlaneOffset = new Vector3(0, .1f, -.2f);

        [Header("Main Material")]
        [Tooltip("The material that will be rendered on this plane of the mesh")]
        public Material MainMaterial;
        [Tooltip("UV tiling for the x plane of the mesh")]
        public Vector2 MainMaterialTiling;
        [Tooltip("UV tiling for the plane of the mesh")]
        [Range(0, 360)] public float MainMaterialRotation;
        [Tooltip("Textures are bent to follow the curve of the terrain")]
        public bool MainPlaneFollowTerrainCurve;

        [Header("Top Material")]
        [Tooltip("Draw top mesh renderer if players can see the top of the mesh")]
        public bool DrawTopMeshRenderer;
        [Tooltip("Draw the top mesh collider to keep from falling through the mesh")]
        public bool DrawTopMeshCollider;
        [Tooltip("The material that will be rendered on this plane of the mesh")]
        public Material TopMaterial;
        [Tooltip("The physics material that will be added to this plane of the mesh")]
        public PhysicsMaterial2D TopPhysicsMaterial2D;
        [Tooltip("UV tiling for the plane of the mesh")]
        public Vector2 TopMaterialTiling;
        [Tooltip("Rotation of the UV tiling for the mesh")]
        [Range(0, 360)] public float TopMaterialRotation;

        [Header("Detail Material")]
        [Tooltip("Check if you want to add a detail mesh on top of the front mesh")]
        public bool DrawDetailMeshRenderer;
        [Tooltip("The material that will be rendered on this plane of the mesh.")]
        public Material DetailMaterial;
        [Tooltip("UV tiling for the plane of the mesh")]
        public Vector2 DetailMaterialTiling;
        [Tooltip("Rotation of the UV tiling for the mesh")]
        [Range(0, 360)] public float DetailMaterialRotation;
        [Tooltip("Textures are bent to follow the curve of the terrain.")]
        public bool DetailPlaneFollowTerrainCurve;

        [Header("Terrain Generation Rules")]
        [Tooltip("Enter terrain generation rules.  At least one is required.")]
        public List<TerrainRule> Rules;

        [Header("Prefab Generation Rules")]
        public List<PrefabRule> PrefabRules;

        [HideInInspector] public bool dimensionFoldout, topPlaneFoldout, frontPlaneFoldout, detailPlaneFoldout, terrainGenerationRules, prefabGenerationRules;

        [HideInInspector] public TerrainDisplayer terrainDisplayer;

        public void PreviewTerrain(float leadAmount)
        {
            if (!terrainDisplayer)
                Debug.LogWarning("Settings " + TerrainManagerName + " must be assigned to a TerrainDisplayer");
            else
            {
                terrainDisplayer.Setup();
                terrainDisplayer.GenerateTerrain(leadAmount);
            }
        }
    }
}

