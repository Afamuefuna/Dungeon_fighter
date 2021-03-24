using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation : MonoBehaviour
{
    public Animator _PlayerAnim;

    public void playerShield(bool _isPlayerShielding)
    {
        if (_isPlayerShielding == true)
        {
            _PlayerAnim.SetBool("Shield", true);
        }
        else if (_isPlayerShielding == false)
        {
            _PlayerAnim.SetBool("Shield", false);
        }
    }

    public void PlayerHit()
    {
        _PlayerAnim.SetTrigger("Hit");
    }

    public void playerJump(bool condition)
    {
        _PlayerAnim.SetBool("Jumping", condition);
    }

    public void move(float move)
    {
        _PlayerAnim.SetFloat("Run", Mathf.Abs(move));
    }

    public void playerAttack()
    {
        _PlayerAnim.SetTrigger("Attack");
    }

    public void death()
    {
        _PlayerAnim.SetTrigger("death");
    }

}
