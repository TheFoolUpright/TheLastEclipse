using System.Collections.Generic;
using UnityEngine;

public class FleeNode : MonoBehaviour
{
    public List<FleeNode> connectedNodes = new List<FleeNode>();

    [Header("Behavior")]
    public bool isRestPoint;
    public bool usableForWander = true;
    public bool usableForFlee = true;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, 0.25f);

        if (connectedNodes == null)
            return;

        Gizmos.color = Color.magenta;

        foreach (FleeNode node in connectedNodes)
        {
            if (node != null)
            {
                Gizmos.DrawLine(transform.position, node.transform.position);
            }
        }
    }
}