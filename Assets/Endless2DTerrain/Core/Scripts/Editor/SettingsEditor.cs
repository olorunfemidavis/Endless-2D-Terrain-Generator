using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Endless2DTerrain
{
    [CustomEditor(typeof(Settings))]
    public class SettingsEditor : Editor
    {
        private Settings s;
        private SerializedObject serializedSettings;

        private SerializedProperty TerrainManagerName;
        private SerializedProperty TerrainLayer;
        private SerializedProperty OriginalStartPoint;

        private SerializedProperty LeadAmount;

        //Front plane
        private SerializedProperty MainMaterial;
        private SerializedProperty MainMaterialTiling;
        private SerializedProperty MainMaterialRotation;
        private SerializedProperty MainPlaneHeight;
        private SerializedProperty MainPlaneFollowTerrainCurve;

        //Top plane
        private SerializedProperty TopMaterial;
        private SerializedProperty TopPhysicsMaterial2D;
        private SerializedProperty TopLayerMask;
        private SerializedProperty TopMaterialTiling;
        private SerializedProperty TopMaterialRotation;
        private SerializedProperty TopPlaneHeight;
        public SerializedProperty DrawTopMeshCollider;
        public SerializedProperty DrawTopMeshRenderer;

        //Detail plane
        public SerializedProperty DetailMaterial;
        public SerializedProperty DetailMaterialTiling;
        public SerializedProperty DetailMaterialRotation;
        public SerializedProperty DetailPlaneHeight;
        public SerializedProperty DrawDetailMeshRenderer;
        private SerializedProperty DetailPlaneFollowTerrainCurve;

        //Terrain rules
        private SerializedProperty Rules;

        //Prefab rules
        private SerializedProperty PrefabRules;

        public SerializedProperty dimensionFoldout, topPlaneFoldout, frontPlaneFoldout, detailPlaneFoldout, terrainGenerationRules, prefabGenerationRules;
        private List<bool> RulesExpanded;
        private List<bool> PrefabRulesExpanded;

        private static GUIContent addButton = new GUIContent("Add Terrain Rule", "Add Terrain Generation Rule");
        private static GUIContent removeButton = new GUIContent("Delete", "Delete Terrain Generation Rule");
        private static GUIContent upButton = new GUIContent("Up", "Move Rule Up One");
        private static GUIContent downButton = new GUIContent("Down", "Move Rule Down One");

        private static GUIContent addPrefabButton = new GUIContent("Add Prefab Rule", "Add Prefab Generation Rule");
        private static GUIContent removePrefabButton = new GUIContent("Delete", "Delete Prefab Generation Rule");

        void OnEnable()
        {
            s = (Settings)target;
            serializedSettings = new SerializedObject(target);

            TerrainManagerName = serializedSettings.FindProperty("TerrainManagerName");
            TerrainLayer = serializedSettings.FindProperty("TerrainLayer");
            OriginalStartPoint = serializedSettings.FindProperty("OriginalStartPoint");
            LeadAmount = serializedSettings.FindProperty("LeadAmount");

            //Load all the values from the inspector into the serialized property editor objects
            MainMaterial = serializedSettings.FindProperty("MainMaterial");
            MainMaterialTiling = serializedSettings.FindProperty("MainMaterialTiling");
            MainMaterialRotation = serializedSettings.FindProperty("MainMaterialRotation");
            MainPlaneHeight = serializedSettings.FindProperty("MainPlaneHeight");
            MainPlaneFollowTerrainCurve = serializedSettings.FindProperty("MainPlaneFollowTerrainCurve");

            TopMaterial = serializedSettings.FindProperty("TopMaterial");
            TopPhysicsMaterial2D = serializedSettings.FindProperty("TopPhysicsMaterial2D");
            TopMaterialTiling = serializedSettings.FindProperty("TopMaterialTiling");
            TopMaterialRotation = serializedSettings.FindProperty("TopMaterialRotation");
            TopPlaneHeight = serializedSettings.FindProperty("TopPlaneHeight");
            DrawTopMeshCollider = serializedSettings.FindProperty("DrawTopMeshCollider");
            DrawTopMeshRenderer = serializedSettings.FindProperty("DrawTopMeshRenderer");

            DetailMaterial = serializedSettings.FindProperty("DetailMaterial");
            DetailMaterialTiling = serializedSettings.FindProperty("DetailMaterialTiling");
            DetailMaterialRotation = serializedSettings.FindProperty("DetailMaterialRotation");
            DetailPlaneHeight = serializedSettings.FindProperty("DetailPlaneHeight");
            DrawDetailMeshRenderer = serializedSettings.FindProperty("DrawDetailMeshRenderer");
            DetailPlaneFollowTerrainCurve = serializedSettings.FindProperty("DetailPlaneFollowTerrainCurve");

            Rules = serializedSettings.FindProperty("Rules");
            PrefabRules = serializedSettings.FindProperty("PrefabRules");


            dimensionFoldout = serializedSettings.FindProperty("dimensionFoldout");
            topPlaneFoldout = serializedSettings.FindProperty("topPlaneFoldout");
            frontPlaneFoldout = serializedSettings.FindProperty("frontPlaneFoldout");
            detailPlaneFoldout = serializedSettings.FindProperty("detailPlaneFoldout");
            terrainGenerationRules = serializedSettings.FindProperty("terrainGenerationRules");
            prefabGenerationRules = serializedSettings.FindProperty("prefabGenerationRules");

            RulesExpanded = new List<bool>();
            PrefabRulesExpanded = new List<bool>();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(TerrainManagerName, new GUIContent("Terrain Identifier", "A Terrain Manager Unique Name. Provide a different name if There is more than one instance"));
            EditorGUILayout.PropertyField(TerrainLayer, new GUIContent("Terrain Layer", "Layer to assign to the terrain"));

            dimensionFoldout.boolValue = EditorGUILayout.Foldout(dimensionFoldout.boolValue, "Position");
            if (dimensionFoldout.boolValue)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical();

                EditorGUILayout.PropertyField(OriginalStartPoint, new GUIContent("Original Start Point", "Original Start Point"));
                EditorGUILayout.PropertyField(LeadAmount, new GUIContent("Lead Amount", "Lead Amount Left and Right of the Camera to Delete or Create Terrain Pieces"));

                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }

            frontPlaneFoldout.boolValue = EditorGUILayout.Foldout(frontPlaneFoldout.boolValue, "Front Mesh");
            if (frontPlaneFoldout.boolValue)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical();

                EditorGUILayout.PropertyField(MainMaterial, new GUIContent("Material", "The material that will be rendered on this plane of the mesh"));
                EditorGUILayout.PropertyField(MainPlaneHeight, new GUIContent("Height", "The height of this plane of the mesh."));
                EditorGUILayout.PropertyField(MainMaterialTiling, new GUIContent("UV Tiling", "UV tiling for the plane of the mesh."));
                EditorGUILayout.PropertyField(MainMaterialRotation, new GUIContent("UV Rotation", "Rotation of the UV tiling for the mesh."));

                EditorGUILayout.PropertyField(MainPlaneFollowTerrainCurve, new GUIContent("UV Follow Terrain Curve", "Textures are bent to follow the curve of the terrain."));

                /*if (MainPlaneHeight.floatValue < 1) { MainPlaneHeight.floatValue = 1; }
                if (MainMaterialTiling.vector2Value.x < 1) { MainMaterialTiling.vector2Value.x = 1; }
                if (MainMaterialTiling.vector2Value.y < 1) { MainMaterialTiling.y.floatValue = 1; }*/

                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }

            topPlaneFoldout.boolValue = EditorGUILayout.Foldout(topPlaneFoldout.boolValue, "Top Mesh");
            if (topPlaneFoldout.boolValue)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical();

                //Only draw the top plane if the checkbox is checked
                EditorGUILayout.PropertyField(DrawTopMeshRenderer, new GUIContent("Draw Top Mesh Renderer", "Draw top mesh renderer if players can see the top of the mesh."));
                EditorGUILayout.PropertyField(DrawTopMeshCollider, new GUIContent("Draw Top Mesh Collider", "Draw the top mesh collider to keep from falling through the mesh."));

                if (DrawTopMeshCollider.boolValue || DrawTopMeshRenderer.boolValue)
                {
                    EditorGUILayout.PropertyField(TopMaterial, new GUIContent("Material", "The material that will be rendered on this plane of the mesh."));
                    EditorGUILayout.PropertyField(TopPhysicsMaterial2D, new GUIContent("Material", "The material that will be rendered on this plane of the mesh."));
                    EditorGUILayout.PropertyField(TopPlaneHeight, new GUIContent("Height", "The height of this plane of the mesh."));
                    EditorGUILayout.PropertyField(TopMaterialTiling, new GUIContent("UV X Tiling", "UV tiling for the plane of the mesh."));
                    EditorGUILayout.PropertyField(TopMaterialRotation, new GUIContent("UV Rotation", "Rotation of the UV tiling for the mesh."));

                    /*if (s.TopPlaneHeight < 1) { s.TopPlaneHeight = 1; }
                    if (s.TopMaterialTiling.x < 1) { s.TopMaterialTiling.x = 1; }
                    if (s.TopMaterialTiling.y < 1) { s.TopMaterialTiling.y = 1; }*/
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }

            detailPlaneFoldout.boolValue = EditorGUILayout.Foldout(detailPlaneFoldout.boolValue, "Detail Mesh");
            if (detailPlaneFoldout.boolValue)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical();

                //Only draw the detail plane if we are rendering it
                EditorGUILayout.PropertyField(DrawDetailMeshRenderer, new GUIContent("Draw Detail Mesh Renderer", "Check if you want to add a detail mesh on top of the front mesh"));
                if (DrawDetailMeshRenderer.boolValue)
                {
                    EditorGUILayout.PropertyField(DetailMaterial, new GUIContent("Material", "The material that will be rendered on this plane of the mesh."));
                    EditorGUILayout.PropertyField(DetailPlaneHeight, new GUIContent("Height", "The height of this plane of the mesh."));
                    EditorGUILayout.PropertyField(DetailMaterialTiling, new GUIContent("UV X Tiling", "UV tiling for the plane of the mesh."));
                    EditorGUILayout.PropertyField(DetailMaterialRotation, new GUIContent("UV Rotation", "Rotation of the UV tiling for the mesh."));

                    EditorGUILayout.PropertyField(DetailPlaneFollowTerrainCurve, new GUIContent("UV Follow Terrain Curve", "Textures are bent to follow the curve of the terrain."));

                    /*if (s.DetailPlaneHeight < 1) { s.DetailPlaneHeight = 1; }
                    if (s.DetailMaterialTiling.x < 1) { s.DetailMaterialTiling.x = 1; }
                    if (s.DetailMaterialTiling.y < 1) { s.DetailMaterialTiling.y = 1; }*/
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();

            //Group all the terrain generation rules under one foldout
            terrainGenerationRules.boolValue = EditorGUILayout.Foldout(terrainGenerationRules.boolValue, "Terrain Generation Rules");

            if (GUILayout.Button(addButton))
            {
                Rules.arraySize += 1;
                terrainGenerationRules.boolValue = true;
            }

            if (Rules.arraySize == 0) // at least one rule required
            {
                Rules.arraySize += 1;
                terrainGenerationRules.boolValue = true;
            }

            if (terrainGenerationRules.boolValue)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical();

                //Loop through the terrain generation rules
                for (int i = 0; i < Rules.arraySize; i++)
                {

                    SerializedProperty terrainRule = Rules.GetArrayElementAtIndex(i);

                    //Get the properites of each terrain rule
                    SerializedProperty selectedTerrainStyle = terrainRule.FindPropertyRelative("SelectedTerrainStyle");
                    SerializedProperty selectedTerrainLength = terrainRule.FindPropertyRelative("SelectedTerrainLength");
                    SerializedProperty minimumKeyVertexHeight = terrainRule.FindPropertyRelative("MinimumKeyVertexHeight");
                    SerializedProperty maximumKeyVertexHeight = terrainRule.FindPropertyRelative("MaximumKeyVertexHeight");
                    SerializedProperty minimumKeyVertexSpacing = terrainRule.FindPropertyRelative("MinimumKeyVertexSpacing");
                    SerializedProperty maximumKeyVertexSpacing = terrainRule.FindPropertyRelative("MaximumKeyVertexSpacing");
                    SerializedProperty calculatedVertexSpacing = terrainRule.FindPropertyRelative("CalculatedVertexSpacing");
                    SerializedProperty ruleLength = terrainRule.FindPropertyRelative("RuleLength");
                    SerializedProperty meshLength = terrainRule.FindPropertyRelative("MeshLength");
                    SerializedProperty angle = terrainRule.FindPropertyRelative("Angle");
                    SerializedProperty expandedRules = terrainRule.FindPropertyRelative("ExpandedRules");

                    //Determine if the rule is expanded or collapsed
                    if (RulesExpanded.Count <= i)
                    {
                        RulesExpanded.Add(true);
                    }

                    RulesExpanded[i] = EditorGUILayout.Foldout(RulesExpanded[i], "Terrain Rule " + (i + 1).ToString() + (i == 0 ? " (Required)" : ""));


                    if (RulesExpanded[i])
                    {
                        EditorGUILayout.BeginHorizontal();

                        EditorGUILayout.Space();
                        EditorGUILayout.BeginVertical();

                        //Delete this element if the remove button is clicked
                        bool deleted = false;
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button(removeButton, GUILayout.Width(50)))
                        {
                            Rules.DeleteArrayElementAtIndex(i);
                            deleted = true;
                        }

                        if (i != 0)
                        {
                            if (GUILayout.Button(upButton, GUILayout.Width(50)))
                            {
                                Rules.MoveArrayElement(i, i - 1);
                            }
                        }

                        if (i != Rules.arraySize - 1)
                        {
                            if (GUILayout.Button(downButton, GUILayout.Width(50)))
                            {
                                Rules.MoveArrayElement(i, i + 1);
                            }
                        }

                        EditorGUILayout.EndHorizontal();

                        //Don't layout anything else if we deleted the element
                        if (!deleted)
                        {
                            //Now allow the users to edit the rule
                            EditorGUILayout.PropertyField(selectedTerrainStyle, new GUIContent("Terrain Style", "Repeated = Consistent hills of the same height." + System.Environment.NewLine + "Random = Randomly generated hills of different heights."));
                            EditorGUILayout.PropertyField(selectedTerrainLength, new GUIContent("Terrain Length", "Fixed = Terrain will only be generated up the rule length number of units. " + System.Environment.NewLine + "Infinite = Terrain will be generated forever."));

                            EditorGUILayout.PropertyField(minimumKeyVertexHeight, new GUIContent("Min Terrain Height", "Low point for generated mesh verticies."));
                            EditorGUILayout.PropertyField(maximumKeyVertexHeight, new GUIContent("Max Terrain Height", "High point for generated mesh verticies."));
                            minimumKeyVertexSpacing.floatValue = EditorGUILayout.Slider(value: minimumKeyVertexSpacing.floatValue, label: new GUIContent("Min Key Vertex Spacing", "The minimum distance apart your key vertices will be placed.")
                                , leftValue: 1f
                                , rightValue: 1000f);
                            maximumKeyVertexSpacing.floatValue = EditorGUILayout.Slider(value: maximumKeyVertexSpacing.floatValue, label: new GUIContent("Max Key Vertex Spacing", "The maximum distance apart your key vertices will be placed.")
                                , leftValue: minimumKeyVertexSpacing.floatValue > maximumKeyVertexSpacing.floatValue ? minimumKeyVertexSpacing.floatValue : 1f
                                , rightValue: 1000f);
                            calculatedVertexSpacing.floatValue = EditorGUILayout.Slider(value: calculatedVertexSpacing.floatValue, label: new GUIContent("Calculated Vertex Spacing", "How far apart the verticies between key verticies will be placed")
                                , leftValue: 1f
                                , rightValue: 100f);

                            if (selectedTerrainLength.intValue == (int)Endless2DTerrain.TerrainRule.TerrainLength.Fixed)
                            {
                                ruleLength.floatValue = EditorGUILayout.Slider(value: ruleLength.floatValue, label: new GUIContent("Rule Length", "How long terrain will be generated for this rule."), leftValue: 100f, rightValue: 1000f);
                            }

                            meshLength.floatValue = EditorGUILayout.Slider(value: meshLength.floatValue, label: new GUIContent("Mesh Length", "How many units in length the the meshes will be for the rule.")
                                , leftValue: maximumKeyVertexSpacing.floatValue * 2f > meshLength.floatValue ? maximumKeyVertexSpacing.floatValue * 2f : 1f
                                , rightValue: selectedTerrainLength.intValue == (int)Endless2DTerrain.TerrainRule.TerrainLength.Fixed ? ruleLength.floatValue : 100f);

                            angle.floatValue = EditorGUILayout.Slider(value: angle.floatValue, label: new GUIContent("Angle", "The angle your generated terrain will be set at."), leftValue: -80f, rightValue: 80f);

                            //Determine what prefabs can be seen on this rule, provided we have prefab rules
                            if (PrefabRules.arraySize > 0)
                            {
                                expandedRules.boolValue = EditorGUILayout.Foldout(expandedRules.boolValue, "Prefabs Allowed On This Rule");

                                for (int j = 0; j < PrefabRules.arraySize; j++)
                                {
                                    //Add all the prefab rules first
                                    SerializedProperty allowedPrefab = PrefabRules.GetArrayElementAtIndex(j);
                                    SerializedProperty allowedPrefabToClone = allowedPrefab.FindPropertyRelative("PrefabToClone");
                                    SerializedProperty allowedPrefabs = terrainRule.FindPropertyRelative("AllowedPrefabs");

                                    //Delete any values past the existing one
                                    if (allowedPrefabs.arraySize > PrefabRules.arraySize)
                                    {
                                        for (int del = PrefabRules.arraySize - 1; del < allowedPrefabs.arraySize; del++)
                                        {
                                            allowedPrefabs.DeleteArrayElementAtIndex(del);
                                        }
                                    }

                                    //Is there something at this index?  If not, add it
                                    if (j >= allowedPrefabs.arraySize)
                                    {
                                        allowedPrefabs.InsertArrayElementAtIndex(j);
                                    }

                                    //Get our values
                                    SerializedProperty allowedPrefabRule = allowedPrefabs.GetArrayElementAtIndex(j);
                                    SerializedProperty name = allowedPrefabRule.FindPropertyRelative("Name");
                                    SerializedProperty index = allowedPrefabRule.FindPropertyRelative("Index");
                                    SerializedProperty allowed = allowedPrefabRule.FindPropertyRelative("Allowed");

                                    //And set them if they are not set
                                    if (System.String.IsNullOrEmpty(name.stringValue))
                                    {
                                        allowed.boolValue = true;
                                    }

                                    //Don't bother showing this if they haven't set a value for the prefab
                                    if (allowedPrefabToClone.objectReferenceValue != null)
                                    {
                                        name.stringValue = allowedPrefabToClone.objectReferenceValue.name + " - Prefab Rule " + (index.intValue + 1).ToString();
                                        index.intValue = j;

                                        if (expandedRules.boolValue)
                                        {
                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.Space();
                                            EditorGUILayout.LabelField(new GUIContent(name.stringValue));
                                            allowed.boolValue = EditorGUILayout.Toggle(allowed.boolValue);
                                            EditorGUILayout.EndHorizontal();
                                        }
                                    }
                                }
                            }
                        }

                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();
                    }
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            //Group all the terrain generation rules under one foldout
            prefabGenerationRules.boolValue = EditorGUILayout.Foldout(prefabGenerationRules.boolValue, "Prefab Generation Rules");

            if (GUILayout.Button(addPrefabButton))
            {
                PrefabRules.arraySize += 1;
                prefabGenerationRules.boolValue = true;
            }

            if (prefabGenerationRules.boolValue)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical();

                //Loop through the terrain generation rules
                for (int i = 0; i < PrefabRules.arraySize; i++)
                {
                    SerializedProperty prefabRule = PrefabRules.GetArrayElementAtIndex(i);

                    //Get the properites of each terrain rule
                    SerializedProperty prefabToClone = prefabRule.FindPropertyRelative("PrefabToClone");
                    SerializedProperty offset = prefabRule.FindPropertyRelative("Offset");
                    SerializedProperty minRepeatDistance = prefabRule.FindPropertyRelative("MinRepeatDistance");
                    SerializedProperty maxRepeatDistance = prefabRule.FindPropertyRelative("MaxRepeatDistance");
                    SerializedProperty minGroupSize = prefabRule.FindPropertyRelative("MinGroupSize");
                    SerializedProperty maxGroupSize = prefabRule.FindPropertyRelative("MaxGroupSize");
                    SerializedProperty minGroupSpacing = prefabRule.FindPropertyRelative("MinGroupSpacing");
                    SerializedProperty maxGroupSpacing = prefabRule.FindPropertyRelative("MaxGroupSpacing");
                    SerializedProperty minSlope = prefabRule.FindPropertyRelative("MinSlope");
                    SerializedProperty maxSlope = prefabRule.FindPropertyRelative("MaxSlope");
                    SerializedProperty matchGroundAngle = prefabRule.FindPropertyRelative("MatchGroundAngle");

                    SerializedProperty useMinDistance = prefabRule.FindPropertyRelative("UseMinDistance");
                    SerializedProperty minDistance = prefabRule.FindPropertyRelative("MinDistance");
                    SerializedProperty useMaxDistance = prefabRule.FindPropertyRelative("UseMaxDistance");
                    SerializedProperty maxDistance = prefabRule.FindPropertyRelative("MaxDistance");

                    //Determine if the rule is expanded or collapsed
                    if (PrefabRulesExpanded.Count <= i)
                    {
                        PrefabRulesExpanded.Add(true);
                    }

                    PrefabRulesExpanded[i] = EditorGUILayout.Foldout(PrefabRulesExpanded[i], "Prefab Rule " + (i + 1).ToString());


                    if (PrefabRulesExpanded[i])
                    {
                        EditorGUILayout.BeginHorizontal();

                        EditorGUILayout.Space();
                        EditorGUILayout.BeginVertical();

                        //Delete this element if the remove button is clicked
                        bool ruleDeleted = false;
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button(removePrefabButton, GUILayout.Width(50)))
                        {
                            PrefabRules.DeleteArrayElementAtIndex(i);
                            ruleDeleted = true;
                        }

                        EditorGUILayout.EndHorizontal();

                        //Don't layout anything else if we deleted the element
                        if (!ruleDeleted)
                        {
                            //Now allow the users to edit the rule
                            EditorGUILayout.PropertyField(prefabToClone, new GUIContent("Prefab To Clone", "The prefab you want to clone."));
                            EditorGUILayout.PropertyField(offset, new GUIContent("Offset", "The amount the prefab will be offset fron the default placement."), true);
                            EditorGUILayout.PropertyField(minRepeatDistance, new GUIContent("Min Repeat Distance", "Minimum distance between prefab placement."));
                            EditorGUILayout.PropertyField(maxRepeatDistance, new GUIContent("Max Repeat Distance", "Maximum distance between prefab placement."));
                            EditorGUILayout.PropertyField(minGroupSize, new GUIContent("Min Group Size", "Minimum group size for prefabs - used if you want more than one prefab generated at a time."));
                            EditorGUILayout.PropertyField(maxGroupSize, new GUIContent("Max Group Size", "Maximum group size for prefabs - used if you want more than one prefab generated at a time."));
                            EditorGUILayout.PropertyField(minGroupSpacing, new GUIContent("Min Group Spacing", "The minimum spacing between the prefabs in your group."));
                            EditorGUILayout.PropertyField(maxGroupSpacing, new GUIContent("Max Group Spacing", "The maximum spacing between the prefabs in your group."));
                            EditorGUILayout.PropertyField(minSlope, new GUIContent("Min Slope Placement", "Prefabs will only be generated on slopes if the slopoe is greater than this angle."));
                            EditorGUILayout.PropertyField(maxSlope, new GUIContent("Max Slope Placement", "Prefabs will only be generated on slopes if the slope is less than this angle."));
                            EditorGUILayout.PropertyField(matchGroundAngle, new GUIContent("Match Ground Angle", "Rotate the prefabs to match the current slope of the ground."));

                            //Set min and max distances
                            useMinDistance.boolValue = EditorGUILayout.Toggle("Use Min Distance", useMinDistance.boolValue);
                            if (useMinDistance.boolValue)
                            {
                                EditorGUILayout.PropertyField(minDistance, new GUIContent("Min Distance"));
                            }

                            useMaxDistance.boolValue = EditorGUILayout.Toggle("Use Max Distance", useMaxDistance.boolValue);
                            if (useMaxDistance.boolValue)
                            {
                                EditorGUILayout.PropertyField(maxDistance, new GUIContent("Max Distance"));
                            }

                            if (minDistance.floatValue < 0) { minDistance.floatValue = 0; }
                            if (maxDistance.floatValue < 0) { maxDistance.floatValue = 0; }

                            if (minRepeatDistance.floatValue > maxRepeatDistance.floatValue) { maxRepeatDistance.floatValue = minRepeatDistance.floatValue; }
                            if (minGroupSize.intValue > maxGroupSize.intValue) { maxGroupSize.intValue = minGroupSize.intValue; }
                            if (minGroupSpacing.floatValue > maxGroupSpacing.floatValue) { maxGroupSpacing.floatValue = minGroupSpacing.floatValue; }
                            if (minGroupSpacing.floatValue < 1)
                            {
                                minGroupSpacing.floatValue = 1;
                            }
                            if (maxGroupSpacing.floatValue < 1)
                            {
                                maxGroupSpacing.floatValue = 1;
                            }
                            if (minRepeatDistance.floatValue < 1)
                            {
                                minRepeatDistance.floatValue = 1;
                            }

                            //Set some default if these aren't set
                            if (maxSlope.floatValue == 0 && minSlope.floatValue == 0)
                            {
                                maxSlope.floatValue = 90f;
                                minSlope.floatValue = -90f;
                            }
                        }

                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();

                    }
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }

            //Now update all the modified properties
            serializedSettings.ApplyModifiedProperties();

            GUILayout.Space(10f);
            if (GUILayout.Button("Preview Terrain"))
            {
                s.PreviewTerrain(LeadAmount.floatValue);
            }
        }
    }
}