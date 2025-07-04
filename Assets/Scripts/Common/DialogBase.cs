using UnityEngine;
using UnityEngine.UI;

public class DialogBase : MonoBehaviour
{
    [SerializeField]Image _blocker;
    public bool Closable { get; set; } = false;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void OnClickBlocker()
    {
        if (Closable)
        {
            var listener = transform.GetComponentInChildren<DialogBaseListener>();
            if (listener != null)
            {
                if (listener.OnClickBlocker())
                {
                    Close();
                }
            }
            else
            {
                Close();
            }
        }
    }

    public void Close()
    {
        if (true)
        {
            var animator = GetComponentInChildren<Animator>();
            animator.SetTrigger("Close");
        }
    }
    
    public void SetColor(Color color)
    {
        _blocker.color = new Color(color.r, color.g, color.b, color.a);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void CloseNow()
    {
        Destroy();
    }
}
