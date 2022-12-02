using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGrabbable
{
    void AttemptGrab(Interactor interactor);
    void Grab(Interactor interactor);
    void AttemptUnGrab();
    void UnGrab();
    bool IsGrabbed();
    bool IsGrabbedBy(Interactor interactor);
    void ToggleHighlight(bool state);
}
