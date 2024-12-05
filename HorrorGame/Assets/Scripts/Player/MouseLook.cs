using System;
using UnityEngine;

[Serializable]
public class MouseLook
{
    public float x_sensitivity = 2f;
    public float y_sensitivity = 2f;
    public bool clampVerticalRotation = true;
    public float min_x = -90f;
    public float max_x = 90f;
    public bool smooth = false;
    public float smoothTime = 5f;
    public bool lockCursor = true;

    private Quaternion m_CharacterTargetRot;
    private Quaternion m_CameraTargetRot;
    private bool m_cursorIsLocked = true;
    private bool isLookingBehind = false;

    public void Init(Transform character, Transform camera)
    {
        m_CharacterTargetRot = character.localRotation;
        m_CameraTargetRot = camera.localRotation;
        UpdateCursorLock();
    }

    public void LookRotation(Transform character, Transform camera)
    {
        if (!isLookingBehind)
        {
            float y_Rotation = Input.GetAxis("Mouse X") * x_sensitivity;
            float x_Rotation = Input.GetAxis("Mouse Y") * y_sensitivity;

            m_CharacterTargetRot *= Quaternion.Euler(0f, y_Rotation, 0f);
            m_CameraTargetRot *= Quaternion.Euler(-x_Rotation, 0f, 0f);

            if (clampVerticalRotation)
            {
                m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);
            }

            if (smooth)
            {
                character.localRotation = Quaternion.Slerp(character.localRotation, m_CharacterTargetRot, smoothTime * Time.deltaTime);
                camera.localRotation = Quaternion.Slerp(camera.localRotation, m_CameraTargetRot, smoothTime * Time.deltaTime);
            }
            else
            {
                character.localRotation = m_CharacterTargetRot;
                camera.localRotation = m_CameraTargetRot;
            }
        }
        UpdateCursorLock();
    }

    public void SetLookBehind(bool value)
    {
        isLookingBehind = value;
    }

    public void SetCursorLock(bool value)
    {
        lockCursor = value;
        if (!lockCursor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void UpdateCursorLock()
    {
        if (lockCursor)
        {
            InternalLockUpdate();
        }
    }

    private void InternalLockUpdate()
    {
        if (Input.GetKeyUp(KeyCode.O) || Input.GetKey(KeyCode.LeftAlt))
        {
            m_cursorIsLocked = false;
        }
        else if (Input.GetMouseButtonUp(0) || !Input.GetKey(KeyCode.LeftAlt))
        {
            m_cursorIsLocked = true;
        }

        if (m_cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, min_x, max_x);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }
}