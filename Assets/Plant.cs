using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : WorldObject
{
    private int timeToGrow = 120;
    private int growStage = 0;

    private void Awake()
    {
        WorldBlossom.TimeTick += Grow;
    }

    private void OnDestroy()
    {
        WorldBlossom.TimeTick -= Grow;
    }

    public void Grow()
    {
        growStage++;
        if(growStage > timeToGrow)
        {
            TryPlaceEnergy();
        }
    }

    private void TryPlaceEnergy()
    {
        var pos = Position + Dir.dir[Direction.Left];
        if(Data.CanPlaceObject(pos))
        {
            var result = Data.SetTile(pos, ObjectType.Energy);
            Data.AddToWorldObjects(pos, result.GetComponent<ICollidable>(), ObjectType.Energy);
            growStage = 0;
            return;
        }

        pos = Position + Dir.dir[Direction.Up];
        if(Data.CanPlaceObject(pos))
        {
            var result = Data.SetTile(pos, ObjectType.Energy);
            Data.AddToWorldObjects(pos, result.GetComponent<ICollidable>(), ObjectType.Energy);
            growStage = 0;
            return;
        }

        pos = Position + Dir.dir[Direction.Right];
        if(Data.CanPlaceObject(pos))
        {
            var result = Data.SetTile(pos, ObjectType.Energy);
            Data.AddToWorldObjects(pos, result.GetComponent<ICollidable>(), ObjectType.Energy);
            growStage = 0;
            return;
        }

        pos = Position + Dir.dir[Direction.Down];
        if(Data.CanPlaceObject(pos))
        {
            var result = Data.SetTile(pos, ObjectType.Energy);
            Data.AddToWorldObjects(pos, result.GetComponent<ICollidable>(), ObjectType.Energy);
            growStage = 0;
            return;
        }
    }
}
