using System.Collections;
using System.Linq;
using UnityEngine;

public class ThrowableItems : MonoBehaviour
{
    [SerializeField]
    private GameObject throwableItem;
    [SerializeField]
    private float throwForce = 20f;
    [SerializeField]
    private float verticalOffset = 1f;
    [SerializeField]
    private AudioClip hitSound;
    [SerializeField]
    private float hitSoundDistance = 40f;

    private Enemy enemy;
    private Inventory inventory;
    private Camera cam;
    private AudioSource audioSource;
    private bool hasBeenThrown = false;
    private Collider playerCollider;
    private PlayerController playerController;

    public bool canUseItem = true;

    void Start()
    {
        enemy = GameManager.Instance.Enemies.FirstOrDefault();
        inventory = Inventory.Instance;
        cam = Camera.main;
        audioSource = GetComponent<AudioSource>();
        playerCollider = GameManager.Instance.Player.GetComponent<Collider>();
        playerController = GameManager.Instance.Player.GetComponent<PlayerController>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && CanUseItem() && !TrialEndMenu.trialEnded && !DialogueManager.Instance.IsDialogueActive() && !PlayerHealth.PlayerDied && !PauseMenu.GameIsPaused)
        {
            ThrowItem();
        }
    }

    private bool CanUseItem()
    {
        return canUseItem && !playerController.IsPlayerInCloset;
    }

    private void ThrowItem()
    {
        if (inventory != null && inventory.HasItem(throwableItem) && throwableItem.activeSelf)
        {
            Vector3 throwDirection = CalculateThrowDirection();

            throwableItem.transform.SetParent(null);
            SetLayerRecursively(throwableItem, LayerMask.NameToLayer("Interactable"));

            Rigidbody rb = throwableItem.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.AddForce(throwDirection * throwForce, ForceMode.VelocityChange);

                Physics.IgnoreCollision(throwableItem.GetComponent<Collider>(), playerCollider, true);
                Invoke(nameof(RestorePlayerCollision), 0.5f);

                inventory.RemoveItem(throwableItem, false);

                hasBeenThrown = true;
                audioSource.enabled = true;
            }
        }
    }

    private Vector3 CalculateThrowDirection()
    {
        Vector3 initialPosition = throwableItem.transform.position;

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100f);
        }

        Vector3 throwDirection = (targetPoint - initialPosition).normalized;
        throwDirection.y += verticalOffset;

        return throwDirection;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasBeenThrown || !audioSource.enabled) return;

        audioSource.PlayOneShot(hitSound);
        StartCoroutine(DisableAudioSourceAfterDelay(0.7f));

        Enemy collidedEnemy = collision.collider.GetComponent<Enemy>();
        if (collidedEnemy != null)
        {
            collidedEnemy.Flinch();
            Destroy(gameObject, 0.6f);
        }
        else
        {
            foreach (var e in GameManager.Instance.Enemies)
            {
                enemy.NotifyEnemy(transform.position, hitSoundDistance);
            }
            Destroy(gameObject, 0.6f);
        }
    }

    private IEnumerator DisableAudioSourceAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.enabled = false;
    }

    private void RestorePlayerCollision()
    {
        Physics.IgnoreCollision(throwableItem.GetComponent<Collider>(), playerCollider, false);
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}