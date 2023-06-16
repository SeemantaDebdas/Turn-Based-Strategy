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

    public static Vector3 GetPositionOnlyVisible()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] raycastHitArray = Physics.RaycastAll(ray, Mathf.Infinity, instance.mousePlaneLayerMask);

        System.Array.Sort(raycastHitArray, (RaycastHit raycastHitA, RaycastHit raycastHitB) =>
        {
            return Mathf.RoundToInt(raycastHitA.distance - raycastHitB.distance);
        }); 

        foreach(RaycastHit raycastHit in raycastHitArray)
        {
            if(raycastHit.transform.TryGetComponent(out Renderer renderer))
            {
                if (renderer.enabled)
                    return raycastHit.point;
            }
        }

        return Vector3.zero; 
    }
}
