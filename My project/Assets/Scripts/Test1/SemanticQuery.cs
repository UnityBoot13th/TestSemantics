using System.Collections.Generic;
using Niantic.Lightship.AR.Semantics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.InputSystem; // ✅ New Input System

//This script was originally provided by Niantic and modified by Alive Studios
//https://lightship.dev/docs/ardk/how-to/ar/query_semantics_real_objects/
public class SemanticQuery : MonoBehaviour
{
    public ARCameraManager _cameraMan;
    public ARSemanticSegmentationManager _semanticMan;

    public TMP_Text _text;
    public RawImage _image;
    public Material _material;

    private string _channel = "ground";

    [SerializeField] private Transform spawnObjectParent;

    private void OnEnable()
    {
        _image.enabled = false;
        _cameraMan.frameReceived += CameraManOnframeReceived;
    }

    private void OnDisable()
    {
        _cameraMan.frameReceived -= CameraManOnframeReceived;
    }

    private void CameraManOnframeReceived(ARCameraFrameEventArgs args)
    {
        if (!_semanticMan.subsystem.running)
        {
            return;
        }

        Matrix4x4 mat = Matrix4x4.identity;
        var texture = _semanticMan.GetSemanticChannelTexture(_channel, out mat);

        if (texture)
        {
            Matrix4x4 cameraMatrix = args.displayMatrix ?? Matrix4x4.identity;
            _image.material = _material;
            _image.material.SetTexture("_SemanticTex", texture);
            _image.material.SetMatrix("_SemanticMat", mat);
        }
    }

    private float timer = 0.0f;

    void Update()
    {
        if (!_semanticMan.subsystem.running)
        {
            return;
        }

        HandlePointerInput();
    }

    private void HandlePointerInput()
    {
        // Mouse (Editor/PC)
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            var pos = Mouse.current.position.ReadValue();
            ProcessTouchPosition(pos);
        }

        // Touch (Device)
        if (Touchscreen.current != null)
        {
            foreach (var touch in Touchscreen.current.touches)
            {
                if (touch.press.wasPressedThisFrame)
                {
                    var pos = touch.position.ReadValue();
                    ProcessTouchPosition(pos);
                }
            }
        }
    }

    private void ProcessTouchPosition(Vector2 pos)
    {
        if (pos.x > 0 && pos.x < Screen.width && pos.y > 0 && pos.y < Screen.height)
        {
            timer += Time.deltaTime;
            _image.enabled = true;

            if (timer > 0.05f)
            {
                var list = _semanticMan.GetChannelNamesAt((int)pos.x, (int)pos.y);

                if (list.Count > 0)
                {
                    _channel = list[0];
                    _text.text = _channel;
                }
                else
                {
                    _text.text = "?";
                }

                timer = 0.0f;
            }
        }
    }
}