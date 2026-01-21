using UnityEngine;
using TMPro;

using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set;}

    // Non map view objects
    public GameObject CarGO;
    public GameObject CarCameraGO;
    public GameObject PlanetGO;

    // Map view objects
    public GameObject MapPlanetGO;
    public GameObject MapCameraGO;

    public GameObject CameraMarkerGO;

    public GameObject TownMarkerPrefab;

    public bool IsMapViewActive { get { return _isMapViewActive; } }

    private bool _isMapViewActive = false;
    private PlanetGenerationManager _planetGen;

    private List<GameObject> _markers = new List<GameObject>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        _planetGen = MapPlanetGO.GetComponent<PlanetGenerationManager>();
    }

    void Start()
    {
        _isMapViewActive = false;
        UpdateVisuals();
    }

    public void ToggleMapView()
    {
        _isMapViewActive = !_isMapViewActive;
        if (_isMapViewActive) {
            // Allign with car position when we're switching to map view
            MapCameraGO.GetComponent<OrbitCamera>().LookAt(CarGO.transform.position.normalized);
        }
    }

    public void UpdateVisuals()
    {
        CarGO.SetActive(!_isMapViewActive);
        CarCameraGO.SetActive(!_isMapViewActive);
        PlanetGO.SetActive(!_isMapViewActive);

        MapPlanetGO.SetActive(_isMapViewActive);
        MapCameraGO.SetActive(_isMapViewActive);
        CameraMarkerGO.SetActive(_isMapViewActive);
        foreach (GameObject marker in _markers)
        {
            marker.SetActive(_isMapViewActive);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMapView();
            UpdateVisuals();
        }

        SetCarMarker(CarGO.transform.position, CarGO.transform.forward);
    }

    public void SetCarMarker(Vector3 position, Vector3 forward)
    {
        CameraMarkerGO.transform.position = MapPlanetGO.transform.position + position.normalized * (_planetGen.radius + 2.0f);
        CameraMarkerGO.transform.rotation = Quaternion.LookRotation(forward, position.normalized);
    }

    public void AddMarker(Vector3 position, string name)
    {
        GameObject markerGO = Instantiate(TownMarkerPrefab, MapPlanetGO.transform);
        markerGO.transform.parent = transform;
        markerGO.transform.position = MapPlanetGO.transform.position + position.normalized * (_planetGen.radius + 1.0f);
        markerGO.transform.rotation = Quaternion.LookRotation(position.normalized, Vector3.up);
        TextMeshPro label = markerGO.GetComponentInChildren<TextMeshPro>();
        label.text = name;
        _markers.Add(markerGO);

        markerGO.SetActive(_isMapViewActive);
    }

}
