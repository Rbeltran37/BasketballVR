using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractorReference : MonoBehaviour
{
    [SerializeField] private Interactor interactor;

    public Interactor Interactor => interactor;
}
