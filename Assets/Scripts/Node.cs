using System;
using System.Collections;
using UnityEngine;

public class Node : MonoBehaviour
{
    public int number = 2;
    public bool isMoveFree = true;

    private void HorizontalMove()
    {
        var transform1 = transform;
        var position = transform1.position;
        if (Input.GetKeyDown(KeyCode.A))
        {
            position = new Vector3(position.x - (1.1f), position.y, position.z);
            if(position.x < 0)
            {
                isMoveFree = false;
                return;
            }
            StartCoroutine(RayWaiter(Vector3.left, (status) =>
            {
                if (status)
                    transform1.position = position;
            }));
        } else if (Input.GetKeyDown(KeyCode.D))
        {
            position = new Vector3(position.x + (1.1f), position.y, position.z);
            if(position.x > 2.4)
            {
                isMoveFree = false;
                return;
            }

            StartCoroutine(RayWaiter(Vector3.right, (status) =>
            {
                if (status)
                    transform1.position = position;
            }));

        }
    }

    private void VerticalMove()
    {
        var transform1 = transform;
        var position = transform1.position;
        if (Input.GetKeyDown(KeyCode.W))
        {
            position = new Vector3(position.x, position.y, position.z + (1.1f));
            if(position.z > 2.4)
            {
                isMoveFree = false;
                return;
            }
            StartCoroutine(RayWaiter(Vector3.forward, (status) =>
            {
                if (status)
                    transform1.position = position;
            }));
        } else if (Input.GetKeyDown(KeyCode.S))
        {
            position = new Vector3(position.x, position.y, position.z - (1.1f));
            if(position.z < 0)
            {
                isMoveFree = false;
                return;
            }
            StartCoroutine(RayWaiter(Vector3.back, (status) =>
            {
                if (status)
                    transform1.position = position;
            }));
        }
    }

    private bool CheckDirectionNode(Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(direction), out hit, 1.1f))
        {
            if (hit.transform.CompareTag("Node"))
            {
                var node = hit.transform.GetComponent<Node>();
                if (node.number == number)
                {
                    Destroy(hit.transform.gameObject);
                    number *= 2;
                    isMoveFree = true;
                    return true;
                }
                else
                {
                    if (node.isMoveFree)
                        return true;
                        
                    isMoveFree = false;
                    return false;
                }
            }
        }
        isMoveFree = true;
        return true;
    }
    
            IEnumerator RayWaiter(Vector3 directon, Action<bool> moveAction)
        {
            if (directon == Vector3.left)
            {
                if (transform.position.x < 1)
                {
                    yield return new WaitForSeconds(0f);
                }
                else if (transform.position.x < 2)
                {
                    yield return new WaitForSeconds(0.01f);
                }
                else
                {
                    yield return new WaitForSeconds(0.02f);
                }
            }

            if (directon == Vector3.right)
            {
                if (transform.position.x < 1)
                {
                    yield return new WaitForSeconds(0.02f);
                }
                else if (transform.position.x < 2)
                {
                    yield return new WaitForSeconds(0.01f);
                }
                else
                {
                    yield return new WaitForSeconds(0f);
                }
            }
            
            if (directon == Vector3.forward)
            {
                if (transform.position.z < 1)
                {
                    yield return new WaitForSeconds(0.02f);
                }
                else if (transform.position.z < 2)
                {
                    yield return new WaitForSeconds(0.01f);
                }
                else
                {
                    yield return new WaitForSeconds(0f);
                }
            }
            
            if (directon == Vector3.back)
            {
                if (transform.position.z < 1)
                {
                    yield return new WaitForSeconds(0f);
                }
                else if (transform.position.z < 2)
                {
                    yield return new WaitForSeconds(0.01f);
                }
                else
                {
                    yield return new WaitForSeconds(0.02f);
                }
            }
            
            moveAction?.Invoke(CheckDirectionNode(directon));
            
        }

}
