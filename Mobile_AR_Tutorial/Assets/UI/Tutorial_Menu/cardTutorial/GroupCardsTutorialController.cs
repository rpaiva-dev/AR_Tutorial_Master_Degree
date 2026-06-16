using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class GroupCardsTutorialController : MonoBehaviour
{
    public void GroupCardsTutorialControllerConstructor(List<TutorialModel> listTutoriais, List<TutorialModel> completeTutorialList, Action<List<TutorialModel>> ActionReload)
    {
        GameObject card_Tutorial_1 = this.GetComponentsInChildren<Transform>(true)
                        .FirstOrDefault(txt => txt.name == "card_Tutorial_1").gameObject;
        GameObject card_Tutorial_2 = this.GetComponentsInChildren<Transform>(true)
                        .FirstOrDefault(txt => txt.name == "card_Tutorial_2").gameObject;

        if (listTutoriais.Count == 1)
        {
            Destroy(card_Tutorial_2);
            CardTutorialController card_Tutorial_1_controller = card_Tutorial_1.GetComponent<CardTutorialController>();
            card_Tutorial_1_controller.CardTutorialControllerConstructor(listTutoriais[0], completeTutorialList, ActionReload);

        }
        else
        {
            CardTutorialController card_Tutorial_1_controller = card_Tutorial_1.GetComponent<CardTutorialController>();
            card_Tutorial_1_controller.CardTutorialControllerConstructor(listTutoriais[0], completeTutorialList, ActionReload);

            CardTutorialController card_Tutorial_2_controller = card_Tutorial_2.GetComponent<CardTutorialController>();
            card_Tutorial_2_controller.CardTutorialControllerConstructor(listTutoriais[1], completeTutorialList, ActionReload);
        }
    }
}
