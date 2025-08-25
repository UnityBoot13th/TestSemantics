using Niantic.Lightship.AR.WorldPositioning;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class PlaceObjectAtLocation : MonoBehaviour
{
    [SerializeField] private ARWorldPositioningObjectHelper _objectHelper;
    [SerializeField] private ARWorldPositioningManager _positioningManager;
    [SerializeField] private ARCameraManager _cameraManager;
    [SerializeField] private Button button;

    [SerializeField] private GameObject objectToPlace;

    private void Start()
    {
        button.onClick.AddListener(SpawnOnClick);
    }

    private void SpawnOnClick()
    {
        Transform camerTransform = _cameraManager.GetComponent<Transform>();

        (double latOffsetCam, double longOffsetCam) = GetGeographicOffsetFromCameraPosition(camerTransform.forward);

        double latitude = _positioningManager.WorldTransform.OriginLatitude + latOffsetCam;
        double longitutde = _positioningManager.WorldTransform.OriginLongitude + longOffsetCam;
        double altitide = 0.0;

        GameObject newObject = Instantiate(objectToPlace);

        _objectHelper.AddOrUpdateObject(newObject, latitude, longitutde, altitide, Quaternion.identity);

        Debug.Log($"Placing Object {newObject.name} at lat {latitude} and long {longitutde}");
    }

    private (double, double) GetGeographicOffsetFromCameraPosition(Vector3 position)
    {
        double latOffset = position.z / 111000;
        double longOffset = position.x /
                            (11000 * MathF.Cos(
                                (float)_positioningManager.WorldTransform.OriginLatitude * Mathf.Deg2Rad));

        return (latOffset, longOffset);
    }

}
