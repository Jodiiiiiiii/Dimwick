using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashEffect : MonoBehaviour
{
    [ColorUsage(true, true)]
    [SerializeField] private Color _flashColor = Color.white;
    [SerializeField] private float _flashTime = 0.25f;
    [SerializeField] private AnimationCurve _flashSpeedCurve;

    private SpriteRenderer[] _spriteRenderers;
    private Material[] _materials;

    private Coroutine _flashCoroutine;
    private bool _isFlashing = false;

    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        // assign sprite renderer materials to _materials
        _materials = new Material[_spriteRenderers.Length];
        for (int i = 0; i < _materials.Length; i++)
            _materials[i] = _spriteRenderers[i].material;
    }

    private IEnumerator Flasher()
    {
        SetFlashColor();

        float elapsedTime = 0f;
        while(_isFlashing)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime > _flashTime)
                elapsedTime = 0f;

            SetFlashAmount(_flashSpeedCurve.Evaluate(elapsedTime));
            Debug.Log("we in");
            yield return true;
        }
    }

    private void SetFlashColor()
    {
        for(int i = 0; i < _materials.Length; i++)
        {
            _materials[i].SetColor("_FlashColor", _flashColor);
        }
    }

    private void SetFlashAmount(float amount)
    {
        for(int i = 0; i < _materials.Length; i++)
        {
            _materials[i].SetFloat("_FlashAmount", amount);
        }
    }

    #region PUBLIC FUNCTIONS
    public void StartFlash()
    {
        _isFlashing = true;
        _flashCoroutine = StartCoroutine(Flasher());
    }

    public void StopFlash()
    {
        _isFlashing = false;
    }
    #endregion
}
