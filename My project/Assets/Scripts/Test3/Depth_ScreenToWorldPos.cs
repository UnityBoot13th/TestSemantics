using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Niantic.Lightship.AR.Utilities;
using UnityEngine.InputSystem;

public class ScreenToWorldPos : MonoBehaviour
{
    public AROcclusionManager _occlusionManager;
    public Camera _camera;
    public GameObject[] _prefabs2Spawn;

    public bool _spawningPrefab = false;
    XRCpuImage? _depthImage;

    private void Update()
    {
        if (!_occlusionManager.subsystem.running)
            return;

        Matrix4x4 displayMat = Matrix4x4.identity;

        if (_occlusionManager.TryAcquireEnvironmentDepthCpuImage(out var image))
        {
            _depthImage?.Dispose();
            _depthImage = image;
        }
        else
        {
            return;
        }

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            Vector2 screenPosition = Touchscreen.current.primaryTouch.position.ReadValue();

            if (_depthImage.HasValue && !_spawningPrefab)
            {
                _spawningPrefab = true;

                        // 화면 비율로 정규화
                Vector2 uv = new Vector2(screenPosition.x / Screen.width, screenPosition.y / Screen.height);

                        // depth 샘플링
                float eyeDepth = _depthImage.Value.Sample<float>(uv, displayMat);

                        // 월드 좌표 변환
                Vector3 worldPosition = _camera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, eyeDepth));

                        // 프리팹 생성
                StartCoroutine(SpawningPrefab(worldPosition));
            }
        }
    }

    IEnumerator SpawningPrefab(Vector3 worldPosition)
    {
        int random = Random.Range(minInclusive: 0, maxExclusive: _prefabs2Spawn.Length);
        Instantiate(_prefabs2Spawn[random], worldPosition, Quaternion.identity);

        yield return new WaitForSeconds(2f);

        _spawningPrefab = false;
    }
}
