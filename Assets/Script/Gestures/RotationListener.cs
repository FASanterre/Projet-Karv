using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationListener : MonoBehaviour, KinectGestures.GestureListenerInterface
{
    [Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
    public int playerIndex = 0;

    [Tooltip("UI-Text to display gesture-listener messages and gesture information.")]
    public UnityEngine.UI.Text gestureInfo;
    public UnityEngine.UI.Text gestureList;

    // singleton instance of the class
    private static RotationListener instance = null;

    // internal variables to track if progress message has been displayed
    private bool progressDisplayed;
    private float progressGestureTime;

    // whether the needed gesture has been detected or not
    private bool rotation = false;


    /// <summary>
	/// Gets the singleton TestGestureListener instance.
	/// </summary>
	/// <value>The TestGestureListener instance.</value>
	public static RotationListener Instance
    {
        get
        {
            return instance;
        }
    }

    /// <summary>
	/// Determines whether the user has raised his left or right hand.
	/// </summary>
	/// <returns><c>true</c> if the user has raised his left or right hand; otherwise, <c>false</c>.</returns>
	public bool IsRotating()
    {
        if (rotation)
        {
            rotation = false;
            return true;
        }

        return false;
    }



    /// <summary>
    /// Invoked when a new user is detected. Here you can start gesture tracking by invoking 
    /// .DetectGesture()-function.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="userIndex">User index</param>
    public void UserDetected(long userId, int userIndex)
    {
        // the gestures are allowed for the primary user only
        KinectManager manager = KinectManager.Instance;
        if (!manager || (userIndex != playerIndex))
            return;

        // detect these user specific gestures
        //manager.DetectGesture(userId, KinectGestures.Gestures.Rotation);

        if (gestureInfo != null)
        {
            gestureInfo.text = "Move to see the gesture !";
        }
    }

    /// <summary>
	/// Invoked when a user gets lost. All tracked gestures for this user are cleared automatically.
	/// </summary>
	/// <param name="userId">User ID</param>
	/// <param name="userIndex">User index</param>
	public void UserLost(long userId, int userIndex)
    {
        // the gestures are allowed for the primary user only
        if (userIndex != playerIndex)
            return;

        if (gestureInfo != null)
        {
            gestureInfo.text = string.Empty;
        }
    }

    /// <summary>
	/// Invoked when a gesture is in progress.
	/// </summary>
	/// <param name="userId">User ID</param>
	/// <param name="userIndex">User index</param>
	/// <param name="gesture">Gesture type</param>
	/// <param name="progress">Gesture progress [0..1]</param>
	/// <param name="joint">Joint type</param>
	/// <param name="screenPos">Normalized viewport position</param>
	public void GestureInProgress(long userId, int userIndex, KinectGestures.Gestures gesture,
                                  float progress, KinectInterop.JointType joint, Vector3 screenPos)
    {
        // the gestures are allowed for the primary user only
        if (userIndex != playerIndex)
            return;

           
        /*if (gesture == KinectGestures.Gestures.Rotation )
        {
            if (gestureInfo != null)
            {
                string sGestureText = string.Format("{0} - {1:F0}", gesture, screenPos.z);
                gestureInfo.text = sGestureText;

                progressDisplayed = true;
                progressGestureTime = Time.realtimeSinceStartup;
            }
        }*/


    }



    /// <summary>
    /// Invoked if a gesture is completed.
    /// </summary>
    /// <returns>true</returns>
    /// <c>false</c>
    /// <param name="userId">User ID</param>
    /// <param name="userIndex">User index</param>
    /// <param name="gesture">Gesture type</param>
    /// <param name="joint">Joint type</param>
    /// <param name="screenPos">Normalized viewport position</param>
    public bool GestureCompleted(long userId, int userIndex, KinectGestures.Gestures gesture,
                                  KinectInterop.JointType joint, Vector3 screenPos)
    {
        // the gestures are allowed for the primary user only
        if (userIndex != playerIndex)
            return false;

        if (gestureInfo != null)
        {
            string sGestureText = gesture + " detected";
            gestureInfo.text = sGestureText;
        }

        /*if (gesture == KinectGestures.Gestures.Rotation)
            rotation = true;*/

        return true;
    }

    /// <summary>
    /// Invoked if a gesture is cancelled.
    /// </summary>
    /// <returns>true</returns>
    /// <c>false</c>
    /// <param name="userId">User ID</param>
    /// <param name="userIndex">User index</param>
    /// <param name="gesture">Gesture type</param>
    /// <param name="joint">Joint type</param>
    public bool GestureCancelled(long userId, int userIndex, KinectGestures.Gestures gesture,
                                  KinectInterop.JointType joint)
    {
        // the gestures are allowed for the primary user only
        if (userIndex != playerIndex)
            return false;


        /*if (gesture == KinectGestures.Gestures.Rotation)
            rotation = false;*/
        
        if (gestureInfo != null && progressDisplayed)
        {
            progressDisplayed = false;
            gestureInfo.text = "Waiting";
        }

        return true;
    }


    void Awake()
    {
        instance = this;
        gestureList.text =
            "Rotation\n";
    }

    void Update()
    {
        if (progressDisplayed && ((Time.realtimeSinceStartup - progressGestureTime) > 2f))
        {
            progressDisplayed = false;
            gestureInfo.text = string.Empty;

            Debug.Log("Forced progress to end.");
        }
    }
    
}
