using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class FindClosest : MonoBehaviour
{
    public GameObject playerObj;
    public GameObject whitePrefab;
    public GameObject blackPrefab;

    public int whiteCount = 1000;
    public int blackCount = 1000;

    public float rangeRadius = 10f;

    Stopwatch stopwatch;


    KdTree<Actor> whiteActorTrees;
    KdTree<Actor> blackActorTrees;

    private void Start()
    {
        stopwatch = new Stopwatch();

        whiteActorTrees = new KdTree<Actor>();
        blackActorTrees = new KdTree<Actor>();

        for (int i = 0; i < whiteCount; i++)
        {
            var obj = GameObject.Instantiate(whitePrefab) as GameObject;
            obj.SetActive(true);
            obj.transform.localScale = Vector3.one * 0.2f;
            obj.name = "white:" + i;
            var actor = obj.GetComponent<Actor>();
            whiteActorTrees.Add(actor);
            actor.comp = Comp.White;
            actor.onDead += Actor_onDead;
        }
        for (int i = 0; i < blackCount; i++)
        {
            var obj = GameObject.Instantiate(blackPrefab) as GameObject;
            obj.SetActive(true);
            obj.transform.localScale = Vector3.one * 0.2f;
            obj.name = "black:" + i;
            var actor = obj.GetComponent<Actor>();
            blackActorTrees.Add(actor);
            actor.comp = Comp.Black;
            actor.onDead += Actor_onDead;
        }
    }

    private void Actor_onDead(Actor actor)
    {
        KdTree<Actor> targetTree;
        if (actor.comp == Comp.White)
        {
            targetTree = whiteActorTrees;
        }
        else
        {
            targetTree = blackActorTrees;
        }

        targetTree.Remove(actor);
        Debug.Log("left actor count:" + targetTree.Count + ", comp:" + actor.comp.ToString());

    }

    List<Actor> playerRangeActors = new List<Actor>();
    List<Actor> whiteRangeActors = null;
    List<Actor> blackRangeActors = null;
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    stopwatch.Reset();
        //    stopwatch.Start();
        foreach (var whiteActor in whiteActorTrees)
        {
            Actor nearestActor = blackActorTrees.FindClosest(whiteActor.m_transform.position);
            Debug.DrawLine(whiteActor.transform.position, nearestActor.transform.position, Color.red);
        }
        //stopwatch.Stop();
        //    Debug.Log("P used time:" + stopwatch.ElapsedMilliseconds);
        //}




        whiteActorTrees.UpdatePositions();
        blackActorTrees.UpdatePositions();



        foreach (var item in whiteActorTrees)
        {
            item.transform.localScale = Vector3.one;
        }
        foreach (var item in blackActorTrees)
        {
            item.transform.localScale = Vector3.one;
        }


        //visualize player range actors
        playerRangeActors.Clear();

        whiteActorTrees.FindRange(this.playerObj.transform.position, rangeRadius, ref whiteRangeActors);
        blackActorTrees.FindRange(this.playerObj.transform.position, rangeRadius, ref blackRangeActors);
        playerRangeActors.AddRange(whiteRangeActors);
        playerRangeActors.AddRange(blackRangeActors);
        if (playerRangeActors.Count > 0)
        {
            foreach (var item in playerRangeActors)
            {
                item.gameObject.transform.localScale = Vector3.one * 2f;
            }
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.playerObj.transform.position, rangeRadius);

    }
}
