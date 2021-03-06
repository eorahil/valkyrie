using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EditorComponentEventPageTwo : EditorComponent
{
    QuestData.Event eventComponent;
    DialogBoxEditable confirmDBE;
    DialogBoxEditable failDBE;
    List<DialogBoxEditable> delayedEventsDBE;
    EditorSelectionList addEventESL;
    EditorSelectionList delayedEventESL;
    EditorSelectionList flagsESL;
    QuestEditorTextEdit newFlagText;

    public EditorComponentEventPageTwo(string nameIn) : base()
    {
        Game game = Game.Get();
        eventComponent = game.quest.qd.components[nameIn] as QuestData.Event;
        component = eventComponent;
        name = component.name;
        Update();
    }
    
    override public void Update()
    {
        base.Update();
        if (eventComponent.locationSpecified)
        {
            CameraController.SetCamera(eventComponent.location);
        }
        Game game = Game.Get();

        string type = QuestData.Event.type;
        if (eventComponent is QuestData.Door)
        {
            type = QuestData.Door.type;
        }
        if (eventComponent is QuestData.Monster)
        {
            type = QuestData.Monster.type;
        }
        if (eventComponent is QuestData.Token)
        {
            type = QuestData.Token.type;
        }

        TextButton tb = new TextButton(new Vector2(0, 0), new Vector2(4, 1), type, delegate { QuestEditorData.TypeSelect(); });
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();
        tb.button.GetComponent<UnityEngine.UI.Text>().alignment = TextAnchor.MiddleRight;
        tb.ApplyTag("editor");

        tb = new TextButton(new Vector2(4, 0), new Vector2(15, 1), name.Substring(type.Length), delegate { QuestEditorData.ListEvent(); });
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();
        tb.button.GetComponent<UnityEngine.UI.Text>().alignment = TextAnchor.MiddleLeft;
        tb.ApplyTag("editor");

        tb = new TextButton(new Vector2(19, 0), new Vector2(1, 1), "E", delegate { Rename(); });
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();
        tb.ApplyTag("editor");

        string randomButton = "Ordered";
        if (eventComponent.randomEvents) randomButton = "Random";
        tb = new TextButton(new Vector2(0, 1), new Vector2(3, 1), randomButton, delegate { ToggleRandom(); });
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();
        tb.ApplyTag("editor");

        DialogBox db = new DialogBox(new Vector2(3, 1), new Vector2(10, 1), "Trigger Events:");
        db.ApplyTag("editor");

        string confirmLabel = eventComponent.confirmText;
        if (confirmLabel.Equals(""))
        {
            confirmLabel = "Confirm";
            if (eventComponent.failEvent.Length != 0)
            {
                confirmLabel = "Pass";
            }
        }
        confirmDBE = new DialogBoxEditable(new Vector2(11, 1), new Vector2(6, 1), confirmLabel, delegate { UpdateConfirmLabel(); });
        confirmDBE.ApplyTag("editor");
        confirmDBE.AddBorder();

        tb = new TextButton(new Vector2(19, 1), new Vector2(1, 1), "+", delegate { AddEvent(0); }, Color.green);
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();
        tb.ApplyTag("editor");

        int offset = 2;
        int index;
        for (index = 0; index < 8; index++)
        {
            if (eventComponent.nextEvent.Length > index)
            {
                int i = index;
                tb = new TextButton(new Vector2(0, offset), new Vector2(1, 1), "-", delegate { RemoveEvent(i); }, Color.red);
                tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();
                tb.ApplyTag("editor");
                db = new DialogBox(new Vector2(1, offset), new Vector2(18, 1), eventComponent.nextEvent[index]);
                db.AddBorder();
                db.ApplyTag("editor");
                tb = new TextButton(new Vector2(19, offset++), new Vector2(1, 1), "+", delegate { AddEvent(i + 1); }, Color.green);
                tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();
                tb.ApplyTag("editor");
            }
        }

        offset++;
        db = new DialogBox(new Vector2(1, offset), new Vector2(10, 1), "Fail Events:");
        db.ApplyTag("editor");

        string failLabel = eventComponent.failText;
        if (failLabel.Equals(""))
        {
            failLabel = "Fail";
        }
        failDBE = new DialogBoxEditable(new Vector2(11, offset), new Vector2(6, 1), failLabel, delegate { UpdateFailLabel(); });
        failDBE.ApplyTag("editor");
        failDBE.AddBorder();

        tb = new TextButton(new Vector2(19, offset++), new Vector2(1, 1), "+", delegate { AddEvent(0, true); }, Color.green);
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();
        tb.ApplyTag("editor");

        for (index = 0; index < 12; index++)
        {
            if (eventComponent.failEvent.Length > index)
            {
                int i = index;
                tb = new TextButton(new Vector2(0, offset), new Vector2(1, 1), "-", delegate { RemoveEvent(i, true); }, Color.red);
                tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();
                tb.ApplyTag("editor");
                db = new DialogBox(new Vector2(1, offset), new Vector2(18, 1), eventComponent.failEvent[index]);
                db.AddBorder();
                db.ApplyTag("editor");
                tb = new TextButton(new Vector2(19, offset++), new Vector2(1, 1), "+", delegate { AddEvent(i + 1, true); }, Color.green);
                tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();
                tb.ApplyTag("editor");
            }
        }

        offset++;
        db = new DialogBox(new Vector2(1, offset), new Vector2(10, 1), "Delayed Events:");
        db.ApplyTag("editor");

        tb = new TextButton(new Vector2(19, offset++), new Vector2(1, 1), "+", delegate { AddDelayedEvent(); }, Color.green);
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();
        tb.ApplyTag("editor");

        index = 0;
        delayedEventsDBE = new List<DialogBoxEditable>();
        foreach (QuestData.Event.DelayedEvent de in eventComponent.delayedEvents)
        {
            int i = index++;

            DialogBoxEditable dbeDelay = new DialogBoxEditable(new Vector2(0, offset), new Vector2(2, 1), de.delay.ToString(), delegate { SetDelayedEvent(i); });
            dbeDelay.ApplyTag("editor");
            dbeDelay.AddBorder();
            delayedEventsDBE.Add(dbeDelay);

            db = new DialogBox(new Vector2(2, offset), new Vector2(17, 1), de.eventName);
            db.AddBorder();
            db.ApplyTag("editor");
            tb = new TextButton(new Vector2(19, offset++), new Vector2(1, 1), "-", delegate { RemoveDelayedEvent(i); }, Color.red);
            tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();
            tb.ApplyTag("editor");
        }

        offset++;
        db = new DialogBox(new Vector2(0, offset), new Vector2(5, 1), "Flags:");
        db.ApplyTag("editor");

        tb = new TextButton(new Vector2(5, offset), new Vector2(1, 1), "+", delegate { AddFlag("flag"); }, Color.green);
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();
        tb.ApplyTag("editor");

        db = new DialogBox(new Vector2(7, offset), new Vector2(5, 1), "Set:");
        db.ApplyTag("editor");

        tb = new TextButton(new Vector2(12, offset), new Vector2(1, 1), "+", delegate { AddFlag("set"); }, Color.green);
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();
        tb.ApplyTag("editor");

        db = new DialogBox(new Vector2(14, offset), new Vector2(5, 1), "Clear:");
        db.ApplyTag("editor");

        tb = new TextButton(new Vector2(19, offset++), new Vector2(1, 1), "+", delegate { AddFlag("clear"); }, Color.green);
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();
        tb.ApplyTag("editor");

        for (index = 0; index < 8; index++)
        {
            if (eventComponent.flags.Length > index)
            {
                int i = index;
                db = new DialogBox(new Vector2(0, offset + index), new Vector2(5, 1), eventComponent.flags[index]);
                db.AddBorder();
                db.ApplyTag("editor");
                tb = new TextButton(new Vector2(5, offset + index), new Vector2(1, 1), "-", delegate { FlagRemove(i); }, Color.red);
                tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();
                tb.ApplyTag("editor");
            }
        }

        for (index = 0; index < 8; index++)
        {
            if (eventComponent.setFlags.Length > index)
            {
                int i = index;
                db = new DialogBox(new Vector2(7, offset + index), new Vector2(5, 1), eventComponent.setFlags[index]);
                db.AddBorder();
                db.ApplyTag("editor");
                tb = new TextButton(new Vector2(12, offset + index), new Vector2(1, 1), "-", delegate { FlagSetRemove(i); }, Color.red);
                tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();
                tb.ApplyTag("editor");
            }
        }

        for (index = 0; index < 8; index++)
        {
            if (eventComponent.clearFlags.Length > index)
            {
                int i = index;
                db = new DialogBox(new Vector2(14, offset + index), new Vector2(5, 1), eventComponent.clearFlags[index]);
                db.AddBorder();
                db.ApplyTag("editor");
                tb = new TextButton(new Vector2(19, offset + index), new Vector2(1, 1), "-", delegate { FlagClearRemove(i); }, Color.red);
                tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();
                tb.ApplyTag("editor");
            }
        }

        if (eventComponent.locationSpecified)
        {
            game.tokenBoard.AddHighlight(eventComponent.location, "EventLoc", "editor");
        }
    }

    public void ToggleRandom()
    {
        eventComponent.randomEvents = !eventComponent.randomEvents;
        Update();
    }


    public void UpdateConfirmLabel()
    {
        eventComponent.confirmText = confirmDBE.uiInput.text;
        if (eventComponent.confirmText.Equals("Confirm") && eventComponent.failEvent.Length == 0)
        {
            eventComponent.confirmText = "";
        }
        if (eventComponent.confirmText.Equals("Pass") && eventComponent.failEvent.Length != 0)
        {
            eventComponent.confirmText = "";
        }
    }

    public void UpdateFailLabel()
    {
        eventComponent.failText = failDBE.uiInput.text;
        if (eventComponent.failText.Equals("Fail"))
        {
            eventComponent.failText = "";
        }
    }

    public void AddEvent(int index, bool fail = false)
    {
        List<string> events = new List<string>();

        Game game = Game.Get();
        foreach (KeyValuePair<string, QuestData.QuestComponent> kv in game.quest.qd.components)
        {
            if (kv.Value is QuestData.Event)
            {
                events.Add(kv.Key);
            }
        }

        addEventESL = new EditorSelectionList("Select Event", events, delegate { SelectAddEvent(index, fail); });
        addEventESL.SelectItem();
    }

    public void SelectAddEvent(int index, bool fail)
    {
        if (fail)
        {
            System.Array.Resize(ref eventComponent.failEvent, eventComponent.failEvent.Length + 1);

            for (int i = eventComponent.failEvent.Length - 1; i >= 0; i--)
            {
                if (i > index)
                {
                    eventComponent.failEvent[i] = eventComponent.failEvent[i - 1];
                }
                if (i == index)
                {
                    eventComponent.failEvent[i] = addEventESL.selection;
                }
            }
        }
        else
        {
            System.Array.Resize(ref eventComponent.nextEvent, eventComponent.nextEvent.Length + 1);

            for (int i = eventComponent.nextEvent.Length - 1; i >= 0; i--)
            {
                if (i > index)
                {
                    eventComponent.nextEvent[i] = eventComponent.nextEvent[i - 1];

                }
                if (i == index)
                {
                    eventComponent.nextEvent[i] = addEventESL.selection;
                }
            }
        }
        Update();
    }

    public void RemoveEvent(int index, bool fail = false)
    {
        if (fail)
        {
            string[] events = new string[eventComponent.failEvent.Length - 1];

            int j = 0;
            for (int i = 0; i <  eventComponent.failEvent.Length; i++)
            {
                if (i != index)
                {
                    events[j++] = eventComponent.failEvent[i];
                }
            }
            eventComponent.failEvent = events;
        }
        else
        {
            string[] events = new string[eventComponent.nextEvent.Length - 1];

            int j = 0;
            for (int i = 0; i < eventComponent.nextEvent.Length; i++)
            {
                if (i != index)
                {
                    events[j++] = eventComponent.nextEvent[i];
                }
            }
            eventComponent.nextEvent = events;
        }
        Update();
    }

    public void AddDelayedEvent()
    {
        List<string> events = new List<string>();

        Game game = Game.Get();
        foreach (KeyValuePair<string, QuestData.QuestComponent> kv in game.quest.qd.components)
        {
            if (kv.Value is QuestData.Event)
            {
                events.Add(kv.Key);
            }
        }

        delayedEventESL = new EditorSelectionList("Select Event", events, delegate { SelectAddDelayedEvent(); });
        delayedEventESL.SelectItem();
    }

    public void SelectAddDelayedEvent()
    {
        eventComponent.delayedEvents.Add(new QuestData.Event.DelayedEvent(0, delayedEventESL.selection));
        Update();
    }

    public void SetDelayedEvent(int i)
    {
        int.TryParse(delayedEventsDBE[i].uiInput.text, out eventComponent.delayedEvents[i].delay);
        Update();
    }

    public void RemoveDelayedEvent(int i)
    {
        eventComponent.delayedEvents.RemoveAt(i);
        Update();
    }

    public void AddFlag(string type)
    {
        HashSet<string> flags = new HashSet<string>();
        flags.Add("{NEW}");

        Game game = Game.Get();
        foreach (KeyValuePair<string, QuestData.QuestComponent> kv in game.quest.qd.components)
        {
            if (kv.Value is QuestData.Event)
            {
                QuestData.Event e = kv.Value as QuestData.Event;
                foreach (string s in e.flags)
                {
                    if (s.IndexOf("#") != 0) flags.Add(s);
                }
                foreach (string s in e.setFlags) flags.Add(s);
                foreach (string s in e.clearFlags) flags.Add(s);
            }
        }

        if (type.Equals("flag"))
        {
            flags.Add("#monsters");
            flags.Add("#2hero");
            flags.Add("#3hero");
            flags.Add("#4hero");
            flags.Add("#5hero");
            foreach (ContentData.ContentPack pack in Game.Get().cd.allPacks)
            {
                if (pack.id.Length > 0)
                {
                    flags.Add("#" + pack.id);
                }
            }
        }

        List<string> list = new List<string>(flags);
        flagsESL = new EditorSelectionList("Select Flag", list, delegate { SelectAddFlag(type); });
        flagsESL.SelectItem();
    }

    public void SelectAddFlag(string type)
    {
        if (flagsESL.selection.Equals("{NEW}"))
        {
            newFlagText = new QuestEditorTextEdit("Flag Name:", "", delegate { NewFlag(type); });
            newFlagText.EditText();
        }
        else
        {
            SelectAddFlag(type, flagsESL.selection);
        }
    }

    public void SelectAddFlag(string type, string name)
    {
        if (name.Equals("")) return;
        if (type.Equals("flag"))
        {
            System.Array.Resize(ref eventComponent.flags, eventComponent.flags.Length + 1);
            eventComponent.flags[eventComponent.flags.Length - 1] = name;
        }

        if (type.Equals("set"))
        {
            System.Array.Resize(ref eventComponent.setFlags, eventComponent.setFlags.Length + 1);
            eventComponent.setFlags[eventComponent.setFlags.Length - 1] = name;
        }

        if (type.Equals("clear"))
        {
            System.Array.Resize(ref eventComponent.clearFlags, eventComponent.clearFlags.Length + 1);
            eventComponent.clearFlags[eventComponent.clearFlags.Length - 1] = name;
        }
        Update();
    }

    public void NewFlag(string type)
    {
        string name = System.Text.RegularExpressions.Regex.Replace(newFlagText.value, "[^A-Za-z0-9_]", "");
        SelectAddFlag(type, name);
    }

    public void FlagRemove(int index)
    {
        string[] flags = new string[eventComponent.flags.Length - 1];
        int j = 0;
        for (int i = 0; i < eventComponent.flags.Length; i++)
        {
            if (i != index)
            {
                flags[j++] = eventComponent.flags[i];
            }
        }
        eventComponent.flags = flags;
        Update();
    }

    public void FlagSetRemove(int index)
    {
        string[] flags = new string[eventComponent.setFlags.Length - 1];
        int j = 0;
        for (int i = 0; i < eventComponent.setFlags.Length; i++)
        {
            if (i != index)
            {
                flags[j++] = eventComponent.setFlags[i];
            }
        }
        eventComponent.setFlags = flags;
        Update();
    }

    public void FlagClearRemove(int index)
    {
        string[] flags = new string[eventComponent.clearFlags.Length - 1];
        int j = 0;
        for (int i = 0; i < eventComponent.clearFlags.Length; i++)
        {
            if (i != index)
            {
                flags[j++] = eventComponent.clearFlags[i];
            }
        }
        eventComponent.clearFlags = flags;
        Update();
    }
}
