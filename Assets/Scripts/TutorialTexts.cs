using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialTexts : MonoBehaviour
{
    RectTransform myRectTransform;
    TextMeshProUGUI tmpro;

    [SerializeField] float textJumpAmountX = 5f;
    [SerializeField] float textJumpAmountY = 5f;
    [SerializeField] float textAnimationInterval = 1f;
    [SerializeField] float textFadeDistance = 2f;

    [Header("Higher number means slower fade")]
    [SerializeField] float textFadeRate = 3f;

    float timer;
    float distanceModifier;

    int vectorListIndex = 0;
    int vectorCount;

    GameObject player;

    List<Vector2> textMovementVectors = new List<Vector2>();

    void Start() 
    {
        textMovementVectors.Add(new Vector2(-textJumpAmountX, -textJumpAmountY));
        textMovementVectors.Add(new Vector2(-textJumpAmountX, textJumpAmountY));
        textMovementVectors.Add(new Vector2(-textJumpAmountX, -textJumpAmountY));
        textMovementVectors.Add(new Vector2(-textJumpAmountX, textJumpAmountY));
        textMovementVectors.Add(new Vector2(textJumpAmountX, textJumpAmountY));
        textMovementVectors.Add(new Vector2(textJumpAmountX, -textJumpAmountY));
        textMovementVectors.Add(new Vector2(textJumpAmountX, textJumpAmountY));
        textMovementVectors.Add(new Vector2(textJumpAmountX, -textJumpAmountY));

        vectorCount = textMovementVectors.Count;
        timer = textAnimationInterval;

        myRectTransform = GetComponent<RectTransform>();
        tmpro = GetComponent<TextMeshProUGUI>();
        player = GameObject.Find("Player");
    }

    void Update()
    {
        distanceModifier = Vector2.Distance(myRectTransform.position, player.transform.position) / textFadeRate;

        float transparencyValue = textFadeDistance - distanceModifier;

        tmpro.color = new Color(1, 1, 1, transparencyValue);

        if (timer <= 0)
        {
            timer = textAnimationInterval;
            myRectTransform.anchoredPosition += textMovementVectors[vectorListIndex];

            vectorListIndex = (vectorListIndex + 1) % vectorCount;
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }
}
