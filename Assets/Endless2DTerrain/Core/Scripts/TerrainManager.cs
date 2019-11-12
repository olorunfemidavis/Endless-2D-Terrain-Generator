using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Endless2DTerrain
{
    public class TerrainManager
    {
        public TerrainPool Pool { get; set; }
        public VertexGenerator VertexGen { get; set; }
        public Settings settings { get; set; }

        public GameObject TerrainObject { get; set; }
        public List<Vector3> AllFrontTopVerticies { get; set; }

        private Transform parentTransform;

        public TerrainManager(Settings s, Transform parentTransform)
        {
            Pool = new TerrainPool();
            VertexGen = new VertexGenerator(s);
            settings = s;
            this.parentTransform = parentTransform;
            InstantiateTerrainObject(s.TerrainManagerName);
        }

        /// <summary>
        /// Remove terrain pieces once the are out of view of the left side of the camera
        /// </summary> 
        public void Cleanup(float beginX)
        {
            List<TerrainPiece> piecesToRemove = new List<TerrainPiece>();
            for (int i = 0; i < Pool.TerrainPieces.Count; i++)
            {
                TerrainPiece piece = Pool.TerrainPieces[i];
                if (piece.NextTerrainOrigin.x < beginX)
                {
                    piecesToRemove.Add(piece);
                }
            }
            for (int i = 0; i < piecesToRemove.Count(); i++)
            {
                Pool.Remove(piecesToRemove[i]);
            }

            if (Pool.TerrainPieces.Count == 0)
            {
                RemoveTerrainObject(settings.TerrainManagerName);
            }
        }

        /// <summary>
        /// Generate terrain pieces up the the given x point in space
        /// </summary>
        /// <param name="endX"></param>
        public void Generate(float endX)
        {
            //At the end of our rules?  Stop here.
            if (VertexGen.CurrentTerrainRule == null) { return; }

            //The piece we are on right now (before generating a new piece)
            TerrainPiece currentTerrain = GetLastTerrainPiece();

            if (currentTerrain == null)
            {
                currentTerrain = GenerateTerrainPiece(null, settings.OriginalStartPoint);
            }

            while (currentTerrain.NextTerrainOrigin.x < endX)
            {
                //Generate our next terrain
                currentTerrain = GenerateTerrainPiece(currentTerrain, currentTerrain.NextTerrainOrigin);

                //Can't generate any more terrain?  Break out
                if (currentTerrain == null)
                {
                    break;
                }
            }

            //Create a terrain manager if we don't have one, and organize our objects
            if (!TerrainObject)
                InstantiateTerrainObject(settings.TerrainManagerName);

            ParentTerrainPiecesToTerrainObject();

            //Update our list of the top verticies
            UpdateAllFrontTopVerticies();

        }

        public void UpdateAllFrontTopVerticies()
        {
            if (AllFrontTopVerticies == null)
            {
                AllFrontTopVerticies = new List<Vector3>();
            }
            else
            {
                AllFrontTopVerticies.Clear();
            }

            List<TerrainPiece> pieces = Pool.TerrainPieces.OrderBy(terp => terp.FrontMesh.BottomLeftCorner.x).ToList();

            for (int i = 0; i < pieces.Count(); i++)
            {
                TerrainPiece tp = pieces[i];

                MeshPiece mp = tp.FrontMesh;
                for (int k = 0; k < mp.RotatedPlaneVerticies.Count(); k++)
                {
                    //Evens are bottom verts, odds are top
                    if (k % 2 != 0)
                    {
                        AllFrontTopVerticies.Add(mp.RotatedPlaneVerticies[k]);
                    }
                }
            }
        }

        //Get the farthest x point at the end of our last mesh
        public float GetFarthestX()
        {

            float x = 0;
            TerrainPiece last = GetLastTerrainPiece();
            if (last == null)
            {
                return x;
            }
            //Debug.Log(settings.TerrainManagerName + ".TerrainManager.GetFarthestX:return " + last.NextTerrainOrigin.x);
            return last.NextTerrainOrigin.x;
        }

        //Get the lsat terrain piece in our pool
        private TerrainPiece GetLastTerrainPiece()
        {
            if (Pool.TerrainPieces.Count > 0)
            {
                return Pool.TerrainPieces[Pool.TerrainPieces.Count - 1];
            }
            return null;
        }

        //Get the second to last terrain piece in our pool
        private TerrainPiece GetSecondToLastTerrainPiece()
        {
            if (Pool.TerrainPieces.Count > 1)
            {
                return Pool.TerrainPieces[Pool.TerrainPieces.Count - 2];
            }
            return null;
        }

        public TerrainPiece GenerateTerrainPiece(TerrainPiece currentTerrain, Vector3 origin)
        {
            //Don't keep generation if we have no rules left
            if (VertexGen.CurrentTerrainRule == null) { return null; }

            //Create our next terrain piece (consists of multiple meshes)
            TerrainPiece nextTerrain = new TerrainPiece(settings);
            nextTerrain.Create(VertexGen, origin, TerrainObject.transform);

            //We can legitimately get a null terrain object if we don't have enough verts to create another.  This then moves us to the next rule.
            //Retry once to see if we get more terrain, if not, we are at the end of the rules and cannot generate any more terrain
            if (nextTerrain.TerrainObject == null && VertexGen.CurrentTerrainRule != null)
            {
                nextTerrain.Create(VertexGen, origin, TerrainObject.transform);
            }

            //Make sure we had enough verticies to create a terrain piece
            if (nextTerrain.TerrainObject != null)
            {
                //Make sure this is not the first piece
                if (currentTerrain != null)
                {
                    //Is our angle between pieces different?  Then we need a corner piece                  
                    if (currentTerrain.TerrainAngle != nextTerrain.TerrainAngle)
                    {
                        //Move our next terrain mesh pieces out some so we can fit in the corner terrain piece
                        MoveMeshesForCornerPiece(currentTerrain, nextTerrain, origin);

                        //Create corner terrain piece between the previous piece and this one
                        TerrainPiece tpCorner = new TerrainPiece(settings);
                        tpCorner.CreateCorner(VertexGen, currentTerrain, nextTerrain, TerrainObject.transform);
                        Pool.Add(tpCorner);
                    }
                }

                //And now create our standard piece
                Pool.Add(nextTerrain);
                return nextTerrain;
            }
            else
            {
                return null;
            }
        }

        private void MoveMeshesForCornerPiece(TerrainPiece currentTerrain, TerrainPiece nextTerrain, Vector3 origin)
        {
            //Because of the rotations these can overlap.  So get the difference, then add our spacing for the corners
            //TODO: Revisit this, I don't think it is working quite right
            float maxXPrevious = currentTerrain.FrontMesh.RotatedPlaneVerticies.Select(p => p.x).Max();
            float minXCurrent = nextTerrain.FrontMesh.RotatedPlaneVerticies.Select(p => p.x).Min();
            float diff = Math.Abs(maxXPrevious - minXCurrent);
            float spacing = diff + settings.CornerMeshWidth;

            //Move every mesh piece in this terrain to give us space to have our corner mesh
            for (int j = 0; j < nextTerrain.MeshPieces.Count; j++)
            {
                MeshPiece mp = nextTerrain.MeshPieces[j];
                Vector3 moveAmount = new Vector3(origin.x + spacing, origin.y, origin.z);

                if (mp.PlaneType == MeshPiece.Plane.Detail)
                {
                    //Move the detail mesh slightly in front of the front plane mesh   
                    moveAmount = moveAmount + settings.DetailPlaneOffset;
                    mp.MoveMesh(moveAmount, mp.PlaneType);
                }
                else
                {
                    mp.MoveMesh(moveAmount, mp.PlaneType);
                }

            }
        }

        private void InstantiateTerrainObject(string NewManagerName)
        {
            TerrainObject = GameObject.Find(NewManagerName);
            if (TerrainObject)
            {
                if (!Application.isPlaying)
                {
                    foreach (Transform child in TerrainObject.transform)
                        GameObject.DestroyImmediate(child.gameObject);
                }
                else
                {
                    foreach (Transform child in TerrainObject.transform)
                        GameObject.Destroy(child.gameObject);
                }
            }
            else
            {
                TerrainObject = new GameObject(NewManagerName);
            }
            TerrainObject.transform.parent = parentTransform;
        }

        public void RemoveTerrainObject(string NewManagerName)
        {
            Debug.Log(settings.TerrainManagerName + ".TerrainManager.RemoveTerrainObject:" + NewManagerName);
            GameObject obj = GameObject.Find(NewManagerName);
            if (obj != null)
            {
                if (Application.isPlaying)
                    GameObject.Destroy(obj);
                else
                    GameObject.DestroyImmediate(obj);
            }
        }

        private void ParentTerrainPiecesToTerrainObject()
        {
            for (int i = 0; i < Pool.TerrainPieces.Count; i++)
            {
                Pool.TerrainPieces[i].TerrainObject.transform.parent = TerrainObject.transform;
            }
        }
    }
}
