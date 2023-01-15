using UnityEngine;

public class MouseWorld : MonoBehaviour
{
    [SerializeField] LayerMask mousePlaneLayerMask;

    static MouseWorld instance;

    private void Awake()
    {
        instance = this;    
    }

    private void Update()
    {
        transform.position = GetPosition();
    }

    public static Vector3 GetPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, instance.mousePlaneLayerMask);
        return hit.point;
    }
}
