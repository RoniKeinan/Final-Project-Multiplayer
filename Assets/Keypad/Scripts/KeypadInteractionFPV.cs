using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavKeypad { 
public class KeypadInteractionFPV : MonoBehaviour
{
    private Camera cam;
    public GameObject keypad;
        public Vector3 originalScale;

    private void Awake() => cam = Camera.main;

        private void Start()
        {
            originalScale = keypad.transform.localScale;
        }

        private void Update()
    {
        var ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.collider.TryGetComponent(out KeypadButton keypadButton))
                {
                    keypadButton.PressButton();
                    keypad.transform.localScale = new Vector3(2, 2, 2);

                }
            }
            else
            {
                keypad.transform.localScale = originalScale;
            }
        }
    }
}
}