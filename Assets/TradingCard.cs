using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TradingCard : NotificationHandler
{
    public List<IntegerRegister> integerRegistry;
    public List<TradingCard> tradingCardRegistry;

    public Attribute attribute;
    public string cardName;
    public string abilityDescription;
    public string cardID;
    public int rarity;
    public int numDuplicatesAllowed;
    public int position;
    public PositionState positionState;

    [SerializeField] private Sprite faceImage;
    public Sprite getFaceImage()
    {
        return faceImage;
    }
    public void setFaceImage(byte[] array)
    {
        faceImage = createImageFromByteArray(array);
        Material mat = StaticData.findDeepChild(transform, "Face").GetComponent<MeshRenderer>()
            .material;
        mat.SetTexture("FaceImage", faceImage.texture);
    }
    public static Sprite createImageFromByteArray(byte[] array)
    {
        Texture2D tex = new Texture2D(1, 1);
        tex.LoadImage(array);
        Sprite ret = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
        return ret;
    }
    public enum Attribute
    {
        /*0*/FIRE,
        /*1*/WATER,
        /*2*/EARTH,
        /*3*/WIND,
        /*4*/LIGHT,
        /*5*/DARK,
        /*6*/BLOOD
    }

    public override void processLine(string[] codeList)
    {
        string[] parts = codeList[codeIdx].Split(' ');
        if (parts[0] == "//")
        {
            return;
        }
        else if (parts[0] == "RETURN")
        {
            codeIdx = int.MaxValue;
        }
        else if (parts[0] == "MOV")
        {
            IntegerRegister to = getRegister(parts[1]);
            IntegerRegister from = getRegister(parts[2]);
            to.value = from.value;
        }
        else if (parts[0] == "ADD")
        {
            IntegerRegister to = getRegister(parts[1]);
            IntegerRegister from = getRegister(parts[2]);
            to.value += from.value;
        }
        else if (parts[0] == "SUB")
        {
            IntegerRegister to = getRegister(parts[1]);
            IntegerRegister from = getRegister(parts[2]);
            to.value -= from.value;
        }
        else if (parts[0] == "MUL")
        {
            IntegerRegister to = getRegister(parts[1]);
            IntegerRegister from = getRegister(parts[2]);
            to.value *= from.value;
        }
        else if (parts[0] == "DIV")
        {
            IntegerRegister to = getRegister(parts[1]);
            IntegerRegister from = getRegister(parts[2]);
            to.value /= from.value;
        }
        else if (parts[0] == "JUMP")
        {
            codeIdx = getInteger(parts[1]) - 1;
        }
        else if (parts[0] == "EQL")
        {
            IntegerRegister to = getRegister(parts[1]);
            IntegerRegister from = getRegister(parts[2]);
            if (to.effectiveValue() == from.effectiveValue())
            {
                codeIdx = int.Parse(parts[3]) - 1;
            }
        }
        else if (parts[0] == "NEQ")
        {
            IntegerRegister to = getRegister(parts[1]);
            IntegerRegister from = getRegister(parts[2]);
            if (to.effectiveValue() != from.effectiveValue())
            {
                codeIdx = int.Parse(parts[3]) - 1;
            }
        }
        else if (parts[0] == "GRT")
        {
            IntegerRegister to = getRegister(parts[1]);
            IntegerRegister from = getRegister(parts[2]);
            if (to.effectiveValue() > from.effectiveValue())
            {
                codeIdx = int.Parse(parts[3]) - 1;
            }
        }
        else if (parts[0] == "LES")
        {
            IntegerRegister to = getRegister(parts[1]);
            IntegerRegister from = getRegister(parts[2]);
            if (to.effectiveValue() < from.effectiveValue())
            {
                codeIdx = int.Parse(parts[3]) - 1;
            }
        }
        else if (parts[0] == "NOTE")
        {
            GameNotification.Nature nature = (GameNotification.Nature)getInteger(parts[1]);
            bool disputable = parts[2] == "TRUE";
            GameNotification note = new GameNotification(nature, disputable, this);
            int idx = 3;
            while (parts[idx] != "|")
            {
                note.integerRegistry.Add(new IntegerRegister(getInteger(parts[idx])));
                idx++;
            }
            idx++;
            while (parts[idx] != "|")
            {
                note.cardRegistry.Add(getCard(parts[idx]));
                idx++;
            }
            idx++;
            while (idx < parts.Length)
            {
                note.positionStateRegistry.Add(getPositionState(parts[idx]));
                idx++;
            }
            StaticData.board.addNotification(note);
        }
        else if (parts[0] == "DENY")
        {
            StaticData.board.denyCurrentNotification(this);
        }
        else if (parts[0] == "STOREINT")
        {
            int idx = getInteger(parts[1]);
            while (integerRegistry.Count <= idx)
            {
                integerRegistry.Add(null);
            }
            integerRegistry[idx] = new IntegerRegister(int.Parse(parts[2]));
        }
        else if (parts[0] == "STORECARD")
        {
            int idx = getInteger(parts[1]);
            while (tradingCardRegistry.Count <= idx)
            {
                tradingCardRegistry.Add(null);
            }
            tradingCardRegistry[idx] = getCard(parts[2]);
        }
        else if (parts[0] == "CARDEQUALS")
        {
            TradingCard card1 = getCard(parts[1]);
            TradingCard card2 = getCard(parts[2]);
            if (card1 == card2)
            {
                codeIdx = int.Parse(parts[3]) - 1;
            }
        }
        else if (parts[0] == "CARDVALUEEQUALS")
        {
            TradingCard card1 = getCard(parts[1]);
            TradingCard card2 = getCard(parts[2]);
            if (card1.cardName == card2.cardName)
            {
                codeIdx = int.Parse(parts[3]) - 1;
            }
        }
        else if (parts[0] == "POSITIONEQUALS")
        {
            PositionState state1 = getPositionState(parts[1]);
            PositionState state2 = getPositionState(parts[2]);
            if (state1 == state2)
            {
                codeIdx = int.Parse(parts[3]) - 1;
            }
        }
        else if (parts[0] == "ISHUMAN")
        {
            TradingCard card = getCard(parts[1]);
            if (card is Human)
            {
                codeIdx = int.Parse(parts[2]) - 1;
            }
        }
        else if (parts[0] == "ISCAUSE")
        {
            if (StaticData.board.getCurrentNotification().cause == this)
            {
                codeIdx = getInteger(parts[1]) - 1;
            }
        }
        else if (parts[0] == "ISREVEAL")
        {
            if (
                StaticData.board.getCurrentNotification().cardRegistry[0] == this
                &&
                ((StaticData.board.getCurrentNotification().integerRegistry[0].value == 0
                && StaticData.board.getCurrentNotification().integerRegistry[1].value != 0)
                ||
                (StaticData.board.getCurrentNotification().integerRegistry[0].value == 1
                && StaticData.board.getCurrentNotification().integerRegistry[1].value != 0
                && StaticData.board.getCurrentNotification().integerRegistry[2].value == 0))
                )
            {
                codeIdx = getInteger(parts[1]) - 1;
            }
        }
        else if (parts[0] == "STORELENGTH")
        {
            int idx = getInteger(parts[1]);
            while (integerRegistry.Count <= idx)
            {
                integerRegistry.Add(null);
            }
            PositionState state = getPositionState(parts[2]);
            integerRegistry[idx] = new IntegerRegister(state.cardsHere.Count);
        }
    }
    private IntegerRegister getRegister(string regName)
    {
        string[] nameComponents = regName.Split('.');
        int res = getInteger(nameComponents[0]);
        if (res != int.MinValue)
        {
            if (nameComponents.Length == 1)
            {
                if (int.Parse(nameComponents[0]) < 0)
                {
                    return integerRegistry[(int.Parse(nameComponents[0]) * -1) - 1];
                }
                return new IntegerRegister(res);
            }
            PositionState[] statesList = res == 0 ? StaticData.board.getPositionStatesByCard(this, true)
                : StaticData.board.getPositionStatesByCard(this, false);
            PositionState specificState = statesList[getInteger(nameComponents[1])];
            TradingCard card = specificState.cardsHere[getInteger(nameComponents[2])];
            res = getInteger(nameComponents[3]);
            if (res != int.MinValue)
            {
                return card.integerRegistry[res];
            }
            else if (nameComponents[3] == "ATK")
            {
                return ((Human)card).atk;
            }
            else if (nameComponents[3] == "DEF")
            {
                return ((Human)card).def;
            }
            else if (nameComponents[3] == "NRG")
            {
                return ((Human)card).energyCost;
            }
        }
        else if (nameComponents[0] == "STORE")
        {
            res = getInteger(nameComponents[1]);
            TradingCard card = tradingCardRegistry[res];
            if (nameComponents[2] == "ATK")
            {
                return ((Human)card).atk;
            }
            else if (nameComponents[2] == "DEF")
            {
                return ((Human)card).def;
            }
            else if (nameComponents[2] == "NRG")
            {
                return ((Human)card).energyCost;
            }
        }
        else if (nameComponents[0] == "NOTE")
        {
            if (nameComponents[1] == "CARD")
            {
                TradingCard card = StaticData.board.getCurrentNotification().cardRegistry[getInteger(nameComponents[2])];
                if (nameComponents[3] == "ATK")
                {
                    return ((Human)card).atk;
                }
                else if (nameComponents[3] == "DEF")
                {
                    return ((Human)card).def;
                }
                else if (nameComponents[3] == "NRG")
                {
                    return ((Human)card).energyCost;
                }
            }
            else if (nameComponents[1] == "INT")
            {
                return StaticData.board.getCurrentNotification().integerRegistry[getInteger(nameComponents[2])];
            }
        }
        else if (nameComponents[0] == "ATK")
        {
            return ((Human)this).atk;
        }
        else if (nameComponents[0] == "DEF")
        {
            return ((Human)this).def;
        }
        else if (nameComponents[0] == "NRG")
        {
            return ((Human)this).energyCost;
        }
        return null;
    }
    private PositionState getPositionState(string str)
    {
        string[] nameComponents = str.Split('.');
        if (nameComponents[0] == "HERE")
        {
            return positionState;
        }
        int res = getInteger(nameComponents[0]);
        PositionState[] statesList = res == 0 ? StaticData.board.getPositionStatesByCard(this, true)
            : StaticData.board.getPositionStatesByCard(this, false);
        PositionState specificState = statesList[getInteger(nameComponents[1])];
        return specificState;
    }
    private TradingCard getCard(string loc)
    {
        string[] nameComponents = loc.Split('.');
        int res = getInteger(nameComponents[0]);
        if (res == int.MinValue)
        {
            if (nameComponents[0] == "STORE")
            {
                int idx = getInteger(nameComponents[1]);
                return tradingCardRegistry[idx];
            }
            else if (nameComponents[0] == "THIS")
            {
                return this;
            }
            else if (nameComponents[0] == "NOTE")
            {
                return StaticData.board.getCurrentNotification().cardRegistry[getInteger(nameComponents[1])];
            }
        }
        PositionState[] statesList = res == 0 ? StaticData.board.getPositionStatesByCard(this, true)
            : StaticData.board.getPositionStatesByCard(this, false);
        PositionState specificState = statesList[getInteger(nameComponents[1])];
        TradingCard card = specificState.cardsHere[getInteger(nameComponents[2])];
        return card;
    }
    private int getInteger(string str)
    {
        int ret;
        if (int.TryParse(str, out ret))
        {
            if (ret < 0)
            {
                return integerRegistry[(ret * -1) - 1].value;
            }
            return ret;
        }
        return int.MinValue;
    }
}
