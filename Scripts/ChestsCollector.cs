using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CollectingChest : MonoBehaviour
{
    [SerializeField] private Text _points;
    [SerializeField] private GameObject _door;
   
    
    
    
    private int _score = 0;
  

    private void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("chest"))
    {
        other.gameObject.SetActive(false);
        
        _score += 1;
        _points.text = _score.ToString();
        Debug.Log("Сундук найден, очки: " + _score);
    }
    OpenExit();
    if (other.CompareTag("exit"))
    {
        SceneManager.LoadScene(0);
    }
}

    private void OpenExit()
    {
        if (_score == 4)
            _door.SetActive(true);
    }
    
}
