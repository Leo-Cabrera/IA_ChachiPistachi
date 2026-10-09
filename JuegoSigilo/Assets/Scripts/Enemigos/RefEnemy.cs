using UnityEngine;

public class RefEnemy : MonoBehaviour {
    public bool coneVisibility;
    public bool areaVisibility;
    public Renderer targetRenderer;

    public void changeVisibility() {
        targetRenderer.enabled = coneVisibility || areaVisibility;
    }
}