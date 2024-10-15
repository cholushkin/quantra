using UnityEngine;
using DG.Tweening;
using GameLib.Random;

public class BubbleEffect : MonoBehaviour
{
    public Range AppearDuration = new(1f, 3f);
    public Range HeightRange = new(10f, 33f);
    public Range MoveUpSpeedRange = new(3f, 4f);
    public float DelayBeforeExplode = 0.5f; // Time to wait before exploding at random height
    public float ExplodeDuration = 0.5f; // Time for explosion animation
    public LayerMask FloorLayer;

    private Vector3 _originalScale;
    private bool _hasExploded = false; // Prevent multiple explosions
    private int _floorLayer; // Cache the Floor layer index for quick checks

#region Unity callbacks
    private void Start()
    {
        _originalScale = transform.localScale;
        transform.localScale = Vector3.zero; // Set the bubble scale to zero (invisible) at the start
        _floorLayer = LayerMask.NameToLayer("Floor"); // Cache the "Floor" layer integer
        AppearAndMoveUp(); // Start the bubble appearance sequence
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object's layer is NOT the "Floor" layer
        if (other.gameObject.layer != _floorLayer)
        {
            Explode(); // Trigger the explosion if it's not the floor
        }
    }
#endregion

#region Implementation
    private void AppearAndMoveUp()
    {
        // First, the bubble will slowly appear on the surface (grow in scale)
        transform.DOScale(_originalScale, RandomHelper.Rnd.FromRange(AppearDuration))
            .SetEase(Ease.InOutBack)
            .OnComplete(() =>
            {
                // After appearing, the bubble will float upwards to a random height
                float randomHeight = RandomHelper.Rnd.FromRange(HeightRange);
                float randomSpeed = RandomHelper.Rnd.FromRange(MoveUpSpeedRange);
                float flyUpDuration = randomHeight / randomSpeed;

                transform.DOMoveY(randomHeight, flyUpDuration)
                    .SetEase(Ease.InOutSine)
                    .OnComplete(() =>
                    {
                        // Once the bubble reaches the random height, wait for a delay before exploding
                        DOVirtual.DelayedCall(DelayBeforeExplode, Explode);
                    });
            });
    }

    private void Explode()
    {
        if (_hasExploded)
            return; // Prevent multiple explosions

        _hasExploded = true;
        // Explosion effect: make the bubble grow larger and then disappear
        transform.DOPunchScale(Vector3.one, ExplodeDuration, 1, 0.5f)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                // Destroy the bubble after it "explodes"
                Destroy(gameObject);
            });
    }
#endregion
}