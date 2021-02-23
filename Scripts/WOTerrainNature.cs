﻿using UnityEngine;
using System.Collections.Generic;
using DaggerfallConnect.Arena2;
using DaggerfallConnect;
using DaggerfallWorkshop.Utility.AssetInjection;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop
{
  /// <summary>
  /// Drops nature flats based on several factors to simulate a more natural environment.
  /// </summary>
  public class WOTerrainNature : ITerrainNature
  {
    // static GameObject exterior;
    // static List<GameObject> staticGeometryList;

    //Mod Settings
    readonly bool dynamicNatureClearance;
    readonly bool vegetationInLocations;
    static float generalNatureClearance;
    static float natureClearance1;
    static float natureClearance2;
    static float natureClearance3;
    static float natureClearance4;
    static float natureClearance5;

    const float maxSteepness   = 50f; // 50
    const float slopeSinkRatio = 70f; // 70 - Sink flats slightly into ground as slope increases to prevent floaty trees.

    // Chance for different terrain layout
    public float mapStyleChance0 = 30f;
    public float mapStyleChance1 = 40f;
    public float mapStyleChance2 = 50f;
    public float mapStyleChance3 = 60f;
    public float mapStyleChance4 = 70f;
    public float mapStyleChance5 = 80f;
    
    #region Billboard arrays
    List<int> temperateWoodlandFlowers   = new List<int>(new int[] {2,21,22});
    List<int> temperateWoodlandMushroom  = new List<int>(new int[] {7,9,23});
    List<int> temperateWoodlandBushes    = new List<int>(new int[] {1,27,28});
    List<int> temperateWoodlandRocks     = new List<int>(new int[] {3,4,5,6,8,10,26});
    List<int> temperateWoodlandTrees     = new List<int>(new int[] {11,12,13,14,15,16,17,18});
    List<int> temperateWoodlandDeadTrees = new List<int>(new int[] {19,20,24,25,29,30,31});
    List<int> temperateWoodlandBeach     = new List<int>(new int[] {31,3,3,4,4,5,5,6,29});

    List<int> woodlandHillsFlowers       = new List<int>(new int[] {2,7,21,22});
    List<int> woodlandHillsBushes        = new List<int>(new int[] {9,27,31});
    List<int> woodlandHillsRocks         = new List<int>(new int[] {1,3,4,6,8,10,17,18,28});
    List<int> woodlandHillsTrees         = new List<int>(new int[] {5,5,11,11,12,13,13,13,14,14,14,15,15,15,16,16,16,25,30});
    List<int> woodlandHillsNeedleTrees   = new List<int>(new int[] {5,11,12,25,30});
    List<int> woodlandHillsDeadTrees     = new List<int>(new int[] {19,20,24});
    List<int> woodlandHillsDirtPlants    = new List<int>(new int[] {23,26,29});
    List<int> woodlandHillsBeach         = new List<int>(new int[] {26,29,31});

    List<int> mountainsFlowers           = new List<int>(new int[] {22});
    List<int> mountainsGrass             = new List<int>(new int[] {2,7,9,23});
    List<int> mountainsRocks             = new List<int>(new int[] {1,3,4,6,8,10,14,17,18,27,28,31});
    List<int> mountainsTrees             = new List<int>(new int[] {5,11,12,13,15,21,25,30});
    List<int> mountainsNeedleTrees       = new List<int>(new int[] {5,11,12,25,30,12,25,30});
    List<int> mountainsDeadTrees         = new List<int>(new int[] {16,19,20,24,26,29});
    List<int> mountainsBeach             = new List<int>(new int[] {8,26,29,2,7,7,7,23,23});

    List<int> desertFlowers              = new List<int>(new int[] {9,9,9,17,24,24,24,26,26,31,31,31});
    List<int> desertWaterFlowers         = new List<int>(new int[] {7,17,24,24,29,29});
    List<int> desertPlants               = new List<int>(new int[] {7,25,27});
    List<int> desertWaterPlants          = new List<int>(new int[] {7,7,7,7,7,25,27,27,27,27,27});
    List<int> desertStones               = new List<int>(new int[] {2,3,4,6,8,18,19,20,21,22,});
    List<int> desertTrees                = new List<int>(new int[] {5,13,13});
    List<int> desertCactus               = new List<int>(new int[] {1,14,15,16});
    List<int> desertDeadTrees            = new List<int>(new int[] {10,11,12,23,28});

    List<int> hauntedWoodlandFlowers     = new List<int>(new int[] {21});
    List<int> hauntedWoodlandMushroom    = new List<int>(new int[] {22,23});
    List<int> hauntedWoodlandBones       = new List<int>(new int[] {11});
    List<int> hauntedWoodlandPlants      = new List<int>(new int[] {7,14,17,29,31});
    List<int> hauntedWoodlandBushes      = new List<int>(new int[] {2,26,27,28});
    List<int> hauntedWoodlandRocks       = new List<int>(new int[] {1,3,4,5,6,8,10,12});
    List<int> hauntedWoodlandTrees       = new List<int>(new int[] {13,13,13,15});
    List<int> hauntedWoodlandDirtTrees   = new List<int>(new int[] {18,19,20,31});
    List<int> hauntedWoodlandDeadTrees   = new List<int>(new int[] {16,18,24,25,30,31});
    List<int> hauntedWoodlandBeach       = new List<int>(new int[] {31,8,9,14,29,31});

    List<int> rainforestFlowers          = new List<int>(new int[] {6,20,21,22,26,27});
    List<int> rainforestEggs             = new List<int>(new int[] {28,29,31});
    List<int> rainforestPlants           = new List<int>(new int[] {2,5,10,11,23,24});
    List<int> rainforestBushes           = new List<int>(new int[] {3,9,16,18});
    List<int> rainforestRocks            = new List<int>(new int[] {1,4,17,19,25});
    List<int> rainforestTrees            = new List<int>(new int[] {12,13,14,15,30});
    List<int> rainforestBeach            = new List<int>(new int[] {});
    #endregion
    
    #region Variables for different Chances
    // Tree border
    public float treeLine = DaggerfallWorkshop.WOTerrainTexturing.treeLine;
    
    // ------------------------------------
    // TEMPERATE Climate Vegetation Chances
    // ------------------------------------
    // Chance for Mushrom Circle
    public float temperateMushroomRingChance = 0.025f;
    // Distribution Limits
    public float tempForestLimit1 = Random.Range(0.30f, 0.40f);
    public float tempForestLimit2 = Random.Range(0.50f, 0.60f);
    // Noise Parameters
    public float tempForestFrequency = Random.Range(0.04f, 0.06f); //0.05f
    public float tempForestAmplitude =  0.9f;
    public float tempForestPersistence = Random.Range(0.3f, 0.5f); //0.4f
    public int tempForestOctaves = Random.Range(2,3); //2

    // ------------------------------------
    // MOUNTAIN Climate Vegetation Chances
    // ------------------------------------
    // Chance for Stone Circle
    public float mountainStoneCircleChance = 0.025f;
    // Distribution Limits
    public float mountForestLimit1 = Random.Range(0.30f, 0.40f);
    public float mountForestLimit2 = Random.Range(0.40f, 0.50f);
    // Noise Parameters
    public float mountForestFrequency = Random.Range(0.04f, 0.06f); //0.05f
    public float mountForestAmplitude =  0.9f;
    public float mountForestPersistence = Random.Range(0.3f, 0.5f); //0.4f
    public int mountForestOctaves = Random.Range(2,3); //2

    // ------------------------------------
    // DESERT Climate Vegetation Chances
    // ------------------------------------
    // DIRT
    // Chance for Dead Tree instead of Cactus
    public float desert2DirtChance = Random.Range(0,1);
    public float desert1DirtChance = Random.Range(1,6);

    // GRASS
    // Chance for Flowers
    public float desert2GrassChance1 = Random.Range(0,10);
    public float desert1GrassChance1 = Random.Range(0,30);
    // Chance for Plants
    public float desert2GrassChance2 = Random.Range(10,15);
    public float desert1GrassChance2 = Random.Range(30,50);

    // ------------------------------------
    // HILLS Climate Vegetation Chances
    // ------------------------------------

    // DIRT
    // Chance for Dead Tree/Rocks instead of Needle Tree
    public float woodlandHillsDirtChance = Random.Range(20,30);

    // GRASS
    // Chance for Stone Circle
    public float woodlandHillsStoneCircleChance = 0.075f;
    #endregion

    public bool NatureMeshUsed { get; protected set; }

    public WOTerrainNature(
      bool DMEnabled,
      bool dNClearance,
      bool vegInLoc,
      float gNClearance,
      float nClearance1,
      float nClearance2,
      float nClearance3,
      float nClearance4,
      float nClearance5)
    {
      // Change a tree texture in desert if DREAM Sprites Mod enabled
      if(DMEnabled) {
        List<int> desertTrees              = new List<int>(new int[] {5,13,30}); 
      } else {
        List<int> desertTrees              = new List<int>(new int[] {5,13,13}); 
      }

        Debug.Log("Wilderness Overhaul: DREAM Sprites enabled: " + DMEnabled);
        dynamicNatureClearance = dNClearance;
        Debug.Log("Wilderness Overhaul: Setting Dynamic Nature Clearance: " + dynamicNatureClearance);
        vegetationInLocations = vegInLoc;
        Debug.Log("Wilderness Overhaul: Setting Vegetation in Jungle Location: " + vegetationInLocations);
        generalNatureClearance = gNClearance;
        Debug.Log("Wilderness Overhaul: Setting General Nature Clearance: " + generalNatureClearance);
        natureClearance1  = nClearance1;
        Debug.Log("Wilderness Overhaul: Setting Nature Clearance 1: " + natureClearance1);
        natureClearance2  = nClearance2;
        Debug.Log("Wilderness Overhaul: Setting Nature Clearance 2: " + natureClearance2);
        natureClearance3  = nClearance3;
        Debug.Log("Wilderness Overhaul: Setting Nature Clearance 3: " + natureClearance3);
        natureClearance4  = nClearance4;
        Debug.Log("Wilderness Overhaul: Setting Nature Clearance 4: " + natureClearance4);
        natureClearance5  = nClearance5;
        Debug.Log("Wilderness Overhaul: Setting Nature Clearance 5: " + natureClearance5);
    }

    public virtual void LayoutNature(DaggerfallTerrain dfTerrain, DaggerfallBillboardBatch dfBillboardBatch, float terrainScale, int terrainDist)
    {

      // Preparation of StaticGeometry for collision detection
      /* if(dfTerrain.MapData.hasLocation)
      {
        exterior = GameObject.Find("Exterior");
        staticGeometryList = new List<GameObject>();
        GameObject streamingTarget = GameObject.Find("StreamingTarget");
        AddDescendantsWithTag(streamingTarget.transform, "StaticGeometry", staticGeometryList);
      } */

      // Declaration of local variables
      float chanceOnDirt  = 0.0f;
      float chanceOnGrass = 0.0f;
      float chanceOnStone = 0.0f;

      #region Dynamic Nature Clearance Switch
      if(dynamicNatureClearance)
      {
        if(dfTerrain.MapData.LocationType == DFRegion.LocationTypes.TownCity)
          generalNatureClearance = natureClearance1;
        if(dfTerrain.MapData.LocationType == DFRegion.LocationTypes.TownHamlet)
          generalNatureClearance = natureClearance2;
        if(dfTerrain.MapData.LocationType == DFRegion.LocationTypes.TownVillage ||
           dfTerrain.MapData.LocationType == DFRegion.LocationTypes.HomeWealthy ||
           dfTerrain.MapData.LocationType == DFRegion.LocationTypes.ReligionCult)
          generalNatureClearance = natureClearance3;
        if(dfTerrain.MapData.LocationType == DFRegion.LocationTypes.HomeFarms ||
           dfTerrain.MapData.LocationType == DFRegion.LocationTypes.ReligionTemple ||
           dfTerrain.MapData.LocationType == DFRegion.LocationTypes.Tavern ||
           dfTerrain.MapData.LocationType == DFRegion.LocationTypes.HomePoor)
          generalNatureClearance = natureClearance4;
        if(dfTerrain.MapData.LocationType == DFRegion.LocationTypes.DungeonLabyrinth ||
           dfTerrain.MapData.LocationType == DFRegion.LocationTypes.DungeonKeep ||
           dfTerrain.MapData.LocationType == DFRegion.LocationTypes.DungeonRuin ||
           dfTerrain.MapData.LocationType == DFRegion.LocationTypes.Graveyard ||
           dfTerrain.MapData.LocationType == DFRegion.LocationTypes.Coven)
          generalNatureClearance = natureClearance5;
        if(dfTerrain.MapData.worldClimate == (int)MapsFile.Climates.Rainforest)
          generalNatureClearance = 0.5f;
      }
      #endregion

      // Location Rect is expanded slightly to give extra clearance around locations
      Rect rect = dfTerrain.MapData.locationRect;
      if (rect.x > 0 && rect.y > 0)
      {
        rect.xMin -= generalNatureClearance;
        rect.xMax += generalNatureClearance;
        rect.yMin -= generalNatureClearance;
        rect.yMax += generalNatureClearance;
      }

      float terrainElevation = Mathf.Clamp((dfTerrain.MapData.worldHeight / 128f), 0.0f, 1.0f);
      Debug.Log("Terrain Elevation: "+terrainElevation);

      // Get terrain
      Terrain terrain = dfTerrain.gameObject.GetComponent<Terrain>();
      if (!terrain)
          return;

      // Get terrain data
      TerrainData terrainData = terrain.terrainData;
      if (!terrainData)
          return;

      // Remove exiting billboards
      dfBillboardBatch.Clear();
      MeshReplacement.ClearNatureGameObjects(terrain);

      // Seed random with terrain key
      Random.InitState(TerrainHelper.MakeTerrainKey(dfTerrain.MapPixelX, dfTerrain.MapPixelY));

      // Just layout some random flats spread evenly across entire map pixel area
      // Flats are aligned with tiles, max 16129 billboards per batch
      Vector2 tilePos = Vector2.zero;
      int tDim = MapsFile.WorldMapTileDim;
      int hDim = DaggerfallUnity.Instance.TerrainSampler.HeightmapDimension;
      float scale = terrainData.heightmapScale.x * (float)hDim / (float)tDim;
      float maxTerrainHeight = DaggerfallUnity.Instance.TerrainSampler.MaxTerrainHeight;
      float beachLine = DaggerfallUnity.Instance.TerrainSampler.BeachElevation;

      // Chance scaled by base climate type
      DFLocation.ClimateSettings climate = MapsFile.GetWorldClimateSettings(dfTerrain.MapData.worldClimate);

      #region Vegetation Composition per World Height
      // Change of vegetation in regard to world height
      if(terrainElevation > 0.85f)
      {
        temperateWoodlandTrees     = new List<int>(new int[] {13,14,15,17,18,25,25,25});
        temperateWoodlandDeadTrees = new List<int>(new int[] {19,20,24,25,25,25,29,30,31});
        temperateWoodlandFlowers   = new List<int>(new int[] {21,22});
        temperateWoodlandMushroom  = new List<int>(new int[] {7,23});
        temperateWoodlandBushes    = new List<int>(new int[] {1,1,1,27,28});

        if(DaggerfallUnity.Instance.WorldTime.Now.SeasonValue == DaggerfallDateTime.Seasons.Winter)
        {
          mountainsTrees             = new List<int>(new int[] {29});
          mountainsNeedleTrees       = new List<int>(new int[] {11,25});
          mountainsDeadTrees         = new List<int>(new int[] {19,20,24,30,29});
          mountainsFlowers           = new List<int>(new int[] {7});
          mountainsRocks             = new List<int>(new int[] {1,3,4,6,9,8,10,14,17,18,27,28,31});
          mountainsGrass             = new List<int>(new int[] {7,7,7,9,26});

          woodlandHillsFlowers       = new List<int>(new int[] {7,22,22,22,22});
          woodlandHillsBushes        = new List<int>(new int[] {23,27});
          woodlandHillsTrees         = new List<int>(new int[] {5,11,12,12,13,14,14,25,12});
          woodlandHillsNeedleTrees   = new List<int>(new int[] {12,25,12});
          woodlandHillsDirtPlants    = new List<int>(new int[] {22,29,26,31});
        }
        if(DaggerfallUnity.Instance.WorldTime.Now.SeasonValue != DaggerfallDateTime.Seasons.Winter)
        {
          mountainsTrees             = new List<int>(new int[] {22});
          mountainsNeedleTrees       = new List<int>(new int[] {12,25,30,12,25,30});
          mountainsDeadTrees         = new List<int>(new int[] {19,20,24,26,29});
          mountainsFlowers           = new List<int>(new int[] {22});
          mountainsRocks             = new List<int>(new int[] {1,3,4,6,7,8,10,14,17,18,27,28,31});
          mountainsGrass             = new List<int>(new int[] {2,2,2,7,23});

          woodlandHillsFlowers       = new List<int>(new int[] {2,22,22,22,22});
          woodlandHillsBushes        = new List<int>(new int[] {9,27});
          woodlandHillsTrees         = new List<int>(new int[] {5,11,12,12,13,14,15,25,30});
          woodlandHillsNeedleTrees   = new List<int>(new int[] {12,25,30});
          woodlandHillsDirtPlants    = new List<int>(new int[] {26,29,23,31});
        }
      }

      else if(terrainElevation <= 0.85f && terrainElevation > 0.8f)
      {
        temperateWoodlandTrees     = new List<int>(new int[] {13,14,15,17,18,25,25,25});
        temperateWoodlandDeadTrees = new List<int>(new int[] {19,20,24,25,25,25,29,30,31});
        temperateWoodlandFlowers   = new List<int>(new int[] {21,22});
        temperateWoodlandMushroom  = new List<int>(new int[] {7,23});
        temperateWoodlandBushes    = new List<int>(new int[] {1,1,1,27,28});

        if(DaggerfallUnity.Instance.WorldTime.Now.SeasonValue == DaggerfallDateTime.Seasons.Winter)
        {
          mountainsTrees             = new List<int>(new int[] {29,29,29,29,29,29,29,29,29,29,11,25,25});
          mountainsNeedleTrees       = new List<int>(new int[] {11,25,25});
          mountainsDeadTrees         = new List<int>(new int[] {19,20,24,30,29});
          mountainsFlowers           = new List<int>(new int[] {7});
          mountainsRocks             = new List<int>(new int[] {1,3,4,6,9,8,10,14,17,18,27,28,31});
          mountainsGrass             = new List<int>(new int[] {7,7,7,9,26});

          woodlandHillsFlowers       = new List<int>(new int[] {7,22,22,22,22});
          woodlandHillsBushes        = new List<int>(new int[] {23,27});
          woodlandHillsTrees         = new List<int>(new int[] {5,11,12,12,13,14,14,25,12});
          woodlandHillsNeedleTrees   = new List<int>(new int[] {12,25,12});
          woodlandHillsDirtPlants    = new List<int>(new int[] {22,29,26,31});
        }
        if(DaggerfallUnity.Instance.WorldTime.Now.SeasonValue != DaggerfallDateTime.Seasons.Winter)
        {
          mountainsTrees             = new List<int>(new int[] {22,22,22,22,22,22,22,22,22,22,12,25,30});
          mountainsNeedleTrees       = new List<int>(new int[] {12,25,30,12,25,30});
          mountainsDeadTrees         = new List<int>(new int[] {19,20,24,26,29});
          mountainsFlowers           = new List<int>(new int[] {22});
          mountainsRocks             = new List<int>(new int[] {1,3,4,6,7,8,10,14,17,18,27,28,31});
          mountainsGrass             = new List<int>(new int[] {2,2,2,7,23});

          woodlandHillsFlowers       = new List<int>(new int[] {2,22,22,22,22});
          woodlandHillsBushes        = new List<int>(new int[] {9,27});
          woodlandHillsTrees         = new List<int>(new int[] {5,11,12,12,13,14,15,25,30});
          woodlandHillsNeedleTrees   = new List<int>(new int[] {12,25,30});
          woodlandHillsDirtPlants    = new List<int>(new int[] {26,29,23,31});
        }
      }
      else if(terrainElevation <= 0.8f && terrainElevation > 0.75f)
      {
        temperateWoodlandTrees     = new List<int>(new int[] {13,14,15,17,18,25,25,25});
        temperateWoodlandDeadTrees = new List<int>(new int[] {19,20,24,25,25,25,29,30,31});
        temperateWoodlandFlowers   = new List<int>(new int[] {21,22});
        temperateWoodlandMushroom  = new List<int>(new int[] {7,23});
        temperateWoodlandBushes    = new List<int>(new int[] {1,1,1,27,28});

        if(DaggerfallUnity.Instance.WorldTime.Now.SeasonValue == DaggerfallDateTime.Seasons.Winter)
        {
          mountainsTrees             = new List<int>(new int[] {29,29,29,29,29,29,29,11,25,25,25});
          mountainsNeedleTrees       = new List<int>(new int[] {11,25,25});
          mountainsDeadTrees         = new List<int>(new int[] {19,20,24,30,29});
          mountainsFlowers           = new List<int>(new int[] {7});
          mountainsRocks             = new List<int>(new int[] {1,3,4,6,9,8,10,14,17,18,27,28,31});
          mountainsGrass             = new List<int>(new int[] {7,7,7,9,23,26});

          woodlandHillsFlowers       = new List<int>(new int[] {7,22,22,22,22});
          woodlandHillsBushes        = new List<int>(new int[] {23,27});
          woodlandHillsTrees         = new List<int>(new int[] {5,11,12,12,13,14,14,25,12});
          woodlandHillsNeedleTrees   = new List<int>(new int[] {12,25,12});
          woodlandHillsDirtPlants    = new List<int>(new int[] {22,29,26,31});
        }
        if(DaggerfallUnity.Instance.WorldTime.Now.SeasonValue != DaggerfallDateTime.Seasons.Winter)
        {
          mountainsTrees             = new List<int>(new int[] {22,22,22,22,22,22,22,22,22,22,12,12,25,25,30,30});
          mountainsNeedleTrees       = new List<int>(new int[] {12,25,30,12,25,30});
          mountainsDeadTrees         = new List<int>(new int[] {19,20,24,26,29});
          mountainsFlowers           = new List<int>(new int[] {22});
          mountainsRocks             = new List<int>(new int[] {1,3,4,6,7,8,10,14,17,18,27,28,31});
          mountainsGrass             = new List<int>(new int[] {2,2,2,7,9,23});

          woodlandHillsFlowers       = new List<int>(new int[] {2,22,22,22,22});
          woodlandHillsBushes        = new List<int>(new int[] {9,27});
          woodlandHillsTrees         = new List<int>(new int[] {5,11,12,12,13,14,15,25,30});
          woodlandHillsNeedleTrees   = new List<int>(new int[] {12,25,30});
          woodlandHillsDirtPlants    = new List<int>(new int[] {26,29,23,31});
        }
      }
      else if(terrainElevation <= 0.75f && terrainElevation > 0.7f)
      {
        temperateWoodlandTrees     = new List<int>(new int[] {13,14,15,17,18,25,25,25});
        temperateWoodlandDeadTrees = new List<int>(new int[] {19,20,24,25,25,25,29,30,31});
        temperateWoodlandFlowers   = new List<int>(new int[] {21,22});
        temperateWoodlandMushroom  = new List<int>(new int[] {7,23});
        temperateWoodlandBushes    = new List<int>(new int[] {1,1,1,27,28});

        if(DaggerfallUnity.Instance.WorldTime.Now.SeasonValue == DaggerfallDateTime.Seasons.Winter)
        {
          mountainsTrees             = new List<int>(new int[] {29,29,29,29,29,29,29,11,25,25});
          mountainsNeedleTrees       = new List<int>(new int[] {11,25,25});
          mountainsDeadTrees         = new List<int>(new int[] {19,20,24,30,29});
          mountainsFlowers           = new List<int>(new int[] {7});
          mountainsRocks             = new List<int>(new int[] {1,3,4,6,9,8,10,14,17,18,27,28,31});
          mountainsGrass             = new List<int>(new int[] {7,7,7,9,23,26});

          woodlandHillsFlowers       = new List<int>(new int[] {7,22,22,22,22});
          woodlandHillsBushes        = new List<int>(new int[] {23,27});
          woodlandHillsTrees         = new List<int>(new int[] {5,11,12,12,13,14,14,25,12});
          woodlandHillsNeedleTrees   = new List<int>(new int[] {12,25,12});
          woodlandHillsDirtPlants    = new List<int>(new int[] {22,29,26,31});
        }
        if(DaggerfallUnity.Instance.WorldTime.Now.SeasonValue != DaggerfallDateTime.Seasons.Winter)
        {
          mountainsTrees             = new List<int>(new int[] {22,22,22,22,22,22,22,22,12,12,25,25,30,30});
          mountainsNeedleTrees       = new List<int>(new int[] {12,25,30,12,25,30});
          mountainsDeadTrees         = new List<int>(new int[] {19,20,24,26,29});
          mountainsFlowers           = new List<int>(new int[] {22});
          mountainsRocks             = new List<int>(new int[] {1,3,4,6,7,8,10,14,17,18,27,28,31});
          mountainsGrass             = new List<int>(new int[] {2,2,2,7,9,23});

          woodlandHillsFlowers       = new List<int>(new int[] {2,22,22,22,22});
          woodlandHillsBushes        = new List<int>(new int[] {9,27});
          woodlandHillsTrees         = new List<int>(new int[] {5,11,12,12,13,14,15,25,30});
          woodlandHillsNeedleTrees   = new List<int>(new int[] {12,25,30});
          woodlandHillsDirtPlants    = new List<int>(new int[] {26,29,23,31});
        }
      }
      else if(terrainElevation <= 0.7f && terrainElevation > 0.65f)
      {
        temperateWoodlandTrees     = new List<int>(new int[] {13,14,15,17,18,25,25,25});
        temperateWoodlandDeadTrees = new List<int>(new int[] {19,20,24,25,25,25,29,30,31});
        temperateWoodlandFlowers   = new List<int>(new int[] {21,22});
        temperateWoodlandMushroom  = new List<int>(new int[] {7,23});
        temperateWoodlandBushes    = new List<int>(new int[] {1,1,1,27,28});

        if(DaggerfallUnity.Instance.WorldTime.Now.SeasonValue == DaggerfallDateTime.Seasons.Winter)
        {
          mountainsTrees             = new List<int>(new int[] {29,29,29,29,29,5,11,25,25});
          mountainsNeedleTrees       = new List<int>(new int[] {5,11,11,25,25,25});
          mountainsDeadTrees         = new List<int>(new int[] {19,20,24,30,29});
          mountainsFlowers           = new List<int>(new int[] {7});
          mountainsRocks             = new List<int>(new int[] {1,3,4,6,9,8,10,14,17,18,27,28,31});
          mountainsGrass             = new List<int>(new int[] {7,7,7,9,23,26});

          woodlandHillsFlowers       = new List<int>(new int[] {7,22,22,22,22});
          woodlandHillsBushes        = new List<int>(new int[] {23,27});
          woodlandHillsTrees         = new List<int>(new int[] {5,11,12,12,13,14,14,25,12});
          woodlandHillsNeedleTrees   = new List<int>(new int[] {12,25,12});
          woodlandHillsDirtPlants    = new List<int>(new int[] {22,29,26,31});
        }
        if(DaggerfallUnity.Instance.WorldTime.Now.SeasonValue != DaggerfallDateTime.Seasons.Winter)
        {
          mountainsTrees             = new List<int>(new int[] {22,22,22,22,22,22,22,5,11,12,12,25,25,30,30});
          mountainsNeedleTrees       = new List<int>(new int[] {5,11,12,25,30,12,25,30});
          mountainsDeadTrees         = new List<int>(new int[] {19,20,24,26,29});
          mountainsFlowers           = new List<int>(new int[] {22});
          mountainsRocks             = new List<int>(new int[] {1,3,4,6,7,8,10,14,17,18,27,28,31});
          mountainsGrass             = new List<int>(new int[] {2,2,2,7,9,23});

          woodlandHillsFlowers       = new List<int>(new int[] {2,22,22,22,22});
          woodlandHillsBushes        = new List<int>(new int[] {9,27});
          woodlandHillsTrees         = new List<int>(new int[] {5,11,12,12,13,14,15,25,30});
          woodlandHillsNeedleTrees   = new List<int>(new int[] {12,25,30});
          woodlandHillsDirtPlants    = new List<int>(new int[] {26,29,23,31});
        }
      }
      else if(terrainElevation <= 0.65f && terrainElevation > 0.6f)
      {
        temperateWoodlandTrees     = new List<int>(new int[] {13,14,15,17,18,25,25,25});
        temperateWoodlandDeadTrees = new List<int>(new int[] {19,20,24,25,25,25,29,30,31});
        temperateWoodlandFlowers   = new List<int>(new int[] {21,22});
        temperateWoodlandMushroom  = new List<int>(new int[] {7,23});
        temperateWoodlandBushes    = new List<int>(new int[] {1,1,1,27,28});

        if(DaggerfallUnity.Instance.WorldTime.Now.SeasonValue == DaggerfallDateTime.Seasons.Winter)
        {
          mountainsTrees             = new List<int>(new int[] {29,29,29,29,29,5,11,11,25,25});
          mountainsNeedleTrees       = new List<int>(new int[] {5,11,11,25,25,25});
          mountainsDeadTrees         = new List<int>(new int[] {19,20,24,30,29});
          mountainsFlowers           = new List<int>(new int[] {7});
          mountainsRocks             = new List<int>(new int[] {1,3,4,6,9,8,10,14,17,18,27,28,31});
          mountainsGrass             = new List<int>(new int[] {7,7,7,9,23,23,26});

          woodlandHillsFlowers       = new List<int>(new int[] {7,22,22,22,22});
          woodlandHillsBushes        = new List<int>(new int[] {23,27});
          woodlandHillsTrees         = new List<int>(new int[] {5,11,12,12,13,14,14,25,12});
          woodlandHillsNeedleTrees   = new List<int>(new int[] {12,25,12});
          woodlandHillsDirtPlants    = new List<int>(new int[] {22,29,26,31});
        }
        if(DaggerfallUnity.Instance.WorldTime.Now.SeasonValue != DaggerfallDateTime.Seasons.Winter)
        {
          mountainsTrees             = new List<int>(new int[] {22,22,22,22,22,22,5,11,12,12,25,25,30,30});
          mountainsNeedleTrees       = new List<int>(new int[] {5,11,12,25,30,12,25,30});
          mountainsDeadTrees         = new List<int>(new int[] {19,20,24,26,29});
          mountainsFlowers           = new List<int>(new int[] {22});
          mountainsRocks             = new List<int>(new int[] {1,3,4,6,7,8,10,14,17,18,27,28,31});
          mountainsGrass             = new List<int>(new int[] {2,2,2,7,9,9,23});

          woodlandHillsFlowers       = new List<int>(new int[] {2,22,22,22,22});
          woodlandHillsBushes        = new List<int>(new int[] {9,27});
          woodlandHillsTrees         = new List<int>(new int[] {5,11,12,12,13,14,15,25,30});
          woodlandHillsNeedleTrees   = new List<int>(new int[] {12,25,30});
          woodlandHillsDirtPlants    = new List<int>(new int[] {26,29,23,31});
        }
      }
      else if(terrainElevation <= 0.6f && terrainElevation > 0.55f)
      {
        temperateWoodlandTrees     = new List<int>(new int[] {13,14,15,17,18,25,25,25});
        temperateWoodlandDeadTrees = new List<int>(new int[] {19,20,24,25,25,25,29,30,31});
        temperateWoodlandFlowers   = new List<int>(new int[] {21,22});
        temperateWoodlandMushroom  = new List<int>(new int[] {7,23});
        temperateWoodlandBushes    = new List<int>(new int[] {1,1,1,27,28});

        if(DaggerfallUnity.Instance.WorldTime.Now.SeasonValue == DaggerfallDateTime.Seasons.Winter)
        {
          mountainsTrees             = new List<int>(new int[] {29,29,29,29,29,5,11,13,21,25,25,25});
          mountainsNeedleTrees       = new List<int>(new int[] {5,11,11,25,25,25});
          mountainsDeadTrees         = new List<int>(new int[] {16,19,20,24,30,29});
          mountainsFlowers           = new List<int>(new int[] {7});
          mountainsRocks             = new List<int>(new int[] {1,3,4,6,9,8,10,14,17,18,27,28,31});
          mountainsGrass             = new List<int>(new int[] {7,7,9,23,23,23,26});

          woodlandHillsFlowers       = new List<int>(new int[] {7,22,22,22,22});
          woodlandHillsBushes        = new List<int>(new int[] {23,27});
          woodlandHillsTrees         = new List<int>(new int[] {5,11,12,12,13,14,14,25,12});
          woodlandHillsNeedleTrees   = new List<int>(new int[] {12,25,12});
          woodlandHillsDirtPlants    = new List<int>(new int[] {22,29,26,31});
        }
        if(DaggerfallUnity.Instance.WorldTime.Now.SeasonValue != DaggerfallDateTime.Seasons.Winter)
        {
          mountainsTrees             = new List<int>(new int[] {22,22,22,22,22,5,11,12,13,21,25,25,30,30});
          mountainsNeedleTrees       = new List<int>(new int[] {5,11,12,12,25,25,30,30});
          mountainsDeadTrees         = new List<int>(new int[] {16,19,20,24,26,29});
          mountainsFlowers           = new List<int>(new int[] {22});
          mountainsRocks             = new List<int>(new int[] {1,3,4,6,7,8,10,14,17,18,27,28,31});
          mountainsGrass             = new List<int>(new int[] {2,2,7,9,9,9,23});

          woodlandHillsFlowers       = new List<int>(new int[] {2,22,22,22,22});
          woodlandHillsBushes        = new List<int>(new int[] {9,27});
          woodlandHillsTrees         = new List<int>(new int[] {5,11,12,12,13,14,15,25,30});
          woodlandHillsNeedleTrees   = new List<int>(new int[] {12,25,30});
          woodlandHillsDirtPlants    = new List<int>(new int[] {26,29,23,31});
        }
      }
      else if(terrainElevation <= 0.55f && terrainElevation > 0.5f)
      {
        temperateWoodlandTrees     = new List<int>(new int[] {13,14,15,17,18,25,25,25});
        temperateWoodlandDeadTrees = new List<int>(new int[] {19,20,24,25,25,25,29,30,31});
        temperateWoodlandFlowers   = new List<int>(new int[] {21,22});
        temperateWoodlandMushroom  = new List<int>(new int[] {7,23});
        temperateWoodlandBushes    = new List<int>(new int[] {1,1,1,27,28});

        if(DaggerfallUnity.Instance.WorldTime.Now.SeasonValue == DaggerfallDateTime.Seasons.Winter)
        {
          mountainsTrees             = new List<int>(new int[] {29,29,29,5,11,11,13,21,25,25,25});
          mountainsNeedleTrees       = new List<int>(new int[] {5,11,11,25,25,25});
          mountainsDeadTrees         = new List<int>(new int[] {16,19,20,24,30,29});
          mountainsFlowers           = new List<int>(new int[] {7});
          mountainsRocks             = new List<int>(new int[] {1,3,4,6,9,8,10,14,17,18,27,28,31});
          mountainsGrass             = new List<int>(new int[] {7,7,9,23,23,23,26});

          woodlandHillsFlowers       = new List<int>(new int[] {7,22,22,22,22});
          woodlandHillsBushes        = new List<int>(new int[] {23,27});
          woodlandHillsTrees         = new List<int>(new int[] {5,11,12,12,13,14,14,25,12});
          woodlandHillsNeedleTrees   = new List<int>(new int[] {12,25,12});
          woodlandHillsDirtPlants    = new List<int>(new int[] {22,29,26,31});
        }
        if(DaggerfallUnity.Instance.WorldTime.Now.SeasonValue != DaggerfallDateTime.Seasons.Winter)
        {
          mountainsTrees             = new List<int>(new int[] {22,22,22,22,5,11,12,13,21,25,25,30,30});
          mountainsNeedleTrees       = new List<int>(new int[] {5,11,12,12,25,25,30,30});
          mountainsDeadTrees         = new List<int>(new int[] {16,19,20,24,26,29});
          mountainsFlowers           = new List<int>(new int[] {22});
          mountainsRocks             = new List<int>(new int[] {1,3,4,6,7,8,10,14,17,18,27,28,31});
          mountainsGrass             = new List<int>(new int[] {2,2,7,9,9,9,23});

          woodlandHillsFlowers       = new List<int>(new int[] {2,22,22,22,22});
          woodlandHillsBushes        = new List<int>(new int[] {9,27});
          woodlandHillsTrees         = new List<int>(new int[] {5,11,12,12,13,14,15,25,30});
          woodlandHillsNeedleTrees   = new List<int>(new int[] {12,25,30});
          woodlandHillsDirtPlants    = new List<int>(new int[] {26,29,23,31});
        }
      }
      else if(terrainElevation <= 0.5f && terrainElevation > 0.45f)
      {
        temperateWoodlandTrees     = new List<int>(new int[] {13,14,15,17,18,25,25,25});
        temperateWoodlandDeadTrees = new List<int>(new int[] {19,20,24,25,25,25,29,30,31});
        temperateWoodlandFlowers   = new List<int>(new int[] {21,22});
        temperateWoodlandMushroom  = new List<int>(new int[] {7,23});
        temperateWoodlandBushes    = new List<int>(new int[] {1,1,1,27,28});

        if(DaggerfallUnity.Instance.WorldTime.Now.SeasonValue == DaggerfallDateTime.Seasons.Winter)
        {
          mountainsTrees             = new List<int>(new int[] {29,29,29,5,11,11,13,21,25,25,25});
          mountainsNeedleTrees       = new List<int>(new int[] {5,11,11,25,25,25});
          mountainsDeadTrees         = new List<int>(new int[] {16,19,20,24,30,29});
          mountainsFlowers           = new List<int>(new int[] {7});
          mountainsRocks             = new List<int>(new int[] {1,3,4,6,9,8,10,14,17,18,27,28,31});
          mountainsGrass             = new List<int>(new int[] {7,7,9,23,23,23,26});

          woodlandHillsFlowers       = new List<int>(new int[] {7,22,22,22,22});
          woodlandHillsBushes        = new List<int>(new int[] {23,23,27});
          woodlandHillsTrees         = new List<int>(new int[] {5,11,12,12,13,14,14,25,12});
          woodlandHillsNeedleTrees   = new List<int>(new int[] {12,25,12});
          woodlandHillsDirtPlants    = new List<int>(new int[] {22,29,26,31});
        }
        if(DaggerfallUnity.Instance.WorldTime.Now.SeasonValue != DaggerfallDateTime.Seasons.Winter)
        {
          mountainsTrees             = new List<int>(new int[] {22,22,22,5,11,12,13,21,25,25,30,30});
          mountainsNeedleTrees       = new List<int>(new int[] {5,11,12,12,25,25,30,30});
          mountainsDeadTrees         = new List<int>(new int[] {16,19,20,24,26,29});
          mountainsFlowers           = new List<int>(new int[] {22});
          mountainsRocks             = new List<int>(new int[] {1,3,4,6,7,8,10,14,17,18,27,28,31});
          mountainsGrass             = new List<int>(new int[] {2,2,7,9,9,9,23});

          woodlandHillsFlowers       = new List<int>(new int[] {2,22,22,22,22});
          woodlandHillsBushes        = new List<int>(new int[] {9,9,27});
          woodlandHillsTrees         = new List<int>(new int[] {5,11,12,12,13,14,15,25,30});
          woodlandHillsNeedleTrees   = new List<int>(new int[] {12,25,30});
          woodlandHillsDirtPlants    = new List<int>(new int[] {26,29,23,31});
        }
      }
      else if(terrainElevation <= 0.45f && terrainElevation > 0.4f)
      {
        temperateWoodlandTrees     = new List<int>(new int[] {13,14,15,17,18,25,25,25});
        temperateWoodlandDeadTrees = new List<int>(new int[] {19,20,24,25,25,25,29,30,31});
        temperateWoodlandFlowers   = new List<int>(new int[] {21,22});
        temperateWoodlandMushroom  = new List<int>(new int[] {7,23});
        temperateWoodlandBushes    = new List<int>(new int[] {1,1,1,27,28});

        if(DaggerfallUnity.Instance.WorldTime.Now.SeasonValue == DaggerfallDateTime.Seasons.Winter)
        {
          mountainsTrees             = new List<int>(new int[] {29,29,5,11,11,13,21,25,25});
          mountainsNeedleTrees       = new List<int>(new int[] {5,11,11,25,25});
          mountainsDeadTrees         = new List<int>(new int[] {16,19,20,24,30,29});
          mountainsFlowers           = new List<int>(new int[] {7});
          mountainsRocks             = new List<int>(new int[] {1,3,4,6,9,8,10,14,17,18,27,28,31});
          mountainsGrass             = new List<int>(new int[] {7,7,9,23,23,26});

          woodlandHillsFlowers       = new List<int>(new int[] {7,22,22,22});
          woodlandHillsBushes        = new List<int>(new int[] {26,27});
          woodlandHillsTrees         = new List<int>(new int[] {5,11,12,12,13,14,14,25,12});
          woodlandHillsNeedleTrees   = new List<int>(new int[] {12,25,12});
          woodlandHillsDirtPlants    = new List<int>(new int[] {22,29,26,9,31});
        }
        if(DaggerfallUnity.Instance.WorldTime.Now.SeasonValue != DaggerfallDateTime.Seasons.Winter)
        {
          mountainsTrees             = new List<int>(new int[] {22,22,5,11,12,13,21,25,30});
          mountainsNeedleTrees       = new List<int>(new int[] {5,11,12,25,30});
          mountainsDeadTrees         = new List<int>(new int[] {16,19,20,24,26,29});
          mountainsFlowers           = new List<int>(new int[] {22});
          mountainsRocks             = new List<int>(new int[] {1,3,4,6,7,8,10,14,17,18,27,28,31});
          mountainsGrass             = new List<int>(new int[] {2,2,7,9,9,23});

          woodlandHillsFlowers       = new List<int>(new int[] {2,22,22,22});
          woodlandHillsBushes        = new List<int>(new int[] {9,27});
          woodlandHillsTrees         = new List<int>(new int[] {5,11,12,12,13,14,15,25,30});
          woodlandHillsNeedleTrees   = new List<int>(new int[] {12,25,30});
          woodlandHillsDirtPlants    = new List<int>(new int[] {26,29,23,7,31});
        }
      }
      else if(terrainElevation <= 0.4f && terrainElevation > 0.35f)
      {
        temperateWoodlandTrees     = new List<int>(new int[] {13,14,15,17,18,25,25,25});
        temperateWoodlandDeadTrees = new List<int>(new int[] {19,20,24,25,25,25,29,30,31});
        temperateWoodlandFlowers   = new List<int>(new int[] {21,22});
        temperateWoodlandMushroom  = new List<int>(new int[] {7,23});
        temperateWoodlandBushes    = new List<int>(new int[] {1,1,1,27,28});

        if(DaggerfallUnity.Instance.WorldTime.Now.SeasonValue == DaggerfallDateTime.Seasons.Winter)
        {
          mountainsTrees             = new List<int>(new int[] {29,5,11,11,13,24,21,25,25});
          mountainsNeedleTrees       = new List<int>(new int[] {5,11,11,25,25,25});
          mountainsDeadTrees         = new List<int>(new int[] {16,19,20,24,30,29});
          mountainsFlowers           = new List<int>(new int[] {7});
          mountainsRocks             = new List<int>(new int[] {1,3,4,6,9,8,10,14,17,18,27,28,31});
          mountainsGrass             = new List<int>(new int[] {7,7,9,23,23,26});

          woodlandHillsFlowers       = new List<int>(new int[] {7,21,22,22});
          woodlandHillsBushes        = new List<int>(new int[] {27});
          woodlandHillsTrees         = new List<int>(new int[] {5,11,12,12,13,13,14,14,16,25,12});
          woodlandHillsNeedleTrees   = new List<int>(new int[] {5,11,12,25,12});
          woodlandHillsDirtPlants    = new List<int>(new int[] {22,29,26,9,31});
        }
        if(DaggerfallUnity.Instance.WorldTime.Now.SeasonValue != DaggerfallDateTime.Seasons.Winter)
        {
          mountainsTrees             = new List<int>(new int[] {22,5,11,12,13,15,21,25,30});
          mountainsNeedleTrees       = new List<int>(new int[] {5,11,12,25,30,12,25,30});
          mountainsDeadTrees         = new List<int>(new int[] {16,19,20,24,26,29});
          mountainsFlowers           = new List<int>(new int[] {22});
          mountainsRocks             = new List<int>(new int[] {1,3,4,6,7,8,10,14,17,18,27,28,31});
          mountainsGrass             = new List<int>(new int[] {2,2,7,9,9,23});

          woodlandHillsFlowers       = new List<int>(new int[] {2,21,22,22});
          woodlandHillsBushes        = new List<int>(new int[] {27});
          woodlandHillsTrees         = new List<int>(new int[] {5,11,12,12,13,13,14,14,16,25,12});
          woodlandHillsNeedleTrees   = new List<int>(new int[] {5,11,12,25,12});
          woodlandHillsDirtPlants    = new List<int>(new int[] {26,29,23,7,31});
        }
      } 
      else if(terrainElevation <= 0.35f && terrainElevation > 0.3f)
      {
        temperateWoodlandTrees     = new List<int>(new int[] {13,14,15,17,18,25,25,25});
        temperateWoodlandDeadTrees = new List<int>(new int[] {19,20,24,25,25,25,29,30,31});
        temperateWoodlandFlowers   = new List<int>(new int[] {21,22});
        temperateWoodlandMushroom  = new List<int>(new int[] {7,23});
        temperateWoodlandBushes    = new List<int>(new int[] {1,1,1,27,28});

        if(DaggerfallUnity.Instance.WorldTime.Now.SeasonValue == DaggerfallDateTime.Seasons.Winter)
        {
          mountainsTrees             = new List<int>(new int[] {5,5,11,11,11,13,13,24,24,24,21,25});
          mountainsNeedleTrees       = new List<int>(new int[] {5,5,11,11,11,25});
          mountainsDeadTrees         = new List<int>(new int[] {16,19,20,30,30,29,29});
          mountainsFlowers           = new List<int>(new int[] {7});
          mountainsRocks             = new List<int>(new int[] {1,3,4,6,9,8,10,14,17,18,27,28,31});
          mountainsGrass             = new List<int>(new int[] {7,9,9,23,26,26});

          woodlandHillsFlowers       = new List<int>(new int[] {7,21,21,21,22,22});
          woodlandHillsBushes        = new List<int>(new int[] {23,27,27});
          woodlandHillsTrees         = new List<int>(new int[] {12,12,13,14,14,16});
          woodlandHillsNeedleTrees   = new List<int>(new int[] {5,5,11,11,12,25,12});
          woodlandHillsDirtPlants    = new List<int>(new int[] {22,29,26,9,31});
        }
        if(DaggerfallUnity.Instance.WorldTime.Now.SeasonValue != DaggerfallDateTime.Seasons.Winter)
        {
          mountainsTrees             = new List<int>(new int[] {5,5,11,11,12,13,13,15,15,15,21,30});
          mountainsNeedleTrees       = new List<int>(new int[] {5,5,11,11,12,12,30});
          mountainsDeadTrees         = new List<int>(new int[] {16,19,20,26,26,29,29});
          mountainsFlowers           = new List<int>(new int[] {22});
          mountainsRocks             = new List<int>(new int[] {1,3,4,6,7,8,10,14,17,18,27,28,31});
          mountainsGrass             = new List<int>(new int[] {2,7,7,9,23,23});

          woodlandHillsFlowers       = new List<int>(new int[] {2,21,21,21,22,22});
          woodlandHillsBushes        = new List<int>(new int[] {9,27,27});
          woodlandHillsTrees         = new List<int>(new int[] {12,12,13,14,15,16});
          woodlandHillsNeedleTrees   = new List<int>(new int[] {5,5,11,11,12,25,30});
          woodlandHillsDirtPlants    = new List<int>(new int[] {26,29,23,7,31});
        }
      }
      else if(terrainElevation <= 0.3f && terrainElevation > 0.25f)
      {
        temperateWoodlandTrees     = new List<int>(new int[] {13,14,15,17,18,25,25,25});
        temperateWoodlandDeadTrees = new List<int>(new int[] {19,20,24,25,25,25,29,30,31});
        temperateWoodlandFlowers   = new List<int>(new int[] {21,22});
        temperateWoodlandMushroom  = new List<int>(new int[] {7,23});
        temperateWoodlandBushes    = new List<int>(new int[] {1,1,1,27,28});

        if(DaggerfallUnity.Instance.WorldTime.Now.SeasonValue == DaggerfallDateTime.Seasons.Winter)
        {
          mountainsTrees             = new List<int>(new int[] {5,5,11,11,11,13,13,24,24,24,21,25});
          mountainsNeedleTrees       = new List<int>(new int[] {5,5,11,11,11,11,25});
          mountainsDeadTrees         = new List<int>(new int[] {16,19,20,30,30,29,29});
          mountainsFlowers           = new List<int>(new int[] {7});
          mountainsRocks             = new List<int>(new int[] {1,3,4,6,9,8,10,14,17,18,27,28,31});
          mountainsGrass             = new List<int>(new int[] {7,9,9,23,26,26});

          woodlandHillsFlowers       = new List<int>(new int[] {7,21,22});
          woodlandHillsBushes        = new List<int>(new int[] {27,31});
          woodlandHillsTrees         = new List<int>(new int[] {12,12,13,13,14,14,14,16,16});
          woodlandHillsNeedleTrees   = new List<int>(new int[] {5,5,11,11,11,12,25,12});
          woodlandHillsDirtPlants    = new List<int>(new int[] {22,29,26,9});
        }
        if(DaggerfallUnity.Instance.WorldTime.Now.SeasonValue != DaggerfallDateTime.Seasons.Winter)
        {
          mountainsTrees             = new List<int>(new int[] {5,5,11,11,12,13,13,15,15,15,21,30});
          mountainsNeedleTrees       = new List<int>(new int[] {5,5,11,11,12,12,30});
          mountainsDeadTrees         = new List<int>(new int[] {16,19,20,26,26,29,29});
          mountainsFlowers           = new List<int>(new int[] {22});
          mountainsRocks             = new List<int>(new int[] {1,3,4,6,7,8,10,14,17,18,27,28,31});
          mountainsGrass             = new List<int>(new int[] {2,7,7,9,23,23});

          woodlandHillsFlowers       = new List<int>(new int[] {2,21,22});
          woodlandHillsBushes        = new List<int>(new int[] {27});
          woodlandHillsTrees         = new List<int>(new int[] {12,12,13,13,14,14,15,15,16,16});
          woodlandHillsNeedleTrees   = new List<int>(new int[] {5,5,11,11,11,12,25,30});
          woodlandHillsDirtPlants    = new List<int>(new int[] {26,29,23,7,31});
        }
      }
      else if(terrainElevation <= 0.25f && terrainElevation > 0.2f)
      {
        temperateWoodlandTrees     = new List<int>(new int[] {11,13,13,14,14,15,15,16,17,17,18,18,25,25,25,25});
        temperateWoodlandDeadTrees = new List<int>(new int[] {19,20,24,25,25,25,29,30,31});
        temperateWoodlandFlowers   = new List<int>(new int[] {2,21,21,21,22,22});
        temperateWoodlandMushroom  = new List<int>(new int[] {7,7,7,9,23,23});
        temperateWoodlandBushes    = new List<int>(new int[] {1,1,27,28});

        if(DaggerfallUnity.Instance.WorldTime.Now.SeasonValue == DaggerfallDateTime.Seasons.Winter)
        {
          mountainsTrees             = new List<int>(new int[] {5,5,11,11,11,13,13,24,24,24,21,25});
          mountainsNeedleTrees       = new List<int>(new int[] {5,5,11,11,11,25});
          mountainsDeadTrees         = new List<int>(new int[] {16,19,20,30,30,29,29});
          mountainsFlowers           = new List<int>(new int[] {7});
          mountainsRocks             = new List<int>(new int[] {1,3,4,6,9,8,10,14,17,18,27,28,31});
          mountainsGrass             = new List<int>(new int[] {7,9,9,23,26,26});

          woodlandHillsFlowers       = new List<int>(new int[] {7,21,21,21});
          woodlandHillsBushes        = new List<int>(new int[] {27,27});
          woodlandHillsTrees         = new List<int>(new int[] {12,13,13,13,14,14,14,16,16});
          woodlandHillsNeedleTrees   = new List<int>(new int[] {5,5,11,11,12});
          woodlandHillsDirtPlants    = new List<int>(new int[] {22,29,26,9,31});
        }
        if(DaggerfallUnity.Instance.WorldTime.Now.SeasonValue != DaggerfallDateTime.Seasons.Winter)
        {
          mountainsTrees             = new List<int>(new int[] {5,5,11,11,12,13,13,15,15,15,21,30});
          mountainsNeedleTrees       = new List<int>(new int[] {5,5,11,11,12,12,30});
          mountainsDeadTrees         = new List<int>(new int[] {16,19,20,26,26,29,29});
          mountainsFlowers           = new List<int>(new int[] {22});
          mountainsRocks             = new List<int>(new int[] {1,3,4,6,7,8,10,14,17,18,27,28,31});
          mountainsGrass             = new List<int>(new int[] {2,7,7,9,23,23});

          woodlandHillsFlowers       = new List<int>(new int[] {2,21,21,21});
          woodlandHillsBushes        = new List<int>(new int[] {27,27,31});
          woodlandHillsTrees         = new List<int>(new int[] {12,13,13,13,14,14,15,15,16,16});
          woodlandHillsNeedleTrees   = new List<int>(new int[] {5,5,11,11,12});
          woodlandHillsDirtPlants    = new List<int>(new int[] {26,29,23,7});
        }
      }
      else if(terrainElevation <= 0.2f && terrainElevation > 0.15f)
      {
        temperateWoodlandTrees     = new List<int>(new int[] {11,12,13,14,15,16,17,18});
        temperateWoodlandDeadTrees = new List<int>(new int[] {19,20,24,25,29,30,31});
        temperateWoodlandFlowers   = new List<int>(new int[] {2,21,22});
        temperateWoodlandMushroom  = new List<int>(new int[] {7,9,23});
        temperateWoodlandBushes    = new List<int>(new int[] {1,27,28});

        if(DaggerfallUnity.Instance.WorldTime.Now.SeasonValue == DaggerfallDateTime.Seasons.Winter)
        {
          mountainsTrees             = new List<int>(new int[] {5,5,11,11,11,13,13,24,24,24,21,25});
          mountainsNeedleTrees       = new List<int>(new int[] {5,5,11,11,11,11,25});
          mountainsDeadTrees         = new List<int>(new int[] {16,19,20,30,30,29,29});
          mountainsFlowers           = new List<int>(new int[] {7});
          mountainsRocks             = new List<int>(new int[] {1,3,4,6,9,8,10,14,17,18,27,28,31});
          mountainsGrass             = new List<int>(new int[] {7,9,9,9,23,26,26,26});

          woodlandHillsFlowers       = new List<int>(new int[] {7,21,21});
          woodlandHillsBushes        = new List<int>(new int[] {27,27,27,31});
          woodlandHillsTrees         = new List<int>(new int[] {13,14,14,14,16});
          woodlandHillsNeedleTrees   = new List<int>(new int[] {5,11,11});
          woodlandHillsDirtPlants    = new List<int>(new int[] {22,29,26,9});
        }
        if(DaggerfallUnity.Instance.WorldTime.Now.SeasonValue != DaggerfallDateTime.Seasons.Winter)
        {
          mountainsTrees             = new List<int>(new int[] {5,5,11,11,12,13,13,15,15,15,21,30});
          mountainsNeedleTrees       = new List<int>(new int[] {5,5,11,11,12,12,30});
          mountainsDeadTrees         = new List<int>(new int[] {16,19,20,26,26,29,29});
          mountainsFlowers           = new List<int>(new int[] {22});
          mountainsRocks             = new List<int>(new int[] {1,3,4,6,7,8,10,14,17,18,27,28,31});
          mountainsGrass             = new List<int>(new int[] {2,7,7,7,9,23,23,23});

          woodlandHillsFlowers       = new List<int>(new int[] {2,21,21});
          woodlandHillsBushes        = new List<int>(new int[] {27,27,27,31});
          woodlandHillsTrees         = new List<int>(new int[] {13,14,14,15,15,16});
          woodlandHillsNeedleTrees   = new List<int>(new int[] {5,11,11});
          woodlandHillsDirtPlants    = new List<int>(new int[] {26,29,23,7});
        }
      }
      else if(terrainElevation <= 0.15f)
      {
        temperateWoodlandTrees     = new List<int>(new int[] {11,11,12,13,14,14,15,15,16,16,17});
        temperateWoodlandDeadTrees = new List<int>(new int[] {19,19,20,20,24,24,29,29,30,31,31});
        temperateWoodlandFlowers   = new List<int>(new int[] {2,2,2,21,22,22});
        temperateWoodlandMushroom  = new List<int>(new int[] {7,7,9,9,9,23});
        temperateWoodlandBushes    = new List<int>(new int[] {1,27,27,28,28});

        if(DaggerfallUnity.Instance.WorldTime.Now.SeasonValue == DaggerfallDateTime.Seasons.Winter)
        {
          mountainsTrees             = new List<int>(new int[] {5,5,23,23,11,11,11,13,13,24,24,24,21,25});
          mountainsNeedleTrees       = new List<int>(new int[] {5,5,11,11,11,25});
          mountainsDeadTrees         = new List<int>(new int[] {16,19,20,30,30,29,29});
          mountainsFlowers           = new List<int>(new int[] {7});
          mountainsRocks             = new List<int>(new int[] {1,3,4,6,9,8,10,14,17,18,27,28,31});
          mountainsBeach             = new List<int>(new int[] {8,26,29,7,7,7,23,23});
          mountainsGrass             = new List<int>(new int[] {7,9,9,9,23,26,26,26});

          woodlandHillsFlowers       = new List<int>(new int[] {7,7,9,9,21,26,26});
          woodlandHillsBushes        = new List<int>(new int[] {27,27,31});
          woodlandHillsTrees         = new List<int>(new int[] {13,13,14,16,16});
          woodlandHillsNeedleTrees   = new List<int>(new int[] {5,11,11});
          woodlandHillsDirtPlants    = new List<int>(new int[] {22,29,26,9});
        }
        if(DaggerfallUnity.Instance.WorldTime.Now.SeasonValue != DaggerfallDateTime.Seasons.Winter)
        {
          mountainsTrees             = new List<int>(new int[] {5,5,9,9,11,11,12,13,13,15,15,15,21,30});
          mountainsNeedleTrees       = new List<int>(new int[] {5,5,11,11,12,12,30});
          mountainsDeadTrees         = new List<int>(new int[] {16,19,20,26,26,29,29});
          mountainsFlowers           = new List<int>(new int[] {22});
          mountainsRocks             = new List<int>(new int[] {1,3,4,6,7,8,10,14,17,18,27,28,31});
          mountainsGrass             = new List<int>(new int[] {2,7,7,7,9,23,23,23});
      
          woodlandHillsFlowers       = new List<int>(new int[] {2,2,7,7,21,23,23});
          woodlandHillsBushes        = new List<int>(new int[] {27,27,31});
          woodlandHillsTrees         = new List<int>(new int[] {13,14,15,16,16});
          woodlandHillsNeedleTrees   = new List<int>(new int[] {5,11,11});
          woodlandHillsDirtPlants    = new List<int>(new int[] {26,29,23,7});
        }
      }
      #endregion

      #region Vegetation Density per World Height
      // Adjustment to temperate woodlands climate in respect to world height
      if(climate.WorldClimate == (int)MapsFile.Climates.Woodlands)
      {
        if (terrainElevation > 0.25f)
        {
          chanceOnGrass = Mathf.Clamp(Random.Range( 0.200f, 0.275f ), 0f, 1f);
          chanceOnDirt  = Mathf.Clamp(Random.Range( 0.450f, 0.475f ), 0f, 1f);
          chanceOnStone = Mathf.Clamp(Random.Range( 0.500f, 0.525f ), 0f, 1f);
        }
        else if (terrainElevation <= 0.25f && terrainElevation > 0.2f)
        {
          chanceOnGrass = Mathf.Clamp(Random.Range( 0.225f, 0.300f ), 0f, 1f);
          chanceOnDirt  = Mathf.Clamp(Random.Range( 0.425f, 0.450f ), 0f, 1f);
          chanceOnStone = Mathf.Clamp(Random.Range( 0.675f, 0.700f ), 0f, 1f);
        }
        else if(terrainElevation <= 0.2f && terrainElevation > 0.15f)
        {
          chanceOnGrass = Mathf.Clamp(Random.Range( 0.250f, 0.325f ), 0f, 1f);
          chanceOnDirt  = Mathf.Clamp(Random.Range( 0.475f, 0.425f ), 0f, 1f);
          chanceOnStone = Mathf.Clamp(Random.Range( 0.650f, 0.675f ), 0f, 1f);
        }
        else if(terrainElevation <= 0.15f)
        {
          chanceOnGrass = Mathf.Clamp(Random.Range( 0.275f, 0.350f ), 0f, 1f);
          chanceOnDirt  = Mathf.Clamp(Random.Range( 0.150f, 0.175f ), 0f, 1f);
          chanceOnStone = Mathf.Clamp(Random.Range( 0.625f, 0.650f ), 0f, 1f);
        }
      }

      //Adjustment to mountain climate in respect to world height
      if(climate.WorldClimate == (int)MapsFile.Climates.Mountain)
      {
        if (terrainElevation > treeLine)
        {
          chanceOnGrass = Mathf.Clamp(Random.Range( 0.250f, 0.275f ), 0f, 1f);
          chanceOnDirt  = Mathf.Clamp(Random.Range( 0.200f, 0.225f ), 0f, 1f);
          chanceOnStone = Mathf.Clamp(Random.Range( 0.325f, 0.350f ), 0f, 1f);
        }
        else if (terrainElevation <= treeLine && terrainElevation > 0.6f)
        {
          chanceOnGrass = Mathf.Clamp(Random.Range( 0.275f, 0.300f ), 0f, 1f);
          chanceOnDirt  = Mathf.Clamp(Random.Range( 0.375f, 0.450f ), 0f, 1f);
          chanceOnStone = Mathf.Clamp(Random.Range( 0.275f, 0.300f ), 0f, 1f);
        }
        else if(terrainElevation <= 0.6f && terrainElevation > 0.4f)
        {
          chanceOnGrass = Mathf.Clamp(Random.Range( 0.300f, 0.325f ), 0f, 1f);
          chanceOnDirt  = Mathf.Clamp(Random.Range( 0.350f, 0.450f ), 0f, 1f);
          chanceOnStone = Mathf.Clamp(Random.Range( 0.225f, 0.250f ), 0f, 1f);
        }
        else if(terrainElevation <= 0.4f && terrainElevation > 0.2f)
        {
          chanceOnGrass = Mathf.Clamp(Random.Range( 0.325f, 0.375f ), 0f, 1f);
          chanceOnDirt  = Mathf.Clamp(Random.Range( 0.400f, 0.475f ), 0f, 1f);
          chanceOnStone = Mathf.Clamp(Random.Range( 0.175f, 0.200f ), 0f, 1f);
        }
        else if(terrainElevation <= 0.2f)
        {
          chanceOnGrass = Mathf.Clamp(Random.Range( 0.400f, 0.450f ), 0f, 1f);
          chanceOnDirt  = Mathf.Clamp(Random.Range( 0.300f, 0.375f ), 0f, 1f);
          chanceOnStone = Mathf.Clamp(Random.Range( 0.275f, 0.300f ), 0f, 1f);
        }
      }

      //Adjustment to desert climate in respect to world height
      if(climate.WorldClimate == (int)MapsFile.Climates.Desert)
      {
        chanceOnGrass = Mathf.Clamp(Random.Range( 0.100f, 0.150f ), 0f, 1f);
        chanceOnDirt  = Mathf.Clamp(Random.Range( 0.100f, 0.150f ), 0f, 1f);
        chanceOnStone = Mathf.Clamp(Random.Range( 0.050f, 0.100f ), 0f, 1f);
      }

      //Adjustment to desert climate in respect to world height
      if(climate.WorldClimate == (int)MapsFile.Climates.Desert2)
      {
        chanceOnGrass = Mathf.Clamp(Random.Range( 0.05f, 0.25f ), 0f, 1f);
        chanceOnDirt  = Mathf.Clamp(Random.Range( 0.05f, 0.25f ), 0f, 1f);
        chanceOnStone = Mathf.Clamp(Random.Range( 0.05f, 0.20f ), 0f, 1f);
      }

      // Adjustment to temperate woodlands climate in respect to world height
      if(climate.WorldClimate == (int)MapsFile.Climates.HauntedWoodlands)
      {
        chanceOnGrass = Random.Range( 0.05f, 0.09f );
        chanceOnDirt  = Random.Range( 0.045f, 0.065f );
        chanceOnStone = Random.Range( 0.05f, 0.1f );
      }
            
      //Adjustment to mountain woodland climate in respect to world height
      if(climate.WorldClimate == (int)MapsFile.Climates.MountainWoods)
      {
        if (terrainElevation > 0.475f)
        {
          chanceOnGrass = Random.Range(0.035f, 0.080f);
          chanceOnDirt  = Random.Range(0.120f, 0.150f);
          chanceOnStone = Random.Range(0.120f, 0.150f);
        }
        else if (terrainElevation <= 0.475f && terrainElevation > 0.4f)
        {
          chanceOnGrass = Random.Range(0.040f, 0.100f);
          chanceOnDirt  = Random.Range(0.100f, 0.130f);
          chanceOnStone = Random.Range(0.100f, 0.130f);
        }
        else if(terrainElevation <= 0.4f && terrainElevation > 0.325f)
        {
          chanceOnGrass = Random.Range(0.050f, 0.100f);
          chanceOnDirt  = Random.Range(0.090f, 0.110f);
          chanceOnStone = Random.Range(0.090f, 0.110f);
        }
        else if(terrainElevation <= 0.325f && terrainElevation > 0.25f)
        {
          chanceOnGrass = Random.Range(0.090f, 0.120f);
          chanceOnDirt  = Random.Range(0.070f, 0.090f);
          chanceOnStone = Random.Range(0.060f, 0.090f);
        }
        else if(terrainElevation <= 0.25f)
        {
          chanceOnGrass = Random.Range(0.100f, 0.130f);
          chanceOnDirt  = Random.Range(0.060f, 0.090f);
          chanceOnStone = Random.Range(0.050f, 0.080f);
        }
      }
            
      //Adjustment to swamp climate in respect to world height
      if(climate.WorldClimate == (int)MapsFile.Climates.Swamp)
      {
        chanceOnStone = Mathf.Clamp(Random.Range( 0.00f, 0.00f ), 0f, 1f);
        chanceOnDirt  = Mathf.Clamp(Random.Range( 0.00f, 0.00f ), 0f, 1f);
        chanceOnGrass = Mathf.Clamp(Random.Range( 0.00f, 0.00f ), 0f, 1f);
      }   
      
      //Adjustment to rainforest climate in respect to world height
      if(climate.WorldClimate == (int)MapsFile.Climates.Rainforest)
      {
        chanceOnGrass = /* Random.Range(0.035f,  */0.30f/* ) */;
        chanceOnDirt  = /* Random.Range(0.120f,  */0.30f/* ) */;
        chanceOnStone = /* Random.Range(0.120f,  */0.30f/* ) */;
      }   
      
      //Adjustment to subtropical climate in respect to world height
      if(climate.WorldClimate == (int)MapsFile.Climates.Subtropical)
      {
        chanceOnStone = Mathf.Clamp(Random.Range( 0.00f, 0.00f ), 0f, 1f);
        chanceOnDirt  = Mathf.Clamp(Random.Range( 0.00f, 0.00f ), 0f, 1f);
        chanceOnGrass = Mathf.Clamp(Random.Range( 0.00f, 0.00f ), 0f, 1f);
      }
      #endregion            

      // Vegetation Styles of the map pixel
      int mapStyle = Random.Range(0,6);
      float mapStyleChance = 0;

      if (mapStyle < 1)
        mapStyleChance = mapStyleChance0;
      else if (mapStyle >= 1 && mapStyle < 2)
        mapStyleChance = mapStyleChance1;
      else if (mapStyle >= 2 && mapStyle < 3)
        mapStyleChance = mapStyleChance2;
      else if (mapStyle >= 3 && mapStyle < 4)
        mapStyleChance = mapStyleChance3;
      else if (mapStyle >= 4 && mapStyle < 5)
        mapStyleChance = mapStyleChance4;
      else
        mapStyleChance = mapStyleChance5;

      for (int y = 0; y < tDim; y++)
      {
        for (int x = 0; x < tDim; x++)
        {
          // Get latitude and longitude of this tile
          int latitude = (int)(dfTerrain.MapPixelX * MapsFile.WorldMapTileDim + x);
          int longitude = (int)(MapsFile.MaxWorldTileCoordZ - dfTerrain.MapPixelY * MapsFile.WorldMapTileDim + y);

          // Set texture tile using weighted noise
          float weight = 0;

          // Reject based on steepness
          float steepness = terrainData.GetSteepness((float)x / tDim, (float)y / tDim);
          if (steepness > maxSteepness)
            continue;

          // Reject if inside location rect (expanded slightly to give extra clearance around locations)
          tilePos.x = x;
          tilePos.y = y;
          if(vegetationInLocations)
          {
            if (rect.x > 0 && rect.y > 0 && rect.Contains(tilePos) && dfTerrain.MapData.worldClimate != (int)MapsFile.Climates.Rainforest)
              continue;
          }
          else
          {
            if (rect.x > 0 && rect.y > 0 && rect.Contains(tilePos))
              continue;
          }

          // Chance also determined by tile type
          int tile = dfTerrain.MapData.tilemapSamples[x, y] & 0x3F;
          if (tile == 1)
          {   // Dirt
            if (Random.Range(0.0f, 1.0f) > chanceOnDirt)
              continue;
          }
          else if (tile == 2)
          {   // Grass
            if (Random.Range(0.0f, 1.0f) > chanceOnGrass)
              continue;
          }
          else if (tile == 3)
          {   // Stone
            if (Random.Range(0.0f, 1.0f) > chanceOnStone)
              continue;
          }
          else if (
            tile == 4  || tile == 5  || tile == 6  || tile == 7  || tile == 8  ||
            tile == 19 || tile == 20 || tile == 21 || tile == 22 || tile == 23 ||
            tile == 29 || tile == 30 || tile == 31 || tile == 32 || tile == 33 ||
            tile == 34 || tile == 35 || tile == 36 || tile == 37 || tile == 38 ||
            tile == 40 || tile == 41 || tile == 43 || tile == 44 || tile == 48 ||
            tile == 49 || tile == 50)
          {}  //Water
          else
          {   // Anything else
              continue;
          }

          //Defining height for the billboard placement
          int hx = (int)Mathf.Clamp(hDim * ((float)x / (float)tDim), 0, hDim - 1);
          int hy = (int)Mathf.Clamp(hDim * ((float)y / (float)tDim), 0, hDim - 1);
          float height = dfTerrain.MapData.heightmapSamples[hy, hx] * maxTerrainHeight;

          if (tile == 0 || height <= 0)
            continue;

          int record = (int)Mathf.Round(Random.Range(1, 32));
          switch (climate.WorldClimate)
          {
            #region Temperate Spawns
            case (int)MapsFile.Climates.Woodlands:

              weight += GetNoise(latitude, longitude, tempForestFrequency, tempForestAmplitude, tempForestPersistence, tempForestOctaves, 100);

              if (tile == 1) // Dirt
              {
                // Beach
                if(dfTerrain.MapData.heightmapSamples[hy, hx] * maxTerrainHeight < beachLine)
                {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, temperateWoodlandBeach, scale, steepness, terrain, x, y, 0.25f); // Beach

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(0,2)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, temperateWoodlandBeach, scale, steepness, terrain, x, y, 1.5f); // Beach                                      
                  }
                }
                else
                {   
                  if(GetWeightedRecord(weight) == "forest")
                  {
                    if(Random.Range(0,100) < Random.Range(80,90))
                    {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, temperateWoodlandDeadTrees, scale, steepness, terrain, x, y, 0.25f); // Dead Trees
                      
                      for(int i = 0; i < (int)Mathf.Round(Random.Range(0,2)); i++)
                      {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, temperateWoodlandDeadTrees, scale, steepness, terrain, x, y, 1.5f); // Dead Trees                                      
                      }
                    }
                    else
                    {
                      if(Random.Range(0,100) < mapStyleChance)
                      {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, temperateWoodlandDeadTrees, scale, steepness, terrain, x, y, 1f, 3); // Needle Tree
                        
                        for(int i = 0; i < (int)Mathf.Round(Random.Range(0,3)); i++)
                        {
                          AddBillboardToBatch(dfTerrain, dfBillboardBatch, temperateWoodlandDeadTrees, scale, steepness, terrain, x, y, 0.75f, 3); // Needle Tree                                      
                        }

                        for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                        {
                          AddBillboardToBatch(dfTerrain, dfBillboardBatch, temperateWoodlandDeadTrees, scale, steepness, terrain, x, y, 1.25f, 3); // Needle Tree                                     
                        }
                      }
                      else
                      {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, temperateWoodlandDeadTrees, scale, steepness, terrain, x, y, 0.25f); // Dead Trees
                        
                        for(int i = 0; i < (int)Mathf.Round(Random.Range(0,2)); i++)
                        {
                          AddBillboardToBatch(dfTerrain, dfBillboardBatch, temperateWoodlandDeadTrees, scale, steepness, terrain, x, y, 1.5f); // Dead Trees
                        }

                        for(int i = 0; i < (int)Mathf.Round(Random.Range(0,2)); i++)
                        {
                          AddBillboardToBatch(dfTerrain, dfBillboardBatch, temperateWoodlandDeadTrees, scale, steepness, terrain, x, y, 0.75f); // Dead Trees
                        }
                      }
                    }
                  }
                }
              }
              else if (tile == 2) // Grass
              {   
                float rndMajor = Random.Range(0.0f,100.0f);
                if (rndMajor <= temperateMushroomRingChance) // Mushroom Circle
                {
                  Vector3 pos = new Vector3(x * scale, 0, (y + 0.5f) * scale);
                  float height2 = terrain.SampleHeight(pos + terrain.transform.position);
                  pos.y = height2 - (steepness / slopeSinkRatio);
                  dfBillboardBatch.AddItem(23, pos);
                  
                  pos = new Vector3((x + 0.272f) * scale, 0, (y - 0.404f) * scale);
                  height2 = terrain.SampleHeight(pos + terrain.transform.position);
                  pos.y = height2 - (steepness / slopeSinkRatio);
                  dfBillboardBatch.AddItem(23, pos);
                  
                  pos = new Vector3((x - 0.272f) * scale, 0, (y - 0.404f) * scale);
                  height2 = terrain.SampleHeight(pos + terrain.transform.position);
                  pos.y = height2 - (steepness / slopeSinkRatio);
                  dfBillboardBatch.AddItem(23, pos);
                  
                  pos = new Vector3((x - 0.475f) * scale, 0, (y + 0.154f) * scale);
                  height2 = terrain.SampleHeight(pos + terrain.transform.position);
                  pos.y = height2 - (steepness / slopeSinkRatio);
                  dfBillboardBatch.AddItem(23, pos);

                  pos = new Vector3((x + 0.475f) * scale, 0, (y + 0.154f) * scale);
                  height2 = terrain.SampleHeight(pos + terrain.transform.position);
                  pos.y = height2 - (steepness / slopeSinkRatio);
                  dfBillboardBatch.AddItem(23, pos);
                }
                else if (GetWeightedRecord(weight, tempForestLimit1, tempForestLimit2) == "flower")
                {
                  if((int)Mathf.Round(Random.Range(0.00f,1.00f)) > 0.50f)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, temperateWoodlandMushroom, scale, steepness, terrain, x, y, 0.00f); // Mushroom
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(5,10)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, temperateWoodlandFlowers, scale, steepness, terrain, x, y, 0.50f); // Flowers
                  }

                  float rndMinor = Random.Range(0,100);
                  if(rndMinor < mapStyleChance1)
                  {
                    for(int i = 0; i < (int)Mathf.Round(Random.Range(5,10)); i++)
                    {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, temperateWoodlandFlowers, scale, steepness, terrain, x, y, 1.50f); // Flowers
                    }
                  }
                }
                else if (GetWeightedRecord(weight, tempForestLimit1, tempForestLimit2) == "forest")
                {
                  float rndMinor = Random.Range(0,100);
                  if(rndMinor < mapStyleChance)
                  {                                        
                    for(int i = 0; i < (int)Mathf.Round(Random.Range(2,5)); i++)
                    {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, temperateWoodlandTrees, scale, steepness, terrain, x, y, 1.50f); // Trees
                    }

                    for(int i = 0; i < (int)Mathf.Round(Random.Range(0,7)); i++)
                    {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, temperateWoodlandBushes, scale, steepness, terrain, x, y, 1.75f); // Bushes
                    }

                    if(rndMinor < mapStyleChance1)
                    {
                      for(int i = 0; i < (int)Mathf.Round(Random.Range(0,2)); i++)
                      {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, temperateWoodlandTrees, scale, steepness, terrain, x, y, 1.75f); // Trees
                      }
                    }
                    
                    if(rndMinor < mapStyleChance0)
                    {
                      for(int i = 0; i < (int)Mathf.Round(Random.Range(0,2)); i++)
                      {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, temperateWoodlandTrees, scale, steepness, terrain, x, y, 2.00f); // Trees
                      }
                    }
                  }
                }
              }
              else if (tile == 3) // Stone
              {
                AddBillboardToBatch(dfTerrain, dfBillboardBatch, temperateWoodlandRocks, scale, steepness, terrain, x, y, 1.00f); // Stones
                
                for(int i = 0; i < (int)Mathf.Round(Random.Range(0,2)); i++)
                {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, temperateWoodlandRocks, scale, steepness, terrain, x, y, 1.50f); // Stones
                }
              }
              break;
            #endregion

            #region Mountain Spawns
            case (int)MapsFile.Climates.Mountain:

              weight += GetNoise(latitude, longitude, mountForestFrequency, mountForestAmplitude, mountForestPersistence, mountForestOctaves, 100);

              if (tile == 1) // Dirt
              {
                // Beach
                if(dfTerrain.MapData.heightmapSamples[hy, hx] * maxTerrainHeight < beachLine)
                {
                  if(Random.Range(0,100) < mapStyleChance)
                  {
                    for(int i = 0; i < (int)Mathf.Round(Random.Range(0,2)); i++)
                    {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsBeach, scale, steepness, terrain, x, y, 0.75f); // Beach
                    }
                  }
                }
                else if(dfTerrain.MapData.heightmapSamples[hy, hx] * maxTerrainHeight >= beachLine && dfTerrain.MapData.heightmapSamples[hy, hx] < treeLine)
                {
                  if(Random.Range(0,100) < Random.Range(10,20))
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsDeadTrees, scale, steepness, terrain, x, y, 0.75f); // Dead Trees
                    
                    for(int i = 0; i < (int)Mathf.Round(Random.Range(0,1)); i++)
                    {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsDeadTrees, scale, steepness, terrain, x, y, 1.50f); // Dead Trees                                      
                    }
                  }
                  else
                  {
                    if(Random.Range(0,100) < mapStyleChance)
                    {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsNeedleTrees, scale, steepness, terrain, x, y, 0.00f); // Needle Tree
                      
                      for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                      {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsNeedleTrees, scale, steepness, terrain, x, y, 1.00f); // Needle Tree                                      
                      }

                      for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                      {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsNeedleTrees, scale, steepness, terrain, x, y, 1.50f); // Needle Tree                                     
                      }
                    }
                    else
                    {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsDeadTrees, scale, steepness, terrain, x, y, 0.50f); // Dead Trees
              
                      for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                      {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsNeedleTrees, scale, steepness, terrain, x, y, 1.25f); // Needle Trees
                      }
                      
                      for(int i = 0; i < (int)Mathf.Round(Random.Range(0,1)); i++)
                      {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsRocks, scale, steepness, terrain, x, y, 0.75f); // Rocks
                      }
                    }
                  }
                }
                else
                {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(0,3)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsRocks, scale, steepness, terrain, x, y, 1.00f); // Rocks
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(0,3)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsBeach, scale, steepness, terrain, x, y, 1.00f); // Beach
                  }
                }
              }
                else if (tile == 2) // Grass
                {  
                  float rndMajor = Random.Range(0.0f,100.0f);
                  if (Random.Range(0.0f,100.0f) < mountainStoneCircleChance)
                  {
                    Vector3 pos = new Vector3(x * scale, 0, y * scale);
                    float height2 = terrain.SampleHeight(pos + terrain.transform.position);
                    pos.y = height2 - (steepness / slopeSinkRatio);
                    dfBillboardBatch.AddItem(mountainsRocks[5], pos);
                    
                    pos = new Vector3((x+0.4f) * scale, 0, y * scale);
                    height2 = terrain.SampleHeight(pos + terrain.transform.position);
                    pos.y = height2 - (steepness / slopeSinkRatio);
                    dfBillboardBatch.AddItem(mountainsRocks[0], pos);
                    
                    pos = new Vector3((x-0.4f) * scale, 0, y * scale);
                    height2 = terrain.SampleHeight(pos + terrain.transform.position);
                    pos.y = height2 - (steepness / slopeSinkRatio);
                    dfBillboardBatch.AddItem(mountainsRocks[0], pos);
                    
                    pos = new Vector3(x * scale, 0, (y+0.4f) * scale);
                    height2 = terrain.SampleHeight(pos + terrain.transform.position);
                    pos.y = height2 - (steepness / slopeSinkRatio);
                    dfBillboardBatch.AddItem(mountainsRocks[0], pos);
                    
                    pos = new Vector3(x * scale, 0, (y-0.4f) * scale);
                    height2 = terrain.SampleHeight(pos + terrain.transform.position);
                    pos.y = height2 - (steepness / slopeSinkRatio);
                    dfBillboardBatch.AddItem(mountainsRocks[0], pos);

                    pos = new Vector3((x+0.3f) * scale, 0, (y+0.3f) * scale);
                    height2 = terrain.SampleHeight(pos + terrain.transform.position);
                    pos.y = height2 - (steepness / slopeSinkRatio);
                    dfBillboardBatch.AddItem(mountainsRocks[0], pos);

                    pos = new Vector3((x-0.3f) * scale, 0, (y+0.3f) * scale);
                    height2 = terrain.SampleHeight(pos + terrain.transform.position);
                    pos.y = height2 - (steepness / slopeSinkRatio);
                    dfBillboardBatch.AddItem(mountainsRocks[0], pos);

                    pos = new Vector3((x+0.3f) * scale, 0, (y-0.3f) * scale);
                    height2 = terrain.SampleHeight(pos + terrain.transform.position);
                    pos.y = height2 - (steepness / slopeSinkRatio);
                    dfBillboardBatch.AddItem(mountainsRocks[0], pos);

                    pos = new Vector3((x-0.3f) * scale, 0, (y-0.3f) * scale);
                    height2 = terrain.SampleHeight(pos + terrain.transform.position);
                    pos.y = height2 - (steepness / slopeSinkRatio);
                    dfBillboardBatch.AddItem(mountainsRocks[0], pos);
                  }
                  if(dfTerrain.MapData.heightmapSamples[hy, hx] > treeLine)
                  {
                    float rndMinor = Random.Range(0,100);
                    if(rndMinor < mapStyleChance)
                    {
                      if((int)Mathf.Round(Random.Range(0.00f,1.00f)) > 0.97f)
                      {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsRocks, scale, steepness, terrain, x, y, 0.00f); // Rock
                      }

                      for(int i = 0; i < (int)Mathf.Round(Random.Range(2,4)); i++)
                      {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsFlowers, scale, steepness, terrain, x, y, 0.5f); // Flowers
                      }

                      if(rndMinor < mapStyleChance1)
                      {
                        for(int i = 0; i < (int)Mathf.Round(Random.Range(2,4)); i++)
                        {
                          AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsFlowers, scale, steepness, terrain, x, y, 1.00f); // Flowers
                        }
                      }
                    }
                    else
                    {
                      for(int i = 0; i < (int)Mathf.Round(Random.Range(0,4)); i++)
                      {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsFlowers, scale, steepness, terrain, x, y, 0.5f); // Rocks
                      }

                      for(int i = 0; i < (int)Mathf.Round(Random.Range(2,4)); i++)
                      {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsGrass, scale, steepness, terrain, x, y, 0.75f); // Grass
                      }

                      if(rndMinor < mapStyleChance1)
                      {
                        for(int i = 0; i < (int)Mathf.Round(Random.Range(2,4)); i++)
                        {
                          AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsGrass, scale, steepness, terrain, x, y, 1.25f); // Grass
                        }
                      }
                    } 
                  }
                  else if(dfTerrain.MapData.heightmapSamples[hy, hx] < treeLine && dfTerrain.MapData.heightmapSamples[hy, hx] >= Random.Range(0.70f,0.75f))
                  {
                    if (GetWeightedRecord(weight, mountForestLimit1, mountForestLimit2) == "flower")
                    {
                      float rndMinor = Random.Range(0,100);
                      if(rndMinor < mapStyleChance)
                      {                                        
                        for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                        {
                          AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsTrees, scale, steepness, terrain, x, y, 0.50f); // Trees
                        }

                        for(int i = 0; i < (int)Mathf.Round(Random.Range(2,4)); i++)
                        {
                          AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsTrees, scale, steepness, terrain, x, y, 1.00f); // Trees
                        }

                        for(int i = 0; i < (int)Mathf.Round(Random.Range(2,4)); i++)
                        {
                          AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsGrass, scale, steepness, terrain, x, y, 0.50f); // Grass
                        }

                        if(rndMinor < mapStyleChance1)
                        {
                          for(int i = 0; i < (int)Mathf.Round(Random.Range(1,4)); i++)
                          {
                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsTrees, scale, steepness, terrain, x, y, 1.50f); // Trees
                          }

                          for(int i = 0; i < (int)Mathf.Round(Random.Range(2,4)); i++)
                          {
                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsFlowers, scale, steepness, terrain, x, y, 0.75f); // Trees
                          }

                          for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                          {
                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsGrass, scale, steepness, terrain, x, y, 1.00f); // Grass
                          }
                        }
                          
                        if(rndMinor < mapStyleChance0)
                        {
                          for(int i = 0; i < (int)Mathf.Round(Random.Range(0,2)); i++)
                          {
                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsNeedleTrees, scale, steepness, terrain, x, y, 1.75f); // Trees
                          }
                          for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                          {
                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsFlowers, scale, steepness, terrain, x, y, 1.75f); // Trees
                          }

                          if((int)Mathf.Round(Random.Range(0.00f,1.00f)) > 0.98f)
                          {
                            for(int i = 0; i < (int)Mathf.Round(Random.Range(0,2)); i++)
                            {
                              AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsRocks, scale, steepness, terrain, x, y, 1.00f); // Rocks
                            }
                          }
                        }
                      }
                    }
                    else if (GetWeightedRecord(weight, mountForestLimit1, mountForestLimit2) == "forest")
                    {
                      if((int)Mathf.Round(Random.Range(0.00f,1.00f)) > 0.98f)
                      {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsRocks, scale, steepness, terrain, x, y, 0.00f); // Rock
                      }

                      for(int i = 0; i < (int)Mathf.Round(Random.Range(0,2)); i++)
                      {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsTrees, scale, steepness, terrain, x, y, 0.50f); // Trees
                      }

                      for(int i = 0; i < (int)Mathf.Round(Random.Range(0,2)); i++)
                      {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsTrees, scale, steepness, terrain, x, y, 1.00f); // Trees
                      }

                      for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                      {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsFlowers, scale, steepness, terrain, x, y, 0.20f); // Flowers
                      }

                      float rndMinor = Random.Range(0,100);
                      if(rndMinor < mapStyleChance1)
                      {
                        for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                        {
                          AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsFlowers, scale, steepness, terrain, x, y, 0.50f); // Flowers
                        }
                      }
                    }
                  }
                  else if(dfTerrain.MapData.heightmapSamples[hy, hx] < Random.Range(0.70f,0.75f) && dfTerrain.MapData.heightmapSamples[hy, hx] > Random.Range(0.45f,0.40f))
                  {
                    if (GetWeightedRecord(weight, mountForestLimit1, mountForestLimit2) == "forest")
                    {
                      float rndMinor = Random.Range(0,100);
                      if(rndMinor < mapStyleChance)
                      {                                        
                        for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                        {
                          AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsTrees, scale, steepness, terrain, x, y, 0.50f); // Trees
                        }

                        for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                        {
                          AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsGrass, scale, steepness, terrain, x, y, 0.50f); // Grass
                        }

                        if(rndMinor < mapStyleChance1)
                        {
                          for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                          {
                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsFlowers, scale, steepness, terrain, x, y, 1.00f); // Flowers
                          }
                        }
                        
                        if(rndMinor < mapStyleChance0)
                        {
                          for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                          {
                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsFlowers, scale, steepness, terrain, x, y, 1.75f); // Flowers
                          }

                          if((int)Mathf.Round(Random.Range(0.00f,1.00f)) > 0.95f)
                          {
                            for(int i = 0; i < (int)Mathf.Round(Random.Range(0,2)); i++)
                            {
                              AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsRocks, scale, steepness, terrain, x, y, 1.00f); // Rocks
                            }
                          }
                        }
                      }
                    }
                    else if (GetWeightedRecord(weight, mountForestLimit1, mountForestLimit2) == "flower")
                    {
                      if((int)Mathf.Round(Random.Range(0.00f,1.00f)) > 0.95f)
                      {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsRocks, scale, steepness, terrain, x, y, 0.00f); // Rock
                      }

                      for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                      {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsFlowers, scale, steepness, terrain, x, y, 0.50f); // Flowers
                      }

                      float rndMinor = Random.Range(0,100);
                      if(rndMinor < mapStyleChance1)
                      {
                        for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                        {
                          AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsFlowers, scale, steepness, terrain, x, y, 0.75f); // Flowers
                        }
                      }
                    }
                  }
                  else if(dfTerrain.MapData.heightmapSamples[hy, hx] < Random.Range(0.40f,0.45f))
                  {
                    float rndMinor = Random.Range(0,100);
                    if(GetWeightedRecord(weight, mountForestLimit1, mountForestLimit2) == "forest")
                    {                                        
                      for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                      {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsTrees, scale, steepness, terrain, x, y, 0.50f); // Trees
                      }

                      for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                      {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsGrass, scale, steepness, terrain, x, y, 0.50f); // Grass
                      }
                      
                      if(rndMinor < mapStyleChance0)
                      {
                        if((int)Mathf.Round(Random.Range(0.00f,1.00f)) > 0.95f)
                        {
                          for(int i = 0; i < (int)Mathf.Round(Random.Range(0,2)); i++)
                          {
                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsRocks, scale, steepness, terrain, x, y, 1.00f); // Rocks
                          }
                        }
                      }
                    }
                    else if(GetWeightedRecord(weight, mountForestLimit1, mountForestLimit2) == "flower")
                    {                                        
                      for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                      {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsFlowers, scale, steepness, terrain, x, y, 0.50f); // Trees
                      }
                      
                      if(rndMinor < mapStyleChance0)
                      {
                        if((int)Mathf.Round(Random.Range(0.00f,1.00f)) > 0.95f)
                        {
                          for(int i = 0; i < (int)Mathf.Round(Random.Range(0,1)); i++)
                          {
                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsRocks, scale, steepness, terrain, x, y, 1.00f); // Rocks
                          }
                        }
                      }
                    }
                  }
                }
                else if (tile == 3) // Stone
                {
                    if (GetWeightedRecord(weight, mountForestLimit1, mountForestLimit2) == "forest")
                    {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsRocks, scale, steepness, terrain, x, y, 0.00f); // Stones
                    
                        for(int i = 0; i < (int)Mathf.Round(Random.Range(0,2)); i++)
                        {
                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsRocks, scale, steepness, terrain, x, y, 0.25f); // Stones
                        }

                        for(int i = 0; i < (int)Mathf.Round(Random.Range(2,3)); i++)
                        {
                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsFlowers, scale, steepness, terrain, x, y, 0.50f); // Flowers
                        }
                    }
                    if (GetWeightedRecord(weight, mountForestLimit1, mountForestLimit2) == "flower")
                    {
                        for(int i = 0; i < (int)Mathf.Round(Random.Range(2,4)); i++)
                        {
                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, mountainsFlowers, scale, steepness, terrain, x, y, 0.75f); // Flowers
                        }
                    }
                }
                break;
            #endregion

            #region Desert1 Spawns
            case (int)MapsFile.Climates.Desert: //STEPPE
                
              if (tile == 1) // Dirt
              {
                float elevationRnd = Random.Range(0.01f,0.03f);  
                if(dfTerrain.MapData.heightmapSamples[hy, hx] < elevationRnd && Random.Range(0.0f,100.0f) < Random.Range(0.0f,20.0f))
                {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(4,10)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(2f,3f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(10,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(3f,5f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,50)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertPlants, scale, steepness, terrain, x, y, Random.Range(3.5f,7f)); // Plants
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertCactus, scale, steepness, terrain, x, y, Random.Range(3.5f,9f)); // Cactus
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,45)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3f,6f)); // Flowers
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(80,165)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3.5f,12f)); // Flowers
                  }
                } 
                if(dfTerrain.MapData.heightmapSamples[hy, hx] < elevationRnd/2 && Random.Range(0.0f,100.0f) < Random.Range(10.0f,40.0f))
                {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(4,10)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(2f,3f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(10,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(3f,5f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,50)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertPlants, scale, steepness, terrain, x, y, Random.Range(3.5f,7f)); // Plants
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertCactus, scale, steepness, terrain, x, y, Random.Range(3.5f,9f)); // Cactus
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,45)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3f,6f)); // Flowers
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(80,165)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3.5f,12f)); // Flowers
                  }
                }
                if(dfTerrain.MapData.heightmapSamples[hy, hx] < elevationRnd/3 && Random.Range(0.0f,100.0f) < Random.Range(30.0f,70.0f))
                {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(4,10)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(2f,3f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(10,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(3f,5f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,50)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertPlants, scale, steepness, terrain, x, y, Random.Range(3.5f,7f)); // Plants
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertCactus, scale, steepness, terrain, x, y, Random.Range(3.5f,9f)); // Cactus
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,45)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3f,6f)); // Flowers
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(80,165)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3.5f,12f)); // Flowers
                  }
                }
                if(Random.Range(0,100) < desert1DirtChance)
                {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertDeadTrees, scale, steepness, terrain, x, y, 0.25f); // Dead Trees

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(0,10)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertDeadTrees, scale, steepness, terrain, x, y, 3.5f); // Dead Trees
                  }
                }
                else
                {
                  if(Random.Range(0,100) < Random.Range(20,55))
                  {
                    if(Random.Range(0,100) < mapStyleChance)
                    {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertCactus, scale, steepness, terrain, x, y, 1f); // Cactus
                      
                      if(Random.Range(0,100) < Random.Range(2,6))
                      {
                        for(int i = 0; i < (int)Mathf.Round(Random.Range(0,6)); i++)
                        {
                          AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, 1.5f); // Flowers                                      
                        }

                        if(Random.Range(0,100) < Random.Range(0,15))
                        {
                          for(int i = 0; i < (int)Mathf.Round(Random.Range(0,15)); i++)
                          {
                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, 2f); // Flowers                                     
                          }
                        }
                      }
                    }
                  }
                }
              }
              else if (tile == 2) // Grass
              {
                float elevationRnd = Random.Range(0.01f,0.03f);  
                if(dfTerrain.MapData.heightmapSamples[hy, hx] < elevationRnd && Random.Range(0.0f,100.0f) < Random.Range(0.0f,15.0f))
                {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(4,10)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(2f,3f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(10,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(3f,5f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,50)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertPlants, scale, steepness, terrain, x, y, Random.Range(3.5f,7f)); // Plants
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertCactus, scale, steepness, terrain, x, y, Random.Range(3.5f,9f)); // Cactus
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,45)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3f,6f)); // Flowers
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(80,165)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3.5f,12f)); // Flowers
                  }
                }
                if(dfTerrain.MapData.heightmapSamples[hy, hx] < elevationRnd/2 && Random.Range(0.0f,100.0f) < Random.Range(5.0f,30.0f))
                {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(4,10)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(2f,3f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(10,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(3f,5f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,50)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertPlants, scale, steepness, terrain, x, y, Random.Range(3.5f,7f)); // Plants
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertCactus, scale, steepness, terrain, x, y, Random.Range(3.5f,9f)); // Cactus
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,45)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3f,6f)); // Flowers
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(80,165)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3.5f,12f)); // Flowers
                  }
                }
                if(dfTerrain.MapData.heightmapSamples[hy, hx] < elevationRnd/3 && Random.Range(0.0f,100.0f) < Random.Range(20.0f,50.0f))
                {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(4,10)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(2f,3f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(10,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(3f,5f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,50)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertPlants, scale, steepness, terrain, x, y, Random.Range(3.5f,7f)); // Plants
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertCactus, scale, steepness, terrain, x, y, Random.Range(3.5f,9f)); // Cactus
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,45)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3f,6f)); // Flowers
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(80,165)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3.5f,12f)); // Flowers
                  }
                }
                if(Random.Range(0,100) < Random.Range(5,15))
                {
                  float rndMajor = Random.Range(0,100);
                  if(rndMajor < mapStyleChance)
                  {
                    if(Random.Range(0,100) < Random.Range(30,70))
                    {
                      for(int i = 0; i < (int)Mathf.Round(Random.Range(0,6)); i++)
                      {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, 4f); // Flowers                                      
                      }
                    }

                    float rndGrass = Random.Range(0.0f,100.0f);  
                    if (rndGrass <= desert1GrassChance1)
                    {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertCactus, scale, steepness, terrain, x, y, 0.25f); // Cactus
                      
                      if(Random.Range(0,100) < Random.Range(10,15))
                      {
                        for(int i = 0; i < (int)Mathf.Round(Random.Range(0,8)); i++)
                        {
                          AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, 0.5f); // Flowers
                        }

                        if(Random.Range(0,100) < Random.Range(10,15))
                        {
                          for(int i = 0; i < (int)Mathf.Round(Random.Range(0,15)); i++)
                          {
                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, 2f); // Flowers
                          }
                        }
                      }
                    }
                    else if(rndGrass > desert1GrassChance1 && rndGrass <= desert1GrassChance2)
                    {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertPlants, scale, steepness, terrain, x, y, 0.25f); // Plant
                      
                      if(Random.Range(0,100) < Random.Range(10,15))
                      {
                        for(int i = 0; i < (int)Mathf.Round(Random.Range(0,8)); i++)
                        {
                          AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, 0.5f); // Flowers
                        }
                        
                        if(Random.Range(0,100) < Random.Range(10,15))
                        {
                          for(int i = 0; i < (int)Mathf.Round(Random.Range(0,15)); i++)
                          {
                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, 2f); // Flowers
                          }
                        }
                      }
                    }

                    else if(Random.Range(0,100) < Random.Range(3,13))
                    {
                      for(int i = 0; i < (int)Mathf.Round(Random.Range(5,25)); i++)
                      {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertCactus, scale, steepness, terrain, x, y, Random.Range(2.5f,3.5f)); // Cactus
                      }

                      for(int i = 0; i < (int)Mathf.Round(Random.Range(50,100)); i++)
                      {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3.5f,7f)); // Flowers
                      }
                    }

                    else if(Random.Range(0,100) < Random.Range(10,20))
                    {
                      for(int i = 0; i < (int)Mathf.Round(Random.Range(50,100)); i++)
                      {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(2.5f,8f)); // Flowers
                      }
                    }
                  }
                  else
                  {
                    if(Random.Range(0,100) < Random.Range(1,6))
                    {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertDeadTrees, scale, steepness, terrain, x, y, 0.25f); // Dead Tree
                      if (rndMajor > mapStyleChance4)
                      {
                        for(int i = 0; i < (int)Mathf.Round(Random.Range(0,5)); i++)
                        {
                          AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, 0.5f); // Flowers
                        }
                      }

                      if (rndMajor > mapStyleChance3)
                      {
                        for(int i = 0; i < (int)Mathf.Round(Random.Range(0,3)); i++)
                        {
                          AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertDeadTrees, scale, steepness, terrain, x, y, 3.5f); // Dead Trees
                        }

                        for(int i = 0; i < (int)Mathf.Round(Random.Range(0,20)); i++)
                        {
                          AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertStones, scale, steepness, terrain, x, y, 3f); // Flowers
                        }
                      }
                    }
                    else if(Random.Range(0.0f,100.0f) < Random.Range(10.0f,30.0f))
                    {
                      for(int i = 0; i < (int)Mathf.Round(Random.Range(4,15)); i++)
                      {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(2f,3f)); // Trees
                      }

                      for(int i = 0; i < (int)Mathf.Round(Random.Range(5,30)); i++)
                      {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(3f,5f)); // Trees
                      }

                      for(int i = 0; i < (int)Mathf.Round(Random.Range(15,40)); i++)
                      {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertPlants, scale, steepness, terrain, x, y, Random.Range(3.5f,7f)); // Plants
                      }

                      for(int i = 0; i < (int)Mathf.Round(Random.Range(0,10)); i++)
                      {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertCactus, scale, steepness, terrain, x, y, Random.Range(3.5f,9f)); // Cactus
                      }

                      for(int i = 0; i < (int)Mathf.Round(Random.Range(20,45)); i++)
                      {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3f,6f)); // Flowers
                      }

                      for(int i = 0; i < (int)Mathf.Round(Random.Range(60,135)); i++)
                      {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3.5f,12f)); // Flowers
                      }
                    }
                  }
                }
              }
              else if (tile == 3) // Stone
              {
                AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertStones, scale, steepness, terrain, x, y, 1f); // Stones
                    
                for(int i = 0; i < (int)Mathf.Round(Random.Range(0,5)); i++)
                {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertStones, scale, steepness, terrain, x, y, 2f); // Stones
                }
              }
              else if (
                tile == 0 || tile == 4 || tile == 5 || tile == 6 || tile == 7 || tile == 8 || tile == 19 || tile == 20 || tile == 21 || tile == 22 ||
                tile == 23 || tile == 29 || tile == 30 || tile == 31 || tile == 32 || tile == 33 || tile == 34 || tile == 35 || tile == 36 || tile == 37 ||
                tile == 38 || tile == 40 || tile == 41 || tile == 43 || tile == 44 || tile == 48 || tile == 49 || tile == 50)
              {
                int rndMajor = Random.Range(0,3);
                if(rndMajor < 1)
                {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(5,10)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(2f,2.5f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(10,25)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(3.5f,6f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertWaterPlants, scale, steepness, terrain, x, y, Random.Range(4f,7f)); // Plants
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(40,80)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(5f,9f)); // Flowers
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(1.2f,3f)); // Flowers
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(10,20)); i++)
                  {
                    AddBillboardToBatchWater(dfTerrain, dfBillboardBatch, desertWaterFlowers, scale, steepness, terrain, x, y, Random.Range(0.2f,1.5f)); // Flowers
                  }
                }
                else if(rndMajor >= 1 && rndMajor < 2)
                {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(3,8)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(1f,3.5f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(6,18)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(2f,6f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(15,30)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertWaterPlants, scale, steepness, terrain, x, y, Random.Range(2.5f,7f)); // Plants
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(30,60)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(2.5f,9f)); // Flowers
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(10,20)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(1.2f,3f)); // Flowers
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(5,10)); i++)
                  {
                    AddBillboardToBatchWater(dfTerrain, dfBillboardBatch, desertWaterFlowers, scale, steepness, terrain, x, y, Random.Range(0.2f,0.5f)); // Flowers
                  }
                }
                else if(rndMajor >= 2)
                {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(2,5)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(1f,3.5f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(4,12)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(1.5f,6f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(10,20)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertWaterPlants, scale, steepness, terrain, x, y, Random.Range(1.5f,7f)); // Plants
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(15,25)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(1.5f,9f)); // Flowers
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(6,12)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(1.2f,3f)); // Flowers
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(2,5)); i++)
                  {
                    AddBillboardToBatchWater(dfTerrain, dfBillboardBatch, desertWaterFlowers, scale, steepness, terrain, x, y, Random.Range(0.2f,0.5f)); // Flowers
                  }
                }
              }
              break;
            #endregion

            #region Desert2 Spawns
            case (int)MapsFile.Climates.Desert2: //REAL Desert
            
              if (tile == 1) // Dirt
              {
                float elevationRnd = Random.Range(0.01f,0.03f);  
                if(dfTerrain.MapData.heightmapSamples[hy, hx] < elevationRnd && Random.Range(0.0f,100.0f) < Random.Range(0.0f,25.0f))
                {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(4,10)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(2f,3f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(10,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(3f,5f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,50)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertPlants, scale, steepness, terrain, x, y, Random.Range(3.5f,7f)); // Plants
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertCactus, scale, steepness, terrain, x, y, Random.Range(3.5f,9f)); // Cactus
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,45)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3f,6f)); // Flowers
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(80,165)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3.5f,12f)); // Flowers
                  }
                }
                if(dfTerrain.MapData.heightmapSamples[hy, hx] < elevationRnd/2 && Random.Range(0.0f,100.0f) < Random.Range(25.0f,50.0f))
                {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(4,10)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(2f,3f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(10,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(3f,5f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,50)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertPlants, scale, steepness, terrain, x, y, Random.Range(3.5f,7f)); // Plants
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertCactus, scale, steepness, terrain, x, y, Random.Range(3.5f,9f)); // Cactus
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,45)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3f,6f)); // Flowers
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(80,165)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3.5f,12f)); // Flowers
                  }
                }
                if(dfTerrain.MapData.heightmapSamples[hy, hx] < elevationRnd/3 && Random.Range(0.0f,100.0f) < Random.Range(50.0f,75.0f))
                {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(4,10)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(2f,3f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(10,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(3f,5f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,50)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertPlants, scale, steepness, terrain, x, y, Random.Range(3.5f,7f)); // Plants
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertCactus, scale, steepness, terrain, x, y, Random.Range(3.5f,9f)); // Cactus
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,45)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3f,6f)); // Flowers
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(80,165)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3.5f,12f)); // Flowers
                  }
                }
                if(Random.Range(0,100) < desert2DirtChance)
                {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertDeadTrees, scale, steepness, terrain, x, y, 0.25f); // Dead Trees

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(0,5)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertDeadTrees, scale, steepness, terrain, x, y, 3.5f); // Dead Trees
                  }
                }
                if(Random.Range(0,100) < Random.Range(10,15))
                {
                  if(Random.Range(0,100) < mapStyleChance)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertCactus, scale, steepness, terrain, x, y, 1f); // Cactus
                    
                    if(Random.Range(0,100) < Random.Range(2,6))
                    {
                      for(int i = 0; i < (int)Mathf.Round(Random.Range(0,6)); i++)
                      {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, 1.5f); // Flowers                                      
                      }

                      if(Random.Range(0,100) < Random.Range(0,15))
                      {
                        for(int i = 0; i < (int)Mathf.Round(Random.Range(0,15)); i++)
                        {
                          AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, 2f); // Flowers                                     
                        }
                      }
                    }
                  }
                }
              }
              else if (tile == 2) // Grass
              {
                float elevationRnd = Random.Range(0.01f,0.03f);  
                if(dfTerrain.MapData.heightmapSamples[hy, hx] < elevationRnd && Random.Range(0.0f,100.0f) < Random.Range(0.0f,25.0f))
                {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(4,10)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(2f,3f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(10,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(3f,5f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,50)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertPlants, scale, steepness, terrain, x, y, Random.Range(3.5f,7f)); // Plants
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertCactus, scale, steepness, terrain, x, y, Random.Range(3.5f,9f)); // Cactus
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,45)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3f,6f)); // Flowers
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(80,165)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3.5f,12f)); // Flowers
                  }
                }
                if(dfTerrain.MapData.heightmapSamples[hy, hx] < elevationRnd/2 && Random.Range(0.0f,100.0f) < Random.Range(25.0f,50.0f))
                {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(4,10)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(2f,3f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(10,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(3f,5f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,50)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertPlants, scale, steepness, terrain, x, y, Random.Range(3.5f,7f)); // Plants
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertCactus, scale, steepness, terrain, x, y, Random.Range(3.5f,9f)); // Cactus
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,45)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3f,6f)); // Flowers
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(80,165)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3.5f,12f)); // Flowers
                  }
                }
                if(dfTerrain.MapData.heightmapSamples[hy, hx] < elevationRnd/3 && Random.Range(0.0f,100.0f) < Random.Range(50.0f,75.0f))
                {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(4,10)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(2f,3f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(10,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(3f,5f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,50)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertPlants, scale, steepness, terrain, x, y, Random.Range(3.5f,7f)); // Plants
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertCactus, scale, steepness, terrain, x, y, Random.Range(3.5f,9f)); // Cactus
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,45)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3f,6f)); // Flowers
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(80,165)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3.5f,12f)); // Flowers
                  }
                }
                if(Random.Range(0,100) < Random.Range(0,3))
                {
                  float rndMajor = Random.Range(0,100);
                  if(rndMajor < mapStyleChance)
                  {
                    float rndGrass = Random.Range(0.0f,100.0f);                        
                    if (rndGrass <= desert2GrassChance1)
                    {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertCactus, scale, steepness, terrain, x, y, 0.25f); // Cactus
                      
                      if(Random.Range(0,100) < Random.Range(10,15))
                      {
                        for(int i = 0; i < (int)Mathf.Round(Random.Range(0,8)); i++)
                        {
                          AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, 1.5f); // Flowers
                        }

                        if(Random.Range(0,100) < Random.Range(10,15))
                        {
                          for(int i = 0; i < (int)Mathf.Round(Random.Range(0,15)); i++)
                          {
                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, 3f); // Flowers
                          }
                        }
                      }
                    }
                    else if(rndGrass > desert2GrassChance1 && rndGrass <= desert2GrassChance2)
                    {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertPlants, scale, steepness, terrain, x, y, 0.25f); // Plant
                      
                      if(Random.Range(0,100) < Random.Range(10,15))
                      {
                        for(int i = 0; i < (int)Mathf.Round(Random.Range(0,8)); i++)
                        {
                          AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, 1.5f); // Flowers
                        }
                        
                        if(Random.Range(0,100) < Random.Range(10,15))
                        {
                          for(int i = 0; i < (int)Mathf.Round(Random.Range(0,15)); i++)
                          {
                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, 2.75f); // Flowers
                          }
                        }
                      }
                    }
                    else if(Random.Range(0.0f,100.0f) < Random.Range(0.0f,10.0f))
                    {
                      for(int i = 0; i < (int)Mathf.Round(Random.Range(4,10)); i++)
                      {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(2f,3f)); // Trees
                      }

                      for(int i = 0; i < (int)Mathf.Round(Random.Range(10,40)); i++)
                      {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(3f,5f)); // Trees
                      }

                      for(int i = 0; i < (int)Mathf.Round(Random.Range(20,50)); i++)
                      {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertPlants, scale, steepness, terrain, x, y, Random.Range(3.5f,7f)); // Plants
                      }

                      for(int i = 0; i < (int)Mathf.Round(Random.Range(20,40)); i++)
                      {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertCactus, scale, steepness, terrain, x, y, Random.Range(3.5f,9f)); // Cactus
                      }

                      for(int i = 0; i < (int)Mathf.Round(Random.Range(20,45)); i++)
                      {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3f,6f)); // Flowers
                      }

                      for(int i = 0; i < (int)Mathf.Round(Random.Range(80,165)); i++)
                      {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3.5f,12f)); // Flowers
                      }
                    }

                    else if(Random.Range(0,100) < Random.Range(0,30))
                    {
                      for(int i = 0; i < (int)Mathf.Round(Random.Range(10,40)); i++)
                      {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertCactus, scale, steepness, terrain, x, y, Random.Range(2.5f,3.5f)); // Cactus
                      }

                      for(int i = 0; i < (int)Mathf.Round(Random.Range(50,125)); i++)
                      {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3.5f,7f)); // Flowers
                      }
                    }

                    else if(Random.Range(0,100) < Random.Range(0,50))
                    {
                      for(int i = 0; i < (int)Mathf.Round(Random.Range(50,125)); i++)
                      {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(2.5f,6.5f)); // Flowers
                      }
                    }
                  }
                  else
                  {
                    if(Random.Range(0,100) < Random.Range(5,10))
                    {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertDeadTrees, scale, steepness, terrain, x, y, 0.25f); // Dead Tree
                      
                      
                      if (rndMajor > mapStyleChance4)
                      {
                        for(int i = 0; i < (int)Mathf.Round(Random.Range(0,5)); i++)
                        {
                          AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, 2f); // Flowers
                        }
                      }

                      if (rndMajor > mapStyleChance3)
                      {
                        for(int i = 0; i < (int)Mathf.Round(Random.Range(0,10)); i++)
                        {
                          AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertDeadTrees, scale, steepness, terrain, x, y, 3.5f); // Dead Trees
                        }

                        for(int i = 0; i < (int)Mathf.Round(Random.Range(0,10)); i++)
                        {
                          AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertStones, scale, steepness, terrain, x, y, 3f); // Flowers
                        }
                      }
                    }
                  }
                }
              }
              else if (tile == 3) // Stone
              {
                AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertStones, scale, steepness, terrain, x, y, 1f); // Stones
                    
                for(int i = 0; i < (int)Mathf.Round(Random.Range(0,7)); i++)
                {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertStones, scale, steepness, terrain, x, y, 2.5f); // Stones
                }
              }
              else if (
                tile == 0 || tile == 4 || tile == 5 || tile == 6 || tile == 7 || tile == 8 || tile == 19 || tile == 20 || tile == 21 || tile == 22 ||
                tile == 23 || tile == 29 || tile == 30 || tile == 31 || tile == 32 || tile == 33 || tile == 34 || tile == 35 || tile == 36 || tile == 37 ||
                tile == 38 || tile == 40 || tile == 41 || tile == 43 || tile == 44 || tile == 48 || tile == 49 || tile == 50)
              {
                int rndMajor = Random.Range(0,3);
                if(rndMajor < 1)
                {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(5,20)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(2f,2.5f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(10,35)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(3.5f,6f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(30,50)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertWaterPlants, scale, steepness, terrain, x, y, Random.Range(4f,7f)); // Plants
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(60,120)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(5f,9f)); // Flowers
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(1.2f,3f)); // Flowers
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(25,40)); i++)
                  {
                    AddBillboardToBatchWater(dfTerrain, dfBillboardBatch, desertWaterFlowers, scale, steepness, terrain, x, y, Random.Range(0.2f,1.5f)); // Flowers
                  }
                }
                else if(rndMajor >= 1 && rndMajor < 2)
                {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(3,8)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(1f,3.5f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(6,18)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(2f,6f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(17,35)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertWaterPlants, scale, steepness, terrain, x, y, Random.Range(2.5f,7f)); // Plants
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(40,75)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(2.5f,9f)); // Flowers
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(10,20)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(1.2f,3f)); // Flowers
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(10,25)); i++)
                  {
                    AddBillboardToBatchWater(dfTerrain, dfBillboardBatch, desertWaterFlowers, scale, steepness, terrain, x, y, Random.Range(0.2f,1.5f)); // Flowers
                  }
                }
                else if(rndMajor >= 2)
                {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(2,5)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(1f,3.5f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(4,12)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(1.5f,6f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(10,25)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertWaterPlants, scale, steepness, terrain, x, y, Random.Range(1.5f,7f)); // Plants
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(15,35)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(1.5f,9f)); // Flowers
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(6,12)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(1.2f,3f)); // Flowers
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(5,15)); i++)
                  {
                    AddBillboardToBatchWater(dfTerrain, dfBillboardBatch, desertWaterFlowers, scale, steepness, terrain, x, y, Random.Range(0.2f,1.5f)); // Flowers
                  }
                }
              }
              break;
            #endregion
                        
                        #region Haunted Woodlands Spawns
                        case (int)MapsFile.Climates.HauntedWoodlands:

                            /* if (tile == 1) // Dirt
                            {
                                if(Random.Range(0,100) > mapStyleChance2)
                                {
                                    if(Random.Range(0,100) < Random.Range(95,100))
                                    {
                                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandPlants, scale, steepness, terrain, x, y, 0.25f); // Dead Trees
                                        
                                        for(int i = 0; i < (int)Mathf.Round(Random.Range(0,2)); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandDirtTrees, scale, steepness, terrain, x, y, 1.5f); // Dirt Trees                                      
                                        }
                                    }
                                    else
                                    {
                                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandBones, scale, steepness, terrain, x, y, 1.5f); // Bones 

                                        for(int i = 0; i < (int)Mathf.Round(Random.Range(0,2)); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandPlants, scale, steepness, terrain, x, y, 1.5f); // Plants                                      
                                        }
                                    } 
                                }
                                else
                                {
                                    if(Random.Range(0,100) < Random.Range(50,60))
                                    {
                                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandPlants, scale, steepness, terrain, x, y, 0.25f); // Dead Trees
                                        
                                        for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandDirtTrees, scale, steepness, terrain, x, y, 1.5f); // Dirt Trees                                      
                                        }
                                    }
                                    else
                                    {
                                        for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandPlants, scale, steepness, terrain, x, y, 1.5f); // Plants                                      
                                        }
                                    }
                                }                          
                            }
                            else if (tile == 2) // Grass
                            {   
                                if (Random.Range(0.0f,100.0f) <= 0.075f) // Mushroom Circle
                                {
                                    Vector3 pos = new Vector3(x * scale, 0, (y + 0.5f) * scale);
                                    float height2 = terrain.SampleHeight(pos + terrain.transform.position);
                                    pos.y = height2 - (steepness / slopeSinkRatio);
                                    dfBillboardBatch.AddItem(Random.Range(22,23), pos);
                                    
                                    pos = new Vector3((x + 0.272f) * scale, 0, (y - 0.404f) * scale);
                                    height2 = terrain.SampleHeight(pos + terrain.transform.position);
                                    pos.y = height2 - (steepness / slopeSinkRatio);
                                    dfBillboardBatch.AddItem(Random.Range(22,23), pos);
                                    
                                    pos = new Vector3((x - 0.272f) * scale, 0, (y - 0.404f) * scale);
                                    height2 = terrain.SampleHeight(pos + terrain.transform.position);
                                    pos.y = height2 - (steepness / slopeSinkRatio);
                                    dfBillboardBatch.AddItem(Random.Range(22,23), pos);
                                    
                                    pos = new Vector3((x - 0.475f) * scale, 0, (y + 0.154f) * scale);
                                    height2 = terrain.SampleHeight(pos + terrain.transform.position);
                                    pos.y = height2 - (steepness / slopeSinkRatio);
                                    dfBillboardBatch.AddItem(Random.Range(22,23), pos);

                                    pos = new Vector3((x + 0.475f) * scale, 0, (y + 0.154f) * scale);
                                    height2 = terrain.SampleHeight(pos + terrain.transform.position);
                                    pos.y = height2 - (steepness / slopeSinkRatio);
                                    dfBillboardBatch.AddItem(Random.Range(22,23), pos);
                                }
                                
                                float rndMajor = Random.Range(0,100);
                                if(rndMajor < mapStyleChance)
                                {
                                    if(Random.Range(0,100) < Random.Range(0,30))
                                    {
                                        for(int i = 0; i < (int)Mathf.Round(Random.Range(6,8)); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandTrees, scale, steepness, terrain, x, y, 2.5f); // Trees
                                        }

                                        for(int i = 0; i < (int)Mathf.Round(Random.Range(2,4)); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandDeadTrees, scale, steepness, terrain, x, y, 2.5f); // Dead Trees
                                        }

                                        for(int i = 0; i < (int)Mathf.Round(Random.Range(2,4)); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandDirtTrees, scale, steepness, terrain, x, y, 2.5f); // Dirt Trees                                      
                                        }

                                        for(int i = 0; i < (int)Mathf.Round(Random.Range(5,10)); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandBushes, scale, steepness, terrain, x, y, 2f); // Bushes
                                        }

                                        for(int i = 0; i < (int)Mathf.Round(Random.Range(10,20)); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandBushes, scale, steepness, terrain, x, y, 2.75f); // Bushes
                                        }

                                        if(rndMajor < mapStyleChance2)
                                        {
                                            for(int i = 0; i < (int)Mathf.Round(Random.Range(4,6)); i++)
                                            {
                                                AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandTrees, scale, steepness, terrain, x, y, 2f); // Trees
                                            }

                                            for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                                            {
                                                AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandDeadTrees, scale, steepness, terrain, x, y, 2f); // Dead Trees
                                            }

                                            for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                                            {
                                                AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandDirtTrees, scale, steepness, terrain, x, y, 2f); // Dirt Trees                                      
                                            }
                                        }
                                        
                                        if(rndMajor < mapStyleChance1)
                                        {
                                            for(int i = 0; i < (int)Mathf.Round(Random.Range(2,5)); i++)
                                            {
                                                AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandRocks, scale, steepness, terrain, x, y, 2.5f); // Stones
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if(Random.Range(0,100) < Random.Range(0,30))
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandFlowers, scale, steepness, terrain, x, y, 0.35f); // Flowers

                                            for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                                            {
                                                AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandFlowers, scale, steepness, terrain, x, y, 0.55f); // Flowers
                                            }

                                            for(int i = 0; i < (int)Mathf.Round(Random.Range(0,1)); i++)
                                            {
                                                AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandDeadTrees, scale, steepness, terrain, x, y, 1f); // Dead Trees
                                            }
                                            
                                            if (rndMajor > mapStyleChance2)
                                            {
                                                for(int i = 0; i < (int)Mathf.Round(Random.Range(2,4)); i++)
                                                {
                                                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandMushroom, scale, steepness, terrain, x, y, 0.75f); // Mushrooms
                                                }

                                                if(Random.Range(0, 100) < Random.Range(0,5))
                                                {
                                                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandBones, scale, steepness, terrain, x, y, 0.5f); // Bones

                                                    for(int i = 0; i < (int)Mathf.Round(Random.Range(0,1)); i++)
                                                    {
                                                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandDirtTrees, scale, steepness, terrain, x, y, 1.5f); // Dirt Trees                                      
                                                    }
                                                }
                                            }

                                            if (rndMajor > mapStyleChance1)
                                            {
                                                for(int i = 0; i < (int)Mathf.Round(Random.Range(1,2)); i++)
                                                {
                                                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandMushroom, scale, steepness, terrain, x, y, 0.5f); // Mushrooms
                                                }

                                                for(int i = 0; i < (int)Mathf.Round(Random.Range(2,4)); i++)
                                                {
                                                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandBushes, scale, steepness, terrain, x, y, 0.75f); // Bushes
                                                }

                                                for(int i = 0; i < (int)Mathf.Round(Random.Range(0,1)); i++)
                                                {
                                                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandDeadTrees, scale, steepness, terrain, x, y, 1.5f); // Dead Trees
                                                }

                                                if(Random.Range(0, 100) < Random.Range(0,5))
                                                {
                                                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandBones, scale, steepness, terrain, x, y, 0.5f); // Bones
                                                }

                                                for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                                                {
                                                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandRocks, scale, steepness, terrain, x, y, 1.5f); // Stones
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if(Random.Range(0,100) < Random.Range(0,70))
                                    {
                                        for(int i = 0; i < (int)Mathf.Round(Random.Range(6,8)); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandTrees, scale, steepness, terrain, x, y, 2.5f); // Trees
                                        }

                                        for(int i = 0; i < (int)Mathf.Round(Random.Range(2,4)); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandDeadTrees, scale, steepness, terrain, x, y, 2.5f); // Dead Trees
                                        }

                                        for(int i = 0; i < (int)Mathf.Round(Random.Range(2,4)); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandDirtTrees, scale, steepness, terrain, x, y, 2.5f); // Dirt Trees                                      
                                        }

                                        for(int i = 0; i < (int)Mathf.Round(Random.Range(5,10)); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandBushes, scale, steepness, terrain, x, y, 2f); // Bushes
                                        }

                                        for(int i = 0; i < (int)Mathf.Round(Random.Range(10,20)); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandBushes, scale, steepness, terrain, x, y, 2.75f); // Bushes
                                        }

                                        if(rndMajor < mapStyleChance3)
                                        {
                                            for(int i = 0; i < (int)Mathf.Round(Random.Range(4,6)); i++)
                                            {
                                                AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandTrees, scale, steepness, terrain, x, y, 2f); // Trees
                                            }

                                            for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                                            {
                                                AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandDeadTrees, scale, steepness, terrain, x, y, 2f); // Dead Trees
                                            }

                                            for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                                            {
                                                AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandDirtTrees, scale, steepness, terrain, x, y, 2f); // Dirt Trees                                      
                                            }
                                        }
                                        
                                        if(rndMajor < mapStyleChance4)
                                        {
                                            for(int i = 0; i < (int)Mathf.Round(Random.Range(2,5)); i++)
                                            {
                                                AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandRocks, scale, steepness, terrain, x, y, 2.5f); // Stones
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if(Random.Range(0,100) < Random.Range(0,30))
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandFlowers, scale, steepness, terrain, x, y, 0.35f); // Flowers

                                            for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                                            {
                                                AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandFlowers, scale, steepness, terrain, x, y, 0.55f); // Flowers
                                            }

                                            for(int i = 0; i < (int)Mathf.Round(Random.Range(0,1)); i++)
                                            {
                                                AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandDeadTrees, scale, steepness, terrain, x, y, 1f); // Dead Trees
                                            }
                                            
                                            if (rndMajor > mapStyleChance3)
                                            {
                                                for(int i = 0; i < (int)Mathf.Round(Random.Range(2,4)); i++)
                                                {
                                                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandMushroom, scale, steepness, terrain, x, y, 0.75f); // Mushrooms
                                                }

                                                if(Random.Range(0, 100) < Random.Range(0,5))
                                                {
                                                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandBones, scale, steepness, terrain, x, y, 0.5f); // Bones

                                                    for(int i = 0; i < (int)Mathf.Round(Random.Range(0,1)); i++)
                                                    {
                                                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandDirtTrees, scale, steepness, terrain, x, y, 1.5f); // Dirt Trees                                      
                                                    }
                                                }
                                            }

                                            if (rndMajor > mapStyleChance4)
                                            {
                                                for(int i = 0; i < (int)Mathf.Round(Random.Range(1,2)); i++)
                                                {
                                                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandMushroom, scale, steepness, terrain, x, y, 0.5f); // Mushrooms
                                                }

                                                for(int i = 0; i < (int)Mathf.Round(Random.Range(2,4)); i++)
                                                {
                                                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandBushes, scale, steepness, terrain, x, y, 0.75f); // Bushes
                                                }

                                                for(int i = 0; i < (int)Mathf.Round(Random.Range(0,1)); i++)
                                                {
                                                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandDeadTrees, scale, steepness, terrain, x, y, 1.5f); // Dead Trees
                                                }

                                                if(Random.Range(0, 100) < Random.Range(0,5))
                                                {
                                                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandBones, scale, steepness, terrain, x, y, 0.5f); // Bones
                                                }

                                                for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                                                {
                                                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandRocks, scale, steepness, terrain, x, y, 1.5f); // Stones
                                                }
                                            }
                                        }
                                    }                                        
                                }              
                            }
                            else if (tile == 3) // Stone
                            {
                                if(Random.Range(0,100) > mapStyleChance)
                                {
                                    if(Random.Range(0,100) < Random.Range(10,20))
                                    {
                                        if(Random.Range(0,100) < Random.Range(95,100))
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandDeadTrees, scale, steepness, terrain, x, y, 0.25f); // Dead Trees
                                        }
                                        else
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandBones, scale, steepness, terrain, x, y, 1.5f); // Bones 
                                        }  
                                    }
                                    else
                                    {
                                        for(int i = 0; i < (int)Mathf.Round(Random.Range(0,2)); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandRocks, scale, steepness, terrain, x, y, 1.5f); // Stones
                                        }
                                    }
                                }
                                else
                                {
                                    if(Random.Range(0,100) < Random.Range(10,20))
                                    {
                                        if(Random.Range(0,100) < Random.Range(95,100))
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandDeadTrees, scale, steepness, terrain, x, y, 0.25f); // Dead Trees
                                        }
                                        else
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandBones, scale, steepness, terrain, x, y, 1.5f); // Bones 
                                        }  
                                    }
                                    else
                                    {
                                        for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandRocks, scale, steepness, terrain, x, y, 1.5f); // Stones
                                        }
                                    }
                                }
                            } */
                            break;
                        #endregion

                        #region Woodland Hills Spawns
                        case (int)MapsFile.Climates.MountainWoods:

                        /* if (tile == 1) // Dirt
                            {
                                if(dfTerrain.MapData.heightmapSamples[hy, hx] * maxTerrainHeight < beachLine)
                                {
                                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsBeach, scale, steepness, terrain, x, y, 0.25f); // Beach

                                    for(int i = 0; i < (int)Mathf.Round(Random.Range(0,2)); i++)
                                    {
                                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsBeach, scale, steepness, terrain, x, y, 1.5f); // Beach                                      
                                    }
                                }
                                else if(Random.Range(0,100) > mapStyleChance2)
                                {
                                    if(Random.Range(0,100) < Random.Range(95,100))
                                    {
                                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsDirtPlants, scale, steepness, terrain, x, y, 0.75f); // Dirt Plants
                                        
                                        for(int i = 0; i < (int)Mathf.Round(Random.Range(0,2)); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsTrees, scale, steepness, terrain, x, y, 1.5f); // Needle Trees                                      
                                        }
                                    }
                                    else
                                    {
                                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsDeadTrees, scale, steepness, terrain, x, y, 2f); // Dead Tree 

                                        for(int i = 0; i < (int)Mathf.Round(Random.Range(0,2)); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsBushes, scale, steepness, terrain, x, y, 1.5f); // Bushes                                      
                                        }
                                    } 
                                }
                                else
                                {
                                    if(Random.Range(0,100) < Random.Range(50,60))
                                    {
                                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsDirtPlants, scale, steepness, terrain, x, y, 0.25f); // Dirt Plants
                                        
                                        for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsNeedleTrees, scale, steepness, terrain, x, y, 1.5f); // Needle Trees                                      
                                        }
                                    }
                                    else
                                    {
                                        for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsDirtPlants, scale, steepness, terrain, x, y, 1.5f); // Dirt Plants                                      
                                        }
                                    }
                                }                          
                            }
                            else if (tile == 2) // Grass
                            {                                   
                                float rndMajor = Random.Range(0,100);
                                if(rndMajor < mapStyleChance)
                                {
                                    if(Random.Range(0,100) < Random.Range(0,30))
                                    {
                                        for(int i = 0; i < (int)Mathf.Round(Random.Range(5,8)); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsTrees, scale, steepness, terrain, x, y, Random.Range(2.5f,3.5f)); // Trees
                                        }

                                        for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsDeadTrees, scale, steepness, terrain, x, y, Random.Range(2.5f,3.5f)); // Dead Trees
                                        }

                                        if(terrainElevation > 0.3f)
                                        {
                                            for(int i = 0; i < (int)Mathf.Round(Random.Range(2,4)); i++)
                                            {
                                                AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsNeedleTrees, scale, steepness, terrain, x, y, Random.Range(3f,4f)); // Needle Trees                                      
                                            }
                                        }
                                        else
                                        {
                                            for(int i = 0; i < (int)Mathf.Round(Random.Range(2,4)); i++)
                                            {
                                                AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsTrees, scale, steepness, terrain, x, y, Random.Range(3f,4f)); // Trees                                      
                                            }
                                        }

                                        for(int i = 0; i < (int)Mathf.Round(Random.Range(5,10)); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsBushes, scale, steepness, terrain, x, y, Random.Range(2f,2.5f)); // Bushes
                                        }

                                        for(int i = 0; i < (int)Mathf.Round(Random.Range(10,20)); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsBushes, scale, steepness, terrain, x, y, Random.Range(2.5f,3.5f)); // Bushes
                                        }

                                        if(rndMajor < mapStyleChance2)
                                        {
                                            for(int i = 0; i < (int)Mathf.Round(Random.Range(3,5)); i++)
                                            {
                                                AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsTrees, scale, steepness, terrain, x, y, Random.Range(2.5f,3f)); // Trees
                                            }

                                            if(terrainElevation > 0.28f)
                                            {
                                                for(int i = 0; i < (int)Mathf.Round(Random.Range(1,2)); i++)
                                                {
                                                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsDeadTrees, scale, steepness, terrain, x, y, Random.Range(2.5f,3f)); // Dead Trees                                      
                                                }
                                            }
                                            else
                                            {
                                                for(int i = 0; i < (int)Mathf.Round(Random.Range(1,2)); i++)
                                                {
                                                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsTrees, scale, steepness, terrain, x, y, Random.Range(2.5f,3f)); // Trees                                      
                                                }
                                            }

                                            for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                                            {
                                                AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsNeedleTrees, scale, steepness, terrain, x, y, Random.Range(2.5f,3f)); // Needle Trees                                      
                                            }
                                        }
                                        
                                        if(rndMajor < mapStyleChance1)
                                        {
                                            for(int i = 0; i < (int)Mathf.Round(Random.Range(2,5)); i++)
                                            {
                                                AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsRocks, scale, steepness, terrain, x, y, Random.Range(2.5f,3.5f)); // Stones
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if(rndMajor < mapStyleChance)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsTrees, scale, steepness, terrain, x, y, 0f);  // Trees
                                            
                                            for(int i = 0; i < (int)Mathf.Round(Random.Range(0,3)); i++)
                                            {
                                                AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsTrees, scale, steepness, terrain, x, y, Random.Range(2.5f,3f)); // Trees
                                            }

                                            for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                                            {
                                                AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsBushes, scale, steepness, terrain, x, y, Random.Range(2.25f,2.75f)); // Bushes
                                            }
                                            if(rndMajor < mapStyleChance1)
                                            {
                                                for(int i = 0; i < (int)Mathf.Round(Random.Range(1,2)); i++)
                                                {
                                                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsTrees, scale, steepness, terrain, x, y, Random.Range(2.75f,3.25f)); // Trees
                                                }
                                            }
                                            
                                            if(rndMajor < mapStyleChance0)
                                            {
                                                for(int i = 0; i < (int)Mathf.Round(Random.Range(0,3)); i++)
                                                {
                                                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsTrees, scale, steepness, terrain, x, y, Random.Range(2.75f,3.25f)); // Trees
                                                }
                                            }
                                        }
                                        else
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsFlowers, scale, steepness, terrain, x, y, 0.35f); // Flowers
                                            if (rndMajor > mapStyleChance4)
                                            {
                                                for(int i = 0; i < (int)Mathf.Round(Random.Range(3,8)); i++)
                                                {
                                                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsFlowers, scale, steepness, terrain, x, y, Random.Range(0.35f,0.55f)); // Flowers
                                                }
                                            }

                                            if (rndMajor > mapStyleChance3)
                                            {
                                                for(int i = 0; i < (int)Mathf.Round(Random.Range(3,8)); i++)
                                                {
                                                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsFlowers, scale, steepness, terrain, x, y, Random.Range(0.9f,1.3f)); // Flowers
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if(Random.Range(0,100) < Random.Range(0,70))
                                    {
                                        for(int i = 0; i < (int)Mathf.Round(Random.Range(3,5)); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsTrees, scale, steepness, terrain, x, y, Random.Range(2.25f,2.75f)); // Trees
                                        }

                                        for(int i = 0; i < (int)Mathf.Round(Random.Range(0,2)); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsTrees, scale, steepness, terrain, x, y, Random.Range(2.25f,2.75f)); // Trees                                      
                                        }

                                        for(int i = 0; i < (int)Mathf.Round(Random.Range(3,5)); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsBushes, scale, steepness, terrain, x, y, Random.Range(1.75f,2.25f)); // Bushes
                                        }

                                        for(int i = 0; i < (int)Mathf.Round(Random.Range(6,8)); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsBushes, scale, steepness, terrain, x, y, Random.Range(2.25f,2.75f)); // Bushes
                                        }

                                        if(rndMajor < mapStyleChance3)
                                        {
                                            for(int i = 0; i < (int)Mathf.Round(Random.Range(4,6)); i++)
                                            {
                                                AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsTrees, scale, steepness, terrain, x, y, Random.Range(2.25f,3.75f)); // Trees
                                            }

                                            for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                                            {
                                                AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsTrees, scale, steepness, terrain, x, y, Random.Range(2.75f,3.25f)); // Trees
                                            }

                                        }
                                        
                                        if(rndMajor < mapStyleChance4)
                                        {
                                            for(int i = 0; i < (int)Mathf.Round(Random.Range(2,5)); i++)
                                            {
                                                AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsRocks, scale, steepness, terrain, x, y, Random.Range(1.75f,2.25f)); // Stones
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if(Random.Range(0,100) < Random.Range(0,50))
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsFlowers, scale, steepness, terrain, x, y, 0.35f); // Flowers

                                            for(int i = 0; i < (int)Mathf.Round(Random.Range(1,5)); i++)
                                            {
                                                AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsFlowers, scale, steepness, terrain, x, y, Random.Range(0.55f, 0.75f)); // Flowers
                                            }

                                            if (rndMajor > mapStyleChance4)
                                            {
                                                for(int i = 0; i < (int)Mathf.Round(Random.Range(3,8)); i++)
                                                {
                                                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsFlowers, scale, steepness, terrain, x, y, Random.Range(1.25f, 1.75f)); // Flowers
                                                }
                                            }

                                            if (rndMajor > mapStyleChance3)
                                            {
                                                for(int i = 0; i < (int)Mathf.Round(Random.Range(3,8)); i++)
                                                {
                                                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsFlowers, scale, steepness, terrain, x, y, Random.Range(1.25f, 1.8f)); // Flowers
                                                }
                                            }

                                            for(int i = 0; i < (int)Mathf.Round(Random.Range(0,1)); i++)
                                            {
                                                AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsTrees, scale, steepness, terrain, x, y, 1f); // Trees
                                            }
                                            
                                            if (rndMajor > mapStyleChance3)
                                            {
                                                for(int i = 0; i < (int)Mathf.Round(Random.Range(2,4)); i++)
                                                {
                                                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsFlowers, scale, steepness, terrain, x, y, Random.Range(1f,1.5f)); // Flowers
                                                }

                                                if(Random.Range(0, 100) < Random.Range(0,5))
                                                {
                                                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsFlowers, scale, steepness, terrain, x, y, Random.Range(1.35f, 1.65f)); // Flowers

                                                    for(int i = 0; i < (int)Mathf.Round(Random.Range(0,1)); i++)
                                                    {
                                                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsTrees, scale, steepness, terrain, x, y, 1f); // Trees                                      
                                                    }
                                                }
                                            }

                                            if (rndMajor > mapStyleChance4)
                                            {
                                                for(int i = 0; i < (int)Mathf.Round(Random.Range(1,5)); i++)
                                                {
                                                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsFlowers, scale, steepness, terrain, x, y, Random.Range(0.75f, 1.25f)); // Flowers
                                                }

                                                for(int i = 0; i < (int)Mathf.Round(Random.Range(0,1)); i++)
                                                {
                                                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsTrees, scale, steepness, terrain, x, y, 0f); // Trees
                                                }

                                                if(Random.Range(0, 100) < Random.Range(0,5))
                                                {
                                                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsFlowers, scale, steepness, terrain, x, y, Random.Range(1.25f, 1.75f)); // Flowers
                                                }
                                            }
                                        }
                                    }                                        
                                }              
                            }
                            else if (tile == 3) // Stone
                            {
                                if(Random.Range(0,100) > mapStyleChance)
                                {
                                    if(Random.Range(0,100) < Random.Range(10,20))
                                    {
                                        if(Random.Range(0,100) < Random.Range(85,100))
                                        {
                                            if(terrainElevation > 0.28f)
                                            {
                                                AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsNeedleTrees, scale, steepness, terrain, x, y, 1.5f); // Needle Trees                                      
                                            }
                                            else
                                            {
                                                AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsTrees, scale, steepness, terrain, x, y, 1.5f); // Trees                                      
                                            }
                                        }
                                        else
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsRocks, scale, steepness, terrain, x, y, 1.5f); // Rocks 
                                        }  
                                    }
                                    else
                                    {
                                        for(int i = 0; i < (int)Mathf.Round(Random.Range(0,2)); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsRocks, scale, steepness, terrain, x, y, 1.5f); // Rocks
                                        }
                                    }
                                }
                                else
                                {
                                    if(Random.Range(0,100) < Random.Range(10,20))
                                    {
                                        if(Random.Range(0,100) < Random.Range(85,100))
                                        {
                                            if(terrainElevation > 0.28f)
                                            {
                                                AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsNeedleTrees, scale, steepness, terrain, x, y, 1.5f); // Needle Trees                                      
                                            }
                                            else
                                            {
                                                AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsTrees, scale, steepness, terrain, x, y, 1.5f); // Trees                                      
                                            }
                                        }
                                        else
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsRocks, scale, steepness, terrain, x, y, 1.5f); // Rocks 
                                        }  
                                    }
                                    else
                                    {
                                        if(terrainElevation > 0.28f)
                                        {
                                            for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                                            {
                                                AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsNeedleTrees, scale, steepness, terrain, x, y, Random.Range(1.5f, 1.75f)); // Needle Trees
                                            }
                                        }
                                        else
                                        {
                                            for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                                            {
                                                AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsNeedleTrees, scale, steepness, terrain, x, y, Random.Range(1.5f, 1.75f)); // Needle Trees
                                            }
                                        }
                                    }
                                }
                            } */
                            break;
                        #endregion

                        #region Rainforest Spawns
                        case (int)MapsFile.Climates.Rainforest:

                        /* if (tile == 1) // Dirt
                        {
                            if(Random.Range(0,100) < Random.Range(25,75))
                            {
                                if(Random.Range(0,100) < mapStyleChance)
                                {
                                    for(int i = 0; i < Random.Range(1,3); i++)
                                    {
                                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestTrees, scale, steepness, terrain, x, y, 1f); // Trees                                      
                                    }

                                    if(Random.Range(0,100) < mapStyleChance2)
                                    {
                                        for(int i = 0; i < Random.Range(0,6); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestBushes, scale, steepness, terrain, x, y, 1.5f); // Bushes                                      
                                        }
                                    }

                                    if(Random.Range(0,100) < mapStyleChance1)
                                    {
                                        for(int i = 0; i < Random.Range(0,5); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestFlowers, scale, steepness, terrain, x, y, 2f); // Flowers                                     
                                        }
                                    }

                                    if(Random.Range(0,100) < mapStyleChance0)
                                    {
                                        for(int i = 0; i < Random.Range(0,3); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestEggs, scale, steepness, terrain, x, y, 0.25f); // Eggs                                     
                                        }
                                    }
                                }
                                else
                                {
                                    for(int i = 0; i < Random.Range(1,3); i++)
                                    {
                                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestTrees, scale, steepness, terrain, x, y, 1f); // Trees                                      
                                    }

                                    if(Random.Range(0,100) < mapStyleChance0)
                                    {
                                        for(int i = 0; i < Random.Range(0,6); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestBushes, scale, steepness, terrain, x, y, 1.5f); // Bushes                                      
                                        }
                                    }

                                    if(Random.Range(0,100) < mapStyleChance2)
                                    {
                                        for(int i = 0; i < Random.Range(0,5); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestFlowers, scale, steepness, terrain, x, y, 2f); // Flowers                                     
                                        }
                                    }

                                    if(Random.Range(0,100) < mapStyleChance1)
                                    {
                                        for(int i = 0; i < Random.Range(0,3); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestEggs, scale, steepness, terrain, x, y, 0.25f); // Eggs                                     
                                        }
                                    }
                                }
                            }
                        }
                        else if (tile == 2) // Grass
                        {
                            if(Random.Range(0,100) < Random.Range(25,75))
                            {
                                if(Random.Range(0,100) < mapStyleChance)
                                {
                                    for(int i = 0; i < Random.Range(4,6); i++)
                                    {
                                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestTrees, scale, steepness, terrain, x, y, 1.5f); // Trees                                      
                                    }
                                    
                                    if(Random.Range(0,100) < mapStyleChance2)
                                    {
                                        for(int i = 0; i < Random.Range(0,6); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestPlants, scale, steepness, terrain, x, y, 1.5f); // Plants                                      
                                        }
                                    }
                                    
                                    if(Random.Range(0,100) < mapStyleChance1)
                                    {
                                        for(int i = 0; i < Random.Range(0,6); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestBushes, scale, steepness, terrain, x, y, 1.5f); // Bushes                                      
                                        }
                                    }

                                    if(Random.Range(0,100) < mapStyleChance0)
                                    {
                                        for(int i = 0; i < Random.Range(0,5); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestFlowers, scale, steepness, terrain, x, y, 2f); // Flowers                                     
                                        }
                                    }

                                    if(Random.Range(0,100) < mapStyleChance0)
                                    {
                                        for(int i = 0; i < Random.Range(0,4); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestRocks, scale, steepness, terrain, x, y, 2f); // Rocks                                     
                                        }
                                    }
                                }
                                else
                                {
                                    for(int i = 0; i < Random.Range(4,6); i++)
                                    {
                                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestTrees, scale, steepness, terrain, x, y, 1.5f); // Trees                                      
                                    }
                                    
                                    if(Random.Range(0,100) < mapStyleChance4)
                                    {
                                        for(int i = 0; i < Random.Range(0,6); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestPlants, scale, steepness, terrain, x, y, 1.5f); // Plants                                      
                                        }
                                    }
                                    
                                    if(Random.Range(0,100) < mapStyleChance3)
                                    {
                                        for(int i = 0; i < Random.Range(0,6); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestBushes, scale, steepness, terrain, x, y, 1.5f); // Bushes                                      
                                        }
                                    }

                                    if(Random.Range(0,100) < mapStyleChance2)
                                    {
                                        for(int i = 0; i < Random.Range(0,5); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestFlowers, scale, steepness, terrain, x, y, 2f); // Flowers                                     
                                        }
                                    }

                                    if(Random.Range(0,100) < mapStyleChance1)
                                    {
                                        for(int i = 0; i < Random.Range(0,4); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestRocks, scale, steepness, terrain, x, y, 2f); // Rocks                                     
                                        }
                                    }
                                }
                            }
                        }
                        else if (tile == 3) // Stone
                        {
                            if(Random.Range(0,100) < Random.Range(25,75))
                            {
                                if(Random.Range(0,100) < mapStyleChance)
                                {
                                    for(int i = 0; i < Random.Range(1,3); i++)
                                    {
                                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestTrees, scale, steepness, terrain, x, y, 1f); // Trees                                      
                                    }
                                    
                                    if(Random.Range(0,100) < mapStyleChance0)
                                    {
                                        for(int i = 0; i < Random.Range(0,6); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestPlants, scale, steepness, terrain, x, y, 1.5f); // Plants                                      
                                        }
                                    }
                                    
                                    if(Random.Range(0,100) < mapStyleChance0)
                                    {
                                        for(int i = 0; i < Random.Range(0,6); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestBushes, scale, steepness, terrain, x, y, 1.5f); // Bushes                                      
                                        }
                                    }

                                    if(Random.Range(0,100) < mapStyleChance1)
                                    {
                                        for(int i = 0; i < Random.Range(0,5); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestFlowers, scale, steepness, terrain, x, y, 2f); // Flowers                                     
                                        }
                                    }

                                    if(Random.Range(0,100) < mapStyleChance2)
                                    {
                                        for(int i = 0; i < Random.Range(0,4); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestRocks, scale, steepness, terrain, x, y, 2f); // Rocks                                     
                                        }
                                    }
                                }
                                else
                                {
                                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestTrees, scale, steepness, terrain, x, y, 1f); // Trees
                                    
                                    if(Random.Range(0,100) < mapStyleChance2)
                                    {
                                        for(int i = 0; i < Random.Range(0,6); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestPlants, scale, steepness, terrain, x, y, 1.5f); // Plants                                      
                                        }
                                    }
                                    
                                    if(Random.Range(0,100) < mapStyleChance3)
                                    {
                                        for(int i = 0; i < Random.Range(0,6); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestBushes, scale, steepness, terrain, x, y, 1.5f); // Bushes                                      
                                        }
                                    }

                                    if(Random.Range(0,100) < mapStyleChance4)
                                    {
                                        for(int i = 0; i < Random.Range(0,5); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestFlowers, scale, steepness, terrain, x, y, 2f); // Flowers                                     
                                        }
                                    }

                                    if(Random.Range(0,100) < mapStyleChance5)
                                    {
                                        for(int i = 0; i < Random.Range(0,4); i++)
                                        {
                                            AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestRocks, scale, steepness, terrain, x, y, 2f); // Rocks                                     
                                        }
                                    }
                                }
                            }
                        } */
                        break;
                        #endregion

            #region Subtropical Spawns
            case (int)MapsFile.Climates.Subtropical:

            if (tile == 1) // Dirt
            {
                
            }
            else if (tile == 2) // Grass
            {
                
            }
            else if (tile == 3) // Stone
            {
                
            }
            break;
            #endregion

            #region Swamp Spawns
            case (int)MapsFile.Climates.Swamp:

            if (tile == 1) // Dirt
            {
                
            }
            else if (tile == 2) // Grass
            {
                
            }
            else if (tile == 3) // Stone
            {
                
            }
            break;
            #endregion
          }
        }
      }

      // Apply new batch
      dfBillboardBatch.Apply();
    }

    public static void AddBillboardToBatch(
      DaggerfallTerrain dfTerrain,
      DaggerfallBillboardBatch dfBillboardBatch,
      List<int> billboardCollection,
      float scale,
      float steepness,
      Terrain terrain,
      int x,
      int y,
      float posVariance)
    {
      int rnd = (int)Mathf.Round(Random.Range(0, billboardCollection.Count));
      Vector3 pos = new Vector3((x + Random.Range(-posVariance, posVariance)) * scale, 0, (y + Random.Range(-posVariance, posVariance)) * scale);
      float height = terrain.SampleHeight(pos + terrain.transform.position);
      pos.y = height - (steepness / slopeSinkRatio);

      if(!IsOnAnyWaterTile(dfTerrain, pos, scale) && !IsCollidingWithBuilding(dfTerrain, pos, scale) && !IsOnOrCloseToStreetTile(dfTerrain, pos, scale))
      {
        dfBillboardBatch.AddItem(billboardCollection[rnd], pos);   
      }
    }

    public static void AddBillboardToBatch(
      DaggerfallTerrain dfTerrain,
      DaggerfallBillboardBatch dfBillboardBatch,
      List<int> billboardCollection,
      float scale,
      float steepness,
      Terrain terrain,
      int x,
      int y,
      float posVariance,
      int record)
    {
      Vector3 pos = new Vector3((x + Random.Range(-posVariance, posVariance)) * scale, 0, (y + Random.Range(-posVariance, posVariance)) * scale);
      float height = terrain.SampleHeight(pos + terrain.transform.position);
      pos.y = height - (steepness / slopeSinkRatio);

      if(!IsOnAnyWaterTile(dfTerrain, pos, scale) && !IsCollidingWithBuilding(dfTerrain, pos, scale) && !IsOnOrCloseToStreetTile(dfTerrain, pos, scale))
      {
        dfBillboardBatch.AddItem(billboardCollection[record], pos);
      }
    }

    public static void AddBillboardToBatchWater(
      DaggerfallTerrain dfTerrain,
      DaggerfallBillboardBatch dfBillboardBatch,
      List<int> billboardCollection,
      float scale,
      float steepness,
      Terrain terrain,
      int x,
      int y,
      float posVariance)
    {
      int rnd = (int)Mathf.Round(Random.Range(0, billboardCollection.Count));
      Vector3 pos = new Vector3((x + Random.Range(-posVariance, posVariance)) * scale, 0, (y + Random.Range(-posVariance, posVariance)) * scale);
      float height = terrain.SampleHeight(pos + terrain.transform.position);
      pos.y = height - (steepness / slopeSinkRatio);

      if(IsOnOrCloseToShallowWaterTile(dfTerrain, pos, scale) && !IsCollidingWithBuilding(dfTerrain, pos, scale) && !IsOnOrCloseToStreetTile(dfTerrain, pos, scale))
      {
        dfBillboardBatch.AddItem(billboardCollection[rnd], pos);
      }
    }

    static public bool IsOnAnyWaterTile(DaggerfallTerrain dfTerrain, Vector3 pos, float scale)
    {
      bool result = true;
      int roundedX = (int)Mathf.Round(pos.x/scale);
      int roundedY = (int)Mathf.Round(pos.z/scale);
      
      if(ExtensionMethods.In2DArrayBounds(dfTerrain.MapData.tilemapSamples, roundedX, roundedY))
      {
        int sampleGround = dfTerrain.MapData.tilemapSamples[roundedX, roundedY] & 0x3F;
        if(
          sampleGround != 0 &&
          sampleGround != 4 &&
          sampleGround != 5 &&
          sampleGround != 6 &&
          sampleGround != 7 &&
          sampleGround != 8 &&
          sampleGround != 19 &&
          sampleGround != 20 &&
          sampleGround != 21 &&
          sampleGround != 22 &&
          sampleGround != 23 &&
          sampleGround != 29 &&
          sampleGround != 30 &&
          sampleGround != 31 &&
          sampleGround != 32 &&
          sampleGround != 33 &&
          sampleGround != 34 &&
          sampleGround != 35 &&
          sampleGround != 36 &&
          sampleGround != 37 &&
          sampleGround != 38 &&
          sampleGround != 40 &&
          sampleGround != 41 &&
          sampleGround != 43 &&
          sampleGround != 44 &&
          sampleGround != 48 &&
          sampleGround != 49 &&
          sampleGround != 50)
        {
          result = false;
        }
      }
      return result;
    }

    static public bool IsOnOrCloseToShallowWaterTile(DaggerfallTerrain dfTerrain, Vector3 pos, float scale)
    {
      bool result = true;
      bool stopCondition = false;

      float offsetA = 1f;
      float offsetB = 1f;

      int roundedX;
      int roundedY;

      for(int x = 0; x < 2 && stopCondition == false; x++)
      {
        for(int y = 0; y < 2 && stopCondition == false; y++)
        {
            
          if(x == 1)
            roundedX = (int)Mathf.Round((pos.x/scale) + offsetA);
          else
            roundedX = (int)Mathf.Round((pos.x/scale) + offsetB);
          if(y == 1)
            roundedY = (int)Mathf.Round((pos.z/scale) + offsetA);
          else
            roundedY = (int)Mathf.Round((pos.z/scale) + offsetB);

          if(ExtensionMethods.In2DArrayBounds(dfTerrain.MapData.tilemapSamples, roundedX - x, roundedY - y))
          {
            int sampleGround = dfTerrain.MapData.tilemapSamples[roundedX - x, roundedY - y] & 0x3F;
            if(
              sampleGround != 4 &&
              sampleGround != 5 &&
              sampleGround != 6 &&
              sampleGround != 7 &&
              sampleGround != 8 &&
              sampleGround != 19 &&
              sampleGround != 20 &&
              sampleGround != 21 &&
              sampleGround != 22 &&
              sampleGround != 23 &&
              sampleGround != 29 &&
              sampleGround != 30 &&
              sampleGround != 31 &&
              sampleGround != 32 &&
              sampleGround != 33 &&
              sampleGround != 34 &&
              sampleGround != 35 &&
              sampleGround != 36 &&
              sampleGround != 37 &&
              sampleGround != 38 &&
              sampleGround != 40 &&
              sampleGround != 41 &&
              sampleGround != 43 &&
              sampleGround != 44 &&
              sampleGround != 48 &&
              sampleGround != 49 &&
              sampleGround != 50)
            {
              stopCondition = result = false;
            }
            else
              stopCondition = result = true;
          }
        }
      }
      return result;   
    }

    static public bool IsOnOrCloseToStreetTile(DaggerfallTerrain dfTerrain, Vector3 pos, float scale)
    {
      bool result = true;
      bool stopCondition = false;

      float offsetA = 0.7f;
      float offsetB = -0.3f;

      int roundedX;
      int roundedY;

      for(int x = 0; x < 2 && stopCondition == false; x++)
      {
        for(int y = 0; y < 2 && stopCondition == false; y++)
        {   
          if(x == 1)
            roundedX = (int)Mathf.Round((pos.x/scale) + offsetA);
          else
            roundedX = (int)Mathf.Round((pos.x/scale) + offsetB);
          if(y == 1)
            roundedY = (int)Mathf.Round((pos.z/scale) + offsetA);
          else
            roundedY = (int)Mathf.Round((pos.z/scale) + offsetB);

          if(ExtensionMethods.In2DArrayBounds(dfTerrain.MapData.tilemapSamples, roundedX - x, roundedY - y))
          {
            int sampleGround = dfTerrain.MapData.tilemapSamples[roundedX - x, roundedY - y] & 0x3F;
            if(
              sampleGround != 46 &&
              sampleGround != 47 &&
              sampleGround != 55)
            {
              stopCondition = result = false;
            }
            else
              stopCondition = result = true;
          }
        }
      }
      return result;   
    }

    static public bool IsCollidingWithBuilding(DaggerfallTerrain dfTerrain, Vector3 pos, float scale)
    {
      /* if(dfTerrain.MapData.hasLocation)
      {
        foreach(GameObject go in staticGeometryList)
        {
          Vector3 newPos = new Vector3(pos.x,pos.y + 100, pos.z);
          RaycastHit hit;

          if (Physics.Raycast(dfTerrain.transform.TransformPoint(newPos), new Vector3(0,-1,0), out hit, Mathf.Infinity))
          {
            Debug.DrawLine(dfTerrain.transform.TransformPoint(newPos), hit.point, Color.red,30);

            if(hit.collider.gameObject.tag == "StaticGeometry")
            {
              Debug.DrawLine(dfTerrain.transform.TransformPoint(newPos), new Vector3(dfTerrain.transform.TransformPoint(newPos).x,-200,dfTerrain.transform.TransformPoint(newPos).z), Color.yellow,30);
              return true;
            }
          }
        }
      } */
      return false;
    }

    /* static private void AddDescendantsWithTag(Transform parent, string tag, List<GameObject> list)
    {
      foreach (Transform child in parent)
      {
        if (child.gameObject.CompareTag(tag))
        {
          list.Add(child.gameObject);
        }
        AddDescendantsWithTag(child, tag, list);
      }
    } */

    // Noise function
    private float GetNoise(
      float x,
      float y,
      float frequency,
      float amplitude,
      float persistance,
      int octaves,
      int seed = 120)
    {
      float finalValue = 0f;
      for (int i = 0; i < octaves; ++i)
      {
        finalValue += Mathf.PerlinNoise(seed + (x * frequency), seed + (y * frequency)) * amplitude;
        frequency *= 2.0f;
        amplitude *= persistance;
      }

      return Mathf.Clamp(finalValue, -1, 1);
    }

    // Sets texture by range
    private string GetWeightedRecord(float weight, float limit1 = 0.3f, float limit2 = 0.6f)
    {
      if (weight < limit1)
        return "flower";
      else if (weight >= limit1 && weight < limit2)
        return "grass";
      else
        return "forest";
    }
  }

  public static class ExtensionMethods
  {
    public static bool In2DArrayBounds(this byte[,] array, int x, int y)
    {
      if (
        x < array.GetLowerBound(0) ||
        x > array.GetUpperBound(0) ||
        y < array.GetLowerBound(1) ||
        y > array.GetUpperBound(1))
      {
        return false;
      }
      return true;
    }
  }
}