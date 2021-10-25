using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_2 : MonoBehaviour
{
    public PlayerController controller;

    private void Update()
    {
        if (controller.activateAbility)
        {
            switch (controller.abilityInput)
            {
                case AbilityInputs.Q:
                    Q_Ability();
                    break;
                case AbilityInputs.V:
                    W_Ability();
                    break;
                case AbilityInputs.E:
                    E_Ability();
                    break;
                case AbilityInputs.R:
                    R_Ability();
                    break;
                case AbilityInputs.F:
                    F_Ability();
                    break;
            }
            controller.activateAbility = false;
            controller.isAbilitySelected = false;
        }
    }

    public void Q_Ability()
    {
        GameObject ball = Instantiate(controller.abilities[0].abilityObject, controller.firePoint.position, controller.firePoint.rotation);
        ball.GetComponent<SkillShot>().MovingDirection(controller.lookDir.normalized);
        ball.GetComponent<SkillShot>().playerType = controller.playerType;
        ball.GetComponent<SkillShot>().player = gameObject;
    }

    public void W_Ability()
    {
        
    }

    public void E_Ability()
    {
        GameObject ball = Instantiate(controller.abilities[2].abilityObject, controller.firePoint.position, controller.firePoint.rotation);
        ball.GetComponent<SkillShot>().MovingDirection(controller.lookDir.normalized);
        ball.GetComponent<SkillShot>().playerType = controller.playerType;
        ball.GetComponent<SkillShot>().player = gameObject;
    }

    public void R_Ability()
    {
        
    }

    public void F_Ability()
    {
        
    }
}
