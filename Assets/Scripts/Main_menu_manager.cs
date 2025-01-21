using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Main_menu_manager : MonoBehaviour
{

    [SerializeField]
    private string load;

    public void Select()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(load);
    }
}