﻿using UnityEngine;
using System.Collections;

// Class for creation of monster seleciton options
// Extends the standard class for MoM
public class MonsterDialogMoM : MonsterDialog
{
    public MonsterDialogMoM(Quest.Monster m) : base(m)
    {
    }

    public override void CreateWindow()
    {
        Game game = Game.Get();
        // Get the offset position of the monster
        int index = 0;
        for (int i = 0; i < game.quest.monsters.Count; i++)
        {
            if (game.quest.monsters[i] == monster)
            {
                index = i;
            }
        }

        float offset = (index + 0.1f - game.monsterCanvas.offset) * (MonsterCanvas.monsterSize + 0.5f);

        // In horror phase we do horror checks
        if (game.quest.phase == Quest.MoMPhase.horror)
        {
            new TextButton(new Vector2(UIScaler.GetRight(-10.5f - MonsterCanvas.monsterSize), offset), new Vector2(10, 2), "Horror Check", delegate { Horror(); });
            new TextButton(new Vector2(UIScaler.GetRight(-10.5f - MonsterCanvas.monsterSize), offset + 2.5f), new Vector2(10, 2), "Cancel", delegate { OnCancel(); });
        }
        else
        { // In investigator phase we do attacks and evades
            new TextButton(new Vector2(UIScaler.GetRight(-10.5f - MonsterCanvas.monsterSize), offset), new Vector2(10, 2), "Attack", delegate { Attack(); });
            new TextButton(new Vector2(UIScaler.GetRight(-10.5f - MonsterCanvas.monsterSize), offset + 2.5f), new Vector2(10, 2), "Evade", delegate { Evade(); });
            new TextButton(new Vector2(UIScaler.GetRight(-10.5f - MonsterCanvas.monsterSize), offset + 5f), new Vector2(10, 2), "Cancel", delegate { OnCancel(); });
        }
    }

    public void Attack()
    {
        Game game = Game.Get();
        // Save to undo stack
        game.quest.Save();
        new InvestigatorAttack(monster);
    }

    public void Evade()
    {
        new InvestigatorEvade(monster);
    }

    public void Horror()
    {
        new HorrorCheck(monster);
    }
}
