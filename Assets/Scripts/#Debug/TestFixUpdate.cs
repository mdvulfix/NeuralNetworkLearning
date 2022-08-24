using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// GameObject.FixedUpdate example.
//
// Measure frame rate comparing FixedUpdate against Update.
// Show the rates every second.

namespace APP.Test
{

    public class TestFixUpdate : MonoBehaviour
    {
        private float updateCount = 0;
        private float fixedUpdateCount = 0;
        private float updateUpdateCountPerSecond;
        private float updateFixedUpdateCountPerSecond;

        private void Awake()
        {
            // Uncommenting this will cause framerate to drop to 10 frames per second.
            // This will mean that FixedUpdate is called more often than Update.
            //Application.targetFrameRate = 10;
            StartCoroutine(Loop());
        }

        // Increase the number of calls to Update.
        private void Update()
        {
            updateCount += 1;
        }

        // Increase the number of calls to FixedUpdate.
        private void FixedUpdate()
        {
            fixedUpdateCount += 1;
        }

        // Show the number of calls to both messages.
        private void OnGUI()
        {
            GUIStyle fontSize = new GUIStyle(GUI.skin.GetStyle("label"));
            fontSize.fontSize = 24;
            GUI.Label(new Rect(100, 100, 200, 50), "Update: " + updateUpdateCountPerSecond.ToString(), fontSize);
            GUI.Label(new Rect(100, 150, 200, 50), "FixedUpdate: " + updateFixedUpdateCountPerSecond.ToString(), fontSize);
        }

        // Update both CountsPerSecond values every second.
        private IEnumerator Loop()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                updateUpdateCountPerSecond = updateCount;
                updateFixedUpdateCountPerSecond = fixedUpdateCount;

                updateCount = 0;
                fixedUpdateCount = 0;
            }
        }
    }

}