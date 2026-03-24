using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InteractableOutline : MonoBehaviour
{
    [Header("Outline Settings")]
    [Tooltip("Color of the outline when hovering")]
    [SerializeField]
    [ColorUsage(false, true)]
    private Color _outlineColor = Color.yellow;

    [Tooltip("Width of the outline")]
    [SerializeField, Range(0f, 10f)]
    private float _outlineWidth = 2f;

    [Tooltip("Outline mode")]
    [SerializeField]
    private Outline.Mode _outlineMode = Outline.Mode.OutlineVisible;

    private Outline _outline;
    private bool _isHighlighted = false;
    private bool _isInitialized = false;

    private void Awake()
    {
        InitializeOutline();
    }

    private void Start()
    {
        if (!_isInitialized)
        {
            InitializeOutline();
        }
        else
        {
            ApplySettings();
        }
    }

    private void InitializeOutline()
    {
        _outline = GetComponent<Outline>();
        if (_outline == null)
        {
            _outline = gameObject.AddComponent<Outline>();
        }

        _outline.enabled = false;

        _isInitialized = true;

        if (Application.isPlaying)
        {
            StartCoroutine(ApplySettingsDelayed());
        }
        else
        {
            ApplySettings();
        }
    }

    private IEnumerator ApplySettingsDelayed()
    {
        yield return null;
        ApplySettings();
    }

    private void ApplySettings()
    {
        if (_outline == null)
            return;

        ApplySettingsImmediate();
    }

    private void OnValidate()
    {
        if (_outline != null)
        {
            ApplySettingsImmediate();
        }
    }

    /// <summary>
    /// Applies settings immediately, ensuring Outline is enabled temporarily if needed.
    /// </summary>
    private void ApplySettingsImmediate()
    {
        if (_outline == null)
            return;

        bool wasEnabled = _outline.enabled;
        bool shouldBeEnabled = _isHighlighted;

        if (!wasEnabled)
        {
            _outline.enabled = true;
        }

        _outline.OutlineColor = _outlineColor;
        _outline.OutlineWidth = _outlineWidth;
        _outline.OutlineMode = _outlineMode;

        if (Application.isPlaying && !shouldBeEnabled && !wasEnabled)
        {
            StartCoroutine(DisableAfterUpdate());
        }
        else if (!shouldBeEnabled && !wasEnabled)
        {
            _outline.enabled = false;
        }
    }

    private IEnumerator DisableAfterUpdate()
    {
        yield return null;
        yield return null; 
        if (_outline != null && !_isHighlighted)
        {
            _outline.enabled = false;
        }
    }

    /// <summary>
    /// Enables outline highlighting.
    /// </summary>
    public void EnableHighlight()
    {
        if (_outline == null)
        {
            InitializeOutline();
        }

        if (!_isHighlighted)
        {
            _isHighlighted = true;
            if (_outline != null)
            {
                ApplySettings();
                _outline.enabled = true;
            }
        }
    }

    /// <summary>
    /// Disables outline highlighting.
    /// </summary>
    public void DisableHighlight()
    {
        if (_isHighlighted)
        {
            _isHighlighted = false;
            if (_outline != null)
            {
                _outline.enabled = false;
            }
        }
    }

    /// <summary>
    /// Returns whether the outline is currently highlighted.
    /// </summary>
    public bool IsHighlighted()
    {
        return _isHighlighted;
    }

    /// <summary>
    /// Sets the outline color.
    /// </summary>
    public void SetOutlineColor(Color color)
    {
        _outlineColor = color;
        if (_outline != null)
        {
            _outline.OutlineColor = color;
        }
    }

    /// <summary>
    /// Sets the outline width.
    /// </summary>
    public void SetOutlineWidth(float width)
    {
        _outlineWidth = width;
        if (_outline != null)
        {
            _outline.OutlineWidth = width;
        }
    }

    /// <summary>
    /// Forces reapplication of settings. Useful when component is added dynamically.
    /// </summary>
    public void RefreshSettings()
    {
        if (_outline == null)
        {
            InitializeOutline();
        }
        else
        {
            ApplySettings();
        }
    }
}