using UnityEngine;
using System.Collections.Generic;
using Niantic.Lightship.AR.WorldPositioning;
using Niantic.Lightship.AR.XRSubsystems;
using System;

public class PrePlaceWorldObject : MonoBehaviour
{
    [SerializeField] List<Material> _materials = new();
    [SerializeField] List<GameObject> _possibleObjectToPlace = new();
    [SerializeField] List<LatLong> _latlongs = new();
    [SerializeField] ARWorldPositioningManager _positioningManager;
    [SerializeField] ARWorldPositioningObjectHelper _objectHelper;

    private List<GameObject> instantiateObjects = new();

    private void Start()
    {
        foreach (var gpsCoord in _latlongs)
        {
            GameObject newObject = Instantiate(_possibleObjectToPlace[_latlongs.IndexOf(gpsCoord) % _possibleObjectToPlace.Count]);

            _objectHelper.AddOrUpdateObject(newObject, gpsCoord.latitude, gpsCoord.longitude, 0, Quaternion.identity);

            Debug.Log($"Added {newObject.name} with latitude {gpsCoord.latitude} and longitutde {gpsCoord.longitude}");
        }

        _positioningManager.OnStatusChanged += OnStatusChanged;
    }

    private void OnStatusChanged(WorldPositioningStatus status)
    {
        Debug.Log("Status changed to " + status);
    }
}


[System.Serializable]
public struct LatLong
{
    public double latitude;
    public double longitude;
}
