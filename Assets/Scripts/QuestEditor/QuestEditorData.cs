﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This class manages the Quest editor Interface
// FIXME: Rename, not a good name any more
public class QuestEditorData {
    // When a selection list is raised it is stored here
    // This allows the return value to be fetched later
    public EditorSelectionList esl;
    // This is the currently selected component
    public EditorComponent selection;
    // The selection stack is used for the 'back' button
    public Stack<EditorComponent> selectionStack;

    // Start the editor
    public QuestEditorData()
    {
        Game.Get().qed = this;       
        selectionStack = new Stack<EditorComponent>();
        // Start at the quest component
        SelectQuest();
    }

    // Update component selection
    // Used to save selection history
    public void NewSelection(EditorComponent c)
    {
        // selection starts at null
        if (selection != null)
        {
            selectionStack.Push(selection);
        }
        selection = c;
    }

    // Go back in the selection stack
    public static void Back()
    {
        Game game = Game.Get();
        // Check if there is anything to go back to
        if (game.qed.selectionStack.Count == 0)
        {
            // Reset on quest selection
            game.qed.selection = new EditorComponentQuest();
            return;
        }
        game.qed.selection = game.qed.selectionStack.Pop();
        // Quest is OK
        if (game.qed.selection is EditorComponentQuest)
        {
            game.qed.selection.Update();
        }
        else if (game.quest.qd.components.ContainsKey(game.qed.selection.name))
        {
            // Existing component
            game.qed.selection.Update();
        }
        else
        {
            // History item has been deleted/renamed, go back further
            Back();
        }
    }

    // Open component selection top level
    // Menu for selection of all component types, includes delete options
    public static void TypeSelect()
    {
        Game game = Game.Get();
        if (GameObject.FindGameObjectWithTag("dialog") != null)
        {
            return;
        }

        // Border
        DialogBox db = new DialogBox(new Vector2(21, 0), new Vector2(18, 18), "");
        db.AddBorder();

        // Heading
        db = new DialogBox(new Vector2(21, 0), new Vector2(17, 1), "Select Type");

        // Buttons for each component type (and delete buttons)
        TextButton tb = new TextButton(new Vector2(22, 2), new Vector2(9, 1), "Quest", delegate { SelectQuest(); });
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();

        tb = new TextButton(new Vector2(22, 4), new Vector2(9, 1), "Tile", delegate { ListTile(); });
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();

        tb = new TextButton(new Vector2(32, 4), new Vector2(6, 1), "Delete", delegate { game.qed.DeleteComponent("Tile"); }, Color.red);
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();

        tb = new TextButton(new Vector2(22, 6), new Vector2(9, 1), "Door", delegate { ListDoor(); });
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();

        tb = new TextButton(new Vector2(32, 6), new Vector2(6, 1), "Delete", delegate { game.qed.DeleteComponent("Door"); }, Color.red);
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();

        tb = new TextButton(new Vector2(22, 8), new Vector2(9, 1), "Token", delegate { ListToken(); });
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();

        tb = new TextButton(new Vector2(32, 8), new Vector2(6, 1), "Delete", delegate { game.qed.DeleteComponent("Token"); }, Color.red);
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();

        tb = new TextButton(new Vector2(22, 10), new Vector2(9, 1), "Monster", delegate { ListMonster(); });
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();

        tb = new TextButton(new Vector2(32, 10), new Vector2(6, 1), "Delete", delegate { game.qed.DeleteComponent("Monster"); }, Color.red);
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();

        tb = new TextButton(new Vector2(22, 12), new Vector2(9, 1), "MPlace", delegate { ListMPlace(); });
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();

        tb = new TextButton(new Vector2(32, 12), new Vector2(6, 1), "Delete", delegate { game.qed.DeleteComponent("MPlace"); }, Color.red);
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();

        tb = new TextButton(new Vector2(22, 14), new Vector2(9, 1), "Event", delegate { ListEvent(); });
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();

        tb = new TextButton(new Vector2(32, 14), new Vector2(6, 1), "Delete", delegate { game.qed.DeleteComponent("Event"); }, Color.red);
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();

        tb = new TextButton(new Vector2(25.5f, 16), new Vector2(9, 1), "Cancel", delegate { Cancel(); });
        tb.background.GetComponent<UnityEngine.UI.Image>().color = new Color(0.03f, 0.0f, 0f);
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();
    }

    // Create selection list for tiles
    public static void ListTile()
    {
        Game game = Game.Get();

        List<string> tiles = new List<string>();
        // This magic string is picked up later for object creation
        tiles.Add("{NEW:Tile}");
        foreach (KeyValuePair<string, QuestData.QuestComponent> kv in game.quest.qd.components)
        {
            if (kv.Value is QuestData.Tile)
            {
                tiles.Add(kv.Key);
            }
        }

        if (tiles.Count == 0)
        {
            return;
        }
        game.qed.esl = new EditorSelectionList("Select Item", tiles, delegate { game.qed.SelectComponent(); });
        game.qed.esl.SelectItem();
    }

    // Create selection list for Doors
    public static void ListDoor()
    {
        Game game = Game.Get();

        List<string> doors = new List<string>();
        // This magic string is picked up later for object creation
        doors.Add("{NEW:Door}");
        foreach (KeyValuePair<string, QuestData.QuestComponent> kv in game.quest.qd.components)
        {
            if (kv.Value is QuestData.Door)
            {
                doors.Add(kv.Key);
            }
        }

        if (doors.Count == 0)
        {
            return;
        }
        game.qed.esl = new EditorSelectionList("Select Item", doors, delegate { game.qed.SelectComponent(); });
        game.qed.esl.SelectItem();
    }

    // Create selection list for tokens
    public static void ListToken()
    {
        Game game = Game.Get();

        List<string> tokens = new List<string>();
        // This magic string is picked up later for object creation
        tokens.Add("{NEW:Token}");
        foreach (KeyValuePair<string, QuestData.QuestComponent> kv in game.quest.qd.components)
        {
            if (kv.Value is QuestData.Token)
            {
                tokens.Add(kv.Key);
            }
        }

        if (tokens.Count == 0)
        {
            return;
        }
        game.qed.esl = new EditorSelectionList("Select Item", tokens, delegate { game.qed.SelectComponent(); });
        game.qed.esl.SelectItem();
    }

    // Create selection list for monsters
    public static void ListMonster()
    {
        Game game = Game.Get();

        List<string> monsters = new List<string>();
        // This magic string is picked up later for object creation
        monsters.Add("{NEW:Monster}");
        foreach (KeyValuePair<string, QuestData.QuestComponent> kv in game.quest.qd.components)
        {
            if (kv.Value is QuestData.Monster)
            {
                monsters.Add(kv.Key);
            }
        }

        if (monsters.Count == 0)
        {
            return;
        }
        game.qed.esl = new EditorSelectionList("Select Item", monsters, delegate { game.qed.SelectComponent(); });
        game.qed.esl.SelectItem();
    }

    // Create selection list for mplaces
    public static void ListMPlace()
    {
        Game game = Game.Get();

        List<string> mplaces = new List<string>();
        // This magic string is picked up later for object creation
        mplaces.Add("{NEW:MPlace}");
        foreach (KeyValuePair<string, QuestData.QuestComponent> kv in game.quest.qd.components)
        {
            if (kv.Value is QuestData.MPlace)
            {
                mplaces.Add(kv.Key);
            }
        }

        if (mplaces.Count == 0)
        {
            return;
        }
        game.qed.esl = new EditorSelectionList("Select Item", mplaces, delegate { game.qed.SelectComponent(); });
        game.qed.esl.SelectItem();
    }

    // Create selection list for events
    public static void ListEvent()
    {
        Game game = Game.Get();

        List<string> events = new List<string>();
        // This magic string is picked up later for object creation
        events.Add("{NEW:Event}");
        foreach (KeyValuePair<string, QuestData.QuestComponent> kv in game.quest.qd.components)
        {
            if (kv.Value is QuestData.Event)
            {
                if (!kv.Value.GetType().IsSubclassOf(typeof(QuestData.Event)))
                {
                    events.Add(kv.Key);
                }
            }
        }

        if (events.Count == 0)
        {
            return;
        }
        game.qed.esl = new EditorSelectionList("Select Item", events, delegate { game.qed.SelectComponent(); });
        game.qed.esl.SelectItem();
    }

    // Select a component from a list
    public void SelectComponent()
    {
        SelectComponent(esl.selection);
    }

    // Select a component for editing
    public static void SelectComponent(string name)
    {
        Game game = Game.Get();
        QuestEditorData qed = game.qed;

        // Quest is a special component
        if (name.Equals("Quest"))
        {
            SelectQuest();
            return;
        }
        // These are special strings for creating new objects
        if (name.Equals("{NEW:Tile}"))
        {
            qed.NewTile();
            return;
        }
        if (name.Equals("{NEW:Door}"))
        {
            qed.NewDoor();
            return;
        }
        if (name.Equals("{NEW:Token}"))
        {
            qed.NewToken();
            return;
        }
        if (name.Equals("{NEW:Monster}"))
        {
            qed.NewMonster();
            return;
        }
        if (name.Equals("{NEW:MPlace}"))
        {
            qed.NewMPlace();
            return;
        }
        if (name.Equals("{NEW:Event}"))
        {
            qed.NewEvent();
            return;
        }

        // This may happen to due rename/delete
        if (!game.quest.qd.components.ContainsKey(name))
        {
            SelectQuest();
        }

        // Determine the component type and select
        if (game.quest.qd.components[name] is QuestData.Tile)
        {
            SelectAsTile(name);
            return;
        }

        if (game.quest.qd.components[name] is QuestData.Door)
        {
            SelectAsDoor(name);
            return;
        }
        if (game.quest.qd.components[name] is QuestData.Token)
        {
            SelectAsToken(name);
            return;
        }
        if (game.quest.qd.components[name] is QuestData.Monster)
        {
            SelectAsMonster(name);
            return;
        }
        if (game.quest.qd.components[name] is QuestData.MPlace)
        {
            SelectAsMPlace(name);
            return;
        }
        if (game.quest.qd.components[name] is QuestData.Event)
        {
            SelectAsEvent(name);
            return;
        }
    }

    public static void SelectQuest()
    {
        Game game = Game.Get();
        game.qed.NewSelection(new EditorComponentQuest());
    }

    public static void SelectAsTile(string name)
    {
        Game game = Game.Get();
        game.qed.NewSelection(new EditorComponentTile(name));
    }

    public static void SelectAsDoor(string name)
    {
        Game game = Game.Get();
        game.qed.NewSelection(new EditorComponentDoor(name));
    }

    public static void SelectAsToken(string name)
    {
        Game game = Game.Get();
        game.qed.NewSelection(new EditorComponentToken(name));
    }

    // Events, tokens, doors and monsters can all be openned as events
    // and as EventPageTwo
    public static void SelectAsEvent(string name)
    {
        Game game = Game.Get();
        game.qed.NewSelection(new EditorComponentEvent(name));
    }

    // Events, tokens, doors and monsters can all be openned as events
    // and as EventPageTwo
    public static void SelectAsEventPageTwo(string name)
    {
        Game game = Game.Get();
        game.qed.NewSelection(new EditorComponentEventPageTwo(name));
    }

    public static void SelectAsMonster(string name)
    {
        Game game = Game.Get();
        game.qed.NewSelection(new EditorComponentMonster(name));
    }

    // Mosters can be opened as a placement list page
    public static void SelectAsMonsterPlacement(string name)
    {
        Game game = Game.Get();
        game.qed.NewSelection(new EditorComponentMonsterPlacement(name));
    }

    public static void SelectAsMPlace(string name)
    {
        Game game = Game.Get();
        game.qed.NewSelection(new EditorComponentMPlace(name));
    }

    // Create a new tile, use next available number
    public void NewTile()
    {
        Game game = Game.Get();
        int index = 0;

        while (game.quest.qd.components.ContainsKey("Tile" + index))
        {
            index++;
        }
        game.quest.qd.components.Add("Tile" + index, new QuestData.Tile("Tile" + index));
        game.quest.Add("Tile" + index);
        SelectComponent("Tile" + index);
    }

    public void NewDoor()
    {
        Game game = Game.Get();
        int index = 0;

        while (game.quest.qd.components.ContainsKey("Door" + index))
        {
            index++;
        }
        game.quest.qd.components.Add("Door" + index, new QuestData.Door("Door" + index));
        game.quest.Add("Door" + index);
        SelectComponent("Door" + index);
    }

    public void NewToken()
    {
        Game game = Game.Get();
        int index = 0;

        while (game.quest.qd.components.ContainsKey("Token" + index))
        {
            index++;
        }
        game.quest.qd.components.Add("Token" + index, new QuestData.Token("Token" + index));
        game.quest.Add("Token" + index);
        SelectComponent("Token" + index);
    }

    public void NewMonster()
    {
        Game game = Game.Get();
        int index = 0;

        while (game.quest.qd.components.ContainsKey("Monster" + index))
        {
            index++;
        }
        game.quest.qd.components.Add("Monster" + index, new QuestData.Monster("Monster" + index));
        SelectComponent("Monster" + index);
    }

    public void NewMPlace()
    {
        Game game = Game.Get();
        int index = 0;

        while (game.quest.qd.components.ContainsKey("MPlace" + index))
        {
            index++;
        }
        game.quest.qd.components.Add("MPlace" + index, new QuestData.MPlace("MPlace" + index));
        SelectComponent("MPlace" + index);
    }

    public void NewEvent()
    {
        Game game = Game.Get();
        int index = 0;

        while (game.quest.qd.components.ContainsKey("Event" + index))
        {
            index++;
        }
        game.quest.qd.components.Add("Event" + index, new QuestData.Event("Event" + index));
        SelectComponent("Event" + index);
    }

    // Delete a component by type
    public void DeleteComponent(string type)
    {
        Game game = Game.Get();

        List<string> toDelete = new List<string>();

        // List all components of this type
        foreach (KeyValuePair<string, QuestData.QuestComponent> kv in game.quest.qd.components)
        {
            if (kv.Key.IndexOf(type) == 0)
            {
                toDelete.Add(kv.Key);
                toDelete.Add("");
            }
        }
        // Create list for user
        esl = new EditorSelectionList("Component to Delete:", toDelete, delegate { SelectToDelete(); });
        esl.SelectItem();
    }

    // Delete a component
    public void DeleteComponent()
    {
        Game game = Game.Get();

        List<string> toDelete = new List<string>();

        // List all components
        foreach (KeyValuePair<string, QuestData.QuestComponent> kv in game.quest.qd.components)
        {
            toDelete.Add(kv.Key);
            toDelete.Add("");
        }
        // Create list for user
        esl = new EditorSelectionList("Component to Delete:", toDelete, delegate { SelectToDelete(); });
        esl.SelectItem();
    }

    // Item selected from list for deletion
    public void SelectToDelete()
    {
        Game game = Game.Get();

        // Remove all references to the deleted component
        foreach (KeyValuePair<string, QuestData.QuestComponent> kv in game.quest.qd.components)
        {
            kv.Value.RemoveReference(esl.selection);
        }

        // Remove the component
        if (game.quest.qd.components.ContainsKey(esl.selection))
        {
            game.quest.qd.components.Remove(esl.selection);
        }

        // Clean up the current quest environment
        game.quest.Remove(esl.selection);

        Destroyer.Dialog();

        // If we deleted the selected item, go back to the last item
        if (selection.name.Equals(esl.selection))
        {
            Back();
        }
    }

    // Cancel a component selection, clean up
    public static void Cancel()
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("dialog"))
            Object.Destroy(go);
    }

    // This is called game
    public void MouseDown()
    {
        selection.MouseDown();
    }
}
