using UnityEngine;

public class SpawnerElement : MonoBehaviour
{
    public bool IsOccupied { get; private set; }
    public MeshRenderer MeshRenderer { get; private set; }

    private Material _originalMaterial;
    private string _placedIcon = "";

    private void OnEnable()
    {
        MeshRenderer = GetComponent<MeshRenderer>();
        _originalMaterial = MeshRenderer.material;
        ResetSpawner();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 1f);
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }

    public void OccupySpawner(Material mat, string icon)
    {
        IsOccupied = true;
        MeshRenderer.material = mat;
        _placedIcon = icon;
    }

    public bool CheckIcon(string icon)
    {
        return _placedIcon == icon;
    }

    public void ResetSpawner()
    {
        MeshRenderer.material = _originalMaterial;
        IsOccupied = false;
        _placedIcon = "";
    }
}
