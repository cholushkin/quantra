using UnityEngine;
using DG.Tweening;

public class BubbleEffect : MonoBehaviour
{
    [Header("Bubble Settings")]
    public float appearDuration = 1.5f;      // Time to appear
    public float moveUpDuration = 3f;        // Time to move up
    public float minHeight = 3f;             // Minimum height for the bubble to stop
    public float maxHeight = 6f;             // Maximum height for the bubble to stop
    public float explodeScale = 3f;          // Scale when the bubble explodes
    public float explodeDuration = 0.5f;     // Time for explosion animation
    public float delayBeforeExplode = 1f;    // Time to wait before exploding at random height

    private Vector3 originalScale;

    private void Start()
    {
        // Store the original scale of the bubble
        originalScale = transform.localScale;

        // Set the bubble scale to zero (invisible) at the start
        transform.localScale = Vector3.zero;

        // Start the bubble appearance sequence
        AppearAndMoveUp();
    }

    private void AppearAndMoveUp()
    {
        // First, the bubble will slowly appear on the surface (grow in scale)
        transform.DOScale(originalScale, appearDuration).OnComplete(() =>
        {
            // After appearing, the bubble will float upwards to a random height
            float randomHeight = Random.Range(minHeight, maxHeight);
            Vector3 targetPosition = new Vector3(transform.position.x, randomHeight, transform.position.z);

            transform.DOMoveY(randomHeight, moveUpDuration).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                // Once the bubble reaches the random height, wait for a delay before exploding
                Invoke(nameof(Explode), delayBeforeExplode);
            });
        });
    }

    private void Explode()
    {
        // Explosion effect: make the bubble grow larger and then disappear
        transform.DOScale(explodeScale, explodeDuration).SetEase(Ease.OutBack).OnComplete(() =>
        {
            // Destroy the bubble after it "explodes"
            Destroy(gameObject);
        });
    }
}
