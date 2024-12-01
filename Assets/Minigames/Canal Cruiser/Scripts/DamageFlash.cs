using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    [SerializeField] private Color _flashColour = Color.white;
    [SerializeField] private float _flashTime = 0.25f;

    private SpriteRenderer _spriteRenderer;
    private Material _materials;

    private Coroutine _damageFlashCoroutine;

    // Start is called before the first frame update
    private void awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _materials = _spriteRenderer.material;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CallDamageFlash()
    {
        _damageFlashCoroutine = StartCoroutine(DamageFlasher());
    }

    private IEnumerator DamageFlasher()
    {
        //set the colour
        SetFlashColour();
        //lerp the amount
        float currentFlashAmount = 0f;
        float elapsedTime = 0f;
        while(elapsedTime < _flashTime)
        {
            //iterate elapsedtime
            elapsedTime += Time.deltaTime;

            //lerp the flash amount
            currentFlashAmount = Mathf.Lerp(1f, 0f, (elapsedTime / _flashTime));
            SetFlashAmount(currentFlashAmount);

            yield return null;
        }
    }

    private void SetFlashColour()
    {
        _materials.SetColor("_FlashColour", _flashColour);
    }

    private void SetFlashAmount(float amount)
    {
        _materials.SetFloat("_FlashAmount", amount);
    }
}
