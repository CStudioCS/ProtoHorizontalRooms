using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrabCrate : MonoBehaviour
{
    [SerializeField] private InputAction grabAction;

    public RelativeJoint2D CrateJoint;
    [SerializeField] private Collider2D coll;
    [SerializeField] private Vector2 holdingOffset = new Vector2(1, 0);
    [SerializeField] private float maxGrabDistance = 1.5f;
    [SerializeField] private float initbreakForce = 100f;
    [SerializeField] private float breakForce = 30f;
    [SerializeField] private float strengthTime = 0.4f;
    private bool inStrength = false;
    public Player player;

    private void Awake()
    {
        CrateJoint.enabled = false;
    }

    private void OnEnable()
    {
        grabAction.Enable();
    }

    private void OnDisable()
    {
        grabAction.Disable();
    }

    private void Update()
    {
        if (grabAction.WasPressedThisFrame())
        {
            if (CrateJoint.enabled)
            {
                Drop();
            }
            else if (!player.OtherPlayer.GrabCrate.CrateJoint.enabled && coll.Distance(GameManager.Instance.Crate.Collider).distance < maxGrabDistance)
            {
                CrateJoint.connectedBody = GameManager.Instance.Crate.Rb;
                CrateJoint.enabled = true;
                CrateJoint.linearOffset = new Vector2(holdingOffset.x * (GetComponent<Player>().isFacingRight ? 1 : -1), holdingOffset.y);
                CrateJoint.angularOffset = 0;
                StopAllCoroutines();
                StartCoroutine(StrengthWait());
            }
        }
    }

    public void TryDrop()
    {
        if (!inStrength)
            Drop();
    }

    public void Drop()
    {
        CrateJoint.connectedBody = null;
        CrateJoint.enabled = false;
    }

    private IEnumerator StrengthWait()
    {
        inStrength = true;
        CrateJoint.breakForce = initbreakForce;

        yield return new WaitForSeconds(strengthTime);

        inStrength = false;
        CrateJoint.breakForce = breakForce;
    }
}
