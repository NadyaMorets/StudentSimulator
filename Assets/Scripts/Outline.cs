using UnityEngine;

[DisallowMultipleComponent]
public class Outline : MonoBehaviour
{
    public enum Mode
    {
        OutlineAll,
        OutlineVisible,
        OutlineHidden
    }

    public Mode OutlineMode = Mode.OutlineAll;
    public Color OutlineColor = Color.yellow;
    [Range(0f, 10f)] public float OutlineWidth = 2f;

    private Renderer _renderer;
    private Material _outlineMaterial;
    private Material _originalMaterial;

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer == null)
            _renderer = GetComponentInChildren<Renderer>();

        if (_renderer != null)
        {
            _originalMaterial = _renderer.material;
            CreateOutlineMaterial();
        }
    }

    void CreateOutlineMaterial()
    {
        _outlineMaterial = new Material(Shader.Find("Unlit/Color"));
        _outlineMaterial.color = OutlineColor;

        Shader outlineShader = Shader.Find("Custom/Outline");
        if (outlineShader != null)
        {
            _outlineMaterial = new Material(outlineShader);
            _outlineMaterial.SetColor("_OutlineColor", OutlineColor);
            _outlineMaterial.SetFloat("_OutlineWidth", OutlineWidth);
        }
    }

    void OnEnable()
    {
        if (_renderer != null && _outlineMaterial != null)
        {
            if (_renderer.material != _outlineMaterial)
            {
                _originalMaterial = _renderer.material;
                _renderer.material = _outlineMaterial;
            }
        }
    }

    void Update()
    {
        if (_outlineMaterial != null)
        {
            _outlineMaterial.color = OutlineColor;

            if (_outlineMaterial.HasProperty("_OutlineWidth"))
            {
                _outlineMaterial.SetFloat("_OutlineWidth", OutlineWidth);
            }
        }
    }

    void OnDisable()
    {
        if (_renderer != null && _originalMaterial != null)
        {
            _renderer.material = _originalMaterial;
        }
    }

    void OnDestroy()
    {
        if (_outlineMaterial != null)
            Destroy(_outlineMaterial);
    }
}