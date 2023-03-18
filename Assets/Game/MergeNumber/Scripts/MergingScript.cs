using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MergingScript : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(this.gameObject.tag == "fallingBlock")
        {
            if((collision.gameObject.tag == "stableBlock" || collision.gameObject.tag == "Base") && !GameManager2.instance.isMerging)
            {
                GameManager2.instance.isMerging = true;
                this.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                StartCoroutine(startMerge(this.gameObject));
                this.gameObject.GetComponent<Rigidbody2D>().gravityScale = 2;
                GameManager2.instance.playThisSound(GameManager2.instance.fx3);
            }
        }
        if(this.gameObject.tag == "stableBlock" && !GameManager2.instance.isMerging && this.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude !=0)
        {
            GameManager2.instance.isMerging = true;
            this.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            StartCoroutine(startMerge(this.gameObject));
            this.gameObject.GetComponent<Rigidbody2D>().gravityScale = 2;
            GameManager2.instance.playThisSound(GameManager2.instance.fx3);
        }
    }
    IEnumerator startMerge(GameObject block)
    {
        yield return new WaitForSeconds(0.1f);
        GameManager2.instance.mergeNow(block);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (this.gameObject.tag == "fallingBlock")
        {
            if ((collision.gameObject.tag == "stableBlock" || collision.gameObject.tag == "Base") && !GameManager2.instance.isMerging)
            {
                GameManager2.instance.isMerging = true;
                this.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                StartCoroutine(startMerge(this.gameObject));
                this.gameObject.GetComponent<Rigidbody2D>().gravityScale = 2;
                GameManager2.instance.playThisSound(GameManager2.instance.fx3);
            }
        }
    }
    private void FixedUpdate()
    {
        if (this.gameObject.tag == "stableBlock")
        {
            if(this.gameObject.transform.position.y > GameManager2.instance.deadline_.transform.position.y - 1)
            {
                if(!GameManager2.instance.gameOverPanel.gameObject.activeInHierarchy)
                {
                    //GameManager2.instance.sendHighscore(GameManager2.instance.score);
                    GameManager2.instance.isGameOver = true;
                    GameManager2.instance.gameOver();
                }
                
            }
        }
    }
    
}
